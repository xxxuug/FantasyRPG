using UnityEngine;

public class InventoryPlayer : BaseController
{
    //public GameObject Helmet;
    //public GameObject TopArmor;
    //public GameObject BottomArmor;
    public GameObject Weapon;

    protected override void Initialize()
    {

    }

    // 인벤토리 내 플레이어 캐릭터에 장비 장착 함수
    public void Equip(ItemData item)
    {
        foreach (Transform child in transform)
        {
            // 자식 이름과 아이템 이름이 맞는지 확인하고 맞으면 활성화(장착)
            if (child.name == item.name)
            {
                if (!child.gameObject.activeSelf)
                    child.gameObject.SetActive(true);
                else return;
            }
            else
            {
                GameObject EquipItem = Instantiate(item.Prefab, transform.position, Quaternion.identity);
                EquipItem.name = item.name;
            }

        }
    }

    // 장비 아이템 장착 해제 함수
    public void UnEquip(ItemData item)
    {
        foreach (Transform child in transform)
        {
            //Debug.Log($"[InventoryPlayer] 자식 찾기 성공 : {child.name} , item : {item.name}");
            if (child.name == item.name)
            {
                //Debug.Log($"[InventoryPlayer] {child.name} = {item.name}! 오브젝트 삭제");
                child.gameObject.SetActive(false);
                //Debug.Log("[InventoryPlayer] 오브젝트 삭제 성공");
                break;
            }

        }
    }
}
