using System.Collections;
using UnityEngine;

namespace NRFramework
{
    static public class FindCompErrorCode
    {
        //UIView中
        public const int OK = 0;
        public const int ERROR_CAST_TYPE = 1001;                //错误的组件转换类型
        public const int COMP_DEFINE_IS_NULL_OR_EMPTY = 1002;   //compDefine为null或""
        public const int NOT_EXIST_THIS_COMPONENT = 1003;       //View中不存在此组件定义
        public const int NOT_EXIST_ANY_CHILD_WIDGET = 1004;     //View中不存在任何子Widget(不存在此Widget)
        public const int WIDGETS_ID_IS_NULL_OR_EMPTY = 1005;    //widgetIds为null或""
        public const int NOT_EXIST_THIS_CHILD_WIDGET = 1006;    //View不存在此Widget

        //UIRoot
        public const int PANEL_ID_IS_NULL_OR_EMPTY = 1007;      //panelId为null或""
        public const int NOT_EXIST_THIS_PANEL = 1008;           //Root中不存在此Panel

        //UIManager中
        public const int NOT_EXIST_THIS_ROOT = 1009;            //UIManager中不存在此Root
        public const int VIEW_PATH_IS_NULL_OR_EMPTY = 1010;     //viewPath为null或""
        public const int VIEW_PATH_IS_TOO_SHORT = 1011;         //ViewPath应该至少包含一个rootId和一个panelId
    }
}