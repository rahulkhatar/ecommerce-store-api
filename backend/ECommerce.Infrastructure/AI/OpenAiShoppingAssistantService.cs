using System.ComponentModel;
using ECommerce.Application.Features.Orders;
using ECommerce.Application.Features.Products;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using System.ClientModel;

namespace ECommerce.Infrastructure.AI;

// Agentic shopping assistant: builds an IChatClient with UseFunctionInvocation()
// so the model can autonomously call tools (search the catalog, look up a
// specific product, pull the caller's own orders) as many times as it needs
// before answering, rather than us hand-coding "if the message contains X,
// call Y". Tools are built per-request (not DI singletons) because they
// close over the calling customerId - get_my_orders must never be able to
// return another customer's orders, so the scoping has to be baked into the
// closure, not trusted to a parameter the model could set wrong.
public class OpenAiShoppingAssistantService(
    IConfiguration configuration,
    ISender sender,
    IChatHistoryRepository chatHistoryRepository,
    ILogger<OpenAiShoppingAssistantService> logger) : IShoppingAssistantService
{
    private const string SystemPrompt =
        "You are the shopping assistant for ECommerce Store. Help customers find products, " +
        "check product details, and check the status of their own orders. Use the available " +
        "tools rather than guessing - if you don't have enough information from a tool result, " +
        "say so rather than making something up. You can only ever see the current customer's " +
        "own orders, never anyone else's. " +
        "You're also the store's general customer-service contact, not just a product/order " +
        "lookup tool - answer greetings, small talk, and general shopping questions (how checkout " +
        "works, what payment methods this store's checkout page offers, how to track an order) " +
        "conversationally and helpfully. The one thing to never do is invent specific store " +
        "policies or facts you don't actually have (exact return windows, warranty terms, physical " +
        "store locations, etc.) - for those, say you don't have that information rather than " +
        "guessing. Keep every answer concise and friendly.";

    public async Task<AssistantReply> AskAsync(Guid customerId, Guid? sessionId, string message, CancellationToken cancellationToken = default)
    {
        var actualSessionId = sessionId ?? Guid.NewGuid();
        var history = await chatHistoryRepository.GetSessionAsync(customerId, actualSessionId, cancellationToken);

        var messages = new List<ChatMessage> { new(ChatRole.System, SystemPrompt) };
        messages.AddRange(history.Select(h =>
            new ChatMessage(string.Equals(h.MessageRole, "User", StringComparison.OrdinalIgnoreCase) ? ChatRole.User : ChatRole.Assistant, h.MessageContent)));
        messages.Add(new ChatMessage(ChatRole.User, message));

        var chatClient = BuildChatClient();
        var foundProducts = new Dictionary<Guid, AssistantProductRef>();
        var tools = BuildTools(customerId, foundProducts, cancellationToken);

        logger.LogInformation("Assistant request: customer {CustomerId}, session {SessionId}, {ToolCount} tools available.",
            customerId, actualSessionId, tools.Count);

        var response = await chatClient.GetResponseAsync(messages, new ChatOptions { Tools = tools }, cancellationToken);
        var replyText = response.Text;

        logger.LogInformation("Assistant reply: session {SessionId}, {Length} chars.", actualSessionId, replyText.Length);

        var now = DateTime.UtcNow;
        await chatHistoryRepository.AddAsync(new ChatHistory
        {
            Id = Guid.NewGuid(), CustomerId = customerId, SessionId = actualSessionId,
            MessageRole = "User", MessageContent = message, CreatedAt = now,
        }, cancellationToken);
        await chatHistoryRepository.AddAsync(new ChatHistory
        {
            Id = Guid.NewGuid(), CustomerId = customerId, SessionId = actualSessionId,
            MessageRole = "Assistant", MessageContent = replyText, CreatedAt = now,
        }, cancellationToken);
        await chatHistoryRepository.SaveChangesAsync(cancellationToken);

        return new AssistantReply(actualSessionId, replyText, foundProducts.Values.ToList());
    }

    private IChatClient BuildChatClient()
    {
        var apiKey = configuration["OpenAI:ApiKey"] ?? "your-openai-api-key";
        var model = configuration["OpenAI:Model"] ?? "gpt-4";
        var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey));

        return new ChatClientBuilder(openAiClient.GetChatClient(model).AsIChatClient())
            .UseFunctionInvocation()
            .Build();
    }

    private List<AITool> BuildTools(Guid customerId, Dictionary<Guid, AssistantProductRef> foundProducts, CancellationToken cancellationToken)
    {
        async Task<string> SearchProducts([Description("Keywords to search the product catalog for, e.g. a product name or category.")] string query)
        {
            var results = await sender.Send(new SearchProductsQuery(query, 5), cancellationToken);
            foreach (var r in results)
            {
                foundProducts[r.ProductId] = new AssistantProductRef(r.ProductId, r.Name, r.Price, r.CategoryName, r.ImageUrl, r.Description);
            }

            return results.Count == 0
                ? "No products matched that search."
                : string.Join('\n', results.Select(r => $"- {r.Name} (id: {r.ProductId}) - ${r.Price} - {r.CategoryName}"));
        }

        async Task<string> GetProductDetails([Description("The product's id (a GUID), usually from a prior search_products result.")] Guid productId)
        {
            try
            {
                var p = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);
                var price = p.DiscountPrice ?? p.Price;
                foundProducts[p.Id] = new AssistantProductRef(p.Id, p.Name, price, p.CategoryName, p.ImageUrl, p.Description);
                return $"{p.Name}: {p.Description} Price: ${price}. {(p.IsActive ? "Available." : "Not currently available.")}";
            }
            catch (NotFoundException)
            {
                return "No product found with that id.";
            }
        }

        async Task<string> GetMyOrders()
        {
            var orders = await sender.Send(new GetOrdersQuery(customerId), cancellationToken);
            return orders.Count == 0
                ? "This customer has no orders yet."
                : string.Join('\n', orders.Take(5).Select(o => $"- {o.OrderNumber}: {o.OrderStatus}, total ${o.TotalAmount}, placed {o.CreatedAt:yyyy-MM-dd}"));
        }

        return
        [
            AIFunctionFactory.Create(SearchProducts, name: "search_products", description: "Search the store's product catalog by keyword."),
            AIFunctionFactory.Create(GetProductDetails, name: "get_product_details", description: "Get full details for one specific product by id."),
            AIFunctionFactory.Create(GetMyOrders, name: "get_my_orders", description: "Get the current customer's own recent orders and their statuses."),
        ];
    }
}
