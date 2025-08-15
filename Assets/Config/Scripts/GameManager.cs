using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    ThingGenerator thingGenerator;
    [SerializeField]
    RodsController rodsController;
    [SerializeField]
    Config config;

    public List<User> waitUsers { get; } = new List<User>();
    public List<User> nextUsers { get; } = new List<User>();
    public List<User> gamingUsers { get; } = new List<User>();

    public bool isGaming { get; private set; } = false;

    public void StartGame()
    {
        if (isGaming) return;
        isGaming = true;
        nextUsers.Clear();

        for (int i = 0; i < rodsController.RodsCount && waitUsers.Count > 0; i++)
        {
            nextUsers.Add(waitUsers[0]);
            waitUsers.RemoveAt(0);
        }

        thingGenerator.isGenerate = true;
        rodsController.ShowMessage("スタート！", 3);

        config.ListViewRefresh();
    }

    public void Prepare()
    {
        if (isGaming) return;

        gamingUsers.Clear();
        gamingUsers.AddRange(nextUsers);

        thingGenerator.isGenerate = false;
        thingGenerator.Regenerate();

        rodsController.SetUsers(gamingUsers);
        rodsController.UpdateRods();

        config.ListViewRefresh();
    }

    public void SetNextUsers(List<string> users)
    {
        nextUsers.Clear();
        foreach (string userName in users) nextUsers.Add(new User(userName));
    }

    public void SetWaitUsers(List<string> users)
    {
        waitUsers.Clear();
        foreach (string userName in users) waitUsers.Add(new User(userName));
    }

    public void FinishGame()
    {
        isGaming = false;
        foreach (User user in gamingUsers) user.Save();

        rodsController.Reset();
        rodsController.ShowResult();

        thingGenerator.isGenerate = false;
        thingGenerator.Regenerate();
    }
}

[Serializable]
public class User
{
    public User(string name)
    {
        this.name = name;
    }
    public string name = "";
    public int point = 0;
    public HashSet<string> fishedThingNames = new HashSet<string>();

    public void Save()
    {
        Debug.Log($"{name}: {point}pt, [{string.Join(", ", fishedThingNames)}]");
    }
}
