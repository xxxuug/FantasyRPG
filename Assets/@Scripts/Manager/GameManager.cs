using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public string   Name; // 이름
    public float    Atk; // 공격력
    public float    Def; // 방어력
    public int      Level; // 레벨
    public float    CurrentExp; // 현재 경험치
    public float    MaxExp; // 최대 경험치
    public float    CurrentHp; // 현재 체력
    public float    MaxHp; // 최대 체력
    public float    Speed; // 속도
    public float    Gold; // 돈
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
        Name = "전사",
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
        Debug.Log("현재 경험치 : " +  PlayerInfo.CurrentExp);
        if (PlayerInfo.CurrentExp >= PlayerInfo.MaxExp)
        {
            _playerInfo.Level++;
            Debug.Log("레벨업! 현재 레벨 : " + PlayerInfo.Level);
            _playerInfo.CurrentExp = 0;
        }

        OnPlayerInfoChanged?.Invoke();
    }
    #endregion

    #region GetGold
    public void GetGold(float gold)
    {
        _playerInfo.Gold += gold;
        Debug.Log($"[골드 업데이트] 현재 골드: {_playerInfo.Gold}");
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
