using System.Threading.Tasks;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Interface for non-streaming Mistral API operations.
    /// Follows Interface Segregation Principle - streaming and non-streaming are separate interfaces.
    /// Uses Result&lt;T&gt; pattern instead of null returns for better error handling.
    /// </summary>
    public interface IMistralApi
    {
        /// <summary>
        /// Gets the API key being used.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Creates a chat completion request and returns a Result wrapper.
        /// This method will not throw exceptions - errors are captured in the Result object.
        /// </summary>
        /// <param name="request">The chat completion request configuration.</param>
        /// <returns>A Result containing either the response or error information.</returns>
        Task<Result<ChatCompletionResponse>> CreateChatCompletionAsync(ChatCompletionRequest request);

        /// <summary>
        /// Creates a chat completion request (legacy method for backwards compatibility).
        /// Returns null on error. Prefer using CreateChatCompletionAsync for better error handling.
        /// </summary>
        /// <param name="request">The chat completion request configuration.</param>
        /// <returns>The chat completion response, or null if an error occurred.</returns>
        Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request);
    }
}
