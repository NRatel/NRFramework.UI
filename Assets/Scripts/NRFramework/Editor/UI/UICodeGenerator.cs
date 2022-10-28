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
    ///// ͬ����ʼ��/ˢ��ʾ��
    ///// </summary>
    //public void InitOrRefresh_Sync(object data)
    //{
    //    ShowWithData(data);
    //    panelShowState = UIPanelShowState.Idle;
    //}

    ///// <summary>
    ///// �첽��ʼ��/ˢ��ʾ��
    ///// </summary>
    //public void InitOrRefresh_Async(object data1, Action onInited)
    //{
    //    GetData2((data2) =>      //�첽��ȡ����
    //    {
    //        ShowWithDatas(data1, data2, () =>     //�첽��ʾ
    //        {
    //            panelShowState = UIPanelShowState.Idle;
    //            onInited();
    //        });
    //    });
    //}

    protected override void OnFoucus(bool got) { }

    protected override void OnClickBackBtn() { }

    protected override void OnClickWindowBg() { }

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
