using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NRFramework
{
    public class Http
    {
        static string defaultUrl = "http://127.0.0.1:8081/game";

        static IEnumerator Post(UnityWebRequest www, Action<DownloadHandler> onResponded)
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("retCode: " + www.responseCode + "\n, errorMsg: " + www.error);
            }
            else
            {
                using (www)
                {
                    onResponded(www.downloadHandler);
                }
            }
        }

        static public void Post(string url, byte[] data, Action<DownloadHandler> onResponded)
        {
            //每次 new DownloadHandlerBuffer、 new UploadHandlerRaw 是否要优化
            UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST, new DownloadHandlerBuffer(), new UploadHandlerRaw(data));

            //application/x-www-form-urlencoded
            //www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            //www.SetRequestHeader("Accept", "application/x-www-form-urlencoded");

            //application/json
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            //application/octet-stream
            //www.SetRequestHeader("Content-Type", "application/octet-stream");
            //www.SetRequestHeader("Accept", "application/octet-stream");

            //todo
            //XXX.StartCoroutine(Post(www, onResponded));
        }

        static public void Post(string url, string data, Action<string> onResponded)
        {
            Post(url, System.Text.Encoding.UTF8.GetBytes(data), (downloadHandler) => { onResponded(downloadHandler.text); });
        }

        static public void Post(byte[] data, Action<byte[]> onResponded)
        {
            Post(defaultUrl, data, (downloadHandler) => { onResponded(downloadHandler.data); });
        }

        static public void Post(string data, Action<string> onResponded)
        {
            Post(defaultUrl, data, onResponded);
        }
    }
}