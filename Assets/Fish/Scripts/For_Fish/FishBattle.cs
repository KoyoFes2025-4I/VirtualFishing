using UnityEngine;
using UnityEngine.Rendering;

public class FishBattle : MonoBehaviour
{

    // ���ۂ̃o�g�������֐�

    private ThingsToFish fish;

    void Start()
    {
        fish = GetComponent<ThingsToFish>();
    }

    public void Battle(int FishStrength, int FishPower)
    {
        // 50%���ŏ��s�����܂�ݒ�
        if (Random.Range(-1f, 1f) >= 0.5f)
        {
            fish.SetSuccessFishing(); // �����t���O�𗧂ĂĂ���
            fish.SetFinishFishing(); // �o�g���I���i�������j
        }

        else
        {
            fish.SetFinishFishing(); // �o�g���I���i�s�k���j
        }
            
    }

}
