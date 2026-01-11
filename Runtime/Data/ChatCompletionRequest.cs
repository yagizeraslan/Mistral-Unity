using System;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Represents a chat completion request to the Mistral API.
    /// Uses centralized constants instead of magic numbers.
    /// </summary>
    [Serializable]
    public class ChatCompletionRequest
    {
        /// <summary>
        /// The model to use for completion.
        /// </summary>
        public string model;

        /// <summary>
        /// The messages in the conversation.
        /// </summary>
        public ChatMessage[] messages;

        /// <summary>
        /// Sampling temperature (0.0-2.0). Higher values mean more random output.
        /// </summary>
        public float temperature = ApiConstants.DEFAULT_TEMPERATURE;

        /// <summary>
        /// Maximum number of tokens to generate.
        /// </summary>
        public int max_tokens = ApiConstants.DEFAULT_MAX_TOKENS;

        /// <summary>
        /// Whether to stream the response.
        /// </summary>
        public bool stream;

        /// <summary>
        /// Nucleus sampling parameter (0.0-1.0).
        /// </summary>
        public float top_p = ApiConstants.DEFAULT_TOP_P;

        /// <summary>
        /// Whether to enable safe prompt mode.
        /// </summary>
        public bool safe_prompt;

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public ChatCompletionRequest()
        {
        }

        /// <summary>
        /// Creates a new chat completion request with the specified parameters.
        /// </summary>
        /// <param name="modelName">The model to use.</param>
        /// <param name="chatMessages">The messages in the conversation.</param>
        /// <param name="useStreaming">Whether to stream the response.</param>
        public ChatCompletionRequest(string modelName, ChatMessage[] chatMessages, bool useStreaming = false)
        {
            model = modelName;
            messages = chatMessages;
            stream = useStreaming;
        }

        /// <summary>
        /// Creates a request builder for fluent configuration.
        /// </summary>
        /// <returns>A new builder instance.</returns>
        public static ChatCompletionRequestBuilder Builder()
        {
            return new ChatCompletionRequestBuilder();
        }
    }

    /// <summary>
    /// Builder class for creating ChatCompletionRequest with fluent API.
    /// Follows the Builder Pattern for clear and flexible object construction.
    /// </summary>
    public class ChatCompletionRequestBuilder
    {
        private string model;
        private ChatMessage[] messages;
        private float temperature = ApiConstants.DEFAULT_TEMPERATURE;
        private int maxTokens = ApiConstants.DEFAULT_MAX_TOKENS;
        private bool stream;
        private float topP = ApiConstants.DEFAULT_TOP_P;
        private bool safePrompt;

        /// <summary>
        /// Sets the model to use.
        /// </summary>
        public ChatCompletionRequestBuilder WithModel(string modelName)
        {
            model = modelName;
            return this;
        }

        /// <summary>
        /// Sets the model using the MistralModel enum.
        /// </summary>
        public ChatCompletionRequestBuilder WithModel(MistralModel modelType)
        {
            model = modelType.ToModelString();
            return this;
        }

        /// <summary>
        /// Sets the messages for the conversation.
        /// </summary>
        public ChatCompletionRequestBuilder WithMessages(ChatMessage[] chatMessages)
        {
            messages = chatMessages;
            return this;
        }

        /// <summary>
        /// Sets the temperature parameter.
        /// </summary>
        public ChatCompletionRequestBuilder WithTemperature(float value)
        {
            temperature = Math.Clamp(value, 0f, 2f);
            return this;
        }

        /// <summary>
        /// Sets the maximum tokens parameter.
        /// </summary>
        public ChatCompletionRequestBuilder WithMaxTokens(int value)
        {
            maxTokens = Math.Max(1, value);
            return this;
        }

        /// <summary>
        /// Enables or disables streaming.
        /// </summary>
        public ChatCompletionRequestBuilder WithStreaming(bool enabled = true)
        {
            stream = enabled;
            return this;
        }

        /// <summary>
        /// Sets the top_p (nucleus sampling) parameter.
        /// </summary>
        public ChatCompletionRequestBuilder WithTopP(float value)
        {
            topP = Math.Clamp(value, 0f, 1f);
            return this;
        }

        /// <summary>
        /// Enables or disables safe prompt mode.
        /// </summary>
        public ChatCompletionRequestBuilder WithSafePrompt(bool enabled = true)
        {
            safePrompt = enabled;
            return this;
        }

        /// <summary>
        /// Builds the ChatCompletionRequest.
        /// </summary>
        /// <returns>The configured ChatCompletionRequest.</returns>
        public ChatCompletionRequest Build()
        {
            return new ChatCompletionRequest
            {
                model = this.model,
                messages = this.messages,
                temperature = this.temperature,
                max_tokens = this.maxTokens,
                stream = this.stream,
                top_p = this.topP,
                safe_prompt = this.safePrompt
            };
        }
    }
}
