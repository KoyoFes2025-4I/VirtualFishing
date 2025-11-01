using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Flask�T�[�o�[�Ƃ̐ڑ��p�N���X
public class NetworkManager : MonoBehaviour
{
    // Flask�� "/LoadUsers" ��URL
    [SerializeField] private string loadUsersApiUrl;

    // Flask�� "/SetFishObjects" ��URL
    [SerializeField] private string setTextureApiUrl;

    // Flask�� "/RecordResult" ��URL
    [SerializeField] private string recordApiUrl;

    // Flask��API���烆�[�U�[�����擾����waitUsers�ɒǉ�����R���[�`������
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
                            // �d�����Ȃ��悤�Ƀ`�F�b�N���Ă���waitUsers�֒ǉ�����
                            if (!gameManager.waitUsers.Any(u => u.name == name))
                            {
                                gameManager.waitUsers.Add(new User(name));
                                Debug.Log("���[�U�[�������[�h: " + name);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("���[�U�[���X�g���󂩁A�p�[�X�Ɏ��s���܂����B");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON�p�[�X�G���[: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("���[�U�[���[�h�ʐM�G���[: " + request.error);
            }
        }

        // �������R�[���o�b�N
        onComplete?.Invoke();
    }

    // Flask��JSON���X�|���X�󂯎��p�̃N���X
    [Serializable]
    private class UserListResponse
    {
        public List<string> usernames; // ���[�U�[���̕�����^���X�g
    }

    // �e�N�X�`���V�K�o�^���̃I�u�W�F�N�g���Ɛ���Җ���POST���N�G�X�g
    public void PostTextureData(string fishName, string fishCreator)
    {
        FishData fishData = new FishData();
        fishData.fishName = fishName; // �I�u�W�F�N�g��
        fishData.fishCreator = fishCreator; // ����Җ�

        string json = JsonUtility.ToJson(fishData); // fishData��JSON��
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
                Debug.Log("�I�u�W�F�N�g���X�V: �T�[�o�[���������܂���");
            }
            else
            {
                Debug.LogError("DB�ւ̃I�u�W�F�N�g���i�[�G���[: " + request.error);
            }
        }
    }

    // �Q�[���I�����̋L�^�i�[�̂��߂�POST���N�G�X�g
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
                Debug.Log("�L�^�ۑ�: �T�[�o�[���������܂���");
            }
            else
            {
                Debug.LogError("DB�ւ̋L�^�ۑ��G���[: " + request.error);
            }
        }
    }
}

// �����Ɛ���Җ���DB�ɕۑ����邽�߂̃N���X
[Serializable]
public class FishData
{
    public string fishName;
    public string fishCreator;
}
