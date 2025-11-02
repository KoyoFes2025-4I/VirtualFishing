using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Flaskサーバーとの接続用クラス
public class NetworkManager : MonoBehaviour
{
    // Flaskの "/LoadUsers" のURL
    [SerializeField] private string loadUsersApiUrl;

    // Flaskの "/SetFishObjects" のURL
    [SerializeField] private string setTextureApiUrl;

    // Flaskの "/RecordResult" のURL
    [SerializeField] private string recordApiUrl;

    // FlaskのAPIからユーザー情報を取得してwaitUsersに追加するコルーチン処理
    public IEnumerator LoadUsersRequest(GameManager gameManager, Action onComplete = null)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(loadUsersApiUrl))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var json = request.downloadHandler.text;
                    var data = JsonUtility.FromJson<UserListResponse>(json);

                    if (data != null && data.usernames != null)
                    {
                        foreach (var name in data.usernames)
                        {
                            // 重複しないようにチェックしてからwaitUsersへ追加する
                            if (!gameManager.waitUsers.Any(u => u.name == name))
                            {
                                gameManager.waitUsers.Add(new User(name));
                                Debug.Log("ユーザー名をロード: " + name);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("ユーザーリストが空か、パースに失敗しました。");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSONパースエラー: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("ユーザーロード通信エラー: " + request.error);
            }
        }

        // 完了時コールバック
        onComplete?.Invoke();
    }

    // FlaskのJSONレスポンス受け取り用のクラス
    [Serializable]
    private class UserListResponse
    {
        public List<string> usernames; // ユーザー名の文字列型リスト
    }

    // テクスチャ新規登録時のオブジェクト名と製作者名のPOSTリクエスト
    public void PostTextureData(string fishName, string fishCreator)
    {
        FishData fishData = new FishData();
        fishData.fishName = fishName; // オブジェクト名
        fishData.fishCreator = fishCreator; // 製作者名

        string json = JsonUtility.ToJson(fishData); // fishDataをJSON化
        StartCoroutine(PostTextureRequest(json));
    }

    private IEnumerator PostTextureRequest(string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        using (UnityWebRequest request = new UnityWebRequest(setTextureApiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("オブジェクト情報更新: サーバーが応答しました");
            }
            else
            {
                Debug.LogError("DBへのオブジェクト情報格納エラー: " + request.error);
            }
        }
    }

    // ゲーム終了時の記録格納のためのPOSTリクエスト
    public void PostRecordData(string json)
    {
        StartCoroutine(PostRecordRequest(json));
    }

    private IEnumerator PostRecordRequest(string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        using (UnityWebRequest request = new UnityWebRequest(recordApiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("記録保存: サーバーが応答しました");
            }
            else
            {
                Debug.LogError("DBへの記録保存エラー: " + request.error);
            }
        }
    }
}

// 魚名と製作者名をDBに保存するためのクラス
[Serializable]
public class FishData
{
    public string fishName;
    public string fishCreator;
}
