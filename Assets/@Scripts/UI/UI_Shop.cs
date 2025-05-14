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
            Debug.Log("[UI_Shop] 상점에 등록된 아이템이 없습니다.");
            return;
        }

        for (int i = 0; i < ShopItemSlots.Length; i++)
        {
            if (i < ShopItems.Length)
            {
                ShopItemSlots[i].SetItem(ShopItems[i]);
                Debug.Log($"[UI_Shop] {ShopItems[i].ItemName} 설정 완료");
            }

        }
    }

    private void ShopItemInfo()
    {
        //Debug.Log($"[UI_Shop] ShopItemInfo() 호출됨, 현재 골드 : {GameManager.Instance.PlayerInfo.Gold}");
        MyGold.text = $"{GameManager.Instance.PlayerInfo.Gold}";
        Debug.Log($"[ShopItemInfo] MyGold UI 업데이트: {MyGold.text}");
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
                // 1. 아이템 슬롯 생성
                GameObject itemSlot = Instantiate(ItemSlotPrefab, InventoryItemParent);
                ShopInventorySlot shopSlot = itemSlot.GetComponent<ShopInventorySlot>();
                RectTransform rect = itemSlot.GetComponent<RectTransform>();

                // 2. 생성된 슬롯 위치 지정 (세로로 생성)
                rect.anchoredPosition = initPos;
                initPos.y += yOffset;

                // 3. InventorySlot 컴포넌트 가져오기
                InventorySlot inventorySlot = itemSlot.GetComponent<InventorySlot>();

                // 4. 아이콘/개수 아이템 정보 반영
                shopSlot.SetItem(slot.GetItemData(), slot.GetItemCount(), slot);

                // 5. 상점 UI용 인벤토리 슬롯 리스트에 추가
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

        // 인벤토리에 아이템이 들어가는 로직
        UI_Inventory.Instance.AddItemToInventory(_itemData);
        UpdateInventorySlot();

        Debug.Log($"[UI_Shop] {_itemData.ItemName} 구매 완료!");
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

        Debug.Log($"[UI_Shop] 판매할 아이템: {_itemData.ItemName}, 가격: {_itemData.Price}");

        SellYES.onClick.RemoveAllListeners();
        SellYES.onClick.AddListener(OnClickSellYES);
        SellNO.onClick.RemoveAllListeners();
        SellNO.onClick.AddListener(OnClickSellNO);
    }

    void OnClickSellYES()
    {
        SellPopUp.SetActive(false);
        // 인벤토리에서 빠져나가는 로직
        // 개수가 0개면 인벤토리에서 삭제하고 아이템 리스트 위로 정렬
        if (_selectedSellSlot == null)
        {
            Debug.LogError("[UI_Shop] _selectedSellSlot이 null입니다! (판매할 아이템이 선택되지 않았음)");
            return;
        }

        if (_itemData == null)
        {
            Debug.LogError("[UI_Shop] _itemData가 null입니다! (판매할 아이템 정보가 저장되지 않았음)");
            return;
        }

        //var playerInfo = GameManager.Instance.PlayerInfo;
        //float sellPrice = _itemData.Price;
        GameManager.Instance.GetGold(_itemData.Price);
        //Debug.Log($"[UI_Shop] {_itemData.ItemName} 판매 완료! +{_itemData.Price} 골드");

        ShopItemInfo();

        int newCount = _selectedSellSlot.GetItemCount() - 1;

        _selectedSellSlot.SetItem(_itemData, newCount);
        //Debug.Log($"[UI_Shop] {_itemData.ItemName} 개수 감소: {newCount}개 남음");

        if (newCount <= 0)
        {
            //Debug.Log($"[UI_Shop] {_itemData.ItemName} 개수 0, 슬롯 제거");
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
