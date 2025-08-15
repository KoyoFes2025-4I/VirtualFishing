using System;
using System.Collections.Generic;
using UnityEngine;

public class RodsController : MonoBehaviour
{
    [SerializeField]
    private GameObject Rod;
    private float baseRotationY = 0;
    private int rodCountIndex = 0;
    private float throwPower = 0.3f;
    private float power = 0.3f;
    private float maxRodStrength = 100;
    private int idGenerateIndex = 0;
    private float rodUIScale = 1f;
    private List<GameObject> rodInstances = new List<GameObject>();
    public List<BiteScript> bites { get; private set; } = new List<BiteScript>();
    public List<User> users { get; private set; } = new List<User>();
    public int RodsCount => rodInstances.Count;

    void Start()
    {
        UpdateRods();
    }

    void Update()
    {

    }

    public void UpdateRods()
    {
        foreach (GameObject instance in rodInstances) Destroy(instance);
        rodInstances.Clear();
        bites.Clear();

        List<Vector3> poss = StageManager.rodPlaceHolder[rodCountIndex];

        for (int i = 0; i < poss.Count; i++)
        {
            string id = "";
            if (idGenerateIndex == 0) id = i.ToString();
            else if (idGenerateIndex == 1) id = Guid.NewGuid().ToString();

            GameObject instance = Instantiate(Rod, poss[i], Quaternion.identity, transform);
            instance.SetActive(true);
            RodScript rodScript = instance.GetComponent<RodScript>();
            rodScript.SetId(id);
            rodScript.SetBaseRotationY(baseRotationY);
            rodScript.SetUIScale(rodUIScale);
            rodScript.SetThrowPower(throwPower);
            rodScript.SetPower(power);
            rodScript.SetMaxRodStrength(maxRodStrength);
            if (users.Count > i) rodScript.SetUser(users[i]);
            instance.name = $"Rod(id:{id})";
            rodInstances.Add(instance);

            bites.Add(instance.GetComponent<RodScript>().GetBiteScript);
        }
    }

    public void ChangeParams(int rodCountIndex, float baseRotationY, float throwPower, float power, float maxRodStrength, int idGenerateIndex, float rodUIScale)
    {
        this.rodCountIndex = rodCountIndex;
        this.baseRotationY = baseRotationY;
        this.throwPower = throwPower;
        this.maxRodStrength = maxRodStrength;
        this.power = power;
        this.idGenerateIndex = idGenerateIndex;
        this.rodUIScale = rodUIScale;

        UpdateRods();
    }

    public void SetUsers(List<User> users)
    {
        this.users = users;
    }
}
