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
        public enum NoAnimatorEnumForDisplay { NoValidAnimator }

        private SerializedProperty m_PanelTypeSP;
        private SerializedProperty m_CanGetFoucusSP;    //（仅Float界面可选）
        private SerializedProperty m_ColseWhenClickBgSP; //（仅Window界面可选）
        private SerializedProperty m_ThicknessSP;
        private SerializedProperty m_InSafeAreaSP;
        private SerializedProperty m_OpenAnimPlayModeSP;
        private SerializedProperty m_CloseAnimPlayModeSP;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_PanelTypeSP = serializedObject.FindProperty("m_PanelType");
            m_CanGetFoucusSP = serializedObject.FindProperty("m_CanGetFoucus");    //（仅Float界面可选）
            m_ColseWhenClickBgSP = serializedObject.FindProperty("m_ColseWhenClickBg"); //（仅Window界面可选）
            m_ThicknessSP = serializedObject.FindProperty("m_Thickness");
            m_InSafeAreaSP = serializedObject.FindProperty("m_InSafeArea");
            m_OpenAnimPlayModeSP = serializedObject.FindProperty("m_OpenAnimPlayMode");
            m_CloseAnimPlayModeSP = serializedObject.FindProperty("m_CloseAnimPlayMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawUIPanelSetting();
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 4);
            DrawOpElementList();
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

            Animator animator;
            bool isAimatorExsist = ((UIPanelBehaviour)target).TryGetComponent<Animator>(out animator);
            //if (isAimatorExsist && animator.enabled && animator.runtimeAnimatorController != null)
            if (isAimatorExsist && animator.enabled)
            {
                Enum openAnimPlayModeEnum = EditorGUILayout.EnumPopup("OpenAnimPlayMode", (UIPanelOpenAnimPlayMode)m_OpenAnimPlayModeSP.enumValueIndex);
                m_OpenAnimPlayModeSP.enumValueIndex = (int)(UIPanelOpenAnimPlayMode)openAnimPlayModeEnum;
                Enum closeAnimPlayModeEnum = EditorGUILayout.EnumPopup("CloseAnimPlayMode", (UIPanelCloseAnimPlayMode)m_CloseAnimPlayModeSP.enumValueIndex);
                m_CloseAnimPlayModeSP.enumValueIndex = (int)(UIPanelCloseAnimPlayMode)closeAnimPlayModeEnum;
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.EnumPopup("OpenAnimPlayMode", (NoAnimatorEnumForDisplay)0);
                    EditorGUILayout.EnumPopup("CloseAnimPlayMode", (NoAnimatorEnumForDisplay)0);
                }
            }
        }
    }
}