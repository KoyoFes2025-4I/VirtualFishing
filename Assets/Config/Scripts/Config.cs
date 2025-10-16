using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Networking;
using System.Collections.Generic;

// Config�I�u�W�F�N�g��UI Document�Ƃ���Config.cs���A�^�b�`����
// GUI�̌����ڂ�^�u�̐؂�ւ��ݒ�� UI Toolkit��uxml�t�@�C����uss�t�@�C���ɂ���Đݒ�
// UI Toolkit/PanelSettings.asset��UI��ʂ�Target Display��Display3�ɕ\�������l�ɐݒ�
// GUI��ɕ\��������ۂ̕�����̓��x�������o�R����UXML�t�@�C���Őݒ�

// UI��ʂ̃��W�b�N������ݒ肷��N���X
public class Config : MonoBehaviour
{
    [SerializeField] UIDocument ui; // UI Toolkit�̃N���X�iConfigUI.uxml��ConfigUIStyle.uss�j

    [SerializeField] ConfigCameraScript configCamera; // �J�����ݒ�N���X
    [SerializeField] RodsController rodsController; // �ނ�Ƃ̐���N���X
    [SerializeField] StageManager stageManager; // �X�e�[�W�̊Ǘ��N���X
    [SerializeField] ThingGenerator thingGenerator; // ���I�u�W�F�N�g�̊Ǘ��N���X
    [SerializeField] GameManager gameManager; // �Q�[���}�l�[�W���[�N���X
    [SerializeField] NetworkManager networkManager; // Flask�Ƃ̐ڑ��p�N���X

    // �ȉ��eUI�v�f�̎Q�Ƃ̂��߂̕ϐ���p��

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

    // �ݒ�̕ۑ��p�iConfigSaveData�̃C���X�^���X�����j
    private ConfigSaveData config = new ConfigSaveData();
    public ConfigSaveData GetConfig => config;

    // UI�̊e�v�f�̏����ݒ�p���\�b�h
    private void FieldInit()
    {
        // �ȉ���UXML���̊e�v�f�̎Q�Ƃ��擾���ď�����
        
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

        // �Q�[�����̃��[�U�[��ListView�ɕ\������
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
        gamingUsersListView.itemsSource = gameManager.gamingUsers; // �\������f�[�^��gameManager.gamingUsers���Q��

        // ���̃Q�[���ɎQ�����郆�[�U�[��ListView�ɕ\������
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
        nextUsersListView.itemsSource = gameManager.nextUsers; // �\������f�[�^��gameManager.nextUsers���Q��

        // �҂��Ă��郆�[�U�[��ListView�ɕ\������
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
        waitUsersListView.itemsSource = gameManager.waitUsers; // �\������f�[�^��gameManager.waitUsers���Q��

        // �u���ɃQ�[�����郆�[�U�[�v�Ɓu�ҋ@�����[�U�[�v�̂ǂ��炩������x�ɑI�ׂȂ��悤�ɂ���
        waitUsersListView.selectionChanged += (element) => nextUsersListView.selectedIndex = -1;
        nextUsersListView.selectionChanged += (element) => waitUsersListView.selectedIndex = -1;

        // ���[�U�[�̏�ԊǗ��{�^���̎Q�Ƃ��擾���ď�����
        userUpButton = ui.rootVisualElement.Q<Button>("UserUpButton");
        userDownButton = ui.rootVisualElement.Q<Button>("UserDownButton");
        userRemoveButton = ui.rootVisualElement.Q<Button>("UserRemoveButton");
        userAddField = ui.rootVisualElement.Q<TextField>("UserAddField");
        userAddButton = ui.rootVisualElement.Q<Button>("UserAddButton");

        // UserAddField�i���O�j��TextField�Ȃ̂ŕ������͂��\

        // �u�ǉ��v���N���b�N���ꂽ���̏�����ݒ�
        userAddButton.clicked += () =>
        {
            // ���͂��ꂽ���[�U�[����o�^����Users�C���X�^���X��waitUsers���X�g�ɒǉ�����
            gameManager.waitUsers.Add(new User(userAddField.value));

            // DB�ɂ��郆�[�U�[�f�[�^��S�ă��[�h���Ă�����waitUsers���X�g�ɒǉ�����
            StartCoroutine(LoadUsers());

            waitUsersListView.RefreshItems();
        };

        // �u�폜�v���N���b�N���ꂽ���̏�����ݒ�
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

        // �u��v���N���b�N���ꂽ���̏�����ݒ�
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

        // �u���v���N���b�N���ꂽ���̏�����ݒ�
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

        // �u�����v�A�u�X�^�[�g�v�A�u�I���v�{�^���̎Q�Ƃ��擾���ď�����
        prepareButton = ui.rootVisualElement.Q<Button>("PrepareButton");
        startButton = ui.rootVisualElement.Q<Button>("StartButton");
        finishButton = ui.rootVisualElement.Q<Button>("FinishButton");

        // �u�����v�A�u�X�^�[�g�v�A�u�I���v�{�^�����N���b�N���ꂽ���ɌĂяo���֐���ݒ�
        prepareButton.clicked += () => gameManager.Prepare();
        startButton.clicked += () => gameManager.StartGame();
        finishButton.clicked += () => gameManager.FinishGame();

        // �u�K�p�v�{�^���̎Q�Ƃ̎擾�Ƃ��̃N���b�N�������̐ݒ�
        applyButton = ui.rootVisualElement.Q<Button>("ApplyButton");
        applyButton.clicked += Apply;

        // tabView�ɑI������Ă���^�u�̎Q�Ƃ��擾���ď�����
        tabView = ui.rootVisualElement.Q<TabView>("TabView");

        // �ݒ�f�[�^�t�@�C���ɕۑ�����Ă���ݒ��ConfigSaveData�֓ǂݍ���
        // �t�@�C����������΃f�t�H���g�l�̂܂ܕς��Ȃ�
        config.Load();

        // �ȉ���ConfigSaveData�ɂ���l���eUI�t�B�[���h��h���b�v�_�E���ɃZ�b�g����

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

    // Flask��API���烆�[�U�[�����擾����waitUsers�ɒǉ�����R���[�`������
    private IEnumerator LoadUsers()
    {
        // Flask�� /LoadUsers �ɃA�N�Z�X���ă��X�|���X���󂯎��
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
                            // �d�����Ȃ��悤�Ƀ`�F�b�N���Ă���waitUsers�֒ǉ�����
                            if (!gameManager.waitUsers.Any(u => u.name == name))
                            {
                                gameManager.waitUsers.Add(new User(name));
                                Debug.Log("���[�U�[�������[�h: " + name);
                            }
                        }

                        waitUsersListView.RefreshItems();
                    }
                    else
                    {
                        Debug.LogWarning("���[�U�[���X�g���󂩁A�p�[�X�Ɏ��s���܂����B");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"JSON�p�[�X�G���[: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("�ʐM�G���[: " + request.error);
            }
        }
    }

    // Flask��JSON���X�|���X�󂯎��p�̃N���X
    [Serializable]
    private class UserListResponse
    {
        public List<string> usernames; // ���[�U�[���̕�����^���X�g
    }

    // �u�K�p�v�{�^�����N���b�N���ꂽ���̏���
    private void Apply()
    {
        // �R���t�B�O��ʂŕύX�����ݒ�����ۂɔ��f�����čX�V����
        configCamera.ChangeParams(configCameraSpeedField.value, configCameraDashSpeedField.value, cameraSensitivityField.value);
        stageManager.ChangeParams(cameraYField.value, stageSizeField.value, cam1Rotation.value, cam2Rotation.value, stageStyleDropDown.index);
        rodsController.ChangeParams(rodDropDown.index, controllerRotationYField.value, throwPowerField.value, rodPowerField.value, maxRodStrengthField.value, rodIDDropDown.index, rodUIScale.value);

        // ���I�u�W�F�N�g����U�S�Ĕj������
        thingGenerator.Regenerate();

        // �ȉ���UI�Őݒ肵���f�[�^����ConfigSaveData�փR�s�[
        
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

        // �R�s�[�����f�[�^��ݒ�f�[�^�t�@�C���֕ۑ�����
        // �Q�[�����ċN�����Ă��ݒ��ێ��ł���iLoad���\�b�h�ŏ��������ɖ���t�@�C����ǂݍ��ށj
        config.Save();
    }

    // gamingUsers, nextUsers, waitUsers���ύX���ꂽ���ɂ�����ĂԂ��Ƃ�UI��ListView���ŐV�̏�ԂɂȂ�
    public void ListViewRefresh()
    {
        gamingUsersListView.RefreshItems();
        nextUsersListView.RefreshItems();
        waitUsersListView.RefreshItems();
    }

    void Start()
    {
        FieldInit(); // �e�v�f�̏�����
        Apply(); // �e�f�[�^�̈�ԍŏ��̓K�p����
    }

    void Update()
    {
        // Escape�L�[�ŃJ�����Œ�ƃR���t�B�O��ʂ̕\���E��\����؂�ւ���
        if (Input.GetKeyDown(KeyCode.Escape)) configCamera.locked = !configCamera.locked;

        // �J�������쒆�̃}�E�X�J�[�\���̌Œ菈��
        if (configCamera.locked) UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        else UnityEngine.Cursor.lockState = CursorLockMode.None;

        // ui.enabled�ŃR���t�B�O��ʂ̕\���E��\���𐧌�
        bool prev = ui.enabled;
        ui.enabled = !configCamera.locked;

        // UI ����\������\���ɐ؂�ւ�����ꍇ�͊e�v�f���Ď擾
        if (!prev && ui.enabled) FieldInit();

        // �u�K�p�v�{�^���̓Q�[�����łȂ����u�Q�[���}�l�[�W���[�v�^�u�ł̂ݗL��
        // �u�����v�Ɓu�X�^�[�g�v�{�^���̓Q�[�����łȂ����ɗL��
        // �u�I���v�{�^���̓Q�[�����̂ݗL��
        applyButton.SetEnabled(tabView.selectedTabIndex != 0 && !gameManager.isGaming);
        prepareButton.SetEnabled(!gameManager.isGaming);
        startButton.SetEnabled(!gameManager.isGaming);
        finishButton.SetEnabled(gameManager.isGaming);
    }
}

// �e�ݒ�f�[�^�̕ۑ��E�ǂݍ��ݗp�̃f�[�^�R���e�i
[Serializable]
public class ConfigSaveData
{
    // �ۑ��p�̐ݒ�f�[�^�t�@�C�����Ƃ��̃p�X�p�̕ϐ�
    public static string fileName = "config.vf";
    public static string path = "";

    // �ȉ����e�f�[�^�̃f�t�H���g�ݒ�l
    // �ݒ�ύX���ɂ�Apply�֐����珑���ς�����

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

    // ���݂̊e�f�[�^�̐ݒ�l��ݒ�f�[�^�t�@�C���֏������ރ��\�b�h
    public void Save()
    {
        path = Path.Combine(Application.persistentDataPath, fileName);

        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = File.Create(path)) bf.Serialize(fs, this);
    }

    // �ݒ�f�[�^�t�@�C����ǂݍ���Ŋe�f�[�^�������ς��郁�\�b�h
    public bool Load()
    {
        path = Path.Combine(Application.persistentDataPath, fileName);
        if (!File.Exists(path)) return false; // �t�@�C�����Ȃ����͉������Ȃ��i�f�t�H���g�̂܂܁j

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
