using System.Threading.Tasks;

namespace YagizEraslan.Mistral.Unity
{
    public interface IMistralApi
    {
        Task<ChatCompletionResponse> CreateChatCompletion(ChatCompletionRequest request);
    }
}
