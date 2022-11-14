// https://github.com/NRatel/NRFramework.UI

using UnityEngine;
using UnityEditor;

namespace NRFramework
{
    /// <summary>
    /// 意图: 
    ///     调整选中物体
    ///     使其位置等于 所有子物体（不包含自身）的总中心。
    ///     使其大小等于 所有子物体（不包含自身）的总大小。
    ///     调整后，选中物体的锚点、中心点保持不变。
    ///     调整后，选中物体的所有子物体的世界坐标均不变。
    /// 思路：
    ///     第一步：计算所有子物体（不包含自身）的总中心 和 总大小,
    ///     第二步：将Box大小设为所有子物体的总大小、将Box移动至所有子物体的总中心。
    ///     第三步：将子物体移动至原位置。
    /// </summary>
    public class AdjustUIBox : Editor
    {
        [MenuItem("GameObject/NRUITools/AdjustUIBox", false, 3)]
        public static void DoAdjust()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length <= 0) { return; }
            if (Selection.gameObjects.Length > 1)
            {
                EditorUtility.DisplayDialog("Error", "只允许同时处理一个物体", "OK");
                return;
            }

            Transform box = Selection.gameObjects[0].transform;
            Transform boxParent = box.parent;
            Transform child0 = box.GetChild(0);

            //计算所有子物体（不包含自身）的总中心 和 总大小
            Bounds boundsInBox = RectTransformUtility.CalculateRelativeRectTransformBounds(box, child0); //box坐标系下
            Bounds boundsInBoxParent = RectTransformUtility.CalculateRelativeRectTransformBounds(boxParent, child0); //boxParent坐标系下
            for (int i = 1; i < box.childCount; i++)
            {
                Transform childi = box.GetChild(i);
                boundsInBox.Encapsulate(RectTransformUtility.CalculateRelativeRectTransformBounds(box, childi));
                boundsInBoxParent.Encapsulate(RectTransformUtility.CalculateRelativeRectTransformBounds(boxParent, childi));
            }

            //Debug.Log("box: " + boundsInBox.center + ", " + boundsInBox.size);
            //Debug.Log("boxParent: " + boundsInBoxParent.center + ", " + boundsInBoxParent.size);

            Undo.IncrementCurrentGroup();
            int groupIndex = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("AdjustUIBox");
            {
                //设置box位置
                RectTransform boxRT = box as RectTransform;
                Undo.RecordObject(boxRT, "boxRT");
                boxRT.localPosition = boundsInBoxParent.center;
                boxRT.sizeDelta = new Vector2(boundsInBoxParent.size.x, boundsInBoxParent.size.y);  //修改box大小，只会影响 anchoredPosition，不会影响localPosition。

                //设置子物体位置
                for (int i = 0; i < box.childCount; i++)
                {
                    Transform child = box.GetChild(i);
                    Undo.RecordObject(child, "boxChild" + i);
                    child.localPosition = child.localPosition - boundsInBox.center;
                }
            }
            Undo.CollapseUndoOperations(groupIndex);
        }
    }
}