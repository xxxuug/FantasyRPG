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

        Debug.Log("[InventorySlot] ���Կ� ������ �߰���: " + newItem.ItemName);
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
        // ���� ���Կ��� ���� Ŭ������ ����
        if (_lastClickedSlot == this && (Time.time - _lastClickTime) < _doubleClickInterval)
        {
            _clickCount++;
            if (_clickCount == 2)
            {
                Debug.Log($"[InventorySlot] {_itemData} ����Ŭ��");
                _lastClickedSlot = null;
                _lastClickTime = 0;

                // ���� itemData�� Tag�� Equipment���
                if (_itemData != null && _itemData.Prefab.CompareTag("Equipment"))
                {
                    // ��� ������ EquipmentType�� itemData�� EquipmentType�� ���ؼ� ������ �߰�
                    foreach (EquipmentSlot slot in UI_Inventory.Instance._equipmentSlots)
                    {
                        if (slot.SlotType == _itemData.EquipmentType)
                        {
                            // ��� ����
                            slot.SetItem(_itemData);
                            // ��� ���� ���� �ݿ�...
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
