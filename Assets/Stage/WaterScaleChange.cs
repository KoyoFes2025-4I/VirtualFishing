using UnityEngine;

public class WaterScaleChange : MonoBehaviour
{
    GameObject cam1obj, cam2obj; //2�̃J����
    GameObject waterFloor; //��
    Camera cam1, cam2;

    const float PLANE_SIZE = 10f;
    [SerializeField]private float cameraY = 1000f;

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
        WaterFloorScaleChange(cam1obj, cam2obj, waterFloor, cameraY);
    }

    void WaterFloorScaleChange(GameObject camera1, GameObject camera2, GameObject water ,  float camY)
    {
        Camera cam1 = camera1.GetComponent<Camera>();
        Camera cam2 = camera2.GetComponent<Camera>();

        float height = cam1.orthographicSize * 2f;
        float width = height * cam1.aspect;

        float scaleX = (width * 2) / PLANE_SIZE;
        float scaleZ = height / PLANE_SIZE;

        water.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
        camera1.transform.position = new Vector3(-width / 2, camY, 0f);
        camera2.transform.position = new Vector3(width / 2, camY, 0f);
    }
}
