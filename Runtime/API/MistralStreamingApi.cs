using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Implementation of the streaming Mistral API.
    /// Uses SSE parser for protocol handling and implements IMistralStreamingApi.
    /// Follows Single Responsibility Principle - delegates SSE parsing to SseParser.
    /// </summary>
    public class MistralStreamingApi : IMistralStreamingApi
    {
        private UnityWebRequest activeRequest;

        /// <summary>
        /// Creates a streaming chat completion request.
        /// Uses callback pattern for real-time token delivery.
        /// </summary>
        /// <param name="request">The chat completion request configuration.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="onStreamUpdate">Callback invoked for each token received.</param>
        /// <param name="onError">Optional callback invoked when an error occurs.</param>
        /// <param name="onComplete">Optional callback invoked when the stream completes.</param>
        public void CreateChatCompletionStream(
            ChatCompletionRequest request,
            string apiKey,
            Action<string> onStreamUpdate,
            Action<string> onError = null,
            Action onComplete = null)
        {
            if (request == null)
            {
                onError?.Invoke("Request cannot be null");
                return;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                onError?.Invoke("API key is not configured");
                return;
            }

            // Cancel any existing request
            CancelStream();

            // Enable streaming mode
            request.stream = true;
            string jsonBody = JsonUtility.ToJson(request);

            activeRequest = new UnityWebRequest(ApiConstants.API_URL, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            activeRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);

            var streamingHandler = new StreamingDownloadHandler(
                onStreamUpdate,
                onError,
                () =>
                {
                    onComplete?.Invoke();
                    CleanupRequest();
                },
                activeRequest
            );
            activeRequest.downloadHandler = streamingHandler;

            activeRequest.SetRequestHeader(ApiConstants.HEADER_CONTENT_TYPE, ApiConstants.CONTENT_TYPE_JSON);
            activeRequest.SetRequestHeader(ApiConstants.HEADER_AUTHORIZATION, $"{ApiConstants.BEARER_PREFIX}{apiKey}");
            activeRequest.SetRequestHeader(ApiConstants.HEADER_ACCEPT, ApiConstants.ACCEPT_SSE);

            activeRequest.SendWebRequest();
        }

        /// <summary>
        /// Cancels any active streaming operation.
        /// </summary>
        public void CancelStream()
        {
            CleanupRequest();
        }

        private void CleanupRequest()
        {
            if (activeRequest != null)
            {
                activeRequest.Abort();
                activeRequest.Dispose();
                activeRequest = null;
            }
        }

        /// <summary>
        /// Custom download handler for streaming responses.
        /// Uses SseParser for protocol handling (Single Responsibility Principle).
        /// </summary>
        private class StreamingDownloadHandler : DownloadHandlerScript
        {
            private readonly SseParser parser;
            private readonly Action<string> onError;
            private readonly Action onComplete;
            private readonly UnityWebRequest request;

            public StreamingDownloadHandler(
                Action<string> onStreamUpdate,
                Action<string> onError,
                Action onComplete,
                UnityWebRequest request) : base(new byte[1024])
            {
                this.onError = onError;
                this.onComplete = onComplete;
                this.request = request;

                // Delegate SSE parsing to the parser (Single Responsibility)
                parser = new SseParser(
                    onData: onStreamUpdate,
                    onComplete: onComplete,
                    onError: (error) =>
                    {
                        Debug.LogWarning($"{ApiConstants.LOG_PREFIX_STREAMING} Parse warning: {error}");
                    }
                );
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (data == null || dataLength == 0)
                    return true;

                string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
                parser.ProcessChunk(chunk);

                return true;
            }

            protected override void CompleteContent()
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"{ApiConstants.LOG_PREFIX_STREAMING} Error: {request.error}");
                    onError?.Invoke(request.error);
                }
                else
                {
                    onComplete?.Invoke();
                }
            }
        }
    }
}
