using UnityEngine;
using System.Collections.Generic;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.Core; // ScenarioManagerのために追加

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 推論ボード（DeductionBoard）のUIマネージャー
    /// プレイヤーが獲得したトピックを管理・表示する
    /// </summary>
    public class DeductionBoard : MonoBehaviour
    {
        #region Singleton
        private static DeductionBoard s_Instance;

        /// <summary>
        /// DeductionBoardのシングルトンインスタンス
        /// </summary>
        public static DeductionBoard Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<DeductionBoard>();
                    if (s_Instance == null)
                    {
                        Debug.LogWarning("DeductionBoard: Instance not found in scene.");
                    }
                }
                return s_Instance;
            }
        }
        #endregion

        #region Private Fields
        [Header("UI References")]
        [SerializeField] private Transform m_CardContainer;
        [SerializeField] private TopicCard m_TopicCardPrefab;

        [Header("Settings")]
        [SerializeField] private bool m_ShowOnTopicAdded = true;
        [SerializeField] private string m_RecipeLoadPath = "Recipes"; // Resources/Recipes

        /// <summary>
        /// 獲得済みトピックのリスト
        /// </summary>
        private List<TopicData> m_UnlockedTopics = new List<TopicData>();

        /// <summary>
        /// 生成されたTopicCardのリスト
        /// </summary>
        private List<TopicCard> m_TopicCards = new List<TopicCard>();

        /// <summary>
        /// ロードされた合成レシピのリスト
        /// </summary>
        private List<SynthesisRecipe> m_Recipes = new List<SynthesisRecipe>();
        #endregion

        #region Public Properties
        /// <summary>
        /// 獲得済みトピックのリスト（読み取り専用）
        /// </summary>
        public IReadOnlyList<TopicData> UnlockedTopics => m_UnlockedTopics;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // シングルトンの初期化
            if (s_Instance != null && s_Instance != this)
            {
                Debug.LogWarning("DeductionBoard: Duplicate instance found. Destroying this instance.");
                Destroy(gameObject);
                return;
            }
            s_Instance = this;

            LoadRecipes();
        }

        private void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// トピックを推論ボードに追加する
        /// </summary>
        /// <param name="topicData">追加するトピックデータ</param>
        /// <returns>追加に成功した場合true、既に存在する場合false</returns>
        public bool AddTopic(TopicData topicData)
        {
            if (topicData == null)
            {
                Debug.LogWarning("DeductionBoard: Attempted to add null TopicData.");
                return false;
            }

            // 重複チェック
            if (HasTopic(topicData.TopicID))
            {
                Debug.Log($"DeductionBoard: Topic '{topicData.Title}' already exists.");
                return false;
            }

            // トピックをリストに追加
            m_UnlockedTopics.Add(topicData);

            // カードを生成
            CreateTopicCard(topicData);

            Debug.Log($"DeductionBoard: Topic added - {topicData.Title} (ID: {topicData.TopicID})");

            if (m_ShowOnTopicAdded)
            {
                Show();
            }

            return true;
        }

        /// <summary>
        /// 推論ボードを表示する
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 推論ボードを非表示にする
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// トピックを推論ボードから削除する
        /// </summary>
        /// <param name="topicID">削除するトピックのID</param>
        /// <returns>削除に成功した場合true</returns>
        public bool RemoveTopic(string topicID)
        {
            TopicData topicToRemove = m_UnlockedTopics.Find(t => t.TopicID == topicID);
            if (topicToRemove == null)
            {
                Debug.LogWarning($"DeductionBoard: Topic with ID '{topicID}' not found.");
                return false;
            }

            m_UnlockedTopics.Remove(topicToRemove);

            // 対応するカードを削除
            TopicCard cardToRemove = m_TopicCards.Find(c => c.TopicData != null && c.TopicData.TopicID == topicID);
            if (cardToRemove != null)
            {
                m_TopicCards.Remove(cardToRemove);
                Destroy(cardToRemove.gameObject);
            }

            Debug.Log($"DeductionBoard: Topic removed - {topicToRemove.Title} (ID: {topicID})");
            return true;
        }

        /// <summary>
        /// 指定したIDのトピックが既に獲得済みかどうかを確認する
        /// </summary>
        /// <param name="topicID">確認するトピックのID</param>
        /// <returns>獲得済みの場合true</returns>
        public bool HasTopic(string topicID)
        {
            return m_UnlockedTopics.Exists(t => t.TopicID == topicID);
        }

        /// <summary>
        /// 全てのトピックをクリアする
        /// </summary>
        public void ClearAllTopics()
        {
            // 全てのカードを削除
            foreach (var card in m_TopicCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            m_TopicCards.Clear();
            m_UnlockedTopics.Clear();

            Debug.Log("DeductionBoard: All topics cleared.");
        }

        /// <summary>
        /// トピックドラッグ＆ドロップ時の処理
        /// </summary>
        /// <param name="droppedCard">ドラッグされたカード</param>
        /// <param name="targetCard">ドロップ先のカード</param>
        /// <returns>処理が成功（合成成功など）した場合はtrue</returns>
        public bool OnTopicDropped(TopicCard droppedCard, TopicCard targetCard)
        {
            if (droppedCard == null || targetCard == null) return false;
            if (droppedCard == targetCard) return false;

            return CheckSynthesis(droppedCard.TopicData, targetCard.TopicData);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Resourcesから合成レシピをロードする
        /// </summary>
        private void LoadRecipes()
        {
            m_Recipes.Clear();
            var loadedRecipes = Resources.LoadAll<SynthesisRecipe>(m_RecipeLoadPath);
            if (loadedRecipes != null)
            {
                m_Recipes.AddRange(loadedRecipes);
                Debug.Log($"DeductionBoard: Loaded {m_Recipes.Count} synthesis recipes.");
            }
        }

        /// <summary>
        /// TopicCardを生成してコンテナに追加する
        /// </summary>
        /// <param name="topicData">カードに設定するトピックデータ</param>
        private void CreateTopicCard(TopicData topicData)
        {
            if (m_TopicCardPrefab == null)
            {
                Debug.LogError("DeductionBoard: TopicCard prefab is not assigned.");
                return;
            }

            if (m_CardContainer == null)
            {
                Debug.LogError("DeductionBoard: Card container is not assigned.");
                return;
            }

            TopicCard newCard = Instantiate(m_TopicCardPrefab, m_CardContainer);
            newCard.Setup(topicData);
            m_TopicCards.Add(newCard);
        }

        /// <summary>
        /// 2つのトピックから合成を試みる
        /// </summary>
        /// <param name="topicA">トピックA</param>
        /// <param name="topicB">トピックB</param>
        /// <returns>合成成功ならtrue</returns>
        private bool CheckSynthesis(TopicData topicA, TopicData topicB)
        {
            foreach (var recipe in m_Recipes)
            {
                if (recipe.Matches(topicA, topicB))
                {
                    // 合成成功！
                    Debug.Log($"DeductionBoard: Synthesis Successful! {topicA.Title} + {topicB.Title} = {recipe.Result.Title}");
                    
                    // 結果トピックをアンロック
                    // 重複チェックはAddTopic内で行われるのでそのまま呼ぶ
                    if (HasTopic(recipe.Result.TopicID))
                    {
                        // 既に持ってる
                        Debug.Log("DeductionBoard: Result topic already exists.");
                        // エフェクトだけ出すなどの処理をここに追加可能
                        return false; 
                    }
                    else
                    {
                         AddTopic(recipe.Result);

                        // ScenarioManager側にフラグを立てるなどの通知が必要ならここで行う
                        // 例: ScenarioManager.Instance.SetVariable($"has_topic_{recipe.Result.TopicID}", true);
                        var scenarioManager = FindFirstObjectByType<ScenarioManager>();
                        if (scenarioManager != null)
                        {
                            scenarioManager.SetVariable<bool>($"has_topic_{recipe.Result.TopicID}", true);
                        }

                        // 材料となったトピックを消すかどうかは仕様次第
                        // ここでは「消さない」仕様とする（手がかりは残り続ける）
                        // 演出：MetaEffect再生
                        // 画面中央などで祝福エフェクトを出す
                        // "Sparkle" or "Success"などのエフェクト名を使用
                        ProjectFoundPhone.Effects.MetaEffectController.Instance?.PlayEffect("Sparkle", Vector3.zero);

                        return true;
                    }
                }
            }

            Debug.Log("DeductionBoard: No matching recipe found.");
            return false;
        }
        #endregion
    }
}
