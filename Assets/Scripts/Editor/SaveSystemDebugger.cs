using UnityEngine;
using UnityEditor;
using ProjectFoundPhone.Core;
using ProjectFoundPhone.Data;

namespace ProjectFoundPhone.Editor
{
    /// <summary>
    /// セーブシステムのデバッグ用エディタウィンドウ
    /// </summary>
    public class SaveSystemDebugger : EditorWindow
    {
        private int m_SelectedSlot = 0;

        [MenuItem("Project FoundPhone/Debug/Save System Debugger")]
        public static void ShowWindow()
        {
            GetWindow<SaveSystemDebugger>("Save System Debugger");
        }

        private void OnGUI()
        {
            GUILayout.Label("Save System Debugger", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("This tool requires Play Mode to be active.", MessageType.Warning);
                return;
            }

            if (SaveManager.Instance == null)
            {
                EditorGUILayout.HelpBox("SaveManager instance not found in scene.", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("Current Save Slot", EditorStyles.boldLabel);
            m_SelectedSlot = EditorGUILayout.IntSlider("Slot Number", m_SelectedSlot, 0, 2);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Save Game"))
            {
                bool success = SaveManager.Instance.SaveGame(m_SelectedSlot);
                Debug.Log($"Save to slot {m_SelectedSlot}: {(success ? "Success" : "Failed")}");
            }

            if (GUILayout.Button("Load Game"))
            {
                bool success = SaveManager.Instance.LoadGame(m_SelectedSlot);
                Debug.Log($"Load from slot {m_SelectedSlot}: {(success ? "Success" : "Failed")}");
            }

            if (GUILayout.Button("Delete Save"))
            {
                bool success = SaveManager.Instance.DeleteSave(m_SelectedSlot);
                Debug.Log($"Delete slot {m_SelectedSlot}: {(success ? "Success" : "Failed")}");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Save Information", EditorStyles.boldLabel);

            SaveData saveInfo = SaveManager.Instance.GetSaveInfo(m_SelectedSlot);
            if (saveInfo != null)
            {
                EditorGUILayout.LabelField("Slot Status", "Has Save Data");
                EditorGUILayout.LabelField("Save Time", saveInfo.SaveDateTime);
                EditorGUILayout.LabelField("Unlocked Topics", saveInfo.UnlockedTopicIDs.Count.ToString());
                EditorGUILayout.LabelField("Current Node", saveInfo.CurrentNodeName ?? "None");
            }
            else
            {
                EditorGUILayout.LabelField("Slot Status", "Empty");
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("All Slots Overview", EditorStyles.boldLabel);

            SaveData[] allSaves = SaveManager.Instance.GetAllSaveInfo();
            for (int i = 0; i < allSaves.Length; i++)
            {
                string status = allSaves[i] != null ? allSaves[i].GetSummary() : "Empty";
                EditorGUILayout.LabelField($"Slot {i}", status);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Persistent Data Path"))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
        }
    }
}
