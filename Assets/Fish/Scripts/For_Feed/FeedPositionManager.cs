using UnityEngine;

public class FeedPositionManager : MonoBehaviour
{

    [SerializeField] private Vector3 FeedPosition; // �a�I�u�W�F�N�g�̈ʒu

    // �������ɃR���g���[���[����u�ނ�Ƃ��ǂꂾ�������U�������v�̂悤�Ȑ��l��Ԃ��Ă��炤
    // ���̏�񂩂�a�̈ʒu��FeedPosition�i3�����x�N�g���j���v�Z���čX�V������悤�ɂ���

    private void Update()
    {
       transform.position = FeedPosition; // �I�u�W�F�N�g�̍��W��ύX
    }

    // �a�̍��W�̃Q�b�^�[
    public Vector3 GetFeedPosition() => FeedPosition;

}
