using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UIElements;

public class Config : MonoBehaviour
{
    [SerializeField]
    UIDocument ui;
    [SerializeField]
    ConfigCameraScript configCamera;
    [SerializeField]
    RodsController rodsController;
    [SerializeField]
    StageManager stageManager;
    [SerializeField]
    ThingGenerator thingGenerator;
    [SerializeField]
    GameManager gameManager;

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

    private ConfigSaveData config = new ConfigSaveData();
    public ConfigSaveData GetConfig => config;

    private void FieldInit()
    {
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
        gamingUsersListView.itemsSource = gameManager.gamingUsers;

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
        nextUsersListView.itemsSource = gameManager.nextUsers;

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
        waitUsersListView.itemsSource = gameManager.waitUsers;

        waitUsersListView.selectionChanged += (element) => nextUsersListView.selectedIndex = -1;
        nextUsersListView.selectionChanged += (element) => waitUsersListView.selectedIndex = -1;

        userUpButton = ui.rootVisualElement.Q<Button>("UserUpButton");
        userDownButton = ui.rootVisualElement.Q<Button>("UserDownButton");
        userRemoveButton = ui.rootVisualElement.Q<Button>("UserRemoveButton");
        userAddField = ui.rootVisualElement.Q<TextField>("UserAddField");
        userAddButton = ui.rootVisualElement.Q<Button>("UserAddButton");
        userAddButton.clicked += () =>
        {
            gameManager.waitUsers.Add(new User(userAddField.value));
            waitUsersListView.RefreshItems();
        };
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
        userUpButton.clicked += () =>
        {
            if (waitUsersListView.selectedIndex != -1)
            {
                if (waitUsersListView.selectedIndex == 0 && (gameManager.nextUsers.Count == 8 || gameManager.nextUsers.Count == 4 && config.rodCountIndex != 0)) return;
                User user = gameManager.waitUsers[waitUsersListView.selectedIndex];
                gameManager.waitUsers.RemoveAt(waitUsersListView.selectedIndex);
                if (waitUsersListView.selectedIndex != 0) gameManager.waitUsers.Insert(waitUsersListView.selectedIndex-1, user);
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

        prepareButton = ui.rootVisualElement.Q<Button>("PrepareButton");
        startButton = ui.rootVisualElement.Q<Button>("StartButton");
        finishButton = ui.rootVisualElement.Q<Button>("FinishButton");
        prepareButton.clicked += () => gameManager.Prepare();
        startButton.clicked += () => gameManager.StartGame();
        finishButton.clicked += () => gameManager.FinishGame();


        applyButton = ui.rootVisualElement.Q<Button>("ApplyButton");
        applyButton.clicked += Apply;

        tabView = ui.rootVisualElement.Q<TabView>("TabView");


        config.Load();

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
    }

    private void Apply()
    {
        configCamera.ChangeParams(configCameraSpeedField.value, configCameraDashSpeedField.value, cameraSensitivityField.value);
        stageManager.ChangeParams(cameraYField.value, stageSizeField.value, cam1Rotation.value, cam2Rotation.value);
        rodsController.ChangeParams(rodDropDown.index, controllerRotationYField.value, throwPowerField.value, rodPowerField.value, maxRodStrengthField.value, rodIDDropDown.index, rodUIScale.value);

        thingGenerator.Regenerate();



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

        config.Save();
    }

    public void ListViewRefresh()
    {
        gamingUsersListView.RefreshItems();
        nextUsersListView.RefreshItems();
        waitUsersListView.RefreshItems();
    }

    void Start()
    {
        FieldInit();
        Apply();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) configCamera.locked = !configCamera.locked;

        if (configCamera.locked) UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        else UnityEngine.Cursor.lockState = CursorLockMode.None;

        bool prev = ui.enabled;
        ui.enabled = !configCamera.locked;
        if (!prev && ui.enabled) FieldInit();

        applyButton.SetEnabled(tabView.selectedTabIndex != 0 && !gameManager.isGaming);
        prepareButton.SetEnabled(!gameManager.isGaming);
        startButton.SetEnabled(!gameManager.isGaming);
        finishButton.SetEnabled(gameManager.isGaming);
    }
}




[Serializable]
public class ConfigSaveData
{
    public static string fileName = "config.vf";
    public static string path = "";

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

    public void Save()
    {
        path = Path.Combine(Application.persistentDataPath, fileName);

        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = File.Create(path)) bf.Serialize(fs, this);
    }

    public bool Load()
    {
        path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) return false;

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
        return true;
    }
}
