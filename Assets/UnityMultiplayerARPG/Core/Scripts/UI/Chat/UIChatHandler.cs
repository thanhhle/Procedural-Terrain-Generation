﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIChatHandler : UIBase
    {
        public readonly List<ChatMessage> ChatMessages = new List<ChatMessage>();
        public readonly Dictionary<ChatChannel, List<ChatMessage>> ChannelBasedChatMessages = new Dictionary<ChatChannel, List<ChatMessage>>();

        [Header("Configs")]
        [Tooltip("Message channel to send without channel commands, if `showingMessagesFromAllChannels` is `FALSE` it will show messages from this channel only")]
        public ChatChannel chatChannel = ChatChannel.Local;
        public bool showingMessagesFromAllChannels = true;
        [Tooltip("When enter this key it will show enter chat message field")]
        public KeyCode enterChatKey = KeyCode.Return;
        [Range(0, UIChatHistory.MAX_CHAT_HISTORY)]
        public int chatEntrySize = 30;

        [Header("Channel Commands")]
        public string globalCommand = "/a";
        public string whisperCommand = "/w";
        public string partyCommand = "/p";
        public string guildCommand = "/g";
        public string systemCommand = "/s";

        [Header("UIs")]
        [Tooltip("These game objects will be activated while entering chat message")]
        public GameObject[] enterChatActiveObjects;
        public InputFieldWrapper uiReceiverField;
        [FormerlySerializedAs("uiEnterChatField")]
        public InputFieldWrapper uiMessageField;
        [FormerlySerializedAs("uiChatMessagePrefab")]
        public UIChatMessage uiPrefab;
        [FormerlySerializedAs("uiChatMessageContainer")]
        public Transform uiContainer;
        public ScrollRect scrollRect;

        public bool EnterChatFieldVisible { get; private set; }

        public string EnterChatReceiver
        {
            get { return uiReceiverField == null ? string.Empty : uiReceiverField.text; }
            set { if (uiReceiverField != null) uiReceiverField.text = value; }
        }

        public string EnterChatMessage
        {
            get { return uiMessageField == null ? string.Empty : uiMessageField.text; }
            set { if (uiMessageField != null) uiMessageField.text = value; }
        }

        private UIList cacheList;
        public UIList CacheList
        {
            get
            {
                if (cacheList == null)
                {
                    cacheList = gameObject.AddComponent<UIList>();
                    cacheList.uiPrefab = uiPrefab.gameObject;
                    cacheList.uiContainer = uiContainer;
                }
                return cacheList;
            }
        }

        private bool movingToEnd;
        private ChatChannel dirtyChatChannel;

        protected override void Awake()
        {
            base.Awake();
            if (ChatMessages.Count == 0)
            {
                // No received chat messages, try fill it from global chat messages
                int index = UIChatHistory.ChatMessages.Count - chatEntrySize;
                if (index < 0)
                    index = 0;
                while (index < UIChatHistory.ChatMessages.Count)
                {
                    OnReceiveChat(UIChatHistory.ChatMessages[index]);
                    index++;
                }
            }
            SetOnClientReceiveChatMessage();
        }

        private void Start()
        {
            ChatMessages.Clear();
            ChannelBasedChatMessages.Clear();
            StartCoroutine(VerticalScroll(0f));

            HideEnterChatField();
            if (uiMessageField != null)
            {
                uiMessageField.onValueChanged.RemoveListener(OnMessageFieldValueChange);
                uiMessageField.onValueChanged.AddListener(OnMessageFieldValueChange);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(VerticalScroll(0f));
        }

        private void OnDestroy()
        {
            RemoveOnClientReceiveChatMessage();
        }

        public void SetOnClientReceiveChatMessage()
        {
            RemoveOnClientReceiveChatMessage();
            ClientGenericActions.onClientReceiveChatMessage += OnReceiveChat;
        }

        public void RemoveOnClientReceiveChatMessage()
        {
            ClientGenericActions.onClientReceiveChatMessage -= OnReceiveChat;
        }

        private void Update()
        {
            if (dirtyChatChannel != chatChannel)
            {
                dirtyChatChannel = chatChannel;
                if (!showingMessagesFromAllChannels)
                    FillChatMessages();
            }
            if (movingToEnd)
            {
                movingToEnd = false;
                uiMessageField.MoveTextEnd(false);
            }
            if (Input.GetKeyUp(enterChatKey))
            {
                if (!EnterChatFieldVisible)
                    ShowEnterChatField();
                else
                    SendChatMessage();
            }
        }

        public void ToggleEnterChatField()
        {
            if (EnterChatFieldVisible)
                HideEnterChatField();
            else
                ShowEnterChatField();
        }

        public void ShowEnterChatField()
        {
            foreach (GameObject enterChatActiveObject in enterChatActiveObjects)
            {
                if (enterChatActiveObject != null)
                    enterChatActiveObject.SetActive(true);
            }
            if (uiMessageField != null)
            {
                uiMessageField.ActivateInputField();
                EventSystem.current.SetSelectedGameObject(uiMessageField.gameObject);
                movingToEnd = true;
            }
            EnterChatFieldVisible = true;
        }

        public void HideEnterChatField()
        {
            foreach (GameObject enterChatActiveObject in enterChatActiveObjects)
            {
                if (enterChatActiveObject != null)
                    enterChatActiveObject.SetActive(false);
            }
            if (uiMessageField != null)
            {
                uiMessageField.DeactivateInputField();
                EventSystem.current.SetSelectedGameObject(null);
            }
            EnterChatFieldVisible = false;
        }

        public void SendChatMessage()
        {
            if (GameInstance.PlayingCharacter == null)
                return;

            string trimText = EnterChatMessage.Trim();
            if (trimText.Length == 0)
            {
                HideEnterChatField();
                return;
            }

            EnterChatMessage = string.Empty;
            ChatChannel channel = chatChannel;
            string message = trimText;
            string sender = GameInstance.PlayingCharacter.CharacterName;
            string receiver = string.Empty;
            // Set chat channel by command
            string[] splitedText = trimText.Split(' ');
            string cmd = splitedText[0];
            if (cmd.Equals(whisperCommand) &&
                splitedText.Length > 2)
            {
                channel = ChatChannel.Whisper;
                receiver = splitedText[1];
                message = trimText.Substring(cmd.Length + receiver.Length + 2); // +2 for space
                EnterChatMessage = trimText.Substring(0, cmd.Length + receiver.Length + 2); // +2 for space
            }
            if ((cmd.Equals(globalCommand) ||
                cmd.Equals(partyCommand) ||
                cmd.Equals(guildCommand) ||
                cmd.Equals(systemCommand))
                && splitedText.Length > 1)
            {
                if (cmd.Equals(globalCommand))
                    channel = ChatChannel.Global;
                if (cmd.Equals(partyCommand))
                    channel = ChatChannel.Party;
                if (cmd.Equals(guildCommand))
                    channel = ChatChannel.Guild;
                if (cmd.Equals(systemCommand))
                    channel = ChatChannel.System;
                message = trimText.Substring(cmd.Length + 1); // +1 for space
                EnterChatMessage = trimText.Substring(0, cmd.Length + 1); // +1 for space
            }
            if (channel == ChatChannel.Whisper && uiReceiverField)
            {
                receiver = EnterChatReceiver;
            }
            GameInstance.ClientChatHandlers.SendChatMessage(new ChatMessage()
            {
                channel = channel,
                message = message,
                sender = sender,
                receiver = receiver,
            });
            HideEnterChatField();
        }

        private void OnReceiveChat(ChatMessage chatMessage)
        {
            if (this == null)
            {
                RemoveOnClientReceiveChatMessage();
                return;
            }

            if (!ChannelBasedChatMessages.ContainsKey(chatMessage.channel))
                ChannelBasedChatMessages.Add(chatMessage.channel, new List<ChatMessage>());
            ChannelBasedChatMessages[chatMessage.channel].Add(chatMessage);
            if (ChannelBasedChatMessages[chatMessage.channel].Count > chatEntrySize)
                ChannelBasedChatMessages[chatMessage.channel].RemoveAt(0);

            ChatMessages.Add(chatMessage);
            if (ChatMessages.Count > chatEntrySize)
                ChatMessages.RemoveAt(0);

            FillChatMessages();
        }

        private void FillChatMessages()
        {
            if (showingMessagesFromAllChannels)
            {
                UIChatMessage tempUiChatMessage;
                CacheList.Generate(ChatMessages, (index, message, ui) =>
                {
                    tempUiChatMessage = ui.GetComponent<UIChatMessage>();
                    tempUiChatMessage.uiChatHandler = this;
                    tempUiChatMessage.Data = message;
                    tempUiChatMessage.Show();
                });
            }
            else if (ChannelBasedChatMessages.ContainsKey(chatChannel))
            {
                UIChatMessage tempUiChatMessage;
                CacheList.Generate(ChannelBasedChatMessages[chatChannel], (index, message, ui) =>
                {
                    tempUiChatMessage = ui.GetComponent<UIChatMessage>();
                    tempUiChatMessage.uiChatHandler = this;
                    tempUiChatMessage.Data = message;
                    tempUiChatMessage.Show();
                });
            }
            if (gameObject.activeInHierarchy)
                StartCoroutine(VerticalScroll(0f));
        }

        private void OnMessageFieldValueChange(string text)
        {
            if (text.Length > 0 && !EnterChatFieldVisible)
                ShowEnterChatField();
        }

        IEnumerator VerticalScroll(float normalize)
        {
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                yield return null;
                scrollRect.verticalScrollbar.value = normalize;
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}
