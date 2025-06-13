using UnityEngine;
using System.Collections.Generic;

public class Test_rotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Dictionary<string, float> rot_speed;
        rot_speed = GetComponent<RotationSubscriber>().GetRotations();


        Debug.Log(rot_speed);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
