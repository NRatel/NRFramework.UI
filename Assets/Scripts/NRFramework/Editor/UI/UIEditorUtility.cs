using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    public class UIEditorUtility
    {
        public const string kUIBaseCode = @"
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NRFramework;

public class ${ClassName} : ${BaseClassName}
{${VariantsDefine}
    protected override void OnBindCompsAndEvents() 
    {${BindComps}${BindEvents}}

    protected override void OnUnbindCompsAndEvents() 
    {${UnbindEvents}${UnbindComps}}
}";

        public const string kUITemporaryCode = @"
using System;
using UnityEngine;
using UnityEngine.UI;
using NRFramework;

public class ${ClassName} : ${BaseClassName}
{
    protected override void OnCreating() { }

    protected override void OnCreated() { }

    ///// <summary>
    ///// 同步初始化/刷新示例
    ///// </summary>
    //public void InitOrRefresh_Sync(object data)
    //{
    //    ShowWithData(data);
    //    showState = UIShowState.Idle;
    //}

    ///// <summary>
    ///// 异步初始化/刷新示例
    ///// </summary>
    //public void InitOrRefresh_Async(object data1, Action onInited)
    //{
    //    GetData2((data2) =>      //异步获取数据
    //    {
    //        ShowWithDatas(data1, data2, () =>     //异步显示
    //        {
    //            showState = UIShowState.Idle;
    //            onInited();
    //        });
    //    });
    //}

    protected override void OnClicked(Button button) { }

    protected override void OnValueChanged(Toggle toggle, bool value) { }

    protected override void OnValueChanged(Dropdown dropdown, int value) { }

    protected override void OnValueChanged(InputField inputField, string value) { }

    protected override void OnValueChanged(Slider slider, float value) { }

    protected override void OnValueChanged(Scrollbar scrollbar, float value) { }

    protected override void OnValueChanged(ScrollRect scrollRect, Vector2 value) { }
    ${PanelLifeCycleCode}
    protected override void OnClosing() { }

    protected override void OnClosed() { }
}";

        public const string kPanelLifeCycleCode = @"
    protected override void OnFoucus(bool got) { }

    protected override void OnEscButtonClicked() { }

    protected override void OnWindowBgClicked() { }
";

        static public void GenerateCode(string savePath, string content)
        {
            string saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir)) { Directory.CreateDirectory(saveDir); }

            File.WriteAllText(savePath, content);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string scriptAssetPath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(scriptAssetPath));
        }

        /// <summary>
        /// 保留字母数字下划线
        /// </summary>
        /// <param name="goName"></param>
        /// <returns></returns>
        static public string GetFormatedGoName(string goName)
        {
            goName = Regex.Replace(goName, @"[^a-zA-Z0-9_]", "");
            return goName;
        }

        /// <summary>
        /// 取组件短名
        /// 将过长的缩写即可，默认使用原名
        /// （主要是太长且多个单词的）
        /// </summary>
        /// <returns></returns>
        static public string GetCompShortName(string compName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"VerticalLayoutGroup", "VLayoutGroup"},
                {"HorizontalLayoutGroup","HLayoutGroup"},
                {"GridLayoutGroup", "GLayoutGroup"}
            };

            return dict.ContainsKey(compName) ? dict[compName] : compName;
        }

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
