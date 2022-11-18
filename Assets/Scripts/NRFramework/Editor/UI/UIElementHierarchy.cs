// https://github.com/NRatel/NRFramework.UI

using UnityEngine;
using UnityEditor;

namespace NRFramework
{
    [InitializeOnLoad]
    public class UIElementHierarchy
    {
        static UIElementHierarchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static private void HierarchyWindowItemOnGUI(int instanceId, Rect selectRect)
        {
            if (!EditorSetting.Instance.enableOpElementHierarchy) { return; }
            if (EditorApplication.isPlayingOrWillChangePlaymode) { return; }

            GameObject go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (go == null) { return; }

            UIViewBehaviour behaviour = go.GetComponentInParent<UIViewBehaviour>(true);
            if (behaviour == null) { return; }
            
            int posIndex = 0;
            //component图标（仅已保存）
            Component[] existComps = go.GetComponents<Component>();
            for (int i = existComps.Length - 1; i >= 0; i--) //用它遍历能够保证组件在Inspector上的顺序，倒序为了从右往左绘制
            {
                Component comp = existComps[i];
                if (behaviour.HasSavedComponent(go, comp))     //只显示已保存的组件
                {
                    Texture icon = UIEditorUtility.GetIconByType(comp.GetType());
                    DrawIcon(selectRect, posIndex, icon);
                    posIndex++;
                }
            }
            ////gameObject图标 (废弃了)
            //if (behaviour.HasSavedGameObject(go))
            //{
            //    Texture icon = UIEditorUtility.GetIconByType(go.GetType());
            //    DrawIcon(selectRect, posIndex, icon);
            //    posIndex++;
            //}
        }

        static private void DrawIcon(Rect selectRect, int posIndex, Texture icon)
        {
            float iconWidth = EditorGUIUtility.singleLineHeight * 0.8f;
            float iconHeight = iconWidth;
            float rightPadding = 5;
            float iconSpacing = 5;

            float iconX = selectRect.x + selectRect.width - rightPadding - (iconWidth + iconSpacing) * posIndex;  //水平居右
            float iconY = selectRect.y + (selectRect.height - iconHeight) / 2;  //竖直居中

            Rect iconRect = new Rect(iconX, iconY, iconWidth, iconHeight);
            GUI.DrawTexture(iconRect, icon);
        }
    }

}