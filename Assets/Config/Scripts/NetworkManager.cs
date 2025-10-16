using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Flaskサーバーとの接続用クラス
public class NetworkManager : MonoBehaviour
{
    // Flaskの "/RecordResult" のURLへPOST送信する
    private string apiUrl = "http://127.0.0.1:5000/RecordResult";

    public void PostUserData(string json)
    {
        StartCoroutine(PostRequest(json));
    }

    private IEnumerator PostRequest(string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("サーバ応答しました: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("通信エラー: " + request.error);
            }
        }
    }
}
