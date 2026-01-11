namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Centralized constants for the Mistral API integration.
    /// Following the DRY principle - all magic values are defined once.
    /// </summary>
    public static class ApiConstants
    {
        #region API Configuration

        /// <summary>
        /// The base URL for the Mistral API chat completions endpoint.
        /// </summary>
        public const string API_URL = "https://api.mistral.ai/v1/chat/completions";

        /// <summary>
        /// Content type header value for JSON requests.
        /// </summary>
        public const string CONTENT_TYPE_JSON = "application/json";

        /// <summary>
        /// Accept header value for Server-Sent Events (SSE) streaming.
        /// </summary>
        public const string ACCEPT_SSE = "text/event-stream";

        #endregion

        #region SSE Protocol Constants

        /// <summary>
        /// Prefix for SSE data lines.
        /// </summary>
        public const string SSE_DATA_PREFIX = "data: ";

        /// <summary>
        /// SSE stream termination marker.
        /// </summary>
        public const string SSE_DONE_MARKER = "[DONE]";

        #endregion

        #region Default Request Parameters

        /// <summary>
        /// Default temperature for chat completions (0.0-2.0 range).
        /// Controls randomness: lower values are more deterministic.
        /// </summary>
        public const float DEFAULT_TEMPERATURE = 0.7f;

        /// <summary>
        /// Default maximum tokens for response generation.
        /// </summary>
        public const int DEFAULT_MAX_TOKENS = 1000;

        /// <summary>
        /// Default top_p (nucleus sampling) value (0.0-1.0 range).
        /// </summary>
        public const float DEFAULT_TOP_P = 1f;

        #endregion

        #region Memory Management Defaults

        /// <summary>
        /// Default maximum number of messages to keep in conversation history.
        /// </summary>
        public const int DEFAULT_MAX_HISTORY_MESSAGES = 50;

        /// <summary>
        /// Default number of messages to keep when trimming history.
        /// </summary>
        public const int DEFAULT_HISTORY_TRIM_COUNT = 30;

        /// <summary>
        /// Default maximum number of UI message objects to display.
        /// </summary>
        public const int DEFAULT_MAX_UI_MESSAGES = 100;

        /// <summary>
        /// Default number of UI messages to keep when trimming.
        /// </summary>
        public const int DEFAULT_UI_TRIM_COUNT = 70;

        #endregion

        #region HTTP Configuration

        /// <summary>
        /// Authorization header name.
        /// </summary>
        public const string HEADER_AUTHORIZATION = "Authorization";

        /// <summary>
        /// Content-Type header name.
        /// </summary>
        public const string HEADER_CONTENT_TYPE = "Content-Type";

        /// <summary>
        /// Accept header name.
        /// </summary>
        public const string HEADER_ACCEPT = "Accept";

        /// <summary>
        /// Bearer token prefix for authorization.
        /// </summary>
        public const string BEARER_PREFIX = "Bearer ";

        #endregion

        #region Logging Prefixes

        /// <summary>
        /// Log prefix for MistralApi messages.
        /// </summary>
        public const string LOG_PREFIX_API = "[MistralApi]";

        /// <summary>
        /// Log prefix for MistralStreamingApi messages.
        /// </summary>
        public const string LOG_PREFIX_STREAMING = "[MistralStreamingApi]";

        /// <summary>
        /// Log prefix for MistralChat UI messages.
        /// </summary>
        public const string LOG_PREFIX_CHAT = "[MistralChat]";

        #endregion
    }
}
