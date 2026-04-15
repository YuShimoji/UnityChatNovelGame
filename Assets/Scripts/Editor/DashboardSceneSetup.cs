#if UNITY_EDITOR
#if YARN_SPINNER
using UnityEditor;
using UnityEngine;
using ProjectFoundPhone.UI;

namespace ProjectFoundPhone.Editor
{
    public static class DashboardSceneSetup
    {
        [MenuItem("Tools/FoundPhone/Scene Setup/Add Dashboard to Scene")]
        public static void AddDashboardToScene()
        {
            GameObject dashboardObj = GameObject.Find("DashboardManager");
            if (dashboardObj == null)
            {
                dashboardObj = new GameObject("DashboardManager");
            }

            DashboardController dashboard = dashboardObj.GetComponent<DashboardController>();
            if (dashboard == null)
            {
                dashboard = dashboardObj.AddComponent<DashboardController>();
            }

            SerializedObject so = new SerializedObject(dashboard);
            so.Update();
            so.FindProperty("m_ShowOnStart").boolValue = true;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(dashboardObj);
            Debug.Log("DashboardSceneSetup: DashboardController added. m_ShowOnStart=true.");
        }
    }
}
#endif
#endif
