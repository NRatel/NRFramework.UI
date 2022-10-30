using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UICodeGenerator
{
    public const string kPanelBase = @"

";
    public const string kPanelTemplate = @"
using System;
using UnityEngine;
using NRFramework;

public class ${ClassName} : ${ClassName}Base
{
    protected override void OnCreating() { }

    protected override void OnCreated() { }

    ///// <summary>
    ///// 同步初始化/刷新示例
    ///// </summary>
    //public void InitOrRefresh_Sync(object data)
    //{
    //    ShowWithData(data);
    //    panelShowState = UIPanelShowState.Idle;
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
    //            panelShowState = UIPanelShowState.Idle;
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

    protected override void OnFoucus(bool got) { }

    protected override void OnEscButtonClicked() { }

    protected override void OnWindowBgClicked() { }

    protected override void OnClosing() { }

    protected override void OnClosed() { }
}";

    static public void DoGenerate(string savePath, string content)
    {
        string saveDir = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(saveDir)) { Directory.CreateDirectory(saveDir); }

        File.WriteAllText(savePath, content);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string scriptAssetPath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(scriptAssetPath));
    }

}
