using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace NRFramework
{
    [CustomEditor(typeof(UIViewBehaviour))]
    public abstract class UIViewBehaviourEditor : Editor
    {
        protected ReorderableList m_OpElementListRL;

        protected virtual void OnEnable()
        {
            m_OpElementListRL = CreateReorderableList(serializedObject.FindProperty("m_OpElementList"));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            m_OpElementListRL.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private ReorderableList CreateReorderableList(SerializedProperty opElementListSP)
        {
            ReorderableList reorderableList = new ReorderableList(serializedObject, opElementListSP)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 1.2f
            };

            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight * 0.1f;

                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };

            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                GUI.Label(rect, "OpElementList");
            };

            //重写原因（相对ReorderableList源码中的默认实现DrawFooter）：
            //1、删除 list.displayAdd 和 list.displayRemove 的判断逻辑。（不需要，要让+-按钮永远保留）
            //2、删除 增加+按钮时的 onAddDropdownCallback相关逻辑。（不需要）
            //3、增加一个整理按钮。
            reorderableList.drawFooterCallback = (Rect rect) =>
            {
                ReorderableList list = reorderableList;
                ReorderableList.Defaults defaults = ReorderableList.defaultBehaviours;

                float rightMargin = 10f;
                float leftPading = 10f;
                float rightPading = 10f;
                float singleWidth = 25f;
                float singleHeight = 16f;
                float spacing = 5f;
                int btnsCount = 3;

                float rightEdge = rect.xMax - rightMargin;
                float btnsWidth = leftPading + rightPading + singleWidth * btnsCount + spacing * (btnsCount - 1);
                Rect btnsRect = new Rect(rightEdge - btnsWidth, rect.y, btnsWidth, rect.height);

                Rect addRect = new Rect(btnsRect.x + leftPading, btnsRect.y, singleWidth, singleHeight);
                Rect removeRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 1, btnsRect.y, singleWidth, singleHeight);
                Rect refreshRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 2, btnsRect.y, singleWidth, singleHeight);

                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    defaults.footerBackground.Draw(btnsRect, false, false, false, false);
                }

                using (new EditorGUI.DisabledScope(list.onCanAddCallback != null && !list.onCanAddCallback(list)))
                {
                    if (GUI.Button(addRect, defaults.iconToolbarPlus, defaults.preButton))
                    {
                        //defaults.DoAddButton(list);
                        list.onAddCallback(list);
                    }
                }

                using (new EditorGUI.DisabledScope(list.index < 0 || list.index >= list.count || (list.onCanRemoveCallback != null && !list.onCanRemoveCallback(list))))
                {
                    if (GUI.Button(removeRect, defaults.iconToolbarMinus, defaults.preButton))
                    {
                        defaults.DoRemoveButton(list);
                        //list.onRemoveCallback(list);
                    }
                }

                using (new EditorGUI.DisabledScope(list.count <= 0))
                {
                    Texture icon = EditorGUIUtility.IconContent("TreeEditor.Refresh").image;
                    if (GUI.Button(refreshRect, new GUIContent(icon), defaults.preButton))
                    {
                        RefreshOpElementList(list);
                    }
                }
            };

            //重写原因（相对ReorderableList源码中的默认实现DoAddButton）：
            //1、去掉元素为 IList时，实际类型不明的丑陋构造方式，只需将list长度自增即可。
            //2、新增的元素，需要清空。
            reorderableList.onAddCallback = (ReorderableList list) =>
            {
                SerializedProperty listSP = list.serializedProperty;
                listSP.arraySize++;
                list.index = listSP.arraySize - 1;

                SerializedProperty newElementSP = listSP.GetArrayElementAtIndex(listSP.arraySize - 1);
                SerializedProperty targetSP = newElementSP.FindPropertyRelative("m_Target");
                targetSP.objectReferenceValue = null;
                newElementSP.serializedObject.ApplyModifiedProperties();
            };

            return reorderableList;
        }

        protected void RefreshOpElementList(ReorderableList list)
        {
            SerializedProperty listSP = list.serializedProperty;

            //merge components From I to J.
            //将 在I中且不在J中的component加入J，然后将I的Target置为Null。
            for (int i = 1; i < listSP.arraySize; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    SerializedProperty elementSP_I = listSP.GetArrayElementAtIndex(i);
                    SerializedProperty elementSP_J = listSP.GetArrayElementAtIndex(j);

                    SerializedProperty targetSP_I = elementSP_I.FindPropertyRelative("m_Target");
                    SerializedProperty targetSP_J = elementSP_J.FindPropertyRelative("m_Target");

                    if (targetSP_I.objectReferenceValue == null) { continue; }
                    if (targetSP_J.objectReferenceValue == null) { continue; }

                    if (!targetSP_I.objectReferenceValue.Equals(targetSP_J.objectReferenceValue)) { continue; }

                    SerializedProperty componentListSP_I = elementSP_I.FindPropertyRelative("m_ComponentList");
                    SerializedProperty componentListSP_J = elementSP_J.FindPropertyRelative("m_ComponentList");

                    for (int m = 0; m < componentListSP_I.arraySize; m++)
                    {
                        bool isExistInJ = false;
                        SerializedProperty componentSP_IM = componentListSP_I.GetArrayElementAtIndex(m);
                        for (int n = 0; n < componentListSP_J.arraySize; n++)
                        {
                            SerializedProperty componentSP_JN = componentListSP_J.GetArrayElementAtIndex(n);
                            if (componentSP_IM.objectReferenceValue.Equals(componentSP_JN.objectReferenceValue))
                            {
                                isExistInJ = true;
                                break;
                            }
                        }
                        if (!isExistInJ)
                        {
                            componentListSP_J.InsertArrayElementAtIndex(componentListSP_J.arraySize);
                            componentListSP_J.GetArrayElementAtIndex(componentListSP_J.arraySize - 1).objectReferenceValue = componentSP_IM.objectReferenceValue;
                        }
                    }
                    targetSP_I.objectReferenceValue = null;
                }
            }

            //移除所有Target为Null的元素
            for (int i = listSP.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty targetSP = listSP.GetArrayElementAtIndex(i).FindPropertyRelative("m_Target");
                if (targetSP.objectReferenceValue == null)
                {
                    //注意：这里删除直接DeleteArrayElementAtIndex即可。（不要先置为null，会报错）
                    listSP.DeleteArrayElementAtIndex(i);
                }
            }

            listSP.serializedObject.ApplyModifiedProperties();
        }

        protected string GetPrefabPath()
        {
            // 如果正确拿到预设所在路径？
            PrefabAssetType singlePrefabType = PrefabUtility.GetPrefabAssetType(target);
            PrefabInstanceStatus singleInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(target);
            string targetAssetPath = AssetDatabase.GetAssetPath(target);
            string prefabAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();

            //Debug.Log("singlePrefabType: " + singlePrefabType);
            //Debug.Log("singleInstanceStatus: " + singleInstanceStatus);
            //Debug.Log("targetAssetPath: " + targetAssetPath);
            //Debug.Log("prefabAssetPath: " + prefabAssetPath);
            //Debug.Log("prefabStage: " + prefabStage);

            //1、点击预设时:
            //      singlePrefabType: Regular;
            //      singleInstanceStatus: NotAPrefab
            //      targetAssetPath: 可正确拿到
            //      prefabAssetPath: 可正确拿到
            //      prefabStage: Null

            //2、双击预设并在Hierarchy上选择时:
            //      singlePrefabType: NotAPrefab;    
            //      singleInstanceStatus: NotAPrefab
            //      targetAssetPath: "" (空字符串)
            //      prefabAssetPath: "" (空字符串)
            //      prefabStage: 可正确拿到

            //3、预设拖入Hierarchy并选择时:
            //      singlePrefabType: Regular;
            //      singleInstanceStatus: Connected
            //      targetAssetPath: "" (空字符串)
            //      prefabAssetPath: 可正确拿到
            //      prefabStage: Null

            // 需要覆盖并正确判断这三种情况。
            string finalPrefabPath = null;
            if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(targetAssetPath))
            {
                finalPrefabPath = targetAssetPath;   //点击预设时
            }
            else if (singlePrefabType == PrefabAssetType.Regular && !string.IsNullOrEmpty(prefabAssetPath))
            {
                finalPrefabPath = prefabAssetPath;  //预设拖入Hierarchy并选择时
            }
            else if (prefabStage != null)
            {
                finalPrefabPath = prefabStage.assetPath; //双击预设并在Hierarchy上选择时
            }

            return finalPrefabPath;
        }
    }
}