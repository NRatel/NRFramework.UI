// https://github.com/NRatel/NRFramework.UI

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
using TMPro;
using NRFramework;

public class ${ClassName} : ${BaseClassName}
{
    protected override void OnCreating() { }

    protected override void OnCreated() { }

    protected override void OnClicked(Button button) { }

    protected override void OnValueChanged(Toggle toggle, bool value) { }

    protected override void OnValueChanged(Dropdown dropdown, int value) { }

    protected override void OnValueChanged(TMP_Dropdown tmpDropdown, int value) { }

    protected override void OnValueChanged(InputField inputField, string value) { }

    protected override void OnValueChanged(TMP_InputField tmpInputField, string value) { }

    protected override void OnValueChanged(Slider slider, float value) { }

    protected override void OnValueChanged(Scrollbar scrollbar, float value) { }

    protected override void OnValueChanged(ScrollRect scrollRect, Vector2 value) { }
    ${PanelLifeCycleCode}
    protected override void OnDestroying() { }

    protected override void OnDestroyed() { }
}";

        public const string kPanelLifeCycleCode = @"
    protected override void OnVisibleChanged(bool visible) { }
    
    protected override void OnFocusChanged(bool got) { }

    //protected override void OnBackgroundClicked() { }

    //protected override void OnEscButtonPressed() { }
";

        static public void GenerateCode(string savePath, string content)
        {
            string saveDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(saveDir)) { Directory.CreateDirectory(saveDir); }

            File.WriteAllText(savePath, content, Encoding.UTF8);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string scriptAssetPath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, savePath));
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<TextAsset>(scriptAssetPath));
        }

        /// <summary>
        /// 保留字母数字下划线、首字母大写
        /// </summary>
        /// <param name="goName"></param>
        /// <returns></returns>
        static public string GetFormatedGoName(string goName)
        {
            goName = Regex.Replace(goName, @"[^a-zA-Z0-9_]", "");
            goName = Regex.Replace(goName, @"^\w", t => t.Value.ToUpper());
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
                {"GridLayoutGroup", "GLayoutGroup"},

                {"TextMeshProUGUI", "TMPText"},
                {"TMP_Dropdown", "TMPDropdown"},
                {"TMP_InputField", "TMPInputField"},
            };

            return dict.ContainsKey(compName) ? dict[compName] : compName;
        }

        static public Texture GetIconByType(Type type)
        {
            //系统内置图标
            Texture systemIcon = EditorGUIUtility.ObjectContent(null, type).image;

            //自定义组件图标 
            Texture customIcon = null;

            //TMP 三个组件的图标
            if (type == typeof(TMPro.TMP_InputField))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Input Field Icon.psd");
            }
            else if (type == typeof(TMPro.TMP_Dropdown))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd");
            }
            else if (type == typeof(TMPro.TextMeshProUGUI))
            {
                customIcon = (Texture2D)EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Text Component Icon.psd");
            }

            //todo其他自定义图标自行添加

            //默认图标
            Texture csScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            return systemIcon ?? customIcon ?? csScriptIcon;
        }
    }
}
