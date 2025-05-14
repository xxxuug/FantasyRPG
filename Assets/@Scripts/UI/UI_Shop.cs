using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : Singleton<UI_Shop>
{
    [Header("SHOP UI")]
    public GameObject ShopPanel;
    public Button ShopCloseButton;
    public TMP_Text ShopTitle;

    [Header("SHOP ITEM")]
    //public Transform ShopItemParent;
    public Transform InventoryItemParent;
    public GameObject ItemSlotPrefab;
    public ItemData[] ShopItems;
    public ShopItemSlot[] ShopItemSlots;

    [Header("MY ITEM")]
    public TMP_Text MyGold;

    [Header("Purchase Pop-Up")]
    public GameObject PurchasePopUp;
    public Button PurchaseYES;
    public Button PurchaseNO;
    private ItemData _itemData;

    [Header("Sell Pop-Up")]
    public GameObject SellPopUp;
    public Button SellYES;
    public Button SellNO;
    private InventorySlot _selectedSellSlot;

    [Header("No Money Pop-Up")]
    public GameObject NoMoneyPopUp;
    public Button NoMoneyOK;

    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();

    #region UI_Base
    private void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        SetCanvas();
        ShopPanel.SetActive(false);
        PurchasePopUp.SetActive(false);
        SellPopUp.SetActive(false);
        NoMoneyPopUp.SetActive(false);
        ShopCloseButton.onClick.AddListener(CloseShop);
        InitializeShopItems();

        GameManager.Instance.OnPlayerInfoChanged += ShopItemInfo;
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

    public void InitializeShopItems()
    {
        if (ShopItems == null || ShopItems.Length == 0)
        {
            Debug.Log("[UI_Shop] ������ ��ϵ� �������� �����ϴ�.");
            return;
        }

        for (int i = 0; i < ShopItemSlots.Length; i++)
        {
            if (i < ShopItems.Length)
            {
                ShopItemSlots[i].SetItem(ShopItems[i]);
                Debug.Log($"[UI_Shop] {ShopItems[i].ItemName} ���� �Ϸ�");
            }

        }
    }

    private void ShopItemInfo()
    {
        //Debug.Log($"[UI_Shop] ShopItemInfo() ȣ���, ���� ��� : {GameManager.Instance.PlayerInfo.Gold}");
        MyGold.text = $"{GameManager.Instance.PlayerInfo.Gold}";
        Debug.Log($"[ShopItemInfo] MyGold UI ������Ʈ: {MyGold.text}");
    }

    private void CloseShop()
    {
        ShopPanel.SetActive(false);

        foreach (var slot in InventoryItemParent.GetComponentsInChildren<ShopInventorySlot>())
        {
            slot.ClearItem();
        }

        foreach (Transform child in InventoryItemParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void OpenShop()
    {
        ShopPanel.SetActive(true);
        UpdateInventorySlot();
    }

    public void UpdateInventorySlot()
    {
        
        float yOffset = -20f;
        Vector2 initPos = new Vector2(-51, 10);

        foreach (var slot in UI_Inventory.Instance.GetInventorySlots())
        {

            if (!slot.IsEmpty())
            {
                // 1. ������ ���� ����
                GameObject itemSlot = Instantiate(ItemSlotPrefab, InventoryItemParent);
                ShopInventorySlot shopSlot = itemSlot.GetComponent<ShopInventorySlot>();
                RectTransform rect = itemSlot.GetComponent<RectTransform>();

                // 2. ������ ���� ��ġ ���� (���η� ����)
                rect.anchoredPosition = initPos;
                initPos.y += yOffset;

                // 3. InventorySlot ������Ʈ ��������
                InventorySlot inventorySlot = itemSlot.GetComponent<InventorySlot>();

                // 4. ������/���� ������ ���� �ݿ�
                shopSlot.SetItem(slot.GetItemData(), slot.GetItemCount(), slot);

                // 5. ���� UI�� �κ��丮 ���� ����Ʈ�� �߰�
                //_inventorySlots.Add(inventorySlot);
            }
        }
    }

    #region Purchase Pop-Up
    public void SetSelectedItem(ItemData item)
    {
        _itemData = item;
    }

    public void OpenPurchasePopUp()
    {
        PurchasePopUp.SetActive(true);

        PurchaseYES.onClick.RemoveAllListeners();
        PurchaseYES.onClick.AddListener(OnClickPurchaseYES);
        PurchaseNO.onClick.RemoveAllListeners();
        PurchaseNO.onClick.AddListener(OnClickPurchaseNO);
    }

    void OnClickPurchaseYES()
    {
        PurchasePopUp.SetActive(false);

        var playerInfo = GameManager.Instance.PlayerInfo;

        if (playerInfo.Gold < _itemData.Price)
        {
            OpenNoMoneyPopUp();
            return;
        }

        GameManager.Instance.GetGold(-_itemData.Price);
        ShopItemInfo();

        // �κ��丮�� �������� ���� ����
        UI_Inventory.Instance.AddItemToInventory(_itemData);
        UpdateInventorySlot();

        Debug.Log($"[UI_Shop] {_itemData.ItemName} ���� �Ϸ�!");
    }

    void OnClickPurchaseNO()
    {
        PurchasePopUp.SetActive(false);
    }
    #endregion

    #region Sell Pop-Up
    public void OpenSellPopUp(ShopInventorySlot shopSlot, InventorySlot slot)
    {
        _selectedSellSlot = slot;
        SellPopUp.SetActive(true);

        _itemData = slot.GetItemData();

        Debug.Log($"[UI_Shop] �Ǹ��� ������: {_itemData.ItemName}, ����: {_itemData.Price}");

        SellYES.onClick.RemoveAllListeners();
        SellYES.onClick.AddListener(OnClickSellYES);
        SellNO.onClick.RemoveAllListeners();
        SellNO.onClick.AddListener(OnClickSellNO);
    }

    void OnClickSellYES()
    {
        SellPopUp.SetActive(false);
        // �κ��丮���� ���������� ����
        // ������ 0���� �κ��丮���� �����ϰ� ������ ����Ʈ ���� ����
        if (_selectedSellSlot == null)
        {
            Debug.LogError("[UI_Shop] _selectedSellSlot�� null�Դϴ�! (�Ǹ��� �������� ���õ��� �ʾ���)");
            return;
        }

        if (_itemData == null)
        {
            Debug.LogError("[UI_Shop] _itemData�� null�Դϴ�! (�Ǹ��� ������ ������ ������� �ʾ���)");
            return;
        }

        //var playerInfo = GameManager.Instance.PlayerInfo;
        //float sellPrice = _itemData.Price;
        GameManager.Instance.GetGold(_itemData.Price);
        //Debug.Log($"[UI_Shop] {_itemData.ItemName} �Ǹ� �Ϸ�! +{_itemData.Price} ���");

        ShopItemInfo();

        int newCount = _selectedSellSlot.GetItemCount() - 1;

        _selectedSellSlot.SetItem(_itemData, newCount);
        //Debug.Log($"[UI_Shop] {_itemData.ItemName} ���� ����: {newCount}�� ����");

        if (newCount <= 0)
        {
            //Debug.Log($"[UI_Shop] {_itemData.ItemName} ���� 0, ���� ����");
            _selectedSellSlot.ClearItem();
        }
        UpdateInventorySlot();
    }

    void OnClickSellNO()
    {
        SellPopUp.SetActive(false);
    }
    #endregion

    #region No Money Pop-Up

    public void OpenNoMoneyPopUp()
    {
        NoMoneyPopUp.SetActive(true);

        NoMoneyOK.onClick.RemoveAllListeners();
        NoMoneyOK.onClick.AddListener(() => NoMoneyPopUp.SetActive(false));
    }
    #endregion
}
