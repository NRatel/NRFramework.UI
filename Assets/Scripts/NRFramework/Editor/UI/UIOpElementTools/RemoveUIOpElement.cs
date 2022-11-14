// https://github.com/NRatel/NRFramework.UI

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    /// <summary>
    /// 将选择的物体，从最近一层父 UIViewBehaviour 中移除
    /// （移除所有Target为该物体的行记录）
    /// </summary>
    public class RemoveUIOpElement : Editor
    {
        [MenuItem("GameObject/NRUITools/RemoveUIOpElement &r", false, 1)]
        static public void DoRemove()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length <= 0) { return; }

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject go = Selection.gameObjects[i];
                UIViewBehaviour behaviour = go.GetComponentInParent<UIViewBehaviour>(true);

                if (behaviour == null) { return; }

                List<UIOpElement> opElements = behaviour.opElementList.FindAll((x) => { return go.Equals(x.target); });
                for (int j = opElements.Count - 1; j >= 0; j--)
                {
                    behaviour.opElementList.RemoveAt(j);
                }

                EditorUtility.SetDirty(behaviour);
            }

            AssetDatabase.Refresh();
        }
    }
}
