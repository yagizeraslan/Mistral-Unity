using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Controller for managing chat conversations with the Mistral API.
    /// Uses dependency injection for better testability and follows SOLID principles.
    /// Uses HistoryTrimmer for DRY-compliant history management.
    /// Uses ChatMessageFactory for consistent message creation.
    /// </summary>
    public class MistralChatController : IDisposable
    {
        private readonly IMistralApi mistralApi;
        private readonly IMistralStreamingApi streamingApi;
        private readonly MistralSettings settings;
        private readonly List<ChatMessage> history = new List<ChatMessage>();
        private StringBuilder currentStreamContent = new StringBuilder();
        private bool isDisposed;

        /// <summary>
        /// Event raised when a message is received (user or assistant).
        /// The bool parameter indicates if the message is from the user (true) or assistant (false).
        /// </summary>
        public event Action<ChatMessage, bool> OnMessageReceived;

        /// <summary>
        /// Event raised when streaming content is updated.
        /// Contains the accumulated content so far.
        /// </summary>
        public event Action<string> OnStreamingUpdate;

        /// <summary>
        /// Event raised when an error occurs.
        /// </summary>
        public event Action<string> OnError;

        /// <summary>
        /// Creates a new chat controller with default API implementations.
        /// </summary>
        /// <param name="config">The configuration settings.</param>
        public MistralChatController(MistralSettings config)
            : this(new MistralApi(config), new MistralStreamingApi(), config)
        {
        }

        /// <summary>
        /// Creates a new chat controller with injected dependencies.
        /// Follows Dependency Inversion Principle - depends on abstractions, not concretions.
        /// </summary>
        /// <param name="api">The non-streaming API implementation.</param>
        /// <param name="streaming">The streaming API implementation.</param>
        /// <param name="config">The configuration settings.</param>
        public MistralChatController(IMistralApi api, IMistralStreamingApi streaming, MistralSettings config)
        {
            mistralApi = api ?? throw new ArgumentNullException(nameof(api));
            streamingApi = streaming ?? throw new ArgumentNullException(nameof(streaming));
            settings = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Sends a message to the Mistral API.
        /// </summary>
        /// <param name="userMessage">The user's message content.</param>
        /// <param name="model">The model to use for completion.</param>
        /// <param name="useStreaming">Whether to use streaming mode.</param>
        public async void SendMessage(string userMessage, MistralModel model, bool useStreaming)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return;

            // Create and add user message using factory
            var userChatMessage = ChatMessageFactory.CreateUserMessage(userMessage);
            history.Add(userChatMessage);
            TrimHistoryIfNeeded();

            OnMessageReceived?.Invoke(userChatMessage, true);

            // Build request using the builder pattern
            var request = ChatCompletionRequest.Builder()
                .WithModel(model)
                .WithMessages(history.ToArray())
                .WithStreaming(useStreaming)
                .Build();

            if (useStreaming)
            {
                HandleStreamingRequest(request);
            }
            else
            {
                await HandleNonStreamingRequest(request);
            }
        }

        private void HandleStreamingRequest(ChatCompletionRequest request)
        {
            currentStreamContent.Clear();

            // Create placeholder message using factory
            var placeholderMessage = ChatMessageFactory.CreateStreamingPlaceholder();
            OnMessageReceived?.Invoke(placeholderMessage, false);

            streamingApi.CreateChatCompletionStream(
                request,
                settings.apiKey,
                onStreamUpdate: (token) =>
                {
                    currentStreamContent.Append(token);
                    OnStreamingUpdate?.Invoke(currentStreamContent.ToString());
                },
                onError: (error) =>
                {
                    string errorMessage = $"Error: {error}";
                    currentStreamContent.Append(errorMessage);
                    OnStreamingUpdate?.Invoke(currentStreamContent.ToString());
                    OnError?.Invoke(error);
                },
                onComplete: () =>
                {
                    var assistantMessage = ChatMessageFactory.CreateAssistantMessage(currentStreamContent.ToString());
                    history.Add(assistantMessage);
                    TrimHistoryIfNeeded();
                }
            );
        }

        private async System.Threading.Tasks.Task HandleNonStreamingRequest(ChatCompletionRequest request)
        {
            try
            {
                // Use the new Result-based API for better error handling
                var result = await mistralApi.CreateChatCompletionAsync(request);

                result.Match(
                    onSuccess: (response) =>
                    {
                        if (response?.choices != null && response.choices.Length > 0)
                        {
                            var assistantMessage = ChatMessageFactory.CreateAssistantMessage(
                                response.choices[0].message.content
                            );
                            history.Add(assistantMessage);
                            TrimHistoryIfNeeded();

                            OnMessageReceived?.Invoke(assistantMessage, false);
                        }
                        else
                        {
                            OnError?.Invoke("No response received from Mistral API");
                        }
                    },
                    onFailure: (error) =>
                    {
                        OnError?.Invoke(error);
                    }
                );
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
            }
        }

        private void TrimHistoryIfNeeded()
        {
            // Use shared utility for DRY compliance
            HistoryTrimmer.TrimIfNeeded(
                history,
                settings.maxHistoryMessages,
                settings.historyTrimCount
            );
        }

        /// <summary>
        /// Gets the current number of messages in history.
        /// </summary>
        /// <returns>The message count.</returns>
        public int GetHistoryCount()
        {
            return history.Count;
        }

        /// <summary>
        /// Clears the conversation history.
        /// </summary>
        public void ClearHistory()
        {
            history.Clear();
        }

        /// <summary>
        /// Disposes of the controller and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            history.Clear();

            // Cancel any active streaming
            streamingApi?.CancelStream();

            // Clear event handlers
            OnMessageReceived = null;
            OnStreamingUpdate = null;
            OnError = null;
        }
    }
}
