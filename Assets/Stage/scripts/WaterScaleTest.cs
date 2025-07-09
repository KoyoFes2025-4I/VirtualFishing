using UnityEngine;
/*カメラの形式を間違えていたスクリプト*/

public class WaterScaleTest : MonoBehaviour
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
        WaterScaleChange(cam1obj, waterFloor);
    }

    void WaterScaleChange(GameObject camera, GameObject water)
    {
        //カメラと水の距離を計算
        Camera cam = camera.GetComponent<Camera>();
        float distanceToWater = Mathf.Abs(camera.transform.position.y - water.transform.position.y);

        //ビューポートの四隅（左下と右上）をワールド座標に変換
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0, 0, distanceToWater));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1, 1, distanceToWater));

        //幅と奥行き（XZ平面上）を計算
        float width = Mathf.Abs(tr.x - bl.x);
        float depth = Mathf.Abs(tr.z - bl.z);

        float scaleX = width / PLANE_SIZE;
        float scaleZ = depth / PLANE_SIZE;

        water.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
    }
}
