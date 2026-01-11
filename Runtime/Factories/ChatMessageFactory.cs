namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Factory for creating ChatMessage objects.
    /// Follows the Factory Pattern - centralizes object creation logic.
    /// Follows DRY principle - eliminates repeated message creation code.
    /// </summary>
    public static class ChatMessageFactory
    {
        /// <summary>
        /// Creates a user message with the specified content.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <returns>A new ChatMessage with the user role.</returns>
        public static ChatMessage CreateUserMessage(string content)
        {
            return new ChatMessage(ChatRole.User, content);
        }

        /// <summary>
        /// Creates an assistant message with the specified content.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <returns>A new ChatMessage with the assistant role.</returns>
        public static ChatMessage CreateAssistantMessage(string content)
        {
            return new ChatMessage(ChatRole.Assistant, content);
        }

        /// <summary>
        /// Creates a system message with the specified content.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <returns>A new ChatMessage with the system role.</returns>
        public static ChatMessage CreateSystemMessage(string content)
        {
            return new ChatMessage(ChatRole.System, content);
        }

        /// <summary>
        /// Creates an empty placeholder message for streaming.
        /// Used to create a UI placeholder before streaming content arrives.
        /// </summary>
        /// <returns>A new ChatMessage with the assistant role and empty content.</returns>
        public static ChatMessage CreateStreamingPlaceholder()
        {
            return new ChatMessage(ChatRole.Assistant, string.Empty);
        }

        /// <summary>
        /// Creates a message from an API response choice.
        /// Converts the API response format to our internal ChatMessage format.
        /// </summary>
        /// <param name="choice">The choice from the API response.</param>
        /// <returns>A new ChatMessage based on the response, or null if invalid.</returns>
        public static ChatMessage CreateFromResponse(Choice choice)
        {
            if (choice?.message == null)
                return null;

            return new ChatMessage(
                ChatRoleExtensions.FromApiString(choice.message.role),
                choice.message.content
            );
        }
    }
}
