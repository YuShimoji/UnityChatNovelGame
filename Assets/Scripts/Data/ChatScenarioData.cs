using UnityEngine;
using System.Collections.Generic;

namespace ProjectFoundPhone.Data
{
    [CreateAssetMenu(fileName = "NewChatScenario", menuName = "ProjectFoundPhone/ChatScenarioData")]
    public class ChatScenarioData : ScriptableObject
    {
        [Header("Scenario Settings")]
        [Tooltip("List of messages in this scenario sequence.")]
        public List<ChatMessage> Messages = new List<ChatMessage>();
    }

    [System.Serializable]
    public class ChatMessage
    {
        [Tooltip("ID of the character sending the message (e.g., 'player', 'npc1').")]
        public string SenderID;

        [TextArea(3, 10)]
        [Tooltip("The text content of the message.")]
        public string Text;

        [Tooltip("Time in seconds to wait before showing this message (simulates typing).")]
        public float TypingDelay = 1.0f;

        [Tooltip("If this message requires user selection, list the choices here.")]
        public List<ChatChoice> Choices = new List<ChatChoice>();
    }

    [System.Serializable]
    public class ChatChoice
    {
        [Tooltip("Text to display on the choice button.")]
        public string Text;

        [Tooltip("The next scenario to load if this choice is selected. Can be null if the conversation ends.")]
        public ChatScenarioData NextScenario;
    }
}
