// https://github.com/NRatel/NRFramework.UI

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

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

        protected void DrawOpElementList()
        {
            m_OpElementListRL.DoLayoutList();
        }

        protected void DrawExpoertButton()
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("ExportBase"))
                {
                    if (Application.isPlaying) { Debug.LogError("请在非运行时导出"); return; }
                    RefreshOpElementList(m_OpElementListRL);
                    GenerateUIBaseCode();
                }

                if (GUILayout.Button("ExportTemp"))
                {
                    if (Application.isPlaying) { Debug.LogError("请在非运行时导出"); return; }
                    RefreshOpElementList(m_OpElementListRL);
                    GenerateUITempCode();
                }
            }
            GUILayout.EndHorizontal();
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
            //4、增加一个清空按钮。
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
                int btnsCount = 4;

                float rightEdge = rect.xMax - rightMargin;
                float btnsWidth = leftPading + rightPading + singleWidth * btnsCount + spacing * (btnsCount - 1);
                Rect btnsRect = new Rect(rightEdge - btnsWidth, rect.y, btnsWidth, rect.height);

                Rect addRect = new Rect(btnsRect.x + leftPading, btnsRect.y, singleWidth, singleHeight);
                Rect removeRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 1, btnsRect.y, singleWidth, singleHeight);
                Rect trashRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 2, btnsRect.y, singleWidth, singleHeight);
                Rect refreshRect = new Rect(btnsRect.x + leftPading + (singleWidth + spacing) * 3, btnsRect.y, singleWidth, singleHeight);

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
                    Texture icon = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
                    if (GUI.Button(trashRect, new GUIContent(icon), defaults.preButton))
                    {
                        TrashOpElementList(list);
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

        private void TrashOpElementList(ReorderableList list)
        {
            SerializedProperty listSP = list.serializedProperty;
            listSP.ClearArray();

            listSP.serializedObject.ApplyModifiedProperties();
        }

        private void RefreshOpElementList(ReorderableList list)
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
        private string GetPrefabPath()
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

        private void GenerateUIBaseCode()
        {
            string prefabPath = GetPrefabPath();

            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError("非预设不可导出");
                return;
            }

            string fullPrefabPath = Path.GetFullPath(Path.Combine(Application.dataPath, Path.GetRelativePath("Assets", prefabPath)));
            string fullRootDir = Path.GetFullPath(Path.Combine(Application.dataPath, EditorSetting.Instance.uiPrefabRootDir));

            //Debug.Log("fullPrefabPath: " + fullPrefabPath);
            //Debug.Log("uiPrefabRootDir: " + fullRootDir);

            if (!fullPrefabPath.StartsWith(fullRootDir))
            {
                Debug.LogError("预设不在可导出的根目录中：" + fullRootDir);
                return;
            }

            string subPath = Path.GetRelativePath(fullRootDir, fullPrefabPath);
            string className = Path.GetFileNameWithoutExtension(subPath);
            string subSavePath = Path.Combine(Path.GetDirectoryName(subPath), className + "Base.cs");
            string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, EditorSetting.Instance.generatedBaseUIRootDir, subSavePath));

            string content = UIEditorUtility.kUIBaseCode.Replace("${ClassName}", className + "Base");
            content = content.Replace("${BaseClassName}", target is UIPanelBehaviour ? "UIPanel" : "UIWidget");

            string variantsDefineStr, bindCompsStr, bindEventsStr, unbindEventsStr, unbindCompsStr;
            int retCode = GetExportBaseCodeStrs(out variantsDefineStr, out bindCompsStr, out bindEventsStr, out unbindEventsStr, out unbindCompsStr);
            if (retCode < 0) { return; }

            content = content.Replace("${VariantsDefine}", variantsDefineStr + (!string.IsNullOrEmpty(variantsDefineStr) ? "\r" : string.Empty));
            content = content.Replace("${BindComps}", bindCompsStr);
            content = content.Replace("${BindEvents}", (!string.IsNullOrEmpty(bindEventsStr) ? "\r" : string.Empty) + bindEventsStr + "\r\t");
            content = content.Replace("${UnbindEvents}", unbindEventsStr);
            content = content.Replace("${UnbindComps}", (!string.IsNullOrEmpty(unbindEventsStr) ? "\r" : string.Empty) + unbindCompsStr + "\r\t");

            UIEditorUtility.GenerateCode(savePath, content);

            Debug.Log("Export success!");
        }

        private void GenerateUITempCode()
        {
            string prefabPath = GetPrefabPath();

            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError("非预设不可导出");
                return;
            }

            string fullPrefabPath = Path.GetFullPath(Path.Combine(Application.dataPath, Path.GetRelativePath("Assets", prefabPath)));
            string fullRootDir = Path.GetFullPath(Path.Combine(Application.dataPath, EditorSetting.Instance.uiPrefabRootDir));

            //Debug.Log("fullPrefabPath: " + fullPrefabPath);
            //Debug.Log("uiPrefabRootDir: " + fullRootDir);

            if (!fullPrefabPath.StartsWith(fullRootDir))
            {
                Debug.LogError("预设不在可导出的根目录中：" + fullRootDir);
                return;
            }

            string subPath = Path.GetRelativePath(fullRootDir, fullPrefabPath);
            string className = Path.GetFileNameWithoutExtension(subPath);
            string subSavePath = Path.Combine(Path.GetDirectoryName(subPath), className + "_Temp.cs");
            string savePath = Path.GetFullPath(Path.Combine(Application.dataPath, EditorSetting.Instance.generatedTempUIRootDir, subSavePath));

            string content = UIEditorUtility.kUITemporaryCode.Replace("${ClassName}", className + "_Temp");
            content = content.Replace("${BaseClassName}", className + "Base");
            content = content.Replace("${PanelLifeCycleCode}", target is UIPanelBehaviour ? UIEditorUtility.kPanelLifeCycleCode : "");
            content = content.Trim();

            UIEditorUtility.GenerateCode(savePath, content);

            Debug.Log("Export success!");
        }

        private int GetExportBaseCodeStrs(out string variantsDefineStr, out string bindCompsStr, out string bindEventsStr, out string unbindEventsStr, out string unbindCompsStr)
        {
            HashSet<string> canBindEventCompSet = new HashSet<string>()
            {
                "Button", "Toggle", "Dropdown", "InputField", "Slider", "Scrollbar", "ScrollRect",
                "TMP_Dropdown", "TMP_InputField",
            };

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

            Dictionary<string, string> goNameDict = new Dictionary<string, string>();

            for (int i = 0; i < uwb.opElementList.Count; i++)
            {
                UIOpElement opElement = uwb.opElementList[i];
                string formatedGoName = UIEditorUtility.GetFormatedGoName(opElement.target.name);

                //不允许重名
                if (goNameDict.ContainsKey(formatedGoName))
                {
                    Debug.LogError(string.Format("重复的GameObjectName: {0}、{1}", goNameDict[formatedGoName], opElement.target.name));

                    variantsDefineStr = string.Empty;
                    bindCompsStr = string.Empty;
                    bindEventsStr = string.Empty;
                    unbindEventsStr = string.Empty;
                    unbindCompsStr = string.Empty;

                    return -1;
                }

                for (int j = 0; j < opElement.componentList.Count; j++)
                {
                    Component comp = opElement.componentList[j];
                    string compType = comp.GetType().Name;
                    string shortCompName = UIEditorUtility.GetCompShortName(compType);

                    string vdLine = new string(variantsDefineTempalte);
                    vdLine = vdLine.Replace("${CompType}", compType);
                    vdLine = vdLine.Replace("${GoName}", formatedGoName);
                    vdLine = vdLine.Replace("${CompName}", shortCompName);
                    vdsb.Append("\r\t").Append(vdLine);

                    string bcLine = new string(bindCompsLine);
                    bcLine = bcLine.Replace("${CompType}", compType);
                    bcLine = bcLine.Replace("${GoName}", formatedGoName);
                    bcLine = bcLine.Replace("${CompName}", shortCompName);
                    bcLine = bcLine.Replace("${i}", i.ToString());
                    bcLine = bcLine.Replace("${j}", j.ToString());
                    bcsb.Append("\r\t\t").Append(bcLine);

                    if (canBindEventCompSet.Contains(compType))
                    {
                        string beLine = new string(bindEventsLine);
                        beLine = beLine.Replace("${GoName}", formatedGoName);
                        beLine = beLine.Replace("${CompName}", shortCompName);
                        besb.Append("\r\t\t").Append(beLine);

                        string ubeLine = new string(unbindEventsLine);
                        ubeLine = ubeLine.Replace("${GoName}", formatedGoName);
                        ubeLine = ubeLine.Replace("${CompName}", shortCompName);
                        ubesb.Append("\r\t\t").Append(ubeLine); ;
                    }

                    string ubcLine = new string(unbindCompsLine);
                    ubcLine = ubcLine.Replace("${GoName}", formatedGoName);
                    ubcLine = ubcLine.Replace("${CompName}", shortCompName);
                    ubcsb.Append("\r\t\t").Append(ubcLine);
                }
            }

            variantsDefineStr = vdsb.ToString();
            bindCompsStr = bcsb.ToString();
            bindEventsStr = besb.ToString();
            unbindEventsStr = ubesb.ToString();
            unbindCompsStr = ubcsb.ToString();

            return 0;
        }
        #endregion
    }
}