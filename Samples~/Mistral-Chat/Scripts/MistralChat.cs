using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YagizEraslan.Mistral.Unity;

/// <summary>
/// Sample MonoBehaviour demonstrating integration with the Mistral API.
/// Uses HistoryTrimmer for DRY-compliant UI message management.
/// </summary>
public class MistralChat : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MistralSettings config;
    [SerializeField] private MistralModel model = MistralModel.Mistral_Small_Latest;
    [SerializeField] private bool useStreaming = true;

    [Header("UI References")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform messageContainer;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Message Prefabs")]
    [SerializeField] private GameObject sentMessagePrefab;
    [SerializeField] private GameObject receivedMessagePrefab;

    [Header("Memory Management")]
    [SerializeField] private int maxUIMessages = ApiConstants.DEFAULT_MAX_UI_MESSAGES;
    [SerializeField] private int trimToMessages = ApiConstants.DEFAULT_UI_TRIM_COUNT;

    private MistralChatController controller;
    private TMP_Text activeStreamingText;
    private readonly List<GameObject> messageGameObjects = new List<GameObject>();

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError($"{ApiConstants.LOG_PREFIX_CHAT} MistralSettings config is not assigned!");
            return;
        }

        controller = new MistralChatController(config);
        controller.OnMessageReceived += HandleMessageReceived;
        controller.OnStreamingUpdate += HandleStreamingUpdate;
        controller.OnError += HandleError;

        sendButton.onClick.AddListener(SendMessage);
        inputField.onSubmit.AddListener(_ => SendMessage());
    }

    private void SendMessage()
    {
        string message = inputField.text.Trim();
        if (string.IsNullOrEmpty(message))
            return;

        inputField.text = "";
        inputField.ActivateInputField();

        controller.SendMessage(message, model, useStreaming);
    }

    private void HandleMessageReceived(ChatMessage message, bool isUser)
    {
        GameObject prefab = isUser ? sentMessagePrefab : receivedMessagePrefab;
        GameObject messageObj = Instantiate(prefab, messageContainer);
        messageGameObjects.Add(messageObj);

        TMP_Text textComponent = messageObj.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = message.content;

            if (!isUser && useStreaming && string.IsNullOrEmpty(message.content))
            {
                activeStreamingText = textComponent;
            }
        }

        TrimUIMessagesIfNeeded();
        ScrollToBottom();
    }

    private void HandleStreamingUpdate(string content)
    {
        if (activeStreamingText != null)
        {
            activeStreamingText.text = content;
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageContainer as RectTransform);
            ScrollToBottom();
        }
    }

    private void HandleError(string error)
    {
        Debug.LogError($"{ApiConstants.LOG_PREFIX_CHAT} {error}");
    }

    private void TrimUIMessagesIfNeeded()
    {
        // Use shared utility for DRY compliance
        HistoryTrimmer.TrimGameObjectsIfNeeded(messageGameObjects, maxUIMessages, trimToMessages);
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// Clears all chat messages and history.
    /// </summary>
    public void ClearChat()
    {
        controller?.ClearHistory();

        foreach (var obj in messageGameObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        messageGameObjects.Clear();
        activeStreamingText = null;
    }

    private void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnMessageReceived -= HandleMessageReceived;
            controller.OnStreamingUpdate -= HandleStreamingUpdate;
            controller.OnError -= HandleError;
            controller.Dispose();
        }

        sendButton?.onClick.RemoveAllListeners();
    }
}
