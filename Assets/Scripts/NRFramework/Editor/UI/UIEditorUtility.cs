using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NRFramework
{
    public class UIEditorUtility
    {
        public const string kUIBaseCode = @"
using UnityEngine;
using UnityEngine.UI;
using NRFramework;

public class ${ClassName} : ${BaseClassName}
{
    ${VariantsDefine}

    protected override void OnBindCompsAndEvents()
    {
        ${BindComps}

        ${BindEvents}
    }

    protected override void OnUnbindCompsAndEvents() 
    {
        ${UnbindEvents}

        ${UnbindComps}
    }
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

    protected override void OnButtonClicked(Button button) { }

    protected override void OnToggleValueChanged(Toggle toggle, bool value) { }

    protected override void OnDropdownValueChanged(Dropdown dropdown, int value) { }

    protected override void OnInputFieldValueChanged(InputField inputField, string value) { }

    protected override void OnSliderValueChanged(Slider slider, float value) { }

    protected override void OnScrollbarValueChanged(Scrollbar scrollbar, float value) { }

    protected override void OnScrollRectValueChanged(ScrollRect scrollRect, Vector2 value) { }
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
