using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace NRFramework
{
    /// <summary>
    /// 将游戏物体组件加入最近一层父 UIViewBehaviour 中。
    /// 规则如下（推测出最可能要操作的）：
    /// 1、如果存在【可交互组件】，则加入。
    /// 2、否则，如果存在【布局相关组件】，则加入。
    /// 3、否则，如果存在【自定义需要添加的组件】，则加入（优先级相对较低，但高于图形）。
    /// 4、否则，如果存在【图形组件】，则加入。
    /// 5、否则，将 【RectTransform 或 Transform】 加入（保底）。
    /// (若要操作多个组件，请手动添加)
    /// (若要操作自定义的脚本，请手动添加)
    /// (嵌套的子 UIViewBehaviour 是否加入父 UIViewBehaviour 中？ 否，如有需要就手动添加。)
    /// </summary>
    public class SetAsUIOpElement : Editor
    {
        static private HashSet<string> sm_CustomSet = new HashSet<string>() 
        {
            "ScrollRect",
            "Canvas", "Camera",
            "EventTrigger",
            "AudioListener", "AudioSource",
            "VideoPlayer",
            "Animator", "Animation",
            "PlayableDirector",
            //...
        };

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
                        break; //若component已保存，则跳出
                    }

                    //可交互组件
                    if (comp is Selectable)
                    {
                        opElement.componentList.Add(comp);
                        break;
                    }
                    //布局相关组件
                    if (comp is LayoutGroup || comp is ContentSizeFitter || comp is AspectRatioFitter || comp is LayoutElement)
                    {
                        opElement.componentList.Add(comp);
                        break;
                    }
                    if (sm_CustomSet.Contains(comp.name))
                    {
                        opElement.componentList.Add(comp);
                        break;
                    }
                    //图形组件
                    else if (comp is Graphic)
                    {
                        opElement.componentList.Add(comp);
                        break;
                    }
                    //RectTransform 或 Transform。
                    else if (comp is Transform)
                    {
                        opElement.componentList.Add(comp);
                        break;
                    }
                }
            }
        }
    }
}
