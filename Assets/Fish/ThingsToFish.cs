using NUnit.Framework;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(FishAnimations))]
[RequireComponent(typeof(BattleManager))]
[RequireComponent(typeof(PointManager))]
// [RequireComponent(typeof(InWaterManager))]

// �u�ނ���́v�̋��ʂ̏�����t�B�[���h���������e�N���X�i���ۃN���X�j

public abstract class ThingsToFish : MonoBehaviour
{

    // �R���g���[���[�ǂɒ���������s���v���O�����͍���Ă��炤
    // �Q�b�^�[�����isInWater�t���O�ŏ�Ԃ��擾����z��
    // �������ɉa�I�u�W�F�N�g���ړ������邽�߂̏����Ԃ��Ă��炤��FeedManager�̕��ŎQ��

    private FishAnimations fishAnimations;
    private BattleManager battleManager;
    private PointManager pointManager;
    // private InWaterManager inWaterManager;�i���j

    // Inspector����l�̃Z�b�g���\�ȃV���A���C�Y�t�B�[���h
    [SerializeField] private GameObject model; // UV�W�J�ς݂�3D���f���iBlender�ō쐬�j
    [SerializeField] private Texture2D objectTexture; // ���f���ɒ���t����2D�e�N�X�`��
    [SerializeField] private string objectName; // �I�u�W�F�N�g��
    [SerializeField] private int strength; //�̗̓p�����[�^
    [SerializeField] private int power; //�̓p�����[�^
    [SerializeField] private int weight; //�d�ʁi�ړ����x�j�p�����[�^
    [SerializeField] private int point; //�ނ������̓��_
    [SerializeField] private string creater; //����ҁiID�j

    // strength��power�̓ǂݎ��p
    public int Strength => strength;
    public int Power => power;

    public Vector3 currentPosition { get; private set; } //�I�u�W�F�N�g�̌��݂�3�������W

    public bool isInWater { get; private set; } = false; // �ނ�Ƃ̒����t���O
    public bool wasCaught { get; private set; } = false; // �a�I�u�W�F�N�g�Ƃ̐ڐG�t���O
    public bool hasStartedBattle { get; private set; } = false; // �o�g���J�n�̃t���O
    public bool isInBattle { get; private set; } = false; // ����ނ肠����o�g�����ł���t���O
    public bool wasSuccessFishing { get; private set; } = false; // �ނ�グ�ɐ��������t���O
    public bool wasFinishFishing { get; private set; } = false; // �ނ�o�g���I���t���O

    // �e�t���O�̃Z�b�^�[�p�̃��\�b�h�i�O���t�@�C���ŌĂяo���j
    public void SetTrueInBattle() => isInBattle = true;
    public void SetFalseInBattle() => isInBattle = false;
    public void SetSuccessFishing() => wasSuccessFishing = true;
    public void SetFinishFishing() => wasFinishFishing= true;

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

    // ���̉j���̓����̃A���S���Y���ƃA�j���[�V��������
    protected virtual void MovementConfig()
    {

        // �d�ʂ̃p�����[�^�ňړ����x�����߂ċ����ړ�������
        transform.Translate(Vector3.forward * Time.deltaTime * weight);

        // �ǂɓ���������i�s������ς���悤�ɂ���i�X�e�[�W�ǂƘA�g�j

        fishAnimations.PlayMoveAnimation(); // ���̉j���A�j���[�V�����̍Đ�

    }

    // �o�g���ɏ��������ɌĂ΂�鏈��
    protected virtual void WinFishing()
    {

        fishAnimations.PlayWinAnimation(); // �������̃A�j���[�V�����̍Đ�

        pointManager.AddPoint(point); // �ނ�グ�����̓��_�����Z
        ResetCatchState(); // �t���O�̏�Ԃ̃��Z�b�g

        // �ނ�グ�����̃��f���A���O�A���_�Ȃǂ̉�ʏ�ւ̕\���v���Z�X����������
        // �K�v�ȃf�[�^�̓f�[�^�x�[�X�֊i�[����悤�ɂ���

        Destroy(gameObject); // �ނ�グ�����������炻�̃I�u�W�F�N�g�͏������

    }

    // �o�g���ɕ��������ɌĂ΂�鏈��
    protected virtual void LoseFishing()
    {

        fishAnimations.PlayLoseAnimation(); // ���s���̃A�j���[�V�����̍Đ�

        ResetCatchState(); // �t���O�̏�Ԃ̃��Z�b�g

        // ���������̌�̋��I�u�W�F�N�g�Ɋւ��鏈��������

    }
   
    // �����߂܂��Ă���̏���
    protected virtual void HandleFishing()
    {

        // �������ɉa�I�u�W�F�N�g�ƐڐG�������͎��̃A�j���[�V�����Ɉڍs
        if (isInWater && wasCaught)
        {

            // �o�g�����ɖ��t���[���Ă΂��
            if (isInBattle)
            {
                fishAnimations.PlayCaughtAnimation(); // �o�g�����̋��������������Ă���A�j���[�V�����̍Đ�
            }

            // �o�g�����n�܂鎞
            if (!isInBattle && !hasStartedBattle)
            {
                hasStartedBattle = true; // 2�x�ڂ͌Ă΂�Ȃ��悤�ɂ���
                battleManager.DoBattle(); // �o�g�������̊J�n�i���ŃR���[�`�������݁j
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
        fishAnimations = GetComponent<FishAnimations>();
        battleManager = GetComponent<BattleManager>();
        pointManager = GetComponent<PointManager>();
        // inWaterManager = GetComponent<InWaterManager>();
    }

    protected virtual void Start()
    {
        ApplyTextureToModel();
    }

    // UV�W�J�ς݂�3D���f���ɐݒ肵��2D�e�N�X�`���𒣂�t���鏈��
    private void ApplyTextureToModel()
    {

        // �V�F�[�_�[���g���ĐV�����}�e���A���i3D���f���̌����ځj���쐬
        Material newMaterial = new Material(Shader.Find("Standard"));

        // �}�e���A���փe�N�X�`�������蓖��
        newMaterial.mainTexture = objectTexture;

        // �ݒ肵��3D���f���imodel�j����MeshRenderer�R���|�[�l���g���擾
        MeshRenderer meshRenderer = model.GetComponent<MeshRenderer>();

        // model��meshRenderer�̃}�e���A����V����������}�e���A���ɍ����ւ���
        meshRenderer.material = newMaterial;

    }


    protected virtual void Update()
    {

        // isInWater = inWaterManger.GetIsInWater(); // �����t���O�̏�Ԃ��擾

        currentPosition = transform.position; //�I�u�W�F�N�g��3�������W�̎擾

        if (!wasCaught)
        {
            MovementConfig(); // ���t���[���I�u�W�F�N�g�̈ʒu�𓮂����Ȃ���A�j���[�V�����Đ�
        }

        HandleFishing();

    }

}
    
    
