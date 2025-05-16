using UnityEngine;
/*�J�����̌`�����ԈႦ�Ă����X�N���v�g*/

public class WaterScaleChange : MonoBehaviour
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
        WaterFloorScaleChange(cam1obj, waterFloor);
    }

    void WaterFloorScaleChange(GameObject camera, GameObject water)
    {
        Camera cam = camera.GetComponent<Camera>();

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        float scaleX = width / PLANE_SIZE;
        float scaleZ = height / PLANE_SIZE;

        water.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
    }
}
