using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RodsController : MonoBehaviour
{
    //Rods��D&D
    [SerializeField]
    private GameObject Rod;
    [SerializeField]
    private List<RodData> rodsData;
    [SerializeField]
    private float baseRotationY;
    private List<GameObject> rodInstances = new List<GameObject>(); //�������ꂽrod�̔z��
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        for (int i = 0; i < rodsData.Count; i++)
        {
            GameObject instance = Instantiate(Rod, rodsData[i].position, Quaternion.identity, transform);
            instance.GetComponent<RodScript>().SetId(rodsData[i].id);
            instance.GetComponent<RodScript>().SetBaseRotationY(baseRotationY);
            instance.name = $"Rod(id:{rodsData[i].id})";
            instance.SetActive(true);
            rodInstances.Add(instance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
