using UnityEngine;
public enum EquipmentType
{
    None,
    Helmet,
    TopArmor,
    BottomArmor,
    Weapon
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public int Price;
    public int MaxCount;
    public int AttackPower;
    public int DefensePower;
    public GameObject Prefab;
    public EquipmentType EquipmentType;
    public int ID;
}

