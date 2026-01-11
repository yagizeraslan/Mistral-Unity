<p align="center">
  <!-- Top small clean badges -->
  <a href="https://unity.com/releases/editor/whats-new/2020.3.0">
    <img alt="Unity 2020.3+" src="https://img.shields.io/badge/Unity-2020.3%2B-green?logo=unity&logoColor=white">
  </a>
  <img alt="UPM Compatible" src="https://img.shields.io/badge/UPM-Compatible-brightgreen">
  <a href="https://opensource.org/licenses/MIT">
    <img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-blue.svg">
  </a>
  <a href="https://github.com/sponsors/yagizeraslan">
    <img alt="Sponsor" src="https://img.shields.io/badge/Sponsor-❤-ff69b4">
  </a>
  <a href="https://github.com/yagizeraslan/Mistral-Unity/commits/main">
    <img alt="Last Commit" src="https://img.shields.io/github/last-commit/yagizeraslan/Mistral-Unity">
  </a>
  <a href="https://github.com/yagizeraslan/Mistral-Unity">
    <img alt="Code Size" src="https://img.shields.io/github/languages/code-size/yagizeraslan/Mistral-Unity">
  </a>
  <br/>

  <!-- Bottom bigger social + platform badges -->
  <img alt="Maintenance" src="https://img.shields.io/badge/Maintained-Yes-brightgreen">
  <a href="https://github.com/yagizeraslan/Mistral-Unity/stargazers">
    <img alt="GitHub stars" src="https://img.shields.io/github/stars/yagizeraslan/Mistral-Unity?style=social">
  </a>
  <a href="https://github.com/yagizeraslan/Mistral-Unity/network/members">
    <img alt="GitHub forks" src="https://img.shields.io/github/forks/yagizeraslan/Mistral-Unity?style=social">
  </a>
  <a href="https://github.com/yagizeraslan/Mistral-Unity/issues">
    <img alt="GitHub issues" src="https://img.shields.io/github/issues/yagizeraslan/Mistral-Unity?style=social">
  </a>
  <img alt="Windows" src="https://img.shields.io/badge/Platform-Windows-blue">
  <img alt="WebGL" src="https://img.shields.io/badge/Platform-WebGL-orange">
  <img alt="Android" src="https://img.shields.io/badge/Platform-Android-green">
</p>

# Mistral AI API for Unity

> A clean, modular Unity integration for Mistral's powerful LLMs — chat, reasoning, and task automation made easy.

**Note**: This is an unofficial integration not affiliated with or endorsed by Mistral AI.

---

## Features

- Clean, reusable SDK for Mistral API
- Supports true SSE-based streaming and non-streaming chat completions
- Robust error handling and automatic resource management
- Compatible with multiple models (Mistral Large, Medium, Small, Codestral, and more)
- Modular & customizable UI chat component
- Secure API key storage (runtime-safe)
- Built with Unity Package Manager (UPM)
- Includes sample scene & prefabs
- Advanced memory management with automatic cleanup

---

### Supported Platforms & Unity Versions

| Platform | Unity 2020.3 | Unity 2021 | Unity 2022 | Unity 6 | Notes |
| --- | --- | --- | --- | --- | --- |
| **Windows** | Yes | Yes | Yes | Yes | Fully supported (tested with IL2CPP & Mono) |
| **Android** | Yes | Yes | Yes | Yes | Requires internet permission in manifest |
| **WebGL** | Partial | Partial | Yes | Yes | Streaming unsupported; add CORS headers on server |
| **Linux** | ? | ? | ? | ? | Likely works, but not yet tested |
| **macOS** | ? | ? | ? | ? | Not tested, expected to work |
| **iOS** | ? | ? | ? | ? | Not tested, expected to work (HTTPS required) |
| **Consoles** | No | No | No | No | Not supported (Unity license + network limitations) |

---

## Requirements

- Unity 2020.3 LTS or newer
- TextMeshPro (via Package Manager)
- Mistral API Key from [console.mistral.ai](https://console.mistral.ai/)

---

## Installation

### Option 1: Via Git URL (Unity Package Manager)

1. Open your Unity project
2. Go to **Window > Package Manager**
3. Click `+` → **Add package from Git URL**
4. Paste:

    ```
    https://github.com/yagizeraslan/Mistral-Unity.git
    ```

5. Done

---

## Getting Started

### Setup

1. After installation, download Sample scene from Package Manager
2. Paste your **API key** into the MistralSettings.asset
3. Hit Play — and chat with Mistral AI in seconds

---

## Sample Scene

To test everything:

1. In **Package Manager**, under **Mistral AI API for Unity**, click **Samples**
2. Click **Import** on `Mistral Chat Example`
3. Open:

    ```
    Assets/Samples/Mistral AI API for Unity/1.0.0/Mistral Chat Example/Scenes/Mistral-Chat.unity
    ```

4. Press Play — you're live.

- You can change model type and streaming mode during play — the SDK picks up changes automatically for each new message.
- You can also press **Enter** instead of clicking Send button — handy for fast testing.

---

## API Key Handling

- During dev: Store key via `EditorPrefs` using the Mistral Editor Window
- In production builds: Use the `MistralSettings` ScriptableObject (recommended)

**DO NOT** hardcode your key in scripts or prefabs — Unity will reject the package.

---

## Architecture Overview

| Layer | Folder | Role |
| --- | --- | --- |
| API Logic | `Runtime/API/` | HTTP & model logic |
| Data Models | `Runtime/Data/` | DTOs for requests/responses |
| UI Component | `Runtime/UI/` | MonoBehaviour & Controller |
| Config Logic | `Runtime/Common/` | Secure key storage |
| Editor Tools | `Editor/` | Editor-only settings UI |
| Example Scene | `Samples~/` | Demo prefab, scene, assets |

---

## Example Integration

### Non-Streaming (Full Response)

```csharp
[SerializeField] private MistralSettings config;

private async void Start()
{
    var api = new MistralApi(config);
    var request = new ChatCompletionRequest
    {
        model = MistralModel.Mistral_Small_Latest.ToModelString(),
        messages = new ChatMessage[]
        {
            new ChatMessage { role = "system", content = "You're a helpful assistant." },
            new ChatMessage { role = "user", content = "Tell me something cool." }
        }
    };

    var response = await api.CreateChatCompletion(request);
    Debug.Log("[FULL RESPONSE] " + response.choices[0].message.content);
}
```

### Streaming (Real-Time Updates)

```csharp
[SerializeField] private MistralSettings config;

private void Start()
{
    var request = new ChatCompletionRequest
    {
        model = MistralModel.Mistral_Small_Latest.ToModelString(),
        messages = new ChatMessage[]
        {
            new ChatMessage { role = "user", content = "Stream a fun fact about space." }
        },
        stream = true
    };

    var streamingApi = new MistralStreamingApi();
    streamingApi.CreateChatCompletionStream(
        request,
        config.apiKey,
        token => Debug.Log("[STREAMING] " + token)
    );
}
```

---

## Advanced Usage

### Streaming Support

Mistral-Unity supports **reliable real-time streaming** using Mistral's official `stream: true` Server-Sent Events (SSE) endpoint.

- Uses Unity's `DownloadHandlerScript` for chunked response handling
- UI updates per-token (no simulated typewriter effect)
- Automatic resource cleanup and memory management
- Built-in error handling with user-friendly messages
- No coroutines, no external libraries — works natively in Unity
- Smart memory limits prevent unbounded growth in long conversations

To enable:
- Check `Use Streaming` in the chat prefab or component
- Partial responses will automatically stream into the UI

You can toggle streaming on/off at runtime.

### Memory Management

Mistral-Unity includes intelligent memory management to prevent memory bloat during long conversations:

**Chat History Limits:**
- Automatically caps conversation history at **50 messages** (configurable)
- Trims to **30 messages** when limit is reached, preserving recent context
- Manual cleanup available via `controller.ClearHistory()`

**UI GameObject Management:**
- Limits message GameObjects to **100 instances** (configurable in Inspector)
- Automatically removes oldest UI elements when limit exceeded
- Prevents UI hierarchy bloat and maintains performance

**Controller Lifecycle:**
- Single controller instance reused throughout chat session
- Prevents memory leaks from abandoned controller instances
- Proper cleanup on component destruction

```csharp
// Access memory management features
public class CustomChat : MonoBehaviour
{
    private MistralChatController controller;

    void SomeMethod()
    {
        // Check current history size
        Debug.Log($"History count: {controller.GetHistoryCount()}");

        // Manual cleanup if needed
        controller.ClearHistory();
    }
}
```

### Supported Models

```csharp
MistralModel.Mistral_Large_Latest   // State-of-the-art flagship model
MistralModel.Mistral_Medium_Latest  // Frontier multimodal model
MistralModel.Mistral_Small_Latest   // Efficient small model
MistralModel.Codestral_Latest       // Code completion specialist
MistralModel.Ministral_8B_Latest    // Edge-efficient 8B model
MistralModel.Ministral_3B_Latest    // Tiny 3B edge model
MistralModel.Open_Mistral_Nemo      // Open-weight Nemo model
MistralModel.Pixtral_Large_Latest   // Vision-enabled large model
```

---

## Troubleshooting

**Can't add component?**

Make sure you dragged `MistralSettings.asset` into the MistralChat.cs's Config field.

**Streaming not working?**

Make sure you're on a platform that supports `DownloadHandlerScript` (Standalone or Editor).
WebGL and iOS may have platform limitations for live SSE streams.

**Getting error messages in the chat?**

Error messages now display directly in the chat interface for better debugging.
Check Unity Console for detailed technical error information.

**Seeing JSON parse warnings in streaming mode?**

These are normal during SSE — they occur when the parser receives partial chunks. They're automatically skipped and won't affect the final output.

---

## Support This Project

If you find **Mistral-Unity** useful, please consider supporting its development!

- [Become a sponsor on GitHub Sponsors](https://github.com/sponsors/yagizeraslan)
- [Buy me a coffee on Ko-fi](https://ko-fi.com/yagizeraslan)

Your support helps me continue maintaining and improving this project. Thank you!

---

## License

Unofficial integration. Mistral is a trademark of Mistral AI.

This project is licensed under the MIT License.

---

## Contact & Support

**Author**: [Yagiz ERASLAN](https://www.linkedin.com/in/yagizeraslan/)

Email: yagizeraslan@gmail.com

GitHub Issues welcome!
