namespace ECommerce.Domain.Interfaces;

public record AssistantReply(Guid SessionId, string Message);

// Agentic (tool-calling) shopping assistant: the LLM autonomously decides
// whether/which tools to invoke (search the catalog, look up a product, pull
// the caller's own order history) rather than following a fixed script - see
// OpenAiShoppingAssistantService for the tool set. Declared in Domain for the
// same reason as the other Infrastructure-implemented interfaces here.
public interface IShoppingAssistantService
{
    Task<AssistantReply> AskAsync(Guid customerId, Guid? sessionId, string message, CancellationToken cancellationToken = default);
}
