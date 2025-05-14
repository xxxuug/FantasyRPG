using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : Singleton<UI_Inventory>
{
    [Header("Inventory Basic")]
    public GameObject InventoryPanel;
    public Button InventoryOpen;
    public Button InventoryClose;

    [Header("Inventory Slot")]
    public GameObject ItemSlotPrefabs;
    public Transform SlotParent;

    [Header("Max Slot Warning")]
    public TMP_Text WarningText;

    [Header("Inventory Player Info")]
    public TMP_Text NickName;
    public TMP_Text Level;
    public Slider Hp;
    public Slider Exp;
    public TMP_Text Gold;
    //private Animator _animator;

    [Header("Inventory Player Equipped Info")]
    public EquipmentSlot Helmet;
    public EquipmentSlot TopArmor;
    public EquipmentSlot BottomArmor;
    public EquipmentSlot Weapon;

    public InventoryPlayer InventoryPlayer;

    public List<EquipmentSlot> _equipmentSlots = new List<EquipmentSlot>();
    public List<InventorySlot> _inventorySlots = new List<InventorySlot>();
    public List<InventorySlot> GetInventorySlots()
    {
        return _inventorySlots;
    }

    private int _maxSlotCount = 20;
    private int _columns = 4;
    private float _slotSize = 5f;
    private float _xSpacing = 5f;
    private float _ySpacing = 10f;
    private Vector2 _initPos = new Vector2(10f, 29f);

    #region UI_Base
    private void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetCanvas();
        GameManager.Instance.OnPlayerInfoChanged += PlayerInfoinInventory;

        // equipmentSlot List 추가
        _equipmentSlots.Add(Helmet);
        _equipmentSlots.Add(TopArmor);
        _equipmentSlots.Add(BottomArmor);
        _equipmentSlots.Add(Weapon);
    }

    private void SetCanvas()
    {
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }
        CanvasScaler canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }
    }
    #endregion
    void PlayerInfoinInventory()
    {
        var playerInfo = GameManager.Instance.PlayerInfo;

        NickName.text = playerInfo.Name;
        Level.text = $"{playerInfo.Level}";
        Gold.text = $"{playerInfo.Gold}";

        Hp.maxValue = playerInfo.MaxHp;
        Hp.value = playerInfo.CurrentHp;

        Exp.minValue = 0;
        Exp.maxValue = playerInfo.MaxExp;
        Exp.value = playerInfo.CurrentExp;
    }

    private void Start()
    {
        InventoryPanel.SetActive(false);
        WarningText.gameObject.SetActive(false);

        InventoryOpen.onClick.AddListener(ToggleInventory);
        InventoryClose.onClick.AddListener(ToggleInventory);

        AddSlots();
        EquipShift();
    }

    void ToggleInventory()
    {
        InventoryPanel.SetActive(!InventoryPanel.activeSelf);
    }

    private void AddSlots()
    {
        for (int i = 0; i < _maxSlotCount; i++)
        {
            // 행과 열 계산
            int row = i / _columns;
            int col = i % _columns;

            // 슬롯 위치 계산
            float xPos = _initPos.x + col * (_slotSize + _xSpacing);
            float yPos = _initPos.y - row * (_slotSize + _ySpacing);

            // 슬롯 생성
            GameObject slot = Instantiate(ItemSlotPrefabs, SlotParent);
            RectTransform rect = slot.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(xPos, yPos);

            _inventorySlots.Add(slot.GetComponent<InventorySlot>());
        }
    }

    public void AddItemToInventory(ItemData item)
    {
        Debug.Log("[UI_Inventory] 아이템 추가 시도 : " + item.ItemName);
        foreach (InventorySlot slot in _inventorySlots)
        {
            if (slot.IsSameItem(item))
            {
                slot.IncreaseCount();
                Debug.Log("[U_Inventory] 기존 아이템 개수 증가 : " + item.ItemName);
                return;
            }
        }

        foreach (InventorySlot slot in _inventorySlots)
        {
            if (slot.IsEmpty())
            {
                slot.SetItem(item, 1);
                Debug.Log("[UI_Inventory] 빈 슬롯에 아이템 추가 : " + item.ItemName);
                return;
            }
        }
        Debug.Log("[UI_Inventory] 인벤토리가 가득 참!");
        StartCoroutine(ShowWarningMessage("인벤토리가 가득 찼습니다."));
    }

    IEnumerator ShowWarningMessage(string message)
    {
        WarningText.text = message;
        WarningText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        WarningText.gameObject.SetActive(false);
    }

    public void EquipShift()
    {
        // 1. UI Player의 자식 오브젝트가 존재한다면
        if (InventoryPlayer.transform.childCount > 0)
        {
            // 1-1. 자식 오브젝트 확인
            foreach (Transform child in InventoryPlayer.transform)
            {
                // 1-2. 자식 오브젝트 변수에 담기
                ItemData itemData = FindItemDataByPrefab(child.gameObject);
                if (itemData != null && IsEquipmentTypeValid(itemData.EquipmentType))
                {
                    foreach (EquipmentSlot slot in _equipmentSlots)
                    {
                        if (slot.SlotType == itemData.EquipmentType)
                        {
                            slot.SetItem(itemData);
                            break;
                        }
                    }
                }
            }
        }
    }

    ItemData FindItemDataByPrefab(GameObject prefab)
    {
        foreach (ItemData item in GameManager.Instance.AllItemData)
        {
            if (item.Prefab.name == prefab.name)
            {
                return item;
            }
        }
        return null;
    }

    bool IsEquipmentTypeValid(EquipmentType type)
    {
        return type == EquipmentType.Weapon || type == EquipmentType.Helmet ||
               type == EquipmentType.TopArmor || type == EquipmentType.BottomArmor;
    }
}
