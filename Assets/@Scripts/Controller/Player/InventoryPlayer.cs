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

    // �κ��丮 �� �÷��̾� ĳ���Ϳ� ��� ���� �Լ�
    public void Equip(ItemData item)
    {
        foreach (Transform child in transform)
        {
            // �ڽ� �̸��� ������ �̸��� �´��� Ȯ���ϰ� ������ Ȱ��ȭ(����)
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

    // ��� ������ ���� ���� �Լ�
    public void UnEquip(ItemData item)
    {
        foreach (Transform child in transform)
        {
            //Debug.Log($"[InventoryPlayer] �ڽ� ã�� ���� : {child.name} , item : {item.name}");
            if (child.name == item.name)
            {
                //Debug.Log($"[InventoryPlayer] {child.name} = {item.name}! ������Ʈ ����");
                child.gameObject.SetActive(false);
                //Debug.Log("[InventoryPlayer] ������Ʈ ���� ����");
                break;
            }

        }
    }
}
