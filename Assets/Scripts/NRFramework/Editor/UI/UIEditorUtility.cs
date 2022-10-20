using System;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    public class UIEditorUtility
    {
        static public Texture GetIconByType(Type type)
        {
            //系统图标
            Texture icon = EditorGUIUtility.ObjectContent(null, type).image;

            //自定义组件图标
            //todo

            //默认图标
            Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            return icon ?? csScriptIcon;
        }
    }
}
