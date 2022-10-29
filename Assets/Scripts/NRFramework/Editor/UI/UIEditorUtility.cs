using System;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    public class UIEditorUtility
    {
        static public Texture GetIconByType(Type type)
        {
            //系统内置图标
            Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;

            //自定义组件图标   //todo自行添加
            Texture customIcon = null;

            //默认图标
            Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            return systemIcon ?? customIcon ?? csScriptIcon;
        }
    }
}
