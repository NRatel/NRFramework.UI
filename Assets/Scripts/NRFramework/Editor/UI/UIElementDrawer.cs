// https://github.com/NRatel/NRFramework.UI

using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    [CustomPropertyDrawer(typeof(UIOpElement), true)]
    public class UIElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty targetSP = property.FindPropertyRelative("m_Target");
            SerializedProperty componentListSP = property.FindPropertyRelative("m_ComponentList");

            if (RemoveNulls(componentListSP))
            {
                componentListSP.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                //自适应的宽度（除间距外，target:1/3, select:2/3）
                float itemSpacing = 5;
                float targetWidth = (position.width - itemSpacing) / 3;
                float selectWidth = targetWidth * 2;

                Rect targetRect = new Rect(position.x, position.y, targetWidth, position.height);
                Rect selectRect = new Rect(position.x + targetWidth + itemSpacing, position.y, selectWidth, position.height);

                int oldIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                {
                    //go
                    EditorGUI.BeginChangeCheck();
                    {
                        GUI.Box(targetRect, GUIContent.none);
                        EditorGUI.PropertyField(targetRect, targetSP, GUIContent.none);
                        if (EditorGUI.EndChangeCheck())
                        {
                            componentListSP.ClearArray();
                            targetSP.serializedObject.ApplyModifiedProperties();
                            componentListSP.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    //select
                    bool isExsistTarget = targetSP.objectReferenceValue != null;
                    using (new EditorGUI.DisabledScope(!isExsistTarget))
                    {
                        if (isExsistTarget)
                        {
                            GameObject target = targetSP.objectReferenceValue as GameObject;
                            Component[] existComps = target.GetComponents<Component>();

                            //按钮（铺在下方）
                            GUIContent guiContent = new GUIContent();
                            guiContent.text = (componentListSP.arraySize == 0) ? "None" : string.Empty; //按钮文本
                            if (GUI.Button(selectRect, guiContent, EditorStyles.popup))
                            {
                                BuildPopupList(existComps, componentListSP).DropDown(selectRect);
                            }

                            //图标列表（覆盖在上方）
                            int posIndex = 0;
                            for (int i = 0; i < existComps.Length; i++) //用它遍历能够保证组件在Inspector上的顺序
                            {
                                Component comp = existComps[i];

                                int savedIndex = GetIndexFromSavedComponentList(componentListSP, comp); //只显示已保存的组件
                                if (savedIndex >= 0)
                                {
                                    Texture icon = UIEditorUtility.GetIconByType(comp.GetType());
                                    DrawIcon(selectRect, posIndex, icon);
                                    posIndex++;
                                }
                            }
                        }
                        else
                        {
                            //按钮（纯占位）
                            if (GUI.Button(selectRect, "None", EditorStyles.popup)) { };
                        }
                    }
                }
                EditorGUI.indentLevel = oldIndent;
            }
            EditorGUI.EndProperty();
        }

        private GenericMenu BuildPopupList(Component[] existComps, SerializedProperty componentListSP)
        {
            GenericMenu menu = new GenericMenu();

            foreach (Component comp in existComps)
            {
                int savedIndex = GetIndexFromSavedComponentList(componentListSP, comp);

                menu.AddItem(new GUIContent(comp.GetType().Name), savedIndex >= 0, (source) =>
                {
                    //点击Item时，若Item已存在执行移除，否则执行添加。
                    if (savedIndex >= 0)
                    {
                        //注意，删除元素前必须先将其置为null，
                        //否则直接调用DeleteArrayElementAtIndex会使元素变为null而不是从列表中移除。
                        componentListSP.GetArrayElementAtIndex(savedIndex).objectReferenceValue = null;
                        componentListSP.DeleteArrayElementAtIndex(savedIndex);
                    }
                    else
                    {
                        componentListSP.InsertArrayElementAtIndex(componentListSP.arraySize);
                        componentListSP.GetArrayElementAtIndex(componentListSP.arraySize - 1).objectReferenceValue = source as Component;
                    }
                    componentListSP.serializedObject.ApplyModifiedProperties();
                    EditorApplication.RepaintHierarchyWindow(); //需要强刷Hierarchy
                }, comp);
            }

            return menu;
        }

        private bool RemoveNulls(SerializedProperty componentListSP)
        {
            bool hasNull = false;
            for (int i = componentListSP.arraySize - 1; i >= 0; i--)
            {
                if (componentListSP.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    componentListSP.DeleteArrayElementAtIndex(i);
                    hasNull = true;
                }
            }
            return hasNull;
        }

        //组件在已保存组件列表中的位置
        private int GetIndexFromSavedComponentList(SerializedProperty componentListSP, Component comp)
        {
            int index = -1;
            for (int i = 0; i < componentListSP.arraySize; i++)
            {
                Component savedComp = componentListSP.GetArrayElementAtIndex(i).objectReferenceValue as Component;
                if (savedComp.Equals(comp))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        static private void DrawIcon(Rect selectRect, int posIndex, Texture icon)
        {
            float iconWidth = EditorGUIUtility.singleLineHeight * 0.8f;
            float iconHeight = iconWidth;
            float leftPadding = 5;
            float iconSpacing = 5;

            float iconX = selectRect.x + leftPadding + (iconWidth + iconSpacing) * posIndex;  //水平居左
            float iconY = selectRect.y + (selectRect.height - iconHeight) / 2;  //竖直居中

            Rect iconRect = new Rect(iconX, iconY, iconWidth, iconHeight);

            GUI.DrawTexture(iconRect, icon);
        }
    }
}