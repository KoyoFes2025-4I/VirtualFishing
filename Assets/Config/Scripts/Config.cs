using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Networking;
using System.Collections.Generic;

// ConfigオブジェクトにUI DocumentとこのConfig.csをアタッチする
// GUIの見た目やタブの切り替え設定は UI Toolkitのuxmlファイルとussファイルによって設定
// UI Toolkit/PanelSettings.assetでUI画面のTarget DisplayはDisplay3に表示される様に設定
// GUI上に表示する実際の文字列はラベル名を経由してUXMLファイルで設定

// UI画面のロジック処理を設定するクラス
public class Config : MonoBehaviour
{
    [SerializeField] UIDocument ui; // UI Toolkitのクラス（ConfigUI.uxmlとConfigUIStyle.uss）

    [SerializeField] ConfigCameraScript configCamera; // カメラ設定クラス
    [SerializeField] RodsController rodsController; // 釣り竿の制御クラス
    [SerializeField] StageManager stageManager; // ステージの管理クラス
    [SerializeField] ThingGenerator thingGenerator; // 魚オブジェクトの管理クラス
    [SerializeField] GameManager gameManager; // ゲームマネージャークラス
    [SerializeField] NetworkManager networkManager; // Flaskとの接続用クラス

    // 以下各UI要素の参照のための変数を用意

    private FloatField configCameraSpeedField;
    private FloatField configCameraDashSpeedField;
    private FloatField cameraSensitivityField;

    private DropdownField rodDropDown;
    private FloatField controllerRotationYField;
    private FloatField throwPowerField;
    private FloatField rodPowerField;
    private FloatField maxRodStrengthField;
    private DropdownField rodIDDropDown;
    private FloatField rodUIScale;

    private UnsignedIntegerField stageSizeField;
    private UnsignedIntegerField cameraYField;
    private FloatField cam1Rotation;
    private FloatField cam2Rotation;
    private DropdownField stageStyleDropDown;

    private Button applyButton;
    private TabView tabView;

    private ListView gamingUsersListView;
    private ListView nextUsersListView;
    private ListView waitUsersListView;
    private Button userUpButton;
    private Button userDownButton;
    private Button userRemoveButton;
    private TextField userAddField;
    private Button userAddButton;
    private Button prepareButton;
    private Button startButton;
    private Button finishButton;

    // 設定の保存用（ConfigSaveDataのインスタンス生成）
    private ConfigSaveData config = new ConfigSaveData();
    public ConfigSaveData GetConfig => config;

    // UIの各要素の初期設定用メソッド
    private void FieldInit()
    {
        // 以下でUXML内の各要素の参照を取得して初期化
        
        configCameraSpeedField = ui.rootVisualElement.Q<FloatField>("ConfigCameraSpeedField");
        configCameraDashSpeedField = ui.rootVisualElement.Q<FloatField>("ConfigCameraDashSpeedField");
        cameraSensitivityField = ui.rootVisualElement.Q<FloatField>("CameraSensitivityField");

        rodDropDown = ui.rootVisualElement.Q<DropdownField>("RodDropdown");
        controllerRotationYField = ui.rootVisualElement.Q<FloatField>("ControllerRotationYField");
        throwPowerField = ui.rootVisualElement.Q<FloatField>("ThrowPowerField");
        rodPowerField = ui.rootVisualElement.Q<FloatField>("RodPowerField");
        maxRodStrengthField = ui.rootVisualElement.Q<FloatField>("MaxRodStrengthField");
        rodIDDropDown = ui.rootVisualElement.Q<DropdownField>("RodIDDropDown");
        rodUIScale = ui.rootVisualElement.Q<FloatField>("RodUIScale");

        stageSizeField = ui.rootVisualElement.Q<UnsignedIntegerField>("StageSizeField");
        cameraYField = ui.rootVisualElement.Q<UnsignedIntegerField>("CameraYField");
        cam1Rotation = ui.rootVisualElement.Q<FloatField>("Cam1Rotation");
        cam2Rotation = ui.rootVisualElement.Q<FloatField>("Cam2Rotation");
        stageStyleDropDown = ui.rootVisualElement.Q<DropdownField>("StageStyleDropDown");

        // ゲーム中のユーザーをListViewに表示する
        gamingUsersListView = ui.rootVisualElement.Q<ListView>("GamingUsersListView");
        gamingUsersListView.makeItem = () =>
        {
            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        };
        gamingUsersListView.bindItem = (element, index) =>
        {
            (element as Label).text = gameManager.gamingUsers[index].name;
        };
        gamingUsersListView.itemsSource = gameManager.gamingUsers; // 表示するデータはgameManager.gamingUsersを参照

        // 次のゲームに参加するユーザーをListViewに表示する
        nextUsersListView = ui.rootVisualElement.Q<ListView>("NextUsersListView");
        nextUsersListView.makeItem = () =>
        {
            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        };
        nextUsersListView.bindItem = (element, index) =>
        {
            (element as Label).text = gameManager.nextUsers[index].name;
        };
        nextUsersListView.itemsSource = gameManager.nextUsers; // 表示するデータはgameManager.nextUsersを参照

        // 待っているユーザーをListViewに表示する
        waitUsersListView = ui.rootVisualElement.Q<ListView>("WaitUsersListView");
        waitUsersListView.makeItem = () =>
        {
            Label label = new Label();
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        };
        waitUsersListView.bindItem = (element, index) =>
        {
            (element as Label).text = gameManager.waitUsers[index].name;
        };
        waitUsersListView.itemsSource = gameManager.waitUsers; // 表示するデータはgameManager.waitUsersを参照

        // 「次にゲームするユーザー」と「待機中ユーザー」のどちらかしか一度に選べないようにする
        waitUsersListView.selectionChanged += (element) => nextUsersListView.selectedIndex = -1;
        nextUsersListView.selectionChanged += (element) => waitUsersListView.selectedIndex = -1;

        // ユーザーの状態管理ボタンの参照を取得して初期化
        userUpButton = ui.rootVisualElement.Q<Button>("UserUpButton");
        userDownButton = ui.rootVisualElement.Q<Button>("UserDownButton");
        userRemoveButton = ui.rootVisualElement.Q<Button>("UserRemoveButton");
        userAddField = ui.rootVisualElement.Q<TextField>("UserAddField");
        userAddButton = ui.rootVisualElement.Q<Button>("UserAddButton");

        // UserAddField（名前）はTextFieldなので文字入力が可能

        // 「追加」がクリックされた時の処理を設定
        userAddButton.clicked += () =>
        {
            // 入力されたユーザー名を登録したUsersインスタンスをwaitUsersリストに追加する
            gameManager.waitUsers.Add(new User(userAddField.value));

            // DBにあるユーザーデータを全てロードしてこれらもwaitUsersリストに追加する
            StartCoroutine(LoadUsers());

            waitUsersListView.RefreshItems();
        };

        // 「削除」がクリックされた時の処理を設定
        userRemoveButton.clicked += () =>
        {
            if (waitUsersListView.selectedIndex != -1)
            {
                gameManager.waitUsers.RemoveAt(waitUsersListView.selectedIndex);
                waitUsersListView.RefreshItems();
            }
            else if (nextUsersListView.selectedIndex != -1)
            {
                gameManager.nextUsers.RemoveAt(nextUsersListView.selectedIndex);
                nextUsersListView.RefreshItems();
            }
        };

        // 「上」がクリックされた時の処理を設定
        userUpButton.clicked += () =>
        {
            if (waitUsersListView.selectedIndex != -1)
            {
                if (waitUsersListView.selectedIndex == 0 && (gameManager.nextUsers.Count == 8 || gameManager.nextUsers.Count == 4 && config.rodCountIndex != 0)) return;
                User user = gameManager.waitUsers[waitUsersListView.selectedIndex];
                gameManager.waitUsers.RemoveAt(waitUsersListView.selectedIndex);
                if (waitUsersListView.selectedIndex != 0) gameManager.waitUsers.Insert(waitUsersListView.selectedIndex - 1, user);
                else gameManager.nextUsers.Add(user);
                waitUsersListView.selectedIndex--;
                if (waitUsersListView.selectedIndex == -1) nextUsersListView.selectedIndex = gameManager.nextUsers.Count - 1;
                waitUsersListView.RefreshItems();
                nextUsersListView.RefreshItems();
            }
            else if (nextUsersListView.selectedIndex > 0)
            {
                User user = gameManager.nextUsers[nextUsersListView.selectedIndex];
                gameManager.nextUsers.RemoveAt(nextUsersListView.selectedIndex);
                gameManager.nextUsers.Insert(--nextUsersListView.selectedIndex, user);
                nextUsersListView.RefreshItems();
            }
        };

        // 「下」がクリックされた時の処理を設定
        userDownButton.clicked += () =>
        {
            if (waitUsersListView.selectedIndex < gameManager.waitUsers.Count - 1 && waitUsersListView.selectedIndex >= 0)
            {
                User user = gameManager.waitUsers[waitUsersListView.selectedIndex];
                gameManager.waitUsers.RemoveAt(waitUsersListView.selectedIndex);
                gameManager.waitUsers.Insert(waitUsersListView.selectedIndex + 1, user);
                waitUsersListView.selectedIndex++;
                waitUsersListView.RefreshItems();
            }
            else if (nextUsersListView.selectedIndex < gameManager.nextUsers.Count && nextUsersListView.selectedIndex >= 0)
            {
                User user = gameManager.nextUsers[nextUsersListView.selectedIndex];
                gameManager.nextUsers.RemoveAt(nextUsersListView.selectedIndex);
                if (nextUsersListView.selectedIndex != gameManager.nextUsers.Count - 1) gameManager.waitUsers.Insert(0, user);
                else gameManager.nextUsers.Insert(nextUsersListView.selectedIndex + 1, user);
                nextUsersListView.selectedIndex++;
                if (nextUsersListView.selectedIndex == gameManager.nextUsers.Count)
                {
                    nextUsersListView.selectedIndex = -1;
                    waitUsersListView.selectedIndex = 0;
                }
                nextUsersListView.RefreshItems();
                waitUsersListView.RefreshItems();
            }
        };

        // 「準備」、「スタート」、「終了」ボタンの参照を取得して初期化
        prepareButton = ui.rootVisualElement.Q<Button>("PrepareButton");
        startButton = ui.rootVisualElement.Q<Button>("StartButton");
        finishButton = ui.rootVisualElement.Q<Button>("FinishButton");

        // 「準備」、「スタート」、「終了」ボタンがクリックされた時に呼び出す関数を設定
        prepareButton.clicked += () => gameManager.Prepare();
        startButton.clicked += () => gameManager.StartGame();
        finishButton.clicked += () => gameManager.FinishGame();

        // 「適用」ボタンの参照の取得とそのクリック時処理の設定
        applyButton = ui.rootVisualElement.Q<Button>("ApplyButton");
        applyButton.clicked += Apply;

        // tabViewに選択されているタブの参照を取得して初期化
        tabView = ui.rootVisualElement.Q<TabView>("TabView");

        // 設定データファイルに保存されている設定をConfigSaveDataへ読み込む
        // ファイルが無ければデフォルト値のまま変わらない
        config.Load();

        // 以下でConfigSaveDataにある値を各UIフィールドやドロップダウンにセットする

        configCameraSpeedField.value = config.configCameraSpeed;
        configCameraDashSpeedField.value = config.configCameraDashSpeed;
        cameraSensitivityField.value = config.configCameraSensitivity;

        rodDropDown.index = config.rodCountIndex;
        controllerRotationYField.value = config.baseRotationY;
        throwPowerField.value = config.throwPower;
        rodPowerField.value = config.rodPower;
        maxRodStrengthField.value = config.maxRodStrength;
        rodIDDropDown.index = config.rodIDGenerateIndex;
        rodUIScale.value = config.rodUIScale;

        stageSizeField.value = (uint)config.stageSize;
        cameraYField.value = (uint)config.cameraY;
        cam1Rotation.value = config.cam1Rotation;
        cam2Rotation.value = config.cam2Rotation;
        stageStyleDropDown.index = config.stageStyle;
    }

    // FlaskのAPIからユーザー情報を取得してwaitUsersに追加するコルーチン処理
    private IEnumerator LoadUsers()
    {
        // Flaskの /LoadUsers にアクセスしてレスポンスを受け取る
        string url = "http://127.0.0.1:5000/LoadUsers";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
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

                        waitUsersListView.RefreshItems();
                    }
                    else
                    {
                        Debug.LogWarning("ユーザーリストが空か、パースに失敗しました。");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSONパースエラー: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("通信エラー: " + request.error);
            }
        }
    }

    // FlaskのJSONレスポンス受け取り用のクラス
    [Serializable]
    private class UserListResponse
    {
        public List<string> usernames; // ユーザー名の文字列型リスト
    }

    // 「適用」ボタンがクリックされた時の処理
    private void Apply()
    {
        // コンフィグ画面で変更した設定を実際に反映させて更新する
        configCamera.ChangeParams(configCameraSpeedField.value, configCameraDashSpeedField.value, cameraSensitivityField.value);
        stageManager.ChangeParams(cameraYField.value, stageSizeField.value, cam1Rotation.value, cam2Rotation.value, stageStyleDropDown.index);
        rodsController.ChangeParams(rodDropDown.index, controllerRotationYField.value, throwPowerField.value, rodPowerField.value, maxRodStrengthField.value, rodIDDropDown.index, rodUIScale.value);

        // 魚オブジェクトを一旦全て破棄する
        thingGenerator.Regenerate();

        // 以下でUIで設定したデータををConfigSaveDataへコピー
        
        config.configCameraSpeed = configCameraSpeedField.value;
        config.configCameraDashSpeed = configCameraDashSpeedField.value;
        config.configCameraSensitivity = cameraSensitivityField.value;

        config.rodCountIndex = rodDropDown.index;
        config.baseRotationY = controllerRotationYField.value;
        config.throwPower = throwPowerField.value;
        config.rodPower = rodPowerField.value;
        config.maxRodStrength = maxRodStrengthField.value;
        config.rodIDGenerateIndex = rodIDDropDown.index;
        config.rodUIScale = rodUIScale.value;

        config.stageSize = (int)stageSizeField.value;
        config.cameraY = (int)cameraYField.value;
        config.cam1Rotation = cam1Rotation.value;
        config.cam2Rotation = cam2Rotation.value;
        config.stageStyle = stageStyleDropDown.index;

        // コピーしたデータを設定データファイルへ保存する
        // ゲームを再起動しても設定を保持できる（Loadメソッドで初期化時に毎回ファイルを読み込む）
        config.Save();
    }

    // gamingUsers, nextUsers, waitUsersが変更された時にこれを呼ぶことでUIのListViewが最新の状態になる
    public void ListViewRefresh()
    {
        gamingUsersListView.RefreshItems();
        nextUsersListView.RefreshItems();
        waitUsersListView.RefreshItems();
    }

    void Start()
    {
        FieldInit(); // 各要素の初期化
        Apply(); // 各データの一番最初の適用処理
    }

    void Update()
    {
        // Escapeキーでカメラ固定とコンフィグ画面の表示・非表示を切り替える
        if (Input.GetKeyDown(KeyCode.Escape)) configCamera.locked = !configCamera.locked;

        // カメラ操作中のマウスカーソルの固定処理
        if (configCamera.locked) UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        else UnityEngine.Cursor.lockState = CursorLockMode.None;

        // ui.enabledでコンフィグ画面の表示・非表示を制御
        bool prev = ui.enabled;
        ui.enabled = !configCamera.locked;

        // UI が非表示から表示に切り替わった場合は各要素を再取得
        if (!prev && ui.enabled) FieldInit();

        // 「適用」ボタンはゲーム中でないかつ「ゲームマネージャー」タブでのみ有効
        // 「準備」と「スタート」ボタンはゲーム中でない時に有効
        // 「終了」ボタンはゲーム中のみ有効
        applyButton.SetEnabled(tabView.selectedTabIndex != 0 && !gameManager.isGaming);
        prepareButton.SetEnabled(!gameManager.isGaming);
        startButton.SetEnabled(!gameManager.isGaming);
        finishButton.SetEnabled(gameManager.isGaming);
    }
}

// 各設定データの保存・読み込み用のデータコンテナ
[Serializable]
public class ConfigSaveData
{
    // 保存用の設定データファイル名とそのパス用の変数
    public static string fileName = "config.vf";
    public static string path = "";

    // 以下が各データのデフォルト設定値
    // 設定変更時にはApply関数から書き変えられる

    public float configCameraSpeed = 5f;
    public float configCameraDashSpeed = 15f;
    public float configCameraSensitivity = 500f;

    public int rodCountIndex = 0;
    public float baseRotationY = 0;
    public float throwPower = 0.3f;
    public float rodPower = 0.3f;
    public float maxRodStrength = 100;
    public float rodUIScale = 1;
    public int rodIDGenerateIndex = 0;

    public int stageSize = 10;
    public int cameraY = 200;
    public float cam1Rotation = 0f;
    public float cam2Rotation = 0f;
    public int stageStyle = 0;

    // 現在の各データの設定値を設定データファイルへ書き込むメソッド
    public void Save()
    {
        path = Path.Combine(Application.persistentDataPath, fileName);

        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = File.Create(path)) bf.Serialize(fs, this);
    }

    // 設定データファイルを読み込んで各データを書き変えるメソッド
    public bool Load()
    {
        path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) return false; // ファイルがない時は何もしない（デフォルトのまま）

        BinaryFormatter bf = new BinaryFormatter();
        ConfigSaveData tmp;
        using (FileStream fs = File.Open(path, FileMode.Open)) tmp = (ConfigSaveData)bf.Deserialize(fs);

        configCameraSpeed = tmp.configCameraSpeed;
        configCameraDashSpeed = tmp.configCameraDashSpeed;
        configCameraSensitivity = tmp.configCameraSensitivity;

        rodCountIndex = tmp.rodCountIndex;
        baseRotationY = tmp.baseRotationY;
        throwPower = tmp.throwPower;
        rodPower = tmp.rodPower;
        maxRodStrength = tmp.maxRodStrength;
        rodUIScale = tmp.rodUIScale;
        rodIDGenerateIndex = tmp.rodIDGenerateIndex;

        stageSize = tmp.stageSize;
        cameraY = tmp.cameraY;
        cam1Rotation = tmp.cam1Rotation;
        cam2Rotation = tmp.cam2Rotation;
        stageStyle = tmp.stageStyle;
        return true;
    }
}
