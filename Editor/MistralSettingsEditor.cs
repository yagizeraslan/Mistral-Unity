using UnityEditor;
using UnityEngine;

namespace YagizEraslan.Mistral.Unity.Editor
{
    public class MistralSettingsEditor : EditorWindow
    {
        private const string API_KEY_PREF = "MistralApiKey_Dev";
        private string apiKey = "";
        private bool showApiKey = false;

        [MenuItem("Mistral/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<MistralSettingsEditor>("Mistral Settings");
            window.minSize = new Vector2(400, 200);
        }

        private void OnEnable()
        {
            apiKey = EditorPrefs.GetString(API_KEY_PREF, "");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Mistral API Settings", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This API key is stored in EditorPrefs for development only.\n" +
                "For production builds, use the MistralSettings ScriptableObject.",
                MessageType.Info);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("API Key", GUILayout.Width(60));

            if (showApiKey)
            {
                apiKey = EditorGUILayout.TextField(apiKey);
            }
            else
            {
                apiKey = EditorGUILayout.PasswordField(apiKey);
            }

            if (GUILayout.Button(showApiKey ? "Hide" : "Show", GUILayout.Width(50)))
            {
                showApiKey = !showApiKey;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                EditorPrefs.SetString(API_KEY_PREF, apiKey);
                Debug.Log("[Mistral] API Key saved to EditorPrefs");
            }

            if (GUILayout.Button("Clear"))
            {
                apiKey = "";
                EditorPrefs.DeleteKey(API_KEY_PREF);
                Debug.Log("[Mistral] API Key cleared from EditorPrefs");
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            EditorGUILayout.HelpBox(
                "Get your API key from: https://console.mistral.ai/",
                MessageType.None);

            if (GUILayout.Button("Open Mistral Console"))
            {
                Application.OpenURL("https://console.mistral.ai/");
            }
        }
    }
}
