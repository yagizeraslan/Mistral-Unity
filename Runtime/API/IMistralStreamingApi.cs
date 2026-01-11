using System;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Interface for streaming chat completion API operations.
    /// Follows Interface Segregation Principle - streaming and non-streaming APIs are separate.
    /// This allows clients to depend only on the interface they need.
    /// </summary>
    public interface IMistralStreamingApi
    {
        /// <summary>
        /// Creates a streaming chat completion request.
        /// Uses callback pattern for real-time token delivery.
        /// </summary>
        /// <param name="request">The chat completion request configuration.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="onStreamUpdate">Callback invoked for each token received.</param>
        /// <param name="onError">Optional callback invoked when an error occurs.</param>
        /// <param name="onComplete">Optional callback invoked when the stream completes.</param>
        void CreateChatCompletionStream(
            ChatCompletionRequest request,
            string apiKey,
            Action<string> onStreamUpdate,
            Action<string> onError = null,
            Action onComplete = null);

        /// <summary>
        /// Cancels any active streaming operation.
        /// </summary>
        void CancelStream();
    }
}
