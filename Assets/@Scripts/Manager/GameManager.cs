using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public string   Name; // �̸�
    public float    Atk; // ���ݷ�
    public float    Def; // ����
    public int      Level; // ����
    public float    CurrentExp; // ���� ����ġ
    public float    MaxExp; // �ִ� ����ġ
    public float    CurrentHp; // ���� ü��
    public float    MaxHp; // �ִ� ü��
    public float    Speed; // �ӵ�
    public float    Gold; // ��
}

public class GameManager : Singleton<GameManager>
{
    #region JoyStick
    public event Action<Vector2> OnMoveDirChanged;

    Vector2 _moveDir;

    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set
        {
            _moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }
    #endregion

    #region PlayerInfo
    public event Action OnPlayerInfoChanged;

    private PlayerInfo _playerInfo = new PlayerInfo()
    {
        Name = "����",
        Atk = 1,
        Def = 0,
        Level = 1,
        CurrentExp = 0,
        MaxExp = 100,
        CurrentHp = 100,
        MaxHp = 100,
        Speed = 3,
        Gold = 0,
    };

    public PlayerInfo PlayerInfo
    {
        get { return _playerInfo; }
        set
        {
            _playerInfo = value;
            OnPlayerInfoChanged?.Invoke();
        }
    }
    #endregion

    #region LevelUp
    public void LevelUp(float exp)
    {
        _playerInfo.CurrentExp += exp;
        _playerInfo.CurrentExp = Mathf.Clamp(_playerInfo.CurrentExp, 0, _playerInfo.MaxExp);
        Debug.Log("���� ����ġ : " +  PlayerInfo.CurrentExp);
        if (PlayerInfo.CurrentExp >= PlayerInfo.MaxExp)
        {
            _playerInfo.Level++;
            Debug.Log("������! ���� ���� : " + PlayerInfo.Level);
            _playerInfo.CurrentExp = 0;
        }

        OnPlayerInfoChanged?.Invoke();
    }
    #endregion

    #region GetGold
    public void GetGold(float gold)
    {
        _playerInfo.Gold += gold;
        Debug.Log($"[��� ������Ʈ] ���� ���: {_playerInfo.Gold}");
        OnPlayerInfoChanged?.Invoke();
    }
    #endregion

    #region TakeDamage
    public void TakeDamage(float damage)
    {
        _playerInfo.CurrentHp -= damage;
        OnPlayerInfoChanged?.Invoke();
    }
    #endregion

    #region StopGame
    public void StopGame()
    {
        Time.timeScale = 0;
    }
    #endregion

    #region ItemManager
    public List<ItemData> AllItemData { get; private set; } = new List<ItemData>();

    private void Start()
    {
        LoadAllItemData();
    }

    private void LoadAllItemData()
    {
        AllItemData = new List<ItemData>(Resources.LoadAll<ItemData>("@Prefabs/Item"));
    }
    #endregion

    #region Equip / UnEquip
    public void Equip(ItemData item)
    {
        if (ObjectManager.Instance.Player != null)
        {
            ObjectManager.Instance.Player.Equip(item);
        }

        if (UI_Inventory.Instance.InventoryPlayer != null)
        {
            UI_Inventory.Instance.InventoryPlayer.Equip(item);
        }
    }

    public void UnEquip(ItemData item)
    {
        if (ObjectManager.Instance.Player != null)
        {
            ObjectManager.Instance.Player.UnEquip(item);
        }

        if (UI_Inventory.Instance.InventoryPlayer != null)
        {
            UI_Inventory.Instance.InventoryPlayer.UnEquip(item);
        }
    }
    #endregion
}
