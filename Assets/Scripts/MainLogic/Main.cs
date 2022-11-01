using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Main : MonoBehaviour
{
    private void Start()
    {
        string t = "(TMP_XXX)";
        string t2 = "£¨TMP_XXX£©*&¡­¡­%/*-";
        string goName = Regex.Replace(t, @"[^a-zA-Z0-9_]", "");
        string goName2 = Regex.Replace(t2, @"[^a-zA-Z0-9_]", "");

        Debug.Log("ttt: " + t);
        Debug.Log("ttt: " + t2);
        Debug.Log("goName: " + goName);
        Debug.Log("goName: " + goName2);
    }
}