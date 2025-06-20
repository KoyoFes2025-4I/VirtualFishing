using UnityEngine;

public class RodsController : MonoBehaviour
{
    //RodsÇD&D
    public GameObject Rod;
    public int numOfRods = 3;
    public Vector3 spawnArea = new Vector3(0, 0, 0);
    private GameObject[] rodInstances; //ê∂ê¨Ç≥ÇÍÇΩrodÇÃîzóÒ
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rodInstances = new GameObject[numOfRods];

        for (int i = 0; i < numOfRods; i++)
        {
            Vector3 RodPosition = new Vector3(2 * i, 0, 0);
            GameObject instance = Instantiate(Rod, RodPosition, Quaternion.identity);
            rodInstances[i] = instance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
