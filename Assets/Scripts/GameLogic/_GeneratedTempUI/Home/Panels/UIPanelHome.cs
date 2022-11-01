using System;
using UnityEngine;
using UnityEngine.UI;
using NRFramework;

public class UIPanelHome : UIPanelHomeBase
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
    

    protected override void OnFoucus(bool got) { }

    protected override void OnEscButtonClicked() { }

    protected override void OnWindowBgClicked() { }

    protected override void OnClosing() { }

    protected override void OnClosed() { }
}