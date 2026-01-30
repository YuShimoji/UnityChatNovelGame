using UnityEngine;
using System.Collections.Generic;
using ProjectFoundPhone.Data;
using ProjectFoundPhone.Core; // ScenarioManager縺ｮ縺溘ａ縺ｫ霑ｽ蜉

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// 謗ｨ隲悶・繝ｼ繝会ｼ・eductionBoard・峨・UI繝槭ロ繝ｼ繧ｸ繝｣繝ｼ
    /// 繝励Ξ繧､繝､繝ｼ縺檎佐蠕励＠縺溘ヨ繝斐ャ繧ｯ繧堤ｮ｡逅・・陦ｨ遉ｺ縺吶ｋ
    /// </summary>
    public class DeductionBoard : MonoBehaviour
    {
        #region Singleton
        private static DeductionBoard s_Instance;

        /// <summary>
        /// DeductionBoard縺ｮ繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ繧､繝ｳ繧ｹ繧ｿ繝ｳ繧ｹ
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
        /// 迯ｲ蠕玲ｸ医∩繝医ヴ繝・け縺ｮ繝ｪ繧ｹ繝・
        /// </summary>
        private List<TopicData> m_UnlockedTopics = new List<TopicData>();

        /// <summary>
        /// 逕滓・縺輔ｌ縺鬱opicCard縺ｮ繝ｪ繧ｹ繝・
        /// </summary>
        private List<TopicCard> m_TopicCards = new List<TopicCard>();

        /// <summary>
        /// 繝ｭ繝ｼ繝峨＆繧後◆蜷域・繝ｬ繧ｷ繝斐・繝ｪ繧ｹ繝・
        /// </summary>
        private List<SynthesisRecipe> m_Recipes = new List<SynthesisRecipe>();
        #endregion

        #region Public Properties
        /// <summary>
        /// 迯ｲ蠕玲ｸ医∩繝医ヴ繝・け縺ｮ繝ｪ繧ｹ繝茨ｼ郁ｪｭ縺ｿ蜿悶ｊ蟆ら畑・・
        /// </summary>
        public IReadOnlyList<TopicData> UnlockedTopics => m_UnlockedTopics;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // 繧ｷ繝ｳ繧ｰ繝ｫ繝医Φ縺ｮ蛻晄悄蛹・
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
        /// 繝医ヴ繝・け繧呈耳隲悶・繝ｼ繝峨↓霑ｽ蜉縺吶ｋ
        /// </summary>
        /// <param name="topicData">霑ｽ蜉縺吶ｋ繝医ヴ繝・け繝・・繧ｿ</param>
        /// <returns>霑ｽ蜉縺ｫ謌仙粥縺励◆蝣ｴ蜷・rue縲∵里縺ｫ蟄伜惠縺吶ｋ蝣ｴ蜷・alse</returns>
        public bool AddTopic(TopicData topicData)
        {
            if (topicData == null)
            {
                Debug.LogWarning("DeductionBoard: Attempted to add null TopicData.");
                return false;
            }

            // 驥崎､・メ繧ｧ繝・け
            if (HasTopic(topicData.TopicID))
            {
                Debug.Log($"DeductionBoard: Topic '{topicData.Title}' already exists.");
                return false;
            }

            // 繝医ヴ繝・け繧偵Μ繧ｹ繝医↓霑ｽ蜉
            m_UnlockedTopics.Add(topicData);

            // 繧ｫ繝ｼ繝峨ｒ逕滓・
            CreateTopicCard(topicData);

            Debug.Log($"DeductionBoard: Topic added - {topicData.Title} (ID: {topicData.TopicID})");

            // 繧ｪ繝励す繝ｧ繝ｳ: 繝医ヴ繝・け霑ｽ蜉譎ゅ↓繝懊・繝峨ｒ陦ｨ遉ｺ
            if (m_ShowOnTopicAdded)
            {
                // TODO: Show()繝｡繧ｽ繝・ラ繧貞他縺ｳ蜃ｺ縺吶°縲√い繝九Γ繝ｼ繧ｷ繝ｧ繝ｳ繧貞・逕・
            }

            return true;
        }

        /// <summary>
        /// 繝医ヴ繝・け繧呈耳隲悶・繝ｼ繝峨°繧牙炎髯､縺吶ｋ
        /// </summary>
        /// <param name="topicID">蜑企勁縺吶ｋ繝医ヴ繝・け縺ｮID</param>
        /// <returns>蜑企勁縺ｫ謌仙粥縺励◆蝣ｴ蜷・rue</returns>
        public bool RemoveTopic(string topicID)
        {
            TopicData topicToRemove = m_UnlockedTopics.Find(t => t.TopicID == topicID);
            if (topicToRemove == null)
            {
                Debug.LogWarning($"DeductionBoard: Topic with ID '{topicID}' not found.");
                return false;
            }

            m_UnlockedTopics.Remove(topicToRemove);

            // 蟇ｾ蠢懊☆繧九き繝ｼ繝峨ｒ蜑企勁
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
        /// 謖・ｮ壹＠縺櫑D縺ｮ繝医ヴ繝・け縺梧里縺ｫ迯ｲ蠕玲ｸ医∩縺九←縺・°繧堤｢ｺ隱阪☆繧・
        /// </summary>
        /// <param name="topicID">遒ｺ隱阪☆繧九ヨ繝斐ャ繧ｯ縺ｮID</param>
        /// <returns>迯ｲ蠕玲ｸ医∩縺ｮ蝣ｴ蜷・rue</returns>
        public bool HasTopic(string topicID)
        {
            return m_UnlockedTopics.Exists(t => t.TopicID == topicID);
        }

        /// <summary>
        /// 蜈ｨ縺ｦ縺ｮ繝医ヴ繝・け繧偵け繝ｪ繧｢縺吶ｋ
        /// </summary>
        public void ClearAllTopics()
        {
            // 蜈ｨ縺ｦ縺ｮ繧ｫ繝ｼ繝峨ｒ蜑企勁
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
        /// 繝医ヴ繝・け繝峨Λ繝・げ・・ラ繝ｭ繝・・譎ゅ・蜃ｦ逅・
        /// </summary>
        /// <param name="droppedCard">繝峨Λ繝・げ縺輔ｌ縺溘き繝ｼ繝・/param>
        /// <param name="targetCard">繝峨Ο繝・・蜈医・繧ｫ繝ｼ繝・/param>
        /// <returns>蜃ｦ逅・′謌仙粥・亥粋謌先・蜉溘↑縺ｩ・峨＠縺溷ｴ蜷医・true</returns>
        public bool OnTopicDropped(TopicCard droppedCard, TopicCard targetCard)
        {
            if (droppedCard == null || targetCard == null) return false;
            if (droppedCard == targetCard) return false;

            return CheckSynthesis(droppedCard.TopicData, targetCard.TopicData);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Resources縺九ｉ蜷域・繝ｬ繧ｷ繝斐ｒ繝ｭ繝ｼ繝峨☆繧・
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
        /// TopicCard繧堤函謌舌＠縺ｦ繧ｳ繝ｳ繝・リ縺ｫ霑ｽ蜉縺吶ｋ
        /// </summary>
        /// <param name="topicData">繧ｫ繝ｼ繝峨↓險ｭ螳壹☆繧九ヨ繝斐ャ繧ｯ繝・・繧ｿ</param>
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
        /// 2縺､縺ｮ繝医ヴ繝・け縺九ｉ蜷域・繧定ｩｦ縺ｿ繧・
        /// </summary>
        /// <param name="topicA">繝医ヴ繝・けA</param>
        /// <param name="topicB">繝医ヴ繝・けB</param>
        /// <returns>蜷域・謌仙粥縺ｪ繧液rue</returns>
        private bool CheckSynthesis(TopicData topicA, TopicData topicB)
        {
            foreach (var recipe in m_Recipes)
            {
                if (recipe.Matches(topicA, topicB))
                {
                    // 蜷域・謌仙粥・・
                    Debug.Log($"DeductionBoard: Synthesis Successful! {topicA.Title} + {topicB.Title} = {recipe.Result.Title}");
                    
                    // 邨先棡繝医ヴ繝・け繧偵い繝ｳ繝ｭ繝・け
                    // 驥崎､・メ繧ｧ繝・け縺ｯAddTopic蜀・〒陦後ｏ繧後ｋ縺ｮ縺ｧ縺昴・縺ｾ縺ｾ蜻ｼ縺ｶ
                    if (HasTopic(recipe.Result.TopicID))
                    {
                        // 譌｢縺ｫ謖√▲縺ｦ繧・
                        Debug.Log("DeductionBoard: Result topic already exists.");
                        // 繧ｨ繝輔ぉ繧ｯ繝医□縺大・縺吶↑縺ｩ縺ｮ蜃ｦ逅・ｒ縺薙％縺ｫ霑ｽ蜉蜿ｯ閭ｽ
                        return false; 
                    }
                    else
                    {
                         AddTopic(recipe.Result);

                        // ScenarioManager蛛ｴ縺ｫ繝輔Λ繧ｰ繧堤ｫ九※繧九↑縺ｩ縺ｮ騾夂衍縺悟ｿ・ｦ√↑繧峨％縺薙〒陦後≧
                        // 萓・ ScenarioManager.Instance.SetVariable($"has_topic_{recipe.Result.TopicID}", true);
                        var scenarioManager = FindFirstObjectByType<ScenarioManager>();
                        if (scenarioManager != null)
                        {
                            scenarioManager.SetVariable<bool>($"has_topic_{recipe.Result.TopicID}", true);
                        }

                        // 譚先侭縺ｨ縺ｪ縺｣縺溘ヨ繝斐ャ繧ｯ繧呈ｶ医☆縺九←縺・°縺ｯ莉墓ｧ俶ｬ｡隨ｬ
                        // 縺薙％縺ｧ縺ｯ縲梧ｶ医＆縺ｪ縺・堺ｻ墓ｧ倥→縺吶ｋ・域焔縺後°繧翫・谿九ｊ邯壹￠繧具ｼ・
                        // 貍泌・・哺etaEffect蜀咲函
                        // 逕ｻ髱｢荳ｭ螟ｮ縺ｪ縺ｩ縺ｧ逾晉ｦ上お繝輔ぉ繧ｯ繝医ｒ蜃ｺ縺・
                        // "Sparkle" or "Success"縺ｪ縺ｩ縺ｮ繧ｨ繝輔ぉ繧ｯ繝亥錐繧剃ｽｿ逕ｨ
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
