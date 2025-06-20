using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BattleManager))]
[RequireComponent(typeof(PointManager))]
[RequireComponent(typeof(InWaterManager))]

// �u�ނ���́v�̋��ʂ̏�����t�B�[���h���������e�N���X�i���ۃN���X�j

public abstract class ThingsToFish : MonoBehaviour
{

    private Animator animator;
    private BattleManager battleManager;
    private PointManager pointManager;
    private InWaterManager inWaterManager;

    [SerializeField] private GameObject model; // 3D���f��
    //[SerializeField] private Texture2D objectTexture; // ���f���ɒ���t����2D�e�N�X�`��
    [SerializeField] private string objectName; // �I�u�W�F�N�g��
    [SerializeField] private int strength; // �̗̓p�����[�^
    [SerializeField] private int power; // �̓p�����[�^
    [SerializeField] private int weight; // �d�ʁi�ړ����x�j�p�����[�^
    [SerializeField] private int point; // �ނ������̓��_
    [SerializeField] private string creater; // ����ҁiID�j
    [SerializeField] private string AnimController; // �K�p����Animation Controller�̖��O�iResources/Animations�z���j

    private float timer = 0f;
    public float reverseInterval = 5.0f; // �������]�̃C���^�[�o��
    private int direction = 1;

    private string AnimPath => "Animations/" + AnimController;// Resources�ȉ��̃p�X���쐬

    // strength��power�̃Q�b�^�[�i�o�g�������Ŏg���j
    public int Strength => strength;
    public int Power => power;

    public Vector3 currentPosition { get; private set; } //�I�u�W�F�N�g�̌��݂�3�������W

    public bool isInWater { get; private set; } = false; // �ނ�Ƃ̒����t���O
    public bool wasCaught { get; private set; } = false; // �a�I�u�W�F�N�g�Ƃ̐ڐG�t���O
    public bool hasStartedBattle { get; private set; } = false; // �o�g���J�n�̃t���O
    public bool isInBattle { get; private set; } = false; // ����ނ肠����o�g�����ł���t���O
    public bool wasSuccessFishing { get; private set; } = false; // �ނ�グ�ɐ��������t���O
    public bool wasFinishFishing { get; private set; } = false; // �ނ�o�g���I���t���O

    // �e�t���O�̃Z�b�^�[
    public void SetTrueInBattle() => isInBattle = true;
    public void SetFalseInBattle() => isInBattle = false;
    public void SetSuccessFishing() => wasSuccessFishing = true;
    public void SetFinishFishing() => wasFinishFishing= true;
    public void SetIsInWater() => isInWater = true;

    // �e�t���O�̃Q�b�^�[�iAnimation Controller�Ŏg���j
    public bool IsInWater => isInWater;
    public bool WasCaught => wasCaught;
    public bool IsInBattle => isInBattle;
    public bool WasSuccessFishing => wasSuccessFishing;
    public bool WasFinishFishing => wasFinishFishing;

    // ��ԃ��Z�b�g�p���\�b�h
    public void ResetCatchState()
    {
        isInWater = false;
        wasCaught = false;
        hasStartedBattle = false;
        wasSuccessFishing = false;
        wasFinishFishing = false;
    }

    // �a�I�u�W�F�N�g�i�^�O��: Feed�j�ƐڐG�������̏���
    public void OnCollisionEnter(Collision collision)
    {
        // �o�g�����͂��̐ڐG����͓��삵�Ȃ��悤�ɂ���
        if (collision.gameObject.CompareTag("Feed") && isInWater && !isInBattle)
        {
            wasCaught = true;
        }
    }

    // ���̉j���̓����̃A���S���Y��
    protected virtual void MovementConfig()
    {

        // �d�ʂ̃p�����[�^�ňړ����x�����߂ċ����ړ�������
        transform.Translate(Vector3.forward * direction * Time.deltaTime / weight);

        // �^�C�}�[�X�V
        timer += Time.deltaTime;

        // ��莞�Ԍo�߂ŕ������]
        if (timer >= reverseInterval)
        {
            direction *= -1; // �������]
            timer = 0f;      // �^�C�}�[���Z�b�g
        }

    }

    // �o�g���ɏ��������ɌĂ΂�鏈��
    protected virtual void WinFishing()
    {

        animator.SetBool("win", true);// win�p�����[�^��true�ɂ��ăA�j���[�V�����J��

        pointManager.AddPoint(point); // �ނ�グ�����̓��_�����Z
        ResetCatchState(); // �t���O�̏�Ԃ̃��Z�b�g

        // �ނ�グ�����̃��f���A���O�A���_�Ȃǂ̉�ʏ�ւ̕\���v���Z�X����������
        // �K�v�ȃf�[�^�̓f�[�^�x�[�X�֊i�[����悤�ɂ���

        animator.SetBool("toExit", true);// toExit�p�����[�^��true�ɂ��ăA�j���[�V�����J��

        Destroy(gameObject); // �I�u�W�F�N�g�̏���

    }

    // �o�g���ɕ��������ɌĂ΂�鏈��
    protected virtual void LoseFishing()
    {

        animator.SetBool("lose", true);// lose�p�����[�^��true�ɂ��ăA�j���[�V�����J��

        ResetCatchState(); // �t���O�̏�Ԃ̃��Z�b�g

        // ���������̌�̋��I�u�W�F�N�g�Ɋւ��鏈��������

        animator.SetBool("toExit", true);// toExit�p�����[�^��true�ɂ��ăA�j���[�V�����J��

        Destroy(gameObject); // �I�u�W�F�N�g�̏���

    }
   
    // �����߂܂��Ă���̏���
    protected virtual void HandleFishing()
    {

        // �������ɉa�I�u�W�F�N�g�ƐڐG�������͎��̃A�j���[�V�����Ɉڍs
        if (isInWater && wasCaught)
        {

            animator.SetBool("wasCaught", true);// wasCaught�p�����[�^��true�ɂ��ăA�j���[�V�����J��

            // �o�g�����n�܂鎞
            if (!isInBattle && !hasStartedBattle)
            {
                hasStartedBattle = true; // 2�x�ڂ͌Ă΂�Ȃ��悤�ɂ���
                battleManager.DoBattle(); // �o�g�������̊J�n
            }

            // �o�g���������I��������
            if (wasFinishFishing)
            {

                // ���̒ނ�グ�ɐ������Ă�����
                if (wasSuccessFishing)
                {
                    WinFishing();
                }

                // ���̒ނ�グ�Ɏ��s���Ă�����
                else
                {
                    LoseFishing();
                }

            }

        }

    }

    // �ˑ��R���|�[�l���g�̏���������
    protected virtual void Awake()
    {
        
        battleManager = GetComponent<BattleManager>();

        // �p�������ꍇ�ɑS�Ă̎q�N���X�ł������ŃR���|�[�l���g���擾������
        if (battleManager == null)
        {
            battleManager = gameObject.AddComponent<BattleManager>();
        }

        pointManager = GetComponent<PointManager>();

        if (pointManager == null)
        {
            pointManager = gameObject.AddComponent<PointManager>();
        }

        inWaterManager = GetComponent<InWaterManager>();

        if (inWaterManager == null)
        {
            inWaterManager = gameObject.AddComponent<InWaterManager>();
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError($"{gameObject.name} ��Animator�R���|�[�l���g������܂���");
            return;
        }

        if (animator.runtimeAnimatorController == null)
        {

            // Assets/Resources/Animations/FishAnimationLogic.controller��ǂݍ���
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(AnimPath);

            // �e�I�u�W�F�N�g�̃A�j���[�V�����J�ڂ�Animator Controller�Ő��䂷��
            if (controller != null)
            {
                animator.runtimeAnimatorController = controller;�@// �w�肵��Animator Controller�������A�^�b�`
            }

            else
            {
                Debug.LogWarning(AnimPath + "�͌�����܂���ł���");
            }

        }

    }

    protected virtual void Start()
    {
        //ApplyTextureToModel();
    }

    // UV�W�J�ς݂�3D���f���ɐݒ肵��2D�e�N�X�`���𒣂�t���鏈��
    //private void ApplyTextureToModel()
    //{

        // �V�F�[�_�[���g���ĐV�����}�e���A���i3D���f���̌����ځj���쐬
        //Material newMaterial = new Material(Shader.Find("Standard"));

        // �}�e���A���փe�N�X�`�������蓖��
        //newMaterial.mainTexture = objectTexture;

        // �ݒ肵��3D���f���imodel�j����MeshRenderer�R���|�[�l���g���擾
        //MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();

        // model��meshRenderer�̃}�e���A����V����������}�e���A���ɍ����ւ���
        //meshRenderer.material = newMaterial;

    //}


    protected virtual void Update()
    {

        currentPosition = transform.position; // �I�u�W�F�N�g��3�������W�̎擾

        if (!wasCaught)
        {
            MovementConfig(); // ���t���[���I�u�W�F�N�g�̈ʒu�𓮂����Ȃ���A�j���[�V�����Đ�
        }

        HandleFishing();

    }

}
    
    
