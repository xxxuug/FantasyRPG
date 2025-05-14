using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerDownHandler
{
    public Image EquipIcon;
    private ItemData _equipmentData;
    public EquipmentType SlotType;

    // ����Ŭ��
    private static EquipmentSlot _lastClickedSlot;
    private static float _lastClickTime;
    private const float _doubleClickInterval = 0.3f;
    private int _clickCount = 0;

    public void SetItem(ItemData equip)
    {
        _equipmentData = equip;
        EquipIcon.sprite = equip.ItemIcon;
        EquipIcon.enabled = true;
    }

    public void ClearItem()
    {
        _equipmentData = null;
        EquipIcon.sprite = null;
        EquipIcon.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // ���� ���Կ��� ���� Ŭ������ ����
        if (_lastClickedSlot == this && (Time.time - _lastClickTime) < _doubleClickInterval)
        {
            _clickCount++;
            if (_clickCount == 2)
            {
                Debug.Log($"[EquipmentSlot] {_equipmentData} ����Ŭ��");
                _lastClickedSlot = null;
                _lastClickTime = 0;

                // �κ��丮���� ItemData�� ������ �ִ� ��� ���� Ŭ���ϸ�
                if (_equipmentData != null)
                {
                    UI_Inventory.Instance.AddItemToInventory( _equipmentData );
                    GameManager.Instance.UnEquip(_equipmentData);
                    ClearItem();
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
