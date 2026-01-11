# Changelog

All notable changes to **Mistral AI API for Unity** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.0.0] - 2025-01-11

### Added
- First public release of unofficial Mistral AI API for Unity
- Support for Unity 2020.3, 2021, 2022, 2023, and 6.0+
- UPM (Unity Package Manager) Git installation support
- Support for multiple Mistral models:
  - Mistral Large Latest (flagship model)
  - Mistral Medium Latest (multimodal model)
  - Mistral Small Latest (efficient model)
  - Codestral Latest (code specialist)
  - Ministral 8B/3B (edge models)
  - Open Mistral Nemo
  - Pixtral Large (vision model)
- Native Mistral streaming (stream: true) API support via SSE
- Runtime-safe API Key storage with ScriptableObject
- Modular, reusable UI Chat components
- Sample Scene, prefabs, and demo UI included
- Advanced memory management system:
  - Chat history automatic trimming (50 message limit, configurable)
  - UI GameObject management with automatic cleanup (100 UI element limit)
  - Controller lifecycle optimization to prevent memory leaks
- Editor window for development API key management

---
