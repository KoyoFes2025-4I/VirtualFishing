using UnityEngine;
/*カメラの形式を間違えていたスクリプト*/

public class WaterScaleChange : MonoBehaviour
{
    GameObject cam1obj, cam2obj; //2つのカメラ
    GameObject waterFloor; //水
    Camera cam1, cam2;

    const float PLANE_SIZE = 10f;

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
