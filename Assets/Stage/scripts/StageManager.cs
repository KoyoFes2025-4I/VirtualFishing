using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    const float PLANE_SIZE = 50f;
    [SerializeField] private float cameraY = 1000f;
    [SerializeField] private float cameraSize = 10f;

    [SerializeField] private GameObject camera1;
    [SerializeField] private GameObject camera2;
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject wall0;
    [SerializeField] private GameObject wall1;
    [SerializeField] private GameObject wall2;
    [SerializeField] private GameObject wall3;
    [SerializeField] private float wallThickness = 0.5f;
    private float padding = 1.5f;

    public static List<List<Vector3>> rodPlaceHolder = new List<List<Vector3>>(); 
    public float width { get; private set; }
    public float height { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateFloorScaleChange();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateFloorScaleChange()
    {
        //ゲームオブジェクトからカメラのコンポーネント取得
        Camera cam1 = camera1.GetComponent<Camera>();
        Camera cam2 = camera2.GetComponent<Camera>();

        float height = cameraSize * 2.0f;
        float width = height * cam1.aspect;

        float scaleX = width / PLANE_SIZE;
        float scaleZ = (height * 2) / PLANE_SIZE;

        //水の大きさを変えてカメラもそれに合わせて動かす
        water.transform.localScale = new Vector3(scaleX, 1f, scaleZ);
        camera1.transform.position = new Vector3(0, cameraY, height / 2);
        camera2.transform.position = new Vector3(0, cameraY, -height / 2);
        cam1.orthographicSize = cameraSize;
        cam2.orthographicSize = cameraSize;

        StageWallSet(width, height * 2, wall0, wall1, wall2, wall3);
        this.width = width; this.height = height * 2;

        rodPlaceHolder.Clear();
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 2 - padding, 3, height / 4 * 3), new Vector3(width / 2 - padding, 3, height / 4), new Vector3(width / 2 - padding, 3, -height / 4), new Vector3(width / 2 - padding, 3, -height / 4 * 3), new Vector3(-width / 2 + padding, 3, -height / 4 * 3), new Vector3(-width / 2 + padding, 3, -height / 4), new Vector3(-width / 2 + padding, 3, height / 4), new Vector3(-width / 2 + padding, 3, height / 4 * 3) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 2 - padding, 3, height / 2), new Vector3(width / 2 - padding, 3, -height / 2), new Vector3(-width / 2 + padding, 3, -height / 2), new Vector3(-width / 2 + padding, 3, height / 2) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 2 - padding, 3, height / 4 * 3), new Vector3(width / 2 - padding, 3, height / 4), new Vector3(width / 2 - padding, 3, -height / 4), new Vector3(width / 2 - padding, 3, -height / 4 * 3) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(-width / 2 + padding, 3, -height / 4 * 3), new Vector3(-width / 2 + padding, 3, -height / 4), new Vector3(-width / 2 + padding, 3, height / 4), new Vector3(-width / 2 + padding, 3, height / 4 * 3) });
    }

    void StageWallSet(float x, float z, GameObject w0, GameObject w1, GameObject w2, GameObject w3)
    {
        w0.transform.position = new Vector3(-x / 2, 0f, 0f);
        w0.transform.localScale = new Vector3(wallThickness, 100f, z);
        w1.transform.position = new Vector3(x / 2, 0f, 0f);
        w1.transform.localScale = new Vector3(wallThickness, 100f, z);
        w2.transform.position = new Vector3(0f, 0f, z / 2);
        w2.transform.localScale = new Vector3(x, 100f, wallThickness);
        w3.transform.position = new Vector3(0f, 0f, -z / 2);
        w3.transform.localScale = new Vector3(x, 100f, wallThickness);
    }

    public void ChangeParams(float cameraY, float cameraSize)
    {
        this.cameraY = cameraY;
        this.cameraSize = cameraSize;
        UpdateFloorScaleChange();
    }
}
