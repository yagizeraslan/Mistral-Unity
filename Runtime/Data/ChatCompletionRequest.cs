using System;

namespace YagizEraslan.Mistral.Unity
{
    [Serializable]
    public class ChatCompletionRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature = 0.7f;
        public int max_tokens = 1000;
        public bool stream;
        public float top_p = 1f;
        public bool safe_prompt;
    }
}
