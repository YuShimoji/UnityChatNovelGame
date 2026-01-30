using UnityEngine;
using System.Collections;
using System.Linq;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.UI;
using ProjectFoundPhone.Data; // For TopicData

namespace ProjectFoundPhone.Tests
{
    public class DeductionBoardSynthesisTest : MonoBehaviour
    {
        [Header("Test Data")]
        [SerializeField] private string m_IngredientA_ID = "topic_found_phone";
        [SerializeField] private string m_IngredientB_ID = "topic_suspicious_message";
        [SerializeField] private string m_Result_ID = "topic_missing_person";
        
        [Header("References")]
        [SerializeField] private DeductionBoard m_Board;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1.0f); // Wait for init

            if (m_Board == null) m_Board = FindFirstObjectByType<DeductionBoard>();
            if (m_Board == null)
            {
                Debug.LogError("[SynthesisTest] DeductionBoard not found.");
                yield break;
            }

            Debug.Log("[SynthesisTest] Starting Synthesis Test...");

            // 1. Add Ingredients
            TopicData topicA = Resources.Load<TopicData>("Topics/topic_found_phone"); // Adjust path if needed or load all
            TopicData topicB = Resources.Load<TopicData>("Topics/topic_suspicious_message");

            // Fallback: Try to use existing topics in DeductionBoard logic if Resources load fails, 
            // but usually we load from Resources.
            // Assuming CreateTestRecipe has been run, the Recipe exists in Resources/Recipes.
            // DeductionBoard.LoadRecipes() is called in Awake().

            if (topicA == null || topicB == null)
            {
                Debug.LogError("[SynthesisTest] Failed to load topic assets topicA or topicB.");
                yield break;
            }

            m_Board.AddTopic(topicA);
            m_Board.AddTopic(topicB);

            yield return null; // Wait for UI update

            // 2. Find Cards (Need a way to find cards from Board. Currently private list.)
            // We'll trust the board created them.
            // Since we can't get cards easily without modification, we might need to modify DeductionBoard
            // OR use reflection. Let's use Reflection for testing purposes.
            
            var fieldInfo = typeof(DeductionBoard).GetField("m_TopicCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cards = fieldInfo.GetValue(m_Board) as System.Collections.Generic.List<TopicCard>;

            TopicCard cardA = cards.Find(c => c.TopicData.TopicID == m_IngredientA_ID);
            TopicCard cardB = cards.Find(c => c.TopicData.TopicID == m_IngredientB_ID);

            if (cardA == null || cardB == null)
            {
                Debug.LogError("[SynthesisTest] Failed to find TopicCards on board.");
                yield break;
            }

            Debug.Log("[SynthesisTest] Simulating Drop A onto B...");

            // 3. Simulate Drop
            bool success = m_Board.OnTopicDropped(cardA, cardB);

            if (success)
            {
                Debug.Log("[SynthesisTest] Drop Logic Returned True.");
            }
            else
            {
                Debug.LogError("[SynthesisTest] Drop Logic Returned False (Recipe missing?).");
            }

            yield return null;

            // 4. Verify Result
            if (m_Board.HasTopic(m_Result_ID))
            {
                 Debug.Log($"[SynthesisTest] SUCCESS: Result Topic '{m_Result_ID}' found!");
            }
            else
            {
                 Debug.LogError($"[SynthesisTest] FAILURE: Result Topic '{m_Result_ID}' NOT found.");
            }
        }
    }
}
