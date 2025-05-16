using UnityEngine;
/*�J�����̌`�����ԈႦ�Ă����X�N���v�g*/

public class WaterScaleTest : MonoBehaviour
{
    GameObject cam1obj, cam2obj; //2�̃J����
    GameObject waterFloor; //��
    Camera cam1, cam2;

    const float PLANE_SIZE = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�J����2���擾����
        cam1obj = GameObject.Find("Camera1"); //�J�����̖��O������ɂ��Ȃ��ƃ_��
        cam2obj = GameObject.Find("Camera2"); //��ɓ���

        //�����擾����
        waterFloor = GameObject.Find("WaterFloor");
    }

    // Update is called once per frame
    void Update()
    {
        WaterScaleChange(cam1obj, waterFloor);
    }

    void WaterScaleChange(GameObject camera, GameObject water)
    {
        //�J�����Ɛ��̋������v�Z
        Camera cam = camera.GetComponent<Camera>();
        float distanceToWater = Mathf.Abs(camera.transform.position.y - water.transform.position.y);

        //�r���[�|�[�g�̎l���i�����ƉE��j�����[���h���W�ɕϊ�
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0, 0, distanceToWater));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1, 1, distanceToWater));

        //���Ɖ��s���iXZ���ʏ�j���v�Z
        float width = Mathf.Abs(tr.x - bl.x);
        float depth = Mathf.Abs(tr.z - bl.z);

        float scaleX = width / PLANE_SIZE;
        float scaleZ = depth / PLANE_SIZE;

        water.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
    }
}
