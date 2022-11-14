// https://github.com/NRatel/NRFramework.UI

using UnityEngine;
using UnityEditor;
using System.Linq;

namespace NRFramework
{
    /// <summary>
    /// 意图：
    ///     为选中的多个同级物体增加一个Box，
    ///     使其位置等于 所有选中物体的总中心
    ///     使其大小等于 所有选中物体的总大小。
    ///     执行后，选中物体的世界坐标均不变。
    /// 思路：
    ///     第一步：计算选中物体的总中心、总大小。
    ///     第二步：创建Box，设置位置和大小。
    ///     第三步：将选中物体依次挂入Box（选中物体世界坐标不发生变化）。
    /// </summary>
    public class CreateUIBox : Editor
    {
        [MenuItem("GameObject/NRUITools/CreateUIBox", false, 2)]
        public static void DoCreate()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length <= 0) { return; }
            if (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].name == "#UIBox#") { return; } //防止多次操作

            Transform parent = Selection.gameObjects[0].transform.parent;

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (Selection.gameObjects[i].transform.parent != parent)
                {
                    EditorUtility.DisplayDialog("Error", "暂只允许处理同级物体", "OK");
                    return;
                }
            }

            //计算选择物体的总Bounds
            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, Selection.gameObjects[0].transform);
            for (int i = 1; i < Selection.gameObjects.Length; i++)
            {
                Bounds b = RectTransformUtility.CalculateRelativeRectTransformBounds(parent, Selection.gameObjects[i].transform);
                bounds.Encapsulate(b);
            }

            Undo.IncrementCurrentGroup();
            int groupIndex = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("CreateUIBox");
            {
                //创建Box
                GameObject box = new GameObject("#UIBox#", typeof(RectTransform));
                Undo.RegisterCreatedObjectUndo(box, "CreateUIBox");

                //设置box位置、大小
                RectTransform boxRT = box.GetComponent<RectTransform>();
                boxRT.SetParent(parent);
                boxRT.localPosition = bounds.center;
                boxRT.localScale = Vector3.one;
                boxRT.sizeDelta = new Vector2(bounds.size.x, bounds.size.y);

                //将选择的物体按原Sibling挂入
                GameObject[] sortedObjs = Selection.gameObjects.OrderBy(x => x.transform.GetSiblingIndex()).ToArray();
                for (int i = 0; i < sortedObjs.Length; i++)
                {
                    Undo.SetTransformParent(sortedObjs[i].transform, boxRT, "MoveItemToBox");
                }
                Selection.activeGameObject = box;
            }
            Undo.CollapseUndoOperations(groupIndex);
        }
    }
}