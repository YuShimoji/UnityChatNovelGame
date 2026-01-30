using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Data;

public class TopicVerifier
{
    public static void Verify()
    {
        string[] ids = new string[] { "T_StrangeSignal", "T_MissingPerson", "T_FoundPhone", "topic_missing_person" };
        int success = 0;
        int failed = 0;

        Debug.Log("--- Starting Topic Verification ---");

        foreach (var id in ids)
        {
            var topic = Resources.Load<TopicData>($"Topics/{id}");
            if (topic != null)
            {
                Debug.Log($"[Success] Loaded {id}: {topic.Title}");
                success++;
            }
            else
            {
                Debug.LogError($"[Fail] Could not load {id}");
                failed++;
            }
        }

        Debug.Log($"Verification Finished. Success: {success}, Failed: {failed}");
        
        if (failed > 0)
        {
            EditorApplication.Exit(1);
        }
        else
        {
            EditorApplication.Exit(0);
        }
    }
}
