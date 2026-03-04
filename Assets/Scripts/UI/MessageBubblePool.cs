using UnityEngine;
using System.Collections.Generic;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// MessageBubble のオブジェクトプール
    /// Instantiate/Destroy の GC 負荷を削減する
    /// </summary>
    public class MessageBubblePool : MonoBehaviour
    {
        [SerializeField] private GameObject m_MessageBubblePrefab;
        [SerializeField] private int m_InitialCapacity = 10;
        [SerializeField] private int m_MaxCapacity = 50;

        private Queue<GameObject> m_Pool = new Queue<GameObject>();
        private List<GameObject> m_ActiveObjects = new List<GameObject>();
        private Transform m_PoolContainer;

        public int ActiveCount => m_ActiveObjects.Count;
        public int PooledCount => m_Pool.Count;
        public int TotalCount => m_ActiveObjects.Count + m_Pool.Count;

        /// <summary>
        /// プールに使用する Prefab を設定
        /// </summary>
        public void SetPrefab(GameObject prefab)
        {
            if (prefab == null) return;
            m_MessageBubblePrefab = prefab;
        }

        private void Awake()
        {
            InitializePool();
        }

        /// <summary>
        /// プールの初期化
        /// </summary>
        private void InitializePool()
        {
            if (m_PoolContainer == null)
            {
                m_PoolContainer = new GameObject("MessageBubblePoolContainer").transform;
                m_PoolContainer.SetParent(transform);
                m_PoolContainer.gameObject.SetActive(false);
            }

            if (m_MessageBubblePrefab == null)
            {
                Debug.LogWarning("MessageBubblePool: Prefab is not assigned.");
                return;
            }

            // 初期容量分を事前生成
            for (int i = 0; i < m_InitialCapacity; i++)
            {
                GameObject obj = CreateNewPooledObject();
                m_Pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// プールからオブジェクトを取得
        /// </summary>
        public GameObject Get(Transform parent)
        {
            GameObject obj;

            if (m_Pool.Count > 0)
            {
                obj = m_Pool.Dequeue();
            }
            else if (TotalCount < m_MaxCapacity)
            {
                obj = CreateNewPooledObject();
            }
            else
            {
                // 最大容量到達時は最も古いアクティブオブジェクトを再利用
                if (m_ActiveObjects.Count > 0)
                {
                    obj = m_ActiveObjects[0];
                    m_ActiveObjects.RemoveAt(0);
                }
                else
                {
                    Debug.LogWarning("MessageBubblePool: Pool exhausted and no active objects to recycle.");
                    return null;
                }
            }

            if (obj == null)
            {
                Debug.LogWarning("MessageBubblePool: Failed to create pooled object. Prefab may not be assigned.");
                return null;
            }

            obj.transform.SetParent(parent, false);
            obj.SetActive(true);
            m_ActiveObjects.Add(obj);

            return obj;
        }

        /// <summary>
        /// オブジェクトをプールに返却
        /// </summary>
        public void Return(GameObject obj)
        {
            if (obj == null) return;

            m_ActiveObjects.Remove(obj);

            obj.SetActive(false);
            obj.transform.SetParent(m_PoolContainer, false);

            // 位置と回転をリセット
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;
            }

            // TextMeshPro のテキストをクリアし、表示状態をリセット
            var textComponent = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                // タイプライター tween が残っていればキル
                DG.Tweening.DOTween.Kill(textComponent, complete: false);
                textComponent.text = string.Empty;
                textComponent.maxVisibleCharacters = int.MaxValue;
            }

            // MessageBubble の状態をリセット（矛盾指摘用データ）
            var messageBubble = obj.GetComponent<MessageBubble>();
            if (messageBubble != null)
            {
                messageBubble.ResetState();
            }

            // Image の色をリセット（ResetState 後に実行。StopHighlight による上書きを防止）
            var bubbleImage = obj.GetComponent<UnityEngine.UI.Image>();
            if (bubbleImage != null)
            {
                bubbleImage.color = Color.white;
            }

            m_Pool.Enqueue(obj);
        }

        /// <summary>
        /// 全てのアクティブオブジェクトを返却
        /// </summary>
        public void ReturnAll()
        {
            // リストをコピーしてから処理（コレクション変更防止）
            var activeCopy = new List<GameObject>(m_ActiveObjects);
            foreach (var obj in activeCopy)
            {
                Return(obj);
            }
        }

        /// <summary>
        /// プールをクリア（シーン切り替え時など）
        /// </summary>
        public void Clear()
        {
            ReturnAll();

            while (m_Pool.Count > 0)
            {
                GameObject obj = m_Pool.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }

        /// <summary>
        /// 新しいプールオブジェクトを生成
        /// </summary>
        private GameObject CreateNewPooledObject()
        {
            if (m_MessageBubblePrefab == null) return null;

            GameObject obj = Instantiate(m_MessageBubblePrefab, m_PoolContainer);
            obj.SetActive(false);
            return obj;
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
