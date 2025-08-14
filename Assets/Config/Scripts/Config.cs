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

    private Button applyButton;


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

        applyButton = ui.rootVisualElement.Q<Button>("ApplyButton");
        applyButton.clicked += Apply;
    }

    private void Apply()
    {
        configCamera.ChangeParams(configCameraSpeedField.value, configCameraDashSpeedField.value, cameraSensitivityField.value);
        stageManager.ChangeParams(cameraYField.value, stageSizeField.value);
        rodsController.ChangeParams(rodDropDown.index, controllerRotationYField.value, throwPowerField.value, rodPowerField.value, maxRodStrengthField.value, rodIDDropDown.index, rodUIScale.value);

        thingGenerator.Regenerate();
    }

    void Start()
    {
        FieldInit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) configCamera.locked = !configCamera.locked;

        if (configCamera.locked) UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        else UnityEngine.Cursor.lockState = CursorLockMode.None;

        ui.enabled = !configCamera.locked;
    }
}
