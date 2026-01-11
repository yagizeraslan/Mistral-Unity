using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YagizEraslan.Mistral.Unity
{
    public class MistralChatController : IDisposable
    {
        private readonly MistralApi mistralApi;
        private readonly MistralStreamingApi streamingApi;
        private readonly MistralSettings settings;
        private readonly List<ChatMessage> history = new List<ChatMessage>();
        private StringBuilder currentStreamContent = new StringBuilder();
        private bool isDisposed;

        public event Action<ChatMessage, bool> OnMessageReceived;
        public event Action<string> OnStreamingUpdate;
        public event Action<string> OnError;

        public MistralChatController(MistralSettings config)
        {
            settings = config;
            mistralApi = new MistralApi(config);
            streamingApi = new MistralStreamingApi();
        }

        public async void SendMessage(string userMessage, MistralModel model, bool useStreaming)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return;

            var userChatMessage = new ChatMessage
            {
                role = "user",
                content = userMessage
            };
            history.Add(userChatMessage);
            TrimHistoryIfNeeded();

            OnMessageReceived?.Invoke(userChatMessage, true);

            var request = new ChatCompletionRequest
            {
                model = model.ToModelString(),
                messages = history.ToArray(),
                stream = useStreaming
            };

            if (useStreaming)
            {
                currentStreamContent.Clear();

                var placeholderMessage = new ChatMessage
                {
                    role = "assistant",
                    content = ""
                };
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
                        var assistantMessage = new ChatMessage
                        {
                            role = "assistant",
                            content = currentStreamContent.ToString()
                        };
                        history.Add(assistantMessage);
                        TrimHistoryIfNeeded();
                    }
                );
            }
            else
            {
                try
                {
                    var response = await mistralApi.CreateChatCompletion(request);

                    if (response?.choices != null && response.choices.Length > 0)
                    {
                        var assistantMessage = new ChatMessage
                        {
                            role = "assistant",
                            content = response.choices[0].message.content
                        };
                        history.Add(assistantMessage);
                        TrimHistoryIfNeeded();

                        OnMessageReceived?.Invoke(assistantMessage, false);
                    }
                    else
                    {
                        OnError?.Invoke("No response received from Mistral API");
                    }
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex.Message);
                }
            }
        }

        private void TrimHistoryIfNeeded()
        {
            if (settings.maxHistoryMessages <= 0)
                return;

            if (history.Count > settings.maxHistoryMessages)
            {
                int messagesToRemove = history.Count - settings.historyTrimCount;
                history.RemoveRange(0, messagesToRemove);
            }
        }

        public int GetHistoryCount()
        {
            return history.Count;
        }

        public void ClearHistory()
        {
            history.Clear();
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;
            history.Clear();
            OnMessageReceived = null;
            OnStreamingUpdate = null;
            OnError = null;
        }
    }
}
