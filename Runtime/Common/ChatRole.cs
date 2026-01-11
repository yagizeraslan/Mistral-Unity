namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Represents the role of a participant in a chat conversation.
    /// Using an enum instead of magic strings ensures type safety and prevents typos.
    /// </summary>
    public enum ChatRole
    {
        /// <summary>
        /// The user sending messages to the AI.
        /// </summary>
        User,

        /// <summary>
        /// The AI assistant responding to the user.
        /// </summary>
        Assistant,

        /// <summary>
        /// System-level instructions or context.
        /// </summary>
        System
    }

    /// <summary>
    /// Extension methods for ChatRole enum.
    /// </summary>
    public static class ChatRoleExtensions
    {
        /// <summary>
        /// Converts the ChatRole enum to its API-compatible string representation.
        /// </summary>
        /// <param name="role">The chat role to convert.</param>
        /// <returns>The lowercase string representation of the role.</returns>
        public static string ToApiString(this ChatRole role)
        {
            return role switch
            {
                ChatRole.User => "user",
                ChatRole.Assistant => "assistant",
                ChatRole.System => "system",
                _ => "user"
            };
        }

        /// <summary>
        /// Parses a string role to ChatRole enum.
        /// </summary>
        /// <param name="roleString">The string representation of the role.</param>
        /// <returns>The corresponding ChatRole enum value.</returns>
        public static ChatRole FromApiString(string roleString)
        {
            return roleString?.ToLowerInvariant() switch
            {
                "user" => ChatRole.User,
                "assistant" => ChatRole.Assistant,
                "system" => ChatRole.System,
                _ => ChatRole.User
            };
        }
    }
}
