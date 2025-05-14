using UnityEngine;

public class ItemSpawn : Singleton<ItemSpawn>
{
    public GameObject Item;
    public GameObject Equipment;

    public GameObject DropItem(Vector3 pos)
    {
        GameObject item = Instantiate(Item, pos, Quaternion.identity);
        return Item;
    }

    public GameObject DropEquip(Vector3 pos)
    {
        GameObject equip = Instantiate(Equipment, pos, Quaternion.identity);
        return Equipment;
    }

}
