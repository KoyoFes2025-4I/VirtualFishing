using UnityEngine;

public class CreateObjects : MonoBehaviour
{

    // �v���n�u�������e�X��Fish�I�u�W�F�N�g�������ɏ]���Đ�������悤�ɂ���

    [SerializeField] private Vector3 prefab_position;// �z�u������W
    [SerializeField] GameObject prefab1;
    [SerializeField] GameObject prefab2;
    // �I�u�W�F�N�g�̎�ނ̕������v���n�u���`����

    void Start()
    {

        // �|�b�v�̃A���S���Y�����l���Ċe������ݒ肷��

        for (int i = 0; i < 2; i++)
        {
            Instantiate(prefab1, prefab_position, Quaternion.identity);
        }

        for (int j = 0; j < 2; j++)
        {
            Instantiate(prefab2, prefab_position, Quaternion.identity);
        }

    }

}
