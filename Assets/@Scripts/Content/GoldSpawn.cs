using System.Collections.Generic;
using UnityEngine;

public class GoldSpawn : Singleton<GoldSpawn>
{
    // ��� ���� ������Ʈ Ǯ�� ����
    public GameObject Gold;
    
    public List<GameObject> GoldList = new List<GameObject>();

    GameObject _goldPool;

    protected override void Initialize()
    {
        base.Initialize();
        _goldPool = new GameObject("GoldPool");
    }

    public GameObject GetGold(Vector3 spawnPos)
    {
        for (int i = 0; i < GoldList.Count; i++)
        {
            if (!GoldList[i].activeSelf)
            {
                GoldList[i].SetActive(true);
                GoldList[i].transform.position = spawnPos;
                return GoldList[i];
            }
        }

        GameObject gold = Instantiate(Gold);
        gold.transform.parent = _goldPool.transform;
        gold.transform.position = spawnPos;
        gold.AddComponent<Rigidbody2D>();
        GoldList.Add(gold);
        return gold;
    }
}
