using UnityEngine;

public class CreateObjects : MonoBehaviour
{

    // �v���n�u�������e�X��Fish�I�u�W�F�N�g�������ɏ]���Đ�������悤�ɂ���

    [SerializeField] private Vector3 prefab_position1; // �z�u���鏉�����W
    [SerializeField] private Vector3 prefab_position2;
    [SerializeField] private Vector3 prefab_position3;
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    [SerializeField] GameObject prefab3;

    // �I�u�W�F�N�g�̎�ނ̕������v���n�u���`����

    void Start()
    {

        // �|�b�v�̃A���S���Y�����l���Ċe������ݒ肷��

        for (int i = 0; i < 2; i++)
        {
            Instantiate(prefab1, prefab_position1, Quaternion.identity);
            Instantiate(prefab2, prefab_position2, Quaternion.identity);
            Instantiate(prefab3, prefab_position3, Quaternion.identity);
        }

    }

}
