using UnityEngine;

// [RequireComponent(typeof(InWaterManager))]

public class FeedManager : MonoBehaviour
{

    // private InWaterManager inWaterManager;�i���j

    [SerializeField] private GameObject model; // �a��3D���f���iBlender�ō쐬�j

    private Vector3 FeedPosition; // �a�I�u�W�F�N�g�̈ʒu

    // ��������InWaterManager����u�ނ�Ƃ��ǂꂾ�������U�������v�̂悤�Ȑ��l��Ԃ��Ă��炤�z��
    // ���̏�񂩂�a�̈ʒuFeedPosition�i3�����x�N�g���j���v�Z���Đݒ肷��

    private void Update()
    {
       �@transform.position = FeedPosition; // �I�u�W�F�N�g�̍��W��ύX
    }

    // �a�̍��W�̃Q�b�^�[
    public Vector3 GetFeedPosition() => FeedPosition;

}
