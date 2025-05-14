using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData ItemData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("[ItemPickUp] 플레이어가 아이템을 먹음 : " + ItemData.ItemName);
            UI_Inventory inventory = FindAnyObjectByType<UI_Inventory>();
            if (inventory != null)
            {
                inventory.AddItemToInventory(ItemData);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("[ItemPickUp] 인벤토리를 찾을 수 없음!");
            }
        }
    }
}
