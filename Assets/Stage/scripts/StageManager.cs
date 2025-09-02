using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private float cam1Rotation = 0f;
    private float cam2Rotation = 180f;
    public static int stageStyle { get; private set; } = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateFloorScaleChange();

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
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

        if (stageStyle == 0)
        {
            StageWallSet(width, height * 2, wall0, wall1, wall2, wall3);
            this.width = width; this.height = height * 2;
        }
        else
        {
            StageWallSet(width * 2, height, wall0, wall1, wall2, wall3);
            this.width = width * 2; this.height = height;
        }

        float scaleX = this.width / PLANE_SIZE;
        float scaleZ = this.height / PLANE_SIZE;

        //水の大きさを変えてカメラもそれに合わせて動かす
        water.transform.localScale = new Vector3(Mathf.Max(scaleX, scaleZ), Mathf.Max(scaleX, scaleZ), Mathf.Max(scaleX, scaleZ));
        if (stageStyle == 0f)
        {
            camera1.transform.position = new Vector3(0, cameraY, this.height / 4);
            camera2.transform.position = new Vector3(0, cameraY, -this.height / 4);
        }
        else
        {
            camera1.transform.position = new Vector3(this.width / 4, cameraY, 0);
            camera2.transform.position = new Vector3(-this.width / 4, cameraY, 0);
        }
        camera1.transform.eulerAngles = new Vector3(90f, 0, cam1Rotation);
        camera2.transform.eulerAngles = new Vector3(90f, 0, cam2Rotation);
        cam1.orthographicSize = cameraSize;
        cam2.orthographicSize = cameraSize;

        rodPlaceHolder.Clear();
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 2 - padding, 3, height / 4 * 3), new Vector3(width / 2 - padding, 3, height / 4), new Vector3(width / 2 - padding, 3, -height / 4), new Vector3(width / 2 - padding, 3, -height / 4 * 3), new Vector3(-width / 2 + padding, 3, -height / 4 * 3), new Vector3(-width / 2 + padding, 3, -height / 4), new Vector3(-width / 2 + padding, 3, height / 4), new Vector3(-width / 2 + padding, 3, height / 4 * 3) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 2 - padding, 3, height / 2), new Vector3(width / 2 - padding, 3, -height / 2), new Vector3(-width / 2 + padding, 3, -height / 2), new Vector3(-width / 2 + padding, 3, height / 2) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 2 - padding, 3, height / 4 * 3), new Vector3(width / 2 - padding, 3, height / 4), new Vector3(width / 2 - padding, 3, -height / 4), new Vector3(width / 2 - padding, 3, -height / 4 * 3) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(-width / 2 + padding, 3, -height / 4 * 3), new Vector3(-width / 2 + padding, 3, -height / 4), new Vector3(-width / 2 + padding, 3, height / 4), new Vector3(-width / 2 + padding, 3, height / 4 * 3) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 8 * 7, 3, -height / 2 + padding), new Vector3(width / 8 * 5, 3, -height / 2 + padding), new Vector3(width / 8 * 3, 3, -height / 2 + padding), new Vector3(width / 8 * 1, 3, -height / 2 + padding), new Vector3(width / 8 * -1, 3, -height / 2 + padding), new Vector3(width / 8 * -3, 3, -height / 2 + padding), new Vector3(width / 8 * -5, 3, -height / 2 + padding), new Vector3(width / 8 * -7, 3, -height / 2 + padding) });
        rodPlaceHolder.Add(new List<Vector3>() { new Vector3(width / 6 * 5, 3, -height / 2 + padding), new Vector3(width / 6 * 3, 3, -height / 2 + padding), new Vector3(width / 6 * 1, 3, -height / 2 + padding), new Vector3(width / 6 * -1, 3, -height / 2 + padding), new Vector3(width / 6 * -3, 3, -height / 2 + padding), new Vector3(width / 6 * -5, 3, -height / 2 + padding) });
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

    public void ChangeParams(float cameraY, float cameraSize, float cam1Rotation, float cam2Rotation, int stageStyle)
    {
        this.cameraY = cameraY;
        this.cameraSize = cameraSize;
        this.cam1Rotation = cam1Rotation;
        this.cam2Rotation = cam2Rotation;
        StageManager.stageStyle = stageStyle;
        UpdateFloorScaleChange();
    }
}
