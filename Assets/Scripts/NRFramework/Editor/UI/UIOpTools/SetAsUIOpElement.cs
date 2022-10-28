using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NRFramework
{
    /// <summary>
    /// 将游戏物体及其组件加入最近一层父UIBehaviour中。//规则如下：
    /// RectTransform 或 Transform：必然加入。
    /// 可交互组件：如果存在，必然加入。
    /// 图形组件：如果存在，必然加入。
    /// UIBehaviour 是否加入父UIBehaviour中？ 否。可能会出一些问题，如果需要就手动添加。
    /// 自定义组件：需要针对性处理。
    /// </summary>
    public class SetAsUIOpElement : Editor
    {
        [MenuItem("GameObject/NRFrameworkUITools/SetAsUIOpElement", false, 0)]
        static public void DoSet()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length <= 0) { return; }

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject go = Selection.gameObjects[i];
                UIViewBehaviour behaviour = go.GetComponentInParent<UIViewBehaviour>(true);

                if (behaviour == null) { return; }

                //找到第一个已保存的element或创建
                UIOpElement opElement = behaviour.opElementList.Find((x) => { return go.Equals(x.target); });
                if (opElement == null)
                {
                    opElement = new UIOpElement();
                    opElement.target = go;
                    behaviour.opElementList.Add(opElement);
                }

                Component[] existComps = go.GetComponents<Component>();
                for (int j = 0; j < existComps.Length; j++)
                {
                    Component comp = existComps[j];
                    if (behaviour.HasSavedComponent(go, comp))
                    {
                        continue; //若component已保存，则忽略
                    }

                    //RectTransform 或 Transform：必然加入。
                    if (comp is Transform)
                    {
                        opElement.componentList.Add(comp);
                        continue;
                    }
                    //可交互组件
                    if (comp is Selectable)
                    {
                        opElement.componentList.Add(comp);
                        continue;
                    }
                    //图形组件
                    if (comp is Graphic)
                    {
                        opElement.componentList.Add(comp);
                        continue;
                    }
                    //自定义组件
                    //todo 针对性处理
                }
            }
        }
    }
}
