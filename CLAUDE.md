# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Mistral-Unity is a Unity Package Manager (UPM) library that integrates Mistral AI's API into Unity projects. It provides both streaming and non-streaming chat completion functionality with a clean, modular architecture and advanced memory management system.

## Development Commands

### Unity Package Development
This project is structured as a Unity package, not a traditional Unity project:

- **Package Testing**: Import into Unity Editor via Package Manager → Add package from Git URL
- **Sample Scene**: Import samples via Package Manager → Samples → Import "Mistral Chat Example"
- **Editor Tools**: Access via Unity menu `Mistral/Settings` for API key management

### Build Commands
No traditional build commands - Unity packages are consumed by Unity projects that handle building.

## Architecture Overview

### Core API Layer (`Runtime/API/`)
- **`IMistralApi`**: Contract for API implementations
- **`MistralApi`**: Standard async/await implementation using UnityWebRequest
- **`MistralStreamingApi`**: Server-Sent Events (SSE) streaming implementation with custom `DownloadHandlerScript`
- **`MistralModel`**: Enum/extension for model selection (mistral-large-latest, mistral-small-latest, etc.)

**Key Architecture Decision**: Two separate API classes rather than unified interface - `MistralApi` for full responses, `MistralStreamingApi` for real-time streaming with different callback patterns.

### Data Layer (`Runtime/Data/`)
- **`ChatCompletionRequest`**: API request DTO (model, messages, temperature, stream flag)
- **`ChatCompletionResponse`**: Standard API response DTO (OpenAI-compatible format with choices[])
- **`ChatMessage`**: Message structure (role, content)

**Note**: Uses OpenAI-compatible response format with `choices[0].message.content`.

### UI Controller (`Runtime/UI/`)
- **`MistralChatController`**: Business logic mediator between API and UI
- Manages conversation history as `List<ChatMessage>`
- Handles streaming state with `StringBuilder` accumulation
- Provides error handling with user-friendly error messages

### Configuration (`Runtime/Common/`)
- **`MistralSettings`**: ScriptableObject for API key storage and memory management settings
- **Security Note**: API key stored as plain text in ScriptableObject - visible in builds

### Editor Tools (`Editor/`)
- **`MistralSettingsEditor`**: EditorWindow for development API key management via EditorPrefs
- Accessible via `Mistral/Settings` menu

### Sample Implementation (`Samples~/Mistral-Chat/`)
- **`MistralChat`**: MonoBehaviour demonstrating complete integration
- Instantiates message prefabs, handles UI updates, manages streaming text updates
- Uses single persistent `MistralChatController` instance with advanced memory management

## Key Implementation Patterns

### Async/Await with Unity (`Runtime/Utilities/`)
- **`UnityWebRequestAwaiter`**: Extension method to make UnityWebRequest awaitable without external dependencies
```csharp
await www.SendWebRequest();
```

### Streaming Architecture
- Custom `StreamingDownloadHandler : DownloadHandlerScript`
- Parses SSE data chunks in `ReceiveData()` method
- Handles incomplete lines with StringBuilder buffer
- Graceful JSON parsing with warnings (not errors) for partial chunks

### Memory Management
- `TrimHistoryIfNeeded()` automatically limits conversation history (configurable in MistralSettings)
- `TrimUIMessagesIfNeeded()` prevents UI GameObject bloat
- `ClearHistory()` method for manual cleanup

## API Endpoint Details

- **URL**: `https://api.mistral.ai/v1/chat/completions`
- **Auth Header**: `Authorization: Bearer {api_key}`
- **Response Format**: OpenAI-compatible with `choices[0].message.content`
- **Streaming**: SSE with `choices[0].delta.content` and `[DONE]` termination

## Platform Limitations

- **WebGL**: Streaming may not work due to browser limitations with DownloadHandlerScript
- **iOS/Android**: HTTPS required for API calls
- All platforms: Requires internet permission in build settings

## Git Commit Instructions
- Do NOT add Claude attribution in commit messages
- Do NOT include "Co-Authored-By: Claude" lines in commits
- Use standard commit messages without co-author tags
- Only commit when explicitly requested by user
- Keep commit messages clean and professional without AI attribution

## Dependencies
- Unity 2020.3 LTS minimum
- TextMeshPro (com.unity.textmeshpro: 3.0.6)
- Mistral API key from console.mistral.ai
