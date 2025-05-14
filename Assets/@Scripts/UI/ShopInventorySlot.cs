using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopInventorySlot : MonoBehaviour, IPointerDownHandler
{
    public Image ItemIcon;
    public TMP_Text ItemCount;
    public TMP_Text ItemName;
    public TMP_Text ItemPrice;

    private ItemData _itemData;
    private int _count = 0;
    private InventorySlot _linkedInventorySlot;

    public void SetItem(ItemData newItem, int itemCount, InventorySlot linkedSlot)
    {
        _itemData = newItem;
        _count = itemCount;
        _linkedInventorySlot = linkedSlot;

        if (newItem == null || newItem.ItemIcon == null)
        {
            Debug.LogError("[InventorySlot] ������ ������ �Ǵ� �������� null�Դϴ�!");
            return;
        }

        ItemIcon.sprite = newItem.ItemIcon;
        ItemName.text = newItem.ItemName;
        ItemPrice.text = $"{newItem.Price}";
        ItemIcon.gameObject.SetActive(true);
        UpdateCount();

    }

    void UpdateCount()
    {
        ItemCount.text = _count > 1 ? _count.ToString() : "";
    }

    private static ShopInventorySlot _lastClickedSlot;
    private static float _lastClickTime;
    private const float _doubleClickInterval = 0.3f;
    private int _clickCount = 0;
    public void OnPointerDown(PointerEventData eventData)
    {
        // ���� ���Կ��� ���� Ŭ������ ����
        if (_lastClickedSlot == this && (Time.time - _lastClickTime) < _doubleClickInterval)
        {
            _clickCount++;
            if (_clickCount == 2)
            {
                Debug.Log($"[ShopInventorySlot] {_itemData} ����Ŭ��");
                _lastClickedSlot = null;
                _lastClickTime = 0;

                UI_Shop.Instance.OpenSellPopUp(this, _linkedInventorySlot);
                return;
            }
        }
        else
        {
            _clickCount = 1;
        }
        _lastClickedSlot = this;
        _lastClickTime = Time.time;
    }

    public void ClearItem()
    {
        _itemData = null;
        ItemIcon.sprite = null;
        ItemName.text = "";
        ItemCount.text = "";
        ItemPrice.text = "";
        ItemIcon.gameObject.SetActive(false);
    }
}
