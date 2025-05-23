using UnityEngine;

public class WaterScaleChange : MonoBehaviour
{
    GameObject cam1obj, cam2obj; //2つのカメラ
    GameObject waterFloor; //水
    Camera cam1, cam2;

    const float PLANE_SIZE = 10f;
    [SerializeField]private float cameraY = 1000f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //カメラ2つを取得する
        cam1obj = GameObject.Find("Camera1"); //カメラの名前をこれにしないとダメ
        cam2obj = GameObject.Find("Camera2"); //上に同じ

        //水を取得する
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
