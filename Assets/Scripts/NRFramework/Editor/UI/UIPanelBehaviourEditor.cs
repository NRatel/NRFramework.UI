// https://github.com/NRatel/NRFramework.UI

using System;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    [CustomEditor(typeof(UIPanelBehaviour))]
    public class UIPanelBehaviourEditor : UIViewBehaviourEditor
    {
        public enum NoValidAnimatorEnumForDisplay { NoValidAnimator }

        private SerializedProperty m_PanelTypeSP;
        private SerializedProperty m_HasBgSP;
        private SerializedProperty m_BgShowTypeSP;
        private SerializedProperty m_CustomBgColorSP;
        private SerializedProperty m_BgClickEventTypeSP;
        private SerializedProperty m_GetFocusTypeSP;
        private SerializedProperty m_EscPressEventTypeSP;
        private SerializedProperty m_ThicknessSP;
        private SerializedProperty m_OpenAnimPlayModeSP;
        private SerializedProperty m_CloseAnimPlayModeSP;

        private int m_LastPanelTypeIndex = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_PanelTypeSP = serializedObject.FindProperty("m_PanelType");
            m_HasBgSP = serializedObject.FindProperty("m_HasBg");
            m_BgShowTypeSP = serializedObject.FindProperty("m_BgShowType");
            m_CustomBgColorSP = serializedObject.FindProperty("m_CustomBgColor");
            m_BgClickEventTypeSP = serializedObject.FindProperty("m_BgClickEventType");
            m_GetFocusTypeSP = serializedObject.FindProperty("m_GetFocusType");
            m_EscPressEventTypeSP = serializedObject.FindProperty("m_EscPressEventType");
            m_ThicknessSP = serializedObject.FindProperty("m_Thickness");
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
                bool disableFocusAndFocusScope = false;
                switch (panelType)
                {
                    case UIPanelType.Underlay:
                        disableFocusAndFocusScope = true;
                        m_HasBgSP.boolValue = true;
                        m_BgShowTypeSP.enumValueIndex = (int)UIPanelBgShowType.Alpha;
                        m_BgClickEventTypeSP.enumValueIndex = (int)UIPanelBgClickEventType.DontRespone;
                        m_GetFocusTypeSP.enumValueIndex = (int)UIPanelGetFocusType.Get;
                        if (m_LastPanelTypeIndex < 0 || m_LastPanelTypeIndex != m_PanelTypeSP.enumValueIndex)
                        { m_EscPressEventTypeSP.enumValueIndex = (int)UIPanelEscPressEventType.DontRespone; }  //默认不响应
                        break;

                    case UIPanelType.Overlay:
                        disableFocusAndFocusScope = true;
                        m_HasBgSP.boolValue = false;
                        m_GetFocusTypeSP.enumValueIndex = (int)UIPanelGetFocusType.GetWithOthers;
                        if (m_LastPanelTypeIndex < 0 || m_LastPanelTypeIndex != m_PanelTypeSP.enumValueIndex)
                        { m_EscPressEventTypeSP.enumValueIndex = (int)UIPanelEscPressEventType.DontCheck; } //默认不检测
                        break;

                    case UIPanelType.Window:
                        disableFocusAndFocusScope = true;
                        m_HasBgSP.boolValue = true;
                        m_BgShowTypeSP.enumValueIndex = (int)UIPanelBgShowType.HalfAlphaBlack;
                        m_BgClickEventTypeSP.enumValueIndex = (int)UIPanelBgClickEventType.CloseSelf;
                        m_GetFocusTypeSP.enumValueIndex = (int)UIPanelGetFocusType.Get;
                        if (m_LastPanelTypeIndex < 0 || m_LastPanelTypeIndex != m_PanelTypeSP.enumValueIndex)
                        { m_EscPressEventTypeSP.enumValueIndex = (int)UIPanelEscPressEventType.CloseSelf; }   //默认关闭自身
                        break;

                    case UIPanelType.Float:
                        disableFocusAndFocusScope = true;
                        m_HasBgSP.boolValue = false;
                        m_GetFocusTypeSP.enumValueIndex = (int)UIPanelGetFocusType.DontGet;
                        if (m_LastPanelTypeIndex < 0 || m_LastPanelTypeIndex != m_PanelTypeSP.enumValueIndex)
                        { m_EscPressEventTypeSP.enumValueIndex = (int)UIPanelEscPressEventType.DontCheck; } //默认不检测
                        break;

                    case UIPanelType.System:
                        disableFocusAndFocusScope = true;
                        m_HasBgSP.boolValue = true;
                        m_BgShowTypeSP.enumValueIndex = (int)UIPanelBgShowType.HalfAlphaBlack;
                        m_BgClickEventTypeSP.enumValueIndex = (int)UIPanelBgClickEventType.DontRespone;
                        m_GetFocusTypeSP.enumValueIndex = (int)UIPanelGetFocusType.DontGet;
                        if (m_LastPanelTypeIndex < 0 || m_LastPanelTypeIndex != m_PanelTypeSP.enumValueIndex)
                        { m_EscPressEventTypeSP.enumValueIndex = (int)UIPanelEscPressEventType.DontRespone; } //默认不响应
                        break;

                    case UIPanelType.Custom:
                        disableFocusAndFocusScope = false;
                        break;
                }

                using (new EditorGUI.DisabledScope(disableFocusAndFocusScope))
                {
                    m_HasBgSP.boolValue = EditorGUILayout.Toggle("HasBg", m_HasBgSP.boolValue);
                    if (m_HasBgSP.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        Enum bgShowTypeEnum = EditorGUILayout.EnumPopup("BgShowType", (UIPanelBgShowType)m_BgShowTypeSP.enumValueIndex);
                        m_BgShowTypeSP.enumValueIndex = (int)(UIPanelBgShowType)bgShowTypeEnum;
                        if (m_BgShowTypeSP.enumValueIndex == (int)UIPanelBgShowType.CustomColor)
                        {
                            EditorGUI.indentLevel++;
                            m_CustomBgColorSP.colorValue = EditorGUILayout.ColorField(m_CustomBgColorSP.colorValue);
                            EditorGUI.indentLevel--;
                        }
                        Enum bgClickEventTypeEnum = EditorGUILayout.EnumPopup("BgClickEventType", (UIPanelBgClickEventType)m_BgClickEventTypeSP.enumValueIndex);
                        m_BgClickEventTypeSP.enumValueIndex = (int)(UIPanelBgClickEventType)bgClickEventTypeEnum;
                        EditorGUI.indentLevel--;
                    }

                    Enum getFocusTypeEnum = EditorGUILayout.EnumPopup("GetFocusType", (UIPanelGetFocusType)m_GetFocusTypeSP.enumValueIndex);
                    m_GetFocusTypeSP.enumValueIndex = (int)(UIPanelGetFocusType)getFocusTypeEnum;
                }

                //Debug.Log("m_EscPressEventTypeSP.enumValueIndex: " + m_EscPressEventTypeSP.enumValueIndex);

                Enum escPressEventTypeEnum = EditorGUILayout.EnumPopup("EscPressEventType", (UIPanelEscPressEventType)m_EscPressEventTypeSP.enumValueIndex);
                m_EscPressEventTypeSP.enumValueIndex = (int)(UIPanelEscPressEventType)escPressEventTypeEnum;

                m_LastPanelTypeIndex = m_PanelTypeSP.enumValueIndex;
            }
            EditorGUI.indentLevel--;

            m_ThicknessSP.intValue = EditorGUILayout.IntField("Thickness", m_ThicknessSP.intValue);

            bool existValidAnimator = ((UIPanelBehaviour)target).ExistValidAnimator();
            if (existValidAnimator)
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
                    EditorGUILayout.EnumPopup("OpenAnimPlayMode", (NoValidAnimatorEnumForDisplay)0);
                    EditorGUILayout.EnumPopup("CloseAnimPlayMode", (NoValidAnimatorEnumForDisplay)0);
                }
            }
        }
    }
}