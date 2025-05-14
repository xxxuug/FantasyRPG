using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour, IPointerDownHandler
{
    private ItemData _itemData;
    public Image ItemIcon;

    public void SetItem(ItemData item)
    {
        _itemData = item;
        ItemIcon.sprite = item.ItemIcon;
        ItemIcon.gameObject.SetActive(true);
    }

    private static ShopItemSlot _lastClickedSlot;
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

                UI_Shop.Instance.SetSelectedItem(_itemData);
                UI_Shop.Instance.OpenPurchasePopUp();
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
}
