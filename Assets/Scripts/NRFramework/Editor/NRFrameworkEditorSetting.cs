// https://github.com/NRatel/NRFramework.UI

using UnityEngine;
using UnityEditor;

namespace NRFramework
{
    public class NRFrameworkEditorSetting : ScriptableObject
    {
        public bool enableOpElementHierarchy = true;

        public string uiPrefabRootDir = "GameRes/GUI/Prefabs";

        // UI类生成根目录（相对于 Application.dataPath）
        // 将在相对路径下创建对应基类。
        // 将在相对路径下创建快捷类。创建后应自行改名（避免覆盖）。
        public string generatedBaseUIRootDir = "Scripts/GameLogic/GeneratedBaseUI";
        public string generatedTempUIRootDir = "Scripts/GameLogic/Modules";

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
