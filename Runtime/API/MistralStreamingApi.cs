using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Mistral.Unity
{
    public class MistralStreamingApi
    {
        private const string API_URL = "https://api.mistral.ai/v1/chat/completions";

        public void CreateChatCompletionStream(
            ChatCompletionRequest request,
            string apiKey,
            Action<string> onStreamUpdate,
            Action<string> onError = null,
            Action onComplete = null)
        {
            request.stream = true;
            string jsonBody = JsonUtility.ToJson(request);

            UnityWebRequest www = new UnityWebRequest(API_URL, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);

            var streamingHandler = new StreamingDownloadHandler(onStreamUpdate, onError, onComplete, www);
            www.downloadHandler = streamingHandler;

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            www.SetRequestHeader("Accept", "text/event-stream");

            www.SendWebRequest();
        }

        private class StreamingDownloadHandler : DownloadHandlerScript
        {
            private readonly Action<string> onStreamUpdate;
            private readonly Action<string> onError;
            private readonly Action onComplete;
            private readonly UnityWebRequest request;
            private readonly StringBuilder buffer = new StringBuilder();

            public StreamingDownloadHandler(
                Action<string> onStreamUpdate,
                Action<string> onError,
                Action onComplete,
                UnityWebRequest request) : base(new byte[1024])
            {
                this.onStreamUpdate = onStreamUpdate;
                this.onError = onError;
                this.onComplete = onComplete;
                this.request = request;
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (data == null || dataLength == 0)
                    return true;

                string chunk = Encoding.UTF8.GetString(data, 0, dataLength);
                buffer.Append(chunk);

                string bufferContent = buffer.ToString();
                string[] lines = bufferContent.Split('\n');

                // Keep the last incomplete line in the buffer
                if (!bufferContent.EndsWith("\n"))
                {
                    buffer.Clear();
                    buffer.Append(lines[lines.Length - 1]);

                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        ProcessLine(lines[i]);
                    }
                }
                else
                {
                    buffer.Clear();
                    foreach (string line in lines)
                    {
                        ProcessLine(line);
                    }
                }

                return true;
            }

            private void ProcessLine(string line)
            {
                if (string.IsNullOrWhiteSpace(line))
                    return;

                if (line.StartsWith("data: "))
                {
                    string jsonData = line.Substring(6).Trim();

                    if (jsonData == "[DONE]")
                    {
                        onComplete?.Invoke();
                        request?.Dispose();
                        return;
                    }

                    try
                    {
                        StreamingResponse response = JsonUtility.FromJson<StreamingResponse>(jsonData);
                        if (response?.choices != null && response.choices.Length > 0)
                        {
                            string content = response.choices[0].delta?.content;
                            if (!string.IsNullOrEmpty(content))
                            {
                                onStreamUpdate?.Invoke(content);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Partial JSON chunks are expected during streaming, skip silently
                    }
                }
            }

            protected override void CompleteContent()
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke(request.error);
                }
                else
                {
                    onComplete?.Invoke();
                }
                request?.Dispose();
            }
        }

        [Serializable]
        private class StreamingResponse
        {
            public string id;
            public string @object;
            public long created;
            public string model;
            public StreamingChoice[] choices;
        }

        [Serializable]
        private class StreamingChoice
        {
            public int index;
            public StreamingDelta delta;
            public string finish_reason;
        }

        [Serializable]
        private class StreamingDelta
        {
            public string role;
            public string content;
        }
    }
}
