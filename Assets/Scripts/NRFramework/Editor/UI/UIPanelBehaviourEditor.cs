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
        private SerializedProperty m_PanelOpenAnimSP;
        private SerializedProperty m_PanelCloseAnimSP;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_PanelTypeSP = serializedObject.FindProperty("m_PanelType");
            m_CanGetFoucusSP = serializedObject.FindProperty("m_CanGetFoucus");    //（仅Float界面可选）
            m_ColseWhenClickBgSP = serializedObject.FindProperty("m_ColseWhenClickBg"); //（仅Window界面可选）
            m_ThicknessSP = serializedObject.FindProperty("m_Thickness");
            m_PanelOpenAnimSP = serializedObject.FindProperty("m_PanelOpenAnim");
            m_PanelCloseAnimSP = serializedObject.FindProperty("m_PanelCloseAnim");
            m_InSafeAreaSP = serializedObject.FindProperty("m_InSafeArea");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawUIPanelSetting();
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            m_OpElementListRL.DoLayoutList();
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

            Enum panelOpenAnimEnum = EditorGUILayout.EnumPopup("PanelOpenAnim", (UIPanelOpenAnimation)m_PanelOpenAnimSP.enumValueIndex);
            m_PanelOpenAnimSP.enumValueIndex = (int)(UIPanelOpenAnimation)panelOpenAnimEnum;
            Enum panelCloseAnimEnum = EditorGUILayout.EnumPopup("PanelCloseAnim", (UIPanelCloseAnimation)m_PanelCloseAnimSP.enumValueIndex);
            m_PanelCloseAnimSP.enumValueIndex = (int)(UIPanelCloseAnimation)panelCloseAnimEnum;
        }

        private void DrawExpoertButton()
        {
            string targetPath = AssetDatabase.GetAssetPath(((UIPanelBehaviour)target).gameObject);
            if (string.IsNullOrEmpty(targetPath)){ return; } //仅允许从预设导出（从预设才可获得相对子路径及准确名称）

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ExportBase"))
                {
                    RefreshOpElementList(m_OpElementListRL);
                    GenerateUIBaseCode();

                    Debug.Log("Export success!");
                }

                if (GUILayout.Button("ExportTemp"))
                {
                    RefreshOpElementList(m_OpElementListRL);
                    GenerateUITempCode();

                    Debug.Log("Export success!");
                }
            }
            GUILayout.EndHorizontal();
        }

        private void GenerateUIBaseCode()
        {
            // 1、如果物体不在预设根路径下，则不允许导出。
            // 2、截取相对子路径（含文件名、不含后缀）。
            // 3、拼接存储路径，并存储生成的代码。

            Debug.Log(((UIPanelBehaviour)target).transform.parent);

            string targetName = target.name;
            string targetPath = AssetDatabase.GetAssetPath(((UIPanelBehaviour)target).gameObject);
            string uiPrefabRootDir = Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.uiPrefabRootDir);

            
            string relativePath1 = Path.GetRelativePath(targetPath, uiPrefabRootDir);
            string relativePath2 = Path.GetRelativePath(uiPrefabRootDir, targetPath);


            Debug.Log("targetName: " + targetName);
            Debug.Log("targetPath: " + targetPath);
            Debug.Log("uiPrefabRootDir: " + uiPrefabRootDir);

            Debug.Log("relativePath1: " + relativePath1);
            Debug.Log("relativePath2: " + relativePath2);


            //string savePath = Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.uiGenerateRootDir);
            //string subPath =  targetName + "Base.cs"

            //StringBuilder sb = new StringBuilder();

            //sb.AppendLine("// 导出测试");
            //sb.AppendLine("// savePath: " + savePath);

            //File.WriteAllText(savePath, sb.ToString());

            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }

        private void GenerateUITempCode()
        {
            
        }
    }
}