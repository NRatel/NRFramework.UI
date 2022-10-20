using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace NRFramework
{
    [MonoSingletonSetting(HideFlags.NotEditable)]
    public class HttpBehaviour : MonoSingleton<HttpBehaviour> { }

    public class Http
    {
        static IEnumerator Post(UnityWebRequest request, Action<DownloadHandler> onResponded)
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                using (request)
                {
                    onResponded(request.downloadHandler);
                }
            }
            else
            {
                Debug.LogError("retCode: " + request.responseCode + "\n, errorMsg: " + request.error);
            }
        }

        static public void Post(string url, byte[] data, Action<DownloadHandler> onResponded)
        {
            //考虑每次 new DownloadHandlerBuffer、 new UploadHandlerRaw 是否要优化
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

            HttpBehaviour.Instance.StartCoroutine(Post(www, onResponded));
        }

        static public void Post(string url, string data, Action<string> onResponded)
        {
            Post(url, System.Text.Encoding.UTF8.GetBytes(data), (downloadHandler) => { onResponded(downloadHandler.text); });
        }
    }
}