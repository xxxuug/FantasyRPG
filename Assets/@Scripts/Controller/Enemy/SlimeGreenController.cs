using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeGreenController : EnemyController
{
    Animator _animator;
    private Vector2 _initPos;
    private float _moveSpeed = 1.5f;
    private bool _isHit = false;
    private bool _isMoving = false;
    private float _maxHp = 3;
    private float _currentHp;

    public float SlimeDamage = 1;
    public float minX = -25f;
    public float maxX = 25f;
    private Vector2 targetPos;

    protected override void Initialize()
    {
        base.Initialize();
        _animator = GetComponent<Animator>();
        _initPos = transform.position;
        _currentHp = _maxHp;

        StartCoroutine(RandomMovement());
    }

    private void OnEnable()
    {
        _currentHp = _maxHp;
    }

    IEnumerator RandomMovement()
    {
        while (true)
        {
            if (_isHit) yield break;
            _isMoving = true;

            targetPos = new Vector2(Random.Range(minX, maxX), transform.position.y);
            float moveDuration = Random.Range(4f, 8f);
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                if (_isHit) yield break;
                transform.position = Vector2.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isMoving = false;
            yield return new WaitForSeconds(Random.Range(1f, 3f));

        }
    }

    void PlayHitAnimation()
    {
        Debug.Log("[������] �ǰ� �ִϸ��̼� �����");
        _animator.SetTrigger(Define.HitHash);
        _isHit = true;
        _isMoving = false;

        StopCoroutine(RandomMovement());
        targetPos = transform.position;
        Invoke(nameof(StopHit), 0.5f);
    }

    void PlayDieAnimation()
    {
        Debug.Log("[������] ���� �ִϸ��̼� ����");
        _animator.SetTrigger(Define.DieHash);
    }

    void StopHit()
    {
        _isHit = false;
        if (!_isMoving)
        {
            _isMoving = true;
            StartCoroutine(RandomMovement());
        }
    }

    protected override void SlimeHitAndDie(float damage)
    {
        base.SlimeHitAndDie(damage);
        _currentHp -= damage;
        if (_currentHp <= 0)
        {
            PlayDieAnimation();
            Invoke(nameof(base.Despawn), 0.5f);
            SpawningPool.Instance.SlimeDie(this);
            Drop();
            GameManager.Instance.LevelUp(50);
        }
        else
        {
            PlayHitAnimation();
        }

    }

    void Drop()
    {
        List<GameObject> dropItems = new List<GameObject>();

        //Debug.Log("Gold ���� �� : " + goldRnd);
        if (Random.Range(0, 100) < 70)
        {
            Debug.Log("��� ���");
            dropItems.Add(GoldSpawn.Instance.GetGold(transform.position));
        }
        if (Random.Range(0, 100) < 70)
        {
            Debug.Log("��� ���");
            dropItems.Add(ItemSpawn.Instance.DropEquip(transform.position));
        }
        if (Random.Range(0, 100) < 70)
        {
            Debug.Log("������ ���");
            dropItems.Add(ItemSpawn.Instance.DropItem(transform.position));
        }

        int itemCount = dropItems.Count;
        if (itemCount == 0) return; // �ƹ� �����۵� ������� ������ ����

        float spreadAngle = 30f;

        for (int i = 0; i < itemCount; i++)
        {
            float randomAngleOffset = Random.Range(-5f, 5f);
            float baseAngle = (i / (float)itemCount) * 2f - 1f;
            // ������ ������ ���� ���� ���� ���
            float angleOffset = baseAngle * spreadAngle + randomAngleOffset;

            GameObject item = dropItems[i];
            item.transform.position = transform.position + new Vector3(0, 1f, 0);

            BaseItem baseItem = item.GetComponent<BaseItem>();
            if (baseItem != null)
            {
                baseItem.SetSpreadAngle(angleOffset);
                baseItem.SetDelay(1.5f);
            }
        }
    }

}
