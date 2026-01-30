using UnityEngine;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// 推論モードで使用するトピック（手がかり）のデータ定義
    /// ScriptableObjectとして作成し、エディタから管理可能にする
    /// </summary>
    [CreateAssetMenu(fileName = "NewTopic", menuName = "Project FoundPhone/Topic Data", order = 1)]
    public class TopicData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string m_TopicID;
        [SerializeField] private string m_Title;
        [TextArea(3, 10)]
        [SerializeField] private string m_Description;
        [SerializeField] private Sprite m_Icon;

        public string TopicID => m_TopicID;
        public string Title => m_Title;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
    }
}
