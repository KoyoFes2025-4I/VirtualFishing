using UnityEngine;
using UnityEngine.InputSystem;

public class InWaterManager : MonoBehaviour
{
    // �����t���O�̊Ǘ�

    public bool isInWater { get; private set; } = false; // �ނ�Ƃ̒����t���O

    void Update()
    {
        // �e�X�g�p�ŁA�X�y�[�X�L�[���������тɒ��������ς���iGame�r���[�łȂ��Ɣ������Ȃ��j
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isInWater = !isInWater;

            if (isInWater)
            {
                Debug.Log("�������� : true");
            }
            else
            {
                Debug.Log("�������� : false");
            }

        }
    }

    public bool GetInWater() => isInWater; // �Q�b�^�[
    public void SetTrueInWater() => isInWater = true; // true�̃Z�b�^�[
    public void SetFalseInWater() => isInWater = false; // false�̃Z�b�^�[

}