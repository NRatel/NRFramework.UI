using UnityEngine;
using UnityEditor;

namespace NRFramework
{
    //路径相对 Application.dataPath
    public class NRFrameworkEditorSetting : ScriptableObject
    {
        //Asset
        //public enum AssetUsageMode
        //{
        //    Editor = 1,
        //    Local = 2,
        //    Remote = 3,
        //}
        //public AssetUsageMode assetUsageMode = AssetUsageMode.Editor;       //编辑器下运行时
        //public string assetBundleRemoteUrl = "https://127.0.0.1";

        // UI
        public bool enableOpElementHierarchy = true;

        public string uiPrefabRootDir = "GameRes/GUI/Prefabs";
        // UI类生成根目录（相对于 Application.dataPath）
        // 将在在 /预设相对路径/ 下创建对应基类。
        // 将在其 /_Temp 下创建快捷模板，创建后自行挪走
        public string uiGenerateRootDir = "Scripts/GameLogic/GeneratedTempUICodes"; 

        private static NRFrameworkEditorSetting sm_Instance = null;
        public static NRFrameworkEditorSetting Instance
        {
            get
            {
                if (sm_Instance == null)
                {
                    sm_Instance = AssetDatabase.LoadAssetAtPath<NRFrameworkEditorSetting>("NRFrameworkEditorSetting");
#if UNITY_EDITOR
                    if (sm_Instance == null)
                    {
                        sm_Instance = CreateInstance<NRFrameworkEditorSetting>();
                        AssetDatabase.CreateAsset(sm_Instance, "Assets/Scripts/NRFramework/Editor/NRFrameworkEditorSetting.asset");
                    }
#else
                    Debug.Assert(sm_Instance != null);
#endif
                }
                return sm_Instance;
            }
        }

#if UNITY_EDITOR
        [MenuItem("NRFramework/Setting", false, 999)]
        public static void Select()
        {
            Debug.Log("Application.dataPath: " + Application.dataPath);
            Selection.activeObject = Instance;
        }
#endif
    }
}
