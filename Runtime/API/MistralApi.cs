using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Implementation of the non-streaming Mistral API.
    /// Uses centralized constants and Result pattern for error handling.
    /// </summary>
    public class MistralApi : IMistralApi
    {
        private readonly MistralSettings settings;

        /// <summary>
        /// Gets the API key being used.
        /// </summary>
        public string ApiKey => settings.apiKey;

        /// <summary>
        /// Creates a new MistralApi instance with the specified settings.
        /// </summary>
        /// <param name="config">The configuration settings.</param>
        public MistralApi(MistralSettings config)
        {
            settings = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Creates a chat completion request and returns a Result wrapper.
        /// This method captures errors in the Result object instead of throwing or returning null.
        /// </summary>
        /// <param name="request">The chat completion request configuration.</param>
        /// <returns>A Result containing either the response or error information.</returns>
        public async Task<Result<ChatCompletionResponse>> CreateChatCompletionAsync(ChatCompletionRequest request)
        {
            if (request == null)
                return Result<ChatCompletionResponse>.Failure("Request cannot be null");

            if (string.IsNullOrEmpty(settings.apiKey))
                return Result<ChatCompletionResponse>.Failure("API key is not configured");

            try
            {
                string jsonBody = JsonUtility.ToJson(request);

                using (UnityWebRequest www = new UnityWebRequest(ApiConstants.API_URL, "POST"))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    www.downloadHandler = new DownloadHandlerBuffer();

                    www.SetRequestHeader(ApiConstants.HEADER_CONTENT_TYPE, ApiConstants.CONTENT_TYPE_JSON);
                    www.SetRequestHeader(ApiConstants.HEADER_AUTHORIZATION, $"{ApiConstants.BEARER_PREFIX}{settings.apiKey}");

                    await www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        string errorDetails = www.downloadHandler?.text ?? www.error;
                        Debug.LogError($"{ApiConstants.LOG_PREFIX_API} Error: {www.error}");
                        Debug.LogError($"{ApiConstants.LOG_PREFIX_API} Response: {errorDetails}");

                        return Result<ChatCompletionResponse>.Failure(
                            $"API request failed: {www.error}",
                            httpStatusCode: (int)www.responseCode
                        );
                    }

                    string responseJson = www.downloadHandler.text;
                    ChatCompletionResponse response = JsonUtility.FromJson<ChatCompletionResponse>(responseJson);

                    if (response == null)
                    {
                        return Result<ChatCompletionResponse>.Failure("Failed to parse API response");
                    }

                    return Result<ChatCompletionResponse>.Success(response);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ApiConstants.LOG_PREFIX_API} Exception: {ex.Message}");
                return Result<ChatCompletionResponse>.Failure(ex.Message, ex);
            }
        }

        /// <summary>
        /// Creates a chat completion request (legacy method for backwards compatibility).
        /// Returns null on error. Prefer using CreateChatCompletionAsync for better error handling.
        /// </summary>
        /// <param name="request">The chat completion request configuration.</param>
        /// <returns>The chat completion response, or null if an error occurred.</returns>
        public async Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request)
        {
            var result = await CreateChatCompletionAsync(request);
            return result.GetValueOrDefault(null);
        }
    }
}
