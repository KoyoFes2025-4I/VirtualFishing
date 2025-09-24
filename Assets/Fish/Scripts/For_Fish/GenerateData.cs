using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GenerateData
{
    public List<ThingData> thingDatas; // �����ΏۂƂȂ�I�u�W�F�N�g��Prefab�̃��X�g
    public int keepThingCount = 5; // �I�u�W�F�N�g�̓����|�b�v���i��Ɉێ�����j
    public float spawnMargin = 3f; // ����̒[���琶�������ʒu�܂ł̗]��
}

[Serializable]
public class ThingData
{
    public GameObject prefab; // ���ۂɐ�������Prefab
    public float weight = 1f; // ���������m���̏d�݂Â��i�傫���قǐ�������₷���j
}