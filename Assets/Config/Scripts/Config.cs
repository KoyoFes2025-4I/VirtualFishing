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

        applyButton = ui.rootVisualElement.Q<Button>("ApplyButton");
        applyButton.clicked += Apply;

        tabView = ui.rootVisualElement.Q<TabView>("TabView");


        ConfigSaveData config = new ConfigSaveData();
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


        ConfigSaveData config = new ConfigSaveData();

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

        applyButton.SetEnabled(tabView.selectedTabIndex != 0);
    }
}




[Serializable]
public class ConfigSaveData
{
    public static string path = Path.Combine(Application.persistentDataPath, "config.vf");

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
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = File.Create(path)) bf.Serialize(fs, this);
    }

    public bool Load()
    {
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
