using System;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Represents a message in a chat conversation.
    /// Uses proper encapsulation with typed ChatRole enum instead of magic strings.
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        /// <summary>
        /// The role of the message sender (user, assistant, or system).
        /// Serialized as a string for API compatibility.
        /// </summary>
        public string role;

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string content;

        /// <summary>
        /// Default constructor required for JSON deserialization.
        /// </summary>
        public ChatMessage()
        {
        }

        /// <summary>
        /// Creates a new ChatMessage with the specified role and content.
        /// </summary>
        /// <param name="chatRole">The role of the message sender.</param>
        /// <param name="messageContent">The content of the message.</param>
        public ChatMessage(ChatRole chatRole, string messageContent)
        {
            role = chatRole.ToApiString();
            content = messageContent ?? string.Empty;
        }

        /// <summary>
        /// Gets the ChatRole enum value from the string role.
        /// </summary>
        /// <returns>The ChatRole enum value.</returns>
        public ChatRole GetRole()
        {
            return ChatRoleExtensions.FromApiString(role);
        }

        /// <summary>
        /// Checks if this message is from a user.
        /// </summary>
        public bool IsUserMessage => GetRole() == ChatRole.User;

        /// <summary>
        /// Checks if this message is from the assistant.
        /// </summary>
        public bool IsAssistantMessage => GetRole() == ChatRole.Assistant;

        /// <summary>
        /// Checks if this message is a system message.
        /// </summary>
        public bool IsSystemMessage => GetRole() == ChatRole.System;

        /// <summary>
        /// Returns a string representation of the message.
        /// </summary>
        public override string ToString()
        {
            return $"[{role}]: {content}";
        }
    }
}
