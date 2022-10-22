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
            if (GUILayout.Button("Export"))
            {
                RefreshOpElementList(m_OpElementListRL);
                
                DealUIPrefab();
                GenerateUIBaseClass();

                Debug.Log("导出成功！");
            }
        }

        private void DealUIPrefab()
        {
        }

        private void GenerateUIBaseClass()
        {
            string targetName = target.name;
            string savePath = Path.Combine(Application.dataPath, NRFrameworkEditorSetting.Instance.uiCodeGenerateDir, targetName + "Base.cs");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("// 导出测试");
            sb.AppendLine("// savePath: " + savePath);

            File.WriteAllText(savePath, sb.ToString());

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}