using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpawningPool : Singleton<SpawningPool>
{
    private Coroutine _coRespawnPool;
    WaitForSeconds _spawnInterval = new WaitForSeconds(5f);

    private int _maxSpawnCount = 24; // 최대 스폰 개수
    //private int _currentSpawnCount = 0; // 현재 살아있는 몬스터 개수
    // 총 슬라임 리스트
    private List<SlimeGreenController> _spawnSlimes = new List<SlimeGreenController>();
    // 죽은 슬라임 리스트
    private List<SlimeGreenController> _deadSlimes = new List<SlimeGreenController>();

    float minX = -30f;
    float maxX = 30f;
    float spacing = 2f;
    List<Vector2> occupiedPos = new List<Vector2>();

    void Start()
    {
        ObjectManager.Instance.ResourceAllLoad();
        ObjectManager.Instance.Spawn<PlayerController>(new Vector2(0, -1.94f));

        for (int i = 0; i < _maxSpawnCount; i++)
        {
            //Vector3 spawnPos = GetRandomPositionField
            //    (ObjectManager.Instance.Player.transform.position);
            //spawnPos.y = -2.59f;

            Vector3 spawnPos = GetRandomPositionOnField(minX, maxX, spacing, occupiedPos);

            SlimeGreenController greenSlime = PoolManager.Instance.GetObject<SlimeGreenController>(spawnPos);
            _spawnSlimes.Add(greenSlime);
        }
    }

    public void SlimeDie(SlimeGreenController greenSlime)
    {
        _deadSlimes.Add(greenSlime);

        if (_coRespawnPool == null)
        {
            _coRespawnPool = StartCoroutine(CoRespawnMonster());
        }
    }

    IEnumerator CoRespawnMonster()
    {
        yield return _spawnInterval;

        foreach (var slime in _deadSlimes)
        {
            //Vector3 spawnPos = GetRandomPositionAround
            //    (ObjectManager.Instance.Player.transform.position);
            //spawnPos.y = -2.59f;

            Vector3 spawnPos = GetRandomPositionOnField(minX, maxX, spacing, occupiedPos);


            slime.transform.position = spawnPos;
            slime.gameObject.SetActive(true);
        }

        _deadSlimes.Clear();
        _coRespawnPool = null;
    }

    public Vector2 GetRandomPositionAround
        (Vector2 origin, float minDistance = 8f, float maxDistance = 10)
    {
        float distance = Random.Range(minDistance, maxDistance);
        float offsetX = Random.Range(-distance, distance);
        float offsetY = 0;

        Vector2 pos = origin + new Vector2(offsetX, offsetY);

        return pos;
    }

    public Vector2 GetRandomPositionOnField
        (float minX, float maxX, float spacing, List<Vector2> occupiedPos)
    {
        Vector2 spawnPos;
        int maxAttempts = 10; // 최대 시도 횟수 (겹치지 않는 위치 찾기)
        float spawnY = -2.59f;

        for (int i = 0; i < maxAttempts; i++)
        {
            float spawnX = Random.Range(minX, maxX);

            spawnX = Mathf.Round(spawnX / spacing) * spacing;
            spawnPos = new Vector2(spawnX, spawnY);

            // 다른 몬스터들과 겹치지 않도록 확인
            bool isOverlapping = false;
            foreach (var pos in occupiedPos)
            {
                if (Vector2.Distance(spawnPos, pos) < spacing)
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (!isOverlapping)
            {
                occupiedPos.Add(spawnPos);
                return spawnPos;
            }
        }

        return new Vector2(Random.Range(minX, maxX), spawnY);
    }
}
