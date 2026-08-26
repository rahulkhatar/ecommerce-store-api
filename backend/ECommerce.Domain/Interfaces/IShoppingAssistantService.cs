namespace ECommerce.Domain.Interfaces;

// Products the assistant actually looked up while answering (via
// search_products/get_product_details) - lets the UI show a real product
// list (image, price, category) alongside the reply text instead of the
// model describing them in prose alone.
public record AssistantProductRef(Guid ProductId, string Name, decimal Price, string CategoryName, string? ImageUrl, string? Description);

public record AssistantReply(Guid SessionId, string Message, List<AssistantProductRef> Products);

// Agentic (tool-calling) shopping assistant: the LLM autonomously decides
// whether/which tools to invoke (search the catalog, look up a product, pull
// the caller's own order history) rather than following a fixed script - see
// OpenAiShoppingAssistantService for the tool set. Declared in Domain for the
// same reason as the other Infrastructure-implemented interfaces here.
public interface IShoppingAssistantService
{
    Task<AssistantReply> AskAsync(Guid customerId, Guid? sessionId, string message, CancellationToken cancellationToken = default);
}
