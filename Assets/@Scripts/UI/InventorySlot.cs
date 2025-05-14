using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
{
    public Image ItemIcon;
    public TMP_Text ItemCount;

    private ItemData _itemData;
    private int _count = 0;

    private UI_Inventory _inventory;

    public void SetInventory(UI_Inventory inventory) => _inventory = inventory;

    public bool IsEmpty() => _itemData == null;
    public bool IsSameItem(ItemData newItem) => _itemData != null && _itemData.ItemName == newItem.ItemName;

    public void SetItem(ItemData newItem, int itemCount)
    {
        _itemData = newItem;
        _count = itemCount;
        
        ItemIcon.sprite = newItem.ItemIcon;
        ItemIcon.gameObject.SetActive(true);
        UpdateCount();

        Debug.Log("[InventorySlot] 슬롯에 아이템 추가됨: " + newItem.ItemName);
    }

    public ItemData GetItemData()
    {
        return _itemData;
    }

    public int GetItemCount()
    {
        return _count;
    }

    public void IncreaseCount()
    {
        _count++;
        UpdateCount();
    }

    void UpdateCount()
    {
        ItemCount.text = _count > 1 ? _count.ToString() : "";
    }

    public void ClearItem()
    {
        _itemData = null;
        _count = 0;
        ItemIcon.sprite = null;
        ItemIcon.gameObject.SetActive(false);
        
    }

    private static InventorySlot _lastClickedSlot;
    private static float _lastClickTime;
    private const float _doubleClickInterval = 0.3f;
    private int _clickCount = 0;
    public void OnPointerDown(PointerEventData eventData)
    {
        // 같은 슬롯에서 연속 클릭했을 때만
        if (_lastClickedSlot == this && (Time.time - _lastClickTime) < _doubleClickInterval)
        {
            _clickCount++;
            if (_clickCount == 2)
            {
                Debug.Log($"[InventorySlot] {_itemData} 더블클릭");
                _lastClickedSlot = null;
                _lastClickTime = 0;

                // 만약 itemData의 Tag가 Equipment라면
                if (_itemData != null && _itemData.Prefab.CompareTag("Equipment"))
                {
                    // 장비 슬롯의 EquipmentType과 itemData의 EquipmentType을 비교해서 같으면 추가
                    foreach (EquipmentSlot slot in UI_Inventory.Instance._equipmentSlots)
                    {
                        if (slot.SlotType == _itemData.EquipmentType)
                        {
                            // 장비 장착
                            slot.SetItem(_itemData);
                            // 장비 실제 장착 반영...
                            GameManager.Instance.Equip(_itemData);
                            ClearItem();
                            break;
                        }
                    }
                    
                }
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
