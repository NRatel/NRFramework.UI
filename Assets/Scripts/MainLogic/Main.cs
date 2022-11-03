using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        //string t = "(TMP_XXX)";
        //string t2 = "£¨TMP_XXX£©*&¡­¡­%/*-";
        //string goName = Regex.Replace(t, @"[^a-zA-Z0-9_]", "");
        //string goName2 = Regex.Replace(t2, @"[^a-zA-Z0-9_]", "");

        //Debug.Log("ttt: " + t);
        //Debug.Log("ttt: " + t2);
        //Debug.Log("goName: " + goName);
        //Debug.Log("goName: " + goName2);

        //Texture2D customIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/A/B/nratel.png", typeof(Texture2D));

        Texture2D customIcon1 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor Resources/Gizmos/nratel.png", typeof(Texture2D));
        Texture2D customIcon2 = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor Resources/Gizmos/TMP - Dropdown Icon.psd", typeof(Texture2D));
        Texture2D customIcon3 = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd", typeof(Texture2D));

        Texture2D customIcon4 = EditorGUIUtility.Load("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Dropdown Icon.psd") as Texture2D;


        Debug.Log(customIcon1);
        Debug.Log(customIcon2);
        Debug.Log(customIcon3);
        Debug.Log(customIcon4);
    }
}