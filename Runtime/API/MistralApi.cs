using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Mistral.Unity
{
    public class MistralApi : IMistralApi
    {
        private const string API_URL = "https://api.mistral.ai/v1/chat/completions";
        private readonly MistralSettings settings;

        public string ApiKey => settings.apiKey;

        public MistralApi(MistralSettings config)
        {
            settings = config;
        }

        public async Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request)
        {
            string jsonBody = JsonUtility.ToJson(request);

            using (UnityWebRequest www = new UnityWebRequest(API_URL, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();

                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("Authorization", $"Bearer {settings.apiKey}");

                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[MistralApi] Error: {www.error}");
                    Debug.LogError($"[MistralApi] Response: {www.downloadHandler.text}");
                    return null;
                }

                string responseJson = www.downloadHandler.text;
                ChatCompletionResponse response = JsonUtility.FromJson<ChatCompletionResponse>(responseJson);
                return response;
            }
        }
    }
}
