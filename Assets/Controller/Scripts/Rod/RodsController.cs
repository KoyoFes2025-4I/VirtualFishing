using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
            rodScript.SetStageStyle(StageManager.stageStyle);
            rodScript.SetIsSmartPhone(Config.config.fishingRodControllerCount <= i);
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

    public void Reset()
    {
        foreach (GameObject rod in rodInstances)
        {
            rod.GetComponent<RodScript>().Reset();
        }
    }

    public void ShowMessage(string message, float duration)
    {
        foreach (GameObject rod in rodInstances)
        {
            rod.GetComponent<RodScript>().ShowMessage(message, duration);
        }
    }

    public void ShowResult()
    {
        var rodPoints = rodInstances
            .Select(rod => new {
                Rod = rod,
                Point = rod.GetComponent<RodScript>().GetPoint()
            })
            .ToList();

        // 2️⃣ ポイント順で並べ替え（高い順）
        var sorted = rodPoints
            .OrderByDescending(r => r.Point)
            .ToList();

        // 3️⃣ ランキングを計算（同率順位対応）
        Dictionary<GameObject, int> rodRanks = new Dictionary<GameObject, int>();

        int currentRank = 1;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (i > 0 && sorted[i].Point < sorted[i - 1].Point)
            {
                // 前よりポイントが低ければ順位を更新（同点なら更新しない）
                currentRank = i + 1;
            }

            rodRanks[sorted[i].Rod] = currentRank;
        }

        foreach (var pair in rodRanks)
        {
            if (pair.Value <= Config.config.rankingShowCount) pair.Key.GetComponent<RodScript>().ShowResult(pair.Value);
            else pair.Key.GetComponent<RodScript>().ShowResult();
        }
    }
}
