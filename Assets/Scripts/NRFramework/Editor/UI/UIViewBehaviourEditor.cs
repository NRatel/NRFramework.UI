using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Text;
using System.Collections.Generic;

namespace NRFramework
{
    [CustomEditor(typeof(UIViewBehaviour))]
    public abstract class UIViewBehaviourEditor : Editor
    {
        protected ReorderableList m_OpElementListRL;
        private SerializedProperty m_UIOpenAnimTypeSP;
        private SerializedProperty m_UICloseAnimTypeSP;

        protected virtual void OnEnable()
        {
            m_UIOpenAnimTypeSP = serializedObject.FindProperty("m_UIOpenAnim");
            m_UICloseAnimTypeSP = serializedObject.FindProperty("m_UICloseAnim");
            m_OpElementListRL = CreateReorderableList(serializedObject.FindProperty("m_OpElementList"));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 4);

            Enum uiOpenAnimTypeEnum = EditorGUILayout.EnumPopup("UIOpenAnimType", (UIOpenAnimType)m_UIOpenAnimTypeSP.enumValueIndex);
            m_UIOpenAnimTypeSP.enumValueIndex = (int)(UIOpenAnimType)uiOpenAnimTypeEnum;
            Enum uiCloseAnimTypeEnum = EditorGUILayout.EnumPopup("UICloseAnimType", (UICloseAnimType)m_UICloseAnimTypeSP.enumValueIndex);
            m_UICloseAnimTypeSP.enumValueIndex = (int)(UICloseAnimType)uiCloseAnimTypeEnum;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 4);

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

            //移除所有target为Null 或 componentList 为空的元素
            for (int i = listSP.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty elementSP = listSP.GetArrayElementAtIndex(i);
                SerializedProperty targetSP = elementSP.FindPropertyRelative("m_Target");
                SerializedProperty componentListSP = elementSP.FindPropertyRelative("m_ComponentList");

                if (targetSP.objectReferenceValue == null || componentListSP.arraySize == 0)
                {
                    //注意：这里删除直接DeleteArrayElementAtIndex即可。（不要先置为null，会报错）
                    listSP.DeleteArrayElementAtIndex(i);
                }
            }

            listSP.serializedObject.ApplyModifiedProperties();
        }

        #region 代码生成相关
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

        protected void GetExportBaseCodeStrs(out string variantsDefineStr, out string bindCompsStr, out string bindEventsStr, out string unbindEventsStr, out string unbindCompsStr) 
        {
            HashSet<string> canBindEventCompSet = new HashSet<string>()
            { "Button", "Toggle", "Dropdown", "InputField", "Slider", "Scrollbar", "ScrollRect" };

            string variantsDefineTempalte = "protected ${CompType} m_${GoName}_${CompName};";
            string bindCompsLine = "m_${GoName}_${CompName} = (${CompType})viewBehaviour.GetComponentByIndexs(${i}, ${j});";
            string bindEventsLine = "BindEvent(m_${GoName}_${CompName});";
            string unbindEventsLine = "UnbindEvent(m_${GoName}_${CompName});";
            string unbindCompsLine = "m_${GoName}_${CompName} = null;";

            UIViewBehaviour uwb = (UIViewBehaviour)target;

            StringBuilder vdsb = new StringBuilder();
            StringBuilder bcsb = new StringBuilder();
            StringBuilder besb = new StringBuilder();
            StringBuilder ubesb = new StringBuilder();
            StringBuilder ubcsb = new StringBuilder();

            for (int i = 0; i < uwb.opElementList.Count; i++)
            {
                UIOpElement opElement = uwb.opElementList[i];
                for (int j = 0; j < opElement.componentList.Count; j++)
                {
                    Component comp = opElement.componentList[j];
                    string compType = comp.GetType().Name;
                    string goName = UIEditorUtility.GetFormatedGoName(comp.gameObject.name);
                    string compName = UIEditorUtility.GetCompShortName(compType);

                    string vdLine = new string(variantsDefineTempalte);
                    vdLine = vdLine.Replace("${CompType}", compType);
                    vdLine = vdLine.Replace("${GoName}", goName);
                    vdLine = vdLine.Replace("${CompName}", compName);
                    vdsb.Append("\r\t").Append(vdLine);

                    string bcLine = new string(bindCompsLine);
                    bcLine = bcLine.Replace("${CompType}", compType);
                    bcLine = bcLine.Replace("${GoName}", goName);
                    bcLine = bcLine.Replace("${CompName}", compName);
                    bcLine = bcLine.Replace("${i}", i.ToString());
                    bcLine = bcLine.Replace("${j}", j.ToString());
                    bcsb.Append("\r\t\t").Append(bcLine);

                    if (canBindEventCompSet.Contains(compName))
                    {
                        string beLine = new string(bindEventsLine);
                        beLine = beLine.Replace("${GoName}", goName);
                        beLine = beLine.Replace("${CompName}", compName);
                        besb.Append("\r\t\t").Append(beLine);

                        string ubeLine = new string(unbindEventsLine);
                        ubeLine = ubeLine.Replace("${GoName}", goName);
                        ubeLine = ubeLine.Replace("${CompName}", compName);
                        ubesb.Append("\r\t\t").Append(ubeLine); ;
                    }

                    string ubcLine = new string(unbindCompsLine);
                    ubcLine = ubcLine.Replace("${GoName}", goName);
                    ubcLine = ubcLine.Replace("${CompName}", compName);
                    ubcsb.Append("\r\t\t").Append(ubcLine);
                }
            }

            variantsDefineStr = vdsb.ToString();
            bindCompsStr = bcsb.ToString();
            bindEventsStr = besb.ToString();
            unbindEventsStr = ubesb.ToString();
            unbindCompsStr = ubcsb.ToString();
        }

        #endregion
    }
}