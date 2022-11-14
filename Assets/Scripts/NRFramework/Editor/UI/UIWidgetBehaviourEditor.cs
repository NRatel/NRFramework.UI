// https://github.com/NRatel/NRFramework.UI

using UnityEditor;

namespace NRFramework
{
    [CustomEditor(typeof(UIWidgetBehaviour))]
    public class UIWidgetBehaviourEditor : UIViewBehaviourEditor 
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawOpElementList();
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            DrawExpoertButton();

            serializedObject.ApplyModifiedProperties();
        }
    }
}