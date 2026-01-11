using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace YagizEraslan.Mistral.Unity
{
    public static class UnityWebRequestAwaiter
    {
        public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            asyncOp.completed += _ => tcs.TrySetResult(asyncOp.webRequest);
            return tcs.Task.GetAwaiter();
        }
    }
}
