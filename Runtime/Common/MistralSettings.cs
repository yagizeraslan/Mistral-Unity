using UnityEngine;

namespace YagizEraslan.Mistral.Unity
{
    [CreateAssetMenu(fileName = "MistralSettings", menuName = "Mistral/Settings", order = 1)]
    public class MistralSettings : ScriptableObject
    {
        [Tooltip("Your Mistral API Key (used at runtime)")]
        public string apiKey;

        [Tooltip("Maximum number of messages to keep in history (0 = unlimited)")]
        public int maxHistoryMessages = 50;

        [Tooltip("Number of messages to keep when trimming history")]
        public int historyTrimCount = 30;
    }
}
