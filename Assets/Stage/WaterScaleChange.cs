using UnityEngine;

public class WaterScaleChange : MonoBehaviour
{
    GameObject cam1obj, cam2obj; //2つのカメラ
    GameObject waterFloor; //水
    Camera cam1, cam2;

    const float PLANE_SIZE = 50f;
    [SerializeField] private float cameraY = 1000f;
    [SerializeField] private float cameraSize = 500f;

    [SerializeField] private GameObject camera1;
    [SerializeField] private GameObject camera2;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject wall0;
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;
    [SerializeField] private GameObject wall3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        WaterFloorScaleChange(camera1, camera2, water, cameraY, cameraSize);
    }

    void WaterFloorScaleChange(GameObject camera1, GameObject camera2, GameObject water,  float camY , float camSize)
        //カメラ1のゲームオブジェクト、カメラ2、水、カメラ高さ、カメラ大きさ
        //これらはゲーム中変更しない
    {
        //ゲームオブジェクトからカメラのコンポーネント取得
        Camera cam1 = camera1.GetComponent<Camera>();
        Camera cam2 = camera2.GetComponent<Camera>();

        float height = camSize * 2.0f;
        float width = height * cam1.aspect;

        float scaleX = width / PLANE_SIZE;
        float scaleZ = (height * 2) / PLANE_SIZE;

        //水の大きさを変えてカメラもそれに合わせて動かす
        water.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
        camera1.transform.position = new Vector3(0, camY, height / 2);
        camera2.transform.position = new Vector3(0, camY, -height / 2);
        cam1.orthographicSize = camSize;
        cam2.orthographicSize = camSize;

        StageWallSet(width, height*2, wall0, wall1, wall2, wall3);
    }

    void StageWallSet(float x, float z, GameObject w0, GameObject w1, GameObject w2, GameObject w3)
    {
        w0.transform.position = new Vector3(-x/2, 0f, 0f);
        w0.transform.localScale = new Vector3(10f, 100f, z);
        w1.transform.position = new Vector3(x / 2, 0f, 0f);
        w1.transform.localScale = new Vector3(10f, 100f, z);
        w2.transform.position = new Vector3(0f, 0f, z/2);
        w2.transform.localScale = new Vector3(x, 100f, 10f);
        w3.transform.position = new Vector3(0f, 0f, -z / 2);
        w3.transform.localScale = new Vector3(x, 100f, 10f);
    }
}
