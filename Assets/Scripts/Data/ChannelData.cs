using UnityEngine;

namespace ProjectFoundPhone.Data
{
    public enum ChannelStatus
    {
        Locked,
        Available,
        InProgress,
        Completed
    }

    [CreateAssetMenu(fileName = "NewChannelData", menuName = "ProjectFoundPhone/Channel Data")]
    public class ChannelData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Internal channel ID (e.g. 'ch1', 'ch2')")]
        [SerializeField] private string m_ChannelID;

        [Tooltip("Display name shown on the card")]
        [SerializeField] private string m_DisplayName;

        [Tooltip("Brief description (1-2 lines)")]
        [TextArea(2, 4)]
        [SerializeField] private string m_Description;

        [Header("Yarn Integration")]
        [Tooltip("Yarn start node name for this chapter (e.g. 'Ch1_Opening')")]
        [SerializeField] private string m_StartNodeName;

        [Tooltip("Chapter number for sorting (1, 2, 3...)")]
        [SerializeField] private int m_ChapterNumber;

        [Header("Unlock Condition")]
        [Tooltip("Channel ID that must be completed to unlock this channel. Empty = always available.")]
        [SerializeField] private string m_RequiredCompletedChannelID;

        public string ChannelID => m_ChannelID;
        public string DisplayName => m_DisplayName;
        public string Description => m_Description;
        public string StartNodeName => m_StartNodeName;
        public int ChapterNumber => m_ChapterNumber;
        public string RequiredCompletedChannelID => m_RequiredCompletedChannelID;
    }
}
