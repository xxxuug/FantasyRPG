using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData ItemData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("[ItemPickUp] �÷��̾ �������� ���� : " + ItemData.ItemName);
            UI_Inventory inventory = FindAnyObjectByType<UI_Inventory>();
            if (inventory != null)
            {
                inventory.AddItemToInventory(ItemData);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("[ItemPickUp] �κ��丮�� ã�� �� ����!");
            }
        }
    }
}
