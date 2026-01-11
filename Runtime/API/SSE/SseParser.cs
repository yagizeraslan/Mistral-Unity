using System;
using System.Text;
using UnityEngine;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Parses Server-Sent Events (SSE) data streams.
    /// Following Single Responsibility Principle - only handles SSE protocol parsing.
    /// Extracted from MistralStreamingApi to improve testability and reusability.
    /// </summary>
    public class SseParser
    {
        private readonly StringBuilder buffer = new StringBuilder();
        private readonly Action<string> onData;
        private readonly Action onComplete;
        private readonly Action<string> onError;

        /// <summary>
        /// Creates a new SSE parser with the specified callbacks.
        /// </summary>
        /// <param name="onData">Callback for parsed data (content from SSE data lines).</param>
        /// <param name="onComplete">Callback when stream completes.</param>
        /// <param name="onError">Optional callback for parse errors.</param>
        public SseParser(Action<string> onData, Action onComplete, Action<string> onError = null)
        {
            this.onData = onData ?? throw new ArgumentNullException(nameof(onData));
            this.onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
            this.onError = onError;
        }

        /// <summary>
        /// Processes a chunk of SSE data.
        /// Handles incomplete lines by buffering until a complete line is received.
        /// </summary>
        /// <param name="chunk">The raw data chunk to process.</param>
        public void ProcessChunk(string chunk)
        {
            if (string.IsNullOrEmpty(chunk))
                return;

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
        }

        /// <summary>
        /// Processes a single SSE line.
        /// </summary>
        /// <param name="line">The line to process.</param>
        private void ProcessLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line.StartsWith(ApiConstants.SSE_DATA_PREFIX))
            {
                string jsonData = line.Substring(ApiConstants.SSE_DATA_PREFIX.Length).Trim();

                if (jsonData == ApiConstants.SSE_DONE_MARKER)
                {
                    onComplete?.Invoke();
                    return;
                }

                try
                {
                    ParseAndExtractContent(jsonData);
                }
                catch (Exception ex)
                {
                    // Partial JSON chunks are expected during streaming
                    // Only log in debug mode for troubleshooting
                    #if UNITY_EDITOR
                    Debug.LogWarning($"{ApiConstants.LOG_PREFIX_STREAMING} Partial JSON chunk: {ex.Message}");
                    #endif
                }
            }
        }

        /// <summary>
        /// Parses the JSON data and extracts the content.
        /// </summary>
        /// <param name="jsonData">The JSON data to parse.</param>
        private void ParseAndExtractContent(string jsonData)
        {
            StreamingResponse response = JsonUtility.FromJson<StreamingResponse>(jsonData);

            if (response?.choices != null && response.choices.Length > 0)
            {
                string content = response.choices[0].delta?.content;
                if (!string.IsNullOrEmpty(content))
                {
                    onData?.Invoke(content);
                }
            }
        }

        /// <summary>
        /// Clears the internal buffer.
        /// Call this when starting a new stream.
        /// </summary>
        public void Reset()
        {
            buffer.Clear();
        }

        #region Streaming Response DTOs

        /// <summary>
        /// DTO for streaming response.
        /// </summary>
        [Serializable]
        private class StreamingResponse
        {
            public string id;
            public string @object;
            public long created;
            public string model;
            public StreamingChoice[] choices;
        }

        /// <summary>
        /// DTO for streaming choice.
        /// </summary>
        [Serializable]
        private class StreamingChoice
        {
            public int index;
            public StreamingDelta delta;
            public string finish_reason;
        }

        /// <summary>
        /// DTO for streaming delta (incremental content).
        /// </summary>
        [Serializable]
        private class StreamingDelta
        {
            public string role;
            public string content;
        }

        #endregion
    }
}
