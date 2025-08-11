using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // ���ۂ̃o�g�������֐�

    private ThingsToFish fish;

    private int strength;
    private int power;
    private bool isBattling = false;

    public void Battle(int FishStrength, int FishPower)
    {
        strength = FishStrength;
        power = FishPower;
        isBattling = true; // Battle�֐����Ăяo���ꂽ�����L��Update�֐����A�N�e�B�u��
    }

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    private void Update()
    {
        if (!isBattling) return; // isBattling��false���Ɖ������Ȃ�

        // �e�X�g�p��W�L�[���������珟��
        if (Input.GetKeyDown(KeyCode.W))
        {
            fish.SetSuccessFishing(); // �����t���O�𗧂Ă�
            fish.SetFinishFishing(); // �o�g���I���i�����j
            isBattling = false;
        }

        // �e�X�g�p��L�L�[����������s�k
        else if (Input.GetKeyDown(KeyCode.L))
        {
            fish.SetFinishFishing(); // �o�g���I���i�s�k�j
            isBattling = false;
        }
    }
}