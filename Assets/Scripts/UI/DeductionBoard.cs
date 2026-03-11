using UnityEngine;
using System.Collections.Generic;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.Core; // ScenarioManagerのために追加

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 推論�Eード！EeductionBoard�E��EUIマネージャー
    /// プレイヤーが獲得したトピックを管琁E�E表示する
    /// </summary>
    public class DeductionBoard : MonoBehaviour
    {
        #region Singleton
        private static DeductionBoard s_Instance;
        private static bool s_WarnedNotFound;

        /// <summary>
        /// DeductionBoardのシングルトンインスタンス。シーンに存在しない場合は null を返す。
        /// </summary>
        public static DeductionBoard Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindFirstObjectByType<DeductionBoard>();
                    if (s_Instance == null && !s_WarnedNotFound)
                    {
                        Debug.LogWarning("DeductionBoard: Instance not found in scene.");
                        s_WarnedNotFound = true;
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
        /// 獲得済みトピチE��のリスチE
        /// </summary>
        private List<TopicData> m_UnlockedTopics = new List<TopicData>();

        /// <summary>
        /// 生�EされたTopicCardのリスチE
        /// </summary>
        private List<TopicCard> m_TopicCards = new List<TopicCard>();

        /// <summary>
        /// ロードされた合�Eレシピ�EリスチE
        /// </summary>
        private List<SynthesisRecipe> m_Recipes = new List<SynthesisRecipe>();
        #endregion

        #region Public Properties
        /// <summary>
        /// 獲得済みトピチE��のリスト（読み取り専用�E�E
        /// </summary>
        public IReadOnlyList<TopicData> UnlockedTopics => m_UnlockedTopics;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // シングルトンの初期匁E
            if (s_Instance != null && s_Instance != this)
            {
                Debug.LogWarning("DeductionBoard: Duplicate instance found. Destroying this instance.");
                Destroy(gameObject);
                return;
            }
            s_Instance = this;

            LoadRecipes();
            SubscribeContradictionEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeContradictionEvents();
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }

        private void SubscribeContradictionEvents()
        {
            var cm = ContradictionManager.Instance;
            if (cm != null)
                cm.OnContradictionDiscovered += HandleContradictionDiscovered;
        }

        private void UnsubscribeContradictionEvents()
        {
            var cm = ContradictionManager.Instance;
            if (cm != null)
                cm.OnContradictionDiscovered -= HandleContradictionDiscovered;
        }

        private void HandleContradictionDiscovered(ContradictionPair pair)
        {
            if (pair.UnlockTopic == null) return;
            if (HasTopic(pair.UnlockTopic.TopicID)) return;

            AddTopic(pair.UnlockTopic);

            var scenarioManager = FindFirstObjectByType<ScenarioManager>();
            if (scenarioManager != null)
            {
                scenarioManager.SetVariable<bool>($"$has_topic_{pair.UnlockTopic.TopicID}", true);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// トピチE��を推論�Eードに追加する
        /// </summary>
        /// <param name="topicData">追加するトピチE��チE�Eタ</param>
        /// <returns>追加に成功した場吁Erue、既に存在する場吁Ealse</returns>
        public bool AddTopic(TopicData topicData)
        {
            if (topicData == null)
            {
                Debug.LogWarning("DeductionBoard: Attempted to add null TopicData.");
                return false;
            }

            // 重褁E��ェチE��
            if (HasTopic(topicData.TopicID))
            {
                Debug.Log($"DeductionBoard: Topic '{topicData.Title}' already exists.");
                return false;
            }

            // トピチE��をリストに追加
            m_UnlockedTopics.Add(topicData);

            // カードを生�E
            CreateTopicCard(topicData);

            Debug.Log($"DeductionBoard: Topic added - {topicData.Title} (ID: {topicData.TopicID})");

            if (m_ShowOnTopicAdded)
            {
                Show();
            }

            return true;
        }

        /// <summary>
        /// 推論�Eードを表示する
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 推論�Eードを非表示にする
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// トピチE��を推論�Eードから削除する
        /// </summary>
        /// <param name="topicID">削除するトピチE��のID</param>
        /// <returns>削除に成功した場吁Erue</returns>
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
                SafeDestroy(cardToRemove.gameObject);
            }

            Debug.Log($"DeductionBoard: Topic removed - {topicToRemove.Title} (ID: {topicID})");
            return true;
        }

        /// <summary>
        /// 持E��したIDのトピチE��が既に獲得済みかどぁE��を確認すめE
        /// </summary>
        /// <param name="topicID">確認するトピックのID</param>
        /// <returns>獲得済みの場吁Erue</returns>
        public bool HasTopic(string topicID)
        {
            return m_UnlockedTopics.Exists(t => t.TopicID == topicID);
        }

        /// <summary>
        /// 全てのトピチE��をクリアする
        /// </summary>
        public void ClearAllTopics()
        {
            // 全てのカードを削除
            foreach (var card in m_TopicCards)
            {
                if (card != null)
                {
                    SafeDestroy(card.gameObject);
                }
            }
            m_TopicCards.Clear();
            m_UnlockedTopics.Clear();

            Debug.Log("DeductionBoard: All topics cleared.");
        }

        /// <summary>
        /// トピチE��ドラチE���E�E��ロチE�E時�E処琁E
        /// </summary>
        /// <param name="droppedCard">ドラチE��されたカーチE/param>
        /// <param name="targetCard">ドロチE�E先�EカーチE/param>
        /// <returns>処琁E��成功�E�合成�E功など�E�した場合�Etrue</returns>
        public bool OnTopicDropped(TopicCard droppedCard, TopicCard targetCard)
        {
            if (droppedCard == null || targetCard == null) return false;
            if (droppedCard == targetCard) return false;

            return CheckSynthesis(droppedCard.TopicData, targetCard.TopicData);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// EditMode�E�テスト環墁E��でも安�Eにオブジェクトを破棁E��めE
        /// </summary>
        private void SafeDestroy(Object obj)
        {
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }

        /// <summary>
        /// Resourcesから合�EレシピをロードすめE
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
        /// TopicCardを生成してコンチE��に追加する
        /// </summary>
        /// <param name="topicData">カードに設定するトピックチE�Eタ</param>
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
        /// 2つのトピチE��から合�Eを試みめE
        /// </summary>
        /// <param name="topicA">トピチE��A</param>
        /// <param name="topicB">トピチE��B</param>
        /// <returns>合�E成功ならtrue</returns>
        private bool CheckSynthesis(TopicData topicA, TopicData topicB)
        {
            foreach (var recipe in m_Recipes)
            {
                if (recipe.Matches(topicA, topicB))
                {
                    // 合�E成功�E�E
                    Debug.Log($"DeductionBoard: Synthesis Successful! {topicA.Title} + {topicB.Title} = {recipe.Result.Title}");
                    
                    // 結果トピチE��をアンロチE��
                    // 重褁E��ェチE��はAddTopic冁E��行われるのでそ�Eまま呼ぶ
                    if (HasTopic(recipe.Result.TopicID))
                    {
                        // 既に持ってめE
                        Debug.Log("DeductionBoard: Result topic already exists.");
                        // エフェクトだけ�Eすなどの処琁E��ここに追加可能
                        return false; 
                    }
                    else
                    {
                         AddTopic(recipe.Result);

                        // ScenarioManager側にフラグを立てるなどの通知が忁E��ならここで行う
                        // 侁E ScenarioManager.Instance.SetVariable($"$has_topic_{recipe.Result.TopicID}", true);
                        var scenarioManager = FindFirstObjectByType<ScenarioManager>();
                        if (scenarioManager != null)
                        {
                            scenarioManager.SetVariable<bool>($"$has_topic_{recipe.Result.TopicID}", true);
                        }

                        // 材料となったトピックを消すかどぁE��は仕様次第
                        // ここでは「消さなぁE��仕様とする�E�手がかり�E残り続ける！E
                        // 演�E�E�MetaEffect再生
                        // 画面中央などで祝福エフェクトを出ぁE
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

