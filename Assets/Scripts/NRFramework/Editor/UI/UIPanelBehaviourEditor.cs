using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    [CustomEditor(typeof(UIPanelBehaviour))]
    public class UIPanelBehaviourEditor : UIViewBehaviourEditor
    {
        private SerializedProperty m_PanelTypeSP;
        private SerializedProperty m_CanGetFoucusSP;    //（仅Float界面可选）
        private SerializedProperty m_ColseWhenClickBgSP; //（仅Window界面可选）
        private SerializedProperty m_ThicknessSP;
        private SerializedProperty m_InSafeAreaSP;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_PanelTypeSP = serializedObject.FindProperty("m_PanelType");
            m_CanGetFoucusSP = serializedObject.FindProperty("m_CanGetFoucus");    //（仅Float界面可选）
            m_ColseWhenClickBgSP = serializedObject.FindProperty("m_ColseWhenClickBg"); //（仅Window界面可选）
            m_ThicknessSP = serializedObject.FindProperty("m_Thickness");
            m_InSafeAreaSP = serializedObject.FindProperty("m_InSafeArea");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawUIPanelSetting();
            base.OnInspectorGUI();
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            DrawExpoertButton();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUIPanelSetting()
        {
            Enum panelTypeEnum = EditorGUILayout.EnumPopup("UIPanelType", (UIPanelType)m_PanelTypeSP.enumValueIndex);
            UIPanelType panelType = (UIPanelType)panelTypeEnum;
            m_PanelTypeSP.enumValueIndex = (int)panelType;

            EditorGUI.indentLevel++;
            {
                if (panelType == UIPanelType.Scene)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        m_CanGetFoucusSP.boolValue = EditorGUILayout.Toggle("CanGetFoucus", true); //固定可获得焦点
                        m_ColseWhenClickBgSP.boolValue = EditorGUILayout.Toggle("ColseWhenClickBg", false);    //固定为false
                    }
                }
                else if (panelType == UIPanelType.Overlap)
                {
                    m_CanGetFoucusSP.boolValue = EditorGUILayout.Toggle("CanGetFoucus", m_CanGetFoucusSP.boolValue); //可选
                    //无背景，不显示背景相关设置。
                    m_ColseWhenClickBgSP.boolValue = false;
                }
                else //if (panelType == UIPanelType.Window)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        m_CanGetFoucusSP.boolValue = EditorGUILayout.Toggle("CanGetFoucus", true); //固定可获得焦点
                    }
                    m_ColseWhenClickBgSP.boolValue = EditorGUILayout.Toggle("ColseWhenClickBg", m_ColseWhenClickBgSP.boolValue); //可选
                }
            }
            EditorGUI.indentLevel--;

            m_ThicknessSP.intValue = EditorGUILayout.IntField("Thickness", m_ThicknessSP.intValue);

            m_InSafeAreaSP.boolValue = EditorGUILayout.Toggle("InSafeArea", m_InSafeAreaSP.boolValue);
        }

        private void DrawExpoertButton()
        {
            //string targetPath = AssetDatabase.GetAssetPath(((UIPanelBehaviour)target).gameObject);
            //if (string.IsNullOrEmpty(targetPath)){ return; } //仅允许从预设导出（从预设才可获得相对子路径及准确名称）

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ExportBase"))
                {
                    RefreshOpElementList(m_OpElementListRL);
                    GenerateUIBaseCode();
                }

                if (GUILayout.Button("ExportTemp"))
                {
                    RefreshOpElementList(m_OpElementListRL);
                    GenerateUITempCode();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void GenerateUIBaseCode()
        {
            // 大体步骤：
            // 1、如果物体不在预设根路径下，则不允许导出。
            // 2、截取相对子路径（含文件名、不含后缀）。
            // 3、拼接存储路径。
            // 4、生成代码并存储。
            string prefabPath = GetPrefabPath();
            string fullPrefabPath = Path.GetFullPath(Path.Combine(Application.dataPath, Path.GetRelativePath("Assets", prefabPath)));
            string fullRootDir = Path.GetFullPath(Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.uiPrefabRootDir));

            //Debug.Log("fullPrefabPath: " + fullPrefabPath);
            //Debug.Log("uiPrefabRootDir: " + fullRootDir);

            if (!fullPrefabPath.StartsWith(fullRootDir))
            {
                Debug.LogError("预设不在可导出的根目录中：" + fullRootDir);
                return;
            }

            string subPath = Path.GetRelativePath(fullRootDir, fullPrefabPath);
            string subSavePath = Path.Combine(Path.GetDirectoryName(subPath), Path.GetFileNameWithoutExtension(subPath) + "Base.cs");
            string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.generatedBaseUIRootDir, subSavePath));

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("// 导出测试");
            sb.AppendLine("// savePath: " + savePath);

            string saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir)) { Directory.CreateDirectory(saveDir); }

            File.WriteAllText(savePath, sb.ToString());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Export success!");

        }

        private void GenerateUITempCode()
        {
            // 大体步骤：
            // 1、如果物体不在预设根路径下，则不允许导出。
            // 2、截取相对子路径（含文件名、不含后缀）。
            // 3、拼接存储路径。
            // 4、生成代码并存储。
            string prefabPath = GetPrefabPath();
            string fullPrefabPath = Path.GetFullPath(Path.Combine(Application.dataPath, Path.GetRelativePath("Assets", prefabPath)));
            string fullRootDir = Path.GetFullPath(Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.uiPrefabRootDir));

            //Debug.Log("fullPrefabPath: " + fullPrefabPath);
            //Debug.Log("uiPrefabRootDir: " + fullRootDir);

            if (!fullPrefabPath.StartsWith(fullRootDir))
            {
                Debug.LogError("预设不在可导出的根目录中：" + fullRootDir);
                return;
            }

            string subPath = Path.GetRelativePath(fullRootDir, fullPrefabPath);
            string className = Path.GetFileNameWithoutExtension(subPath);
            string subSavePath = Path.Combine(Path.GetDirectoryName(subPath), className + ".cs");
            string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.generatedTempUIDir, subSavePath));

            string content = UICodeGenerator.kPanelTemplate.Replace("${ClassName}", className).Trim();

            UICodeGenerator.DoGenerate(savePath, content);
            
            Debug.Log("Export success!");
        }
    }
}