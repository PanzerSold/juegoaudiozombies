using UnityEngine;
//
public class Inventory : MonoBehaviour
{
    public enum itemType
    {
        Empty,
        Weapon,
        Ammo,
        Consumable
    }
    public enum itemIndex
    {
        None,
        M1911,
        M93R,
        M4,
        AK47,
        AWP,
        MAC10,
        MP5,
        RPG,
        Remington,
        Balas_Pistola,
        Balas_Rifle,
        Balas_Subfusil,
        Balas_Escopeta,
        Balas_Lanzacohete,
        Granada,
        Espray_curativo
    }
    //
    public struct LimitesInventario
    {
        public int Balas_Pistola;
        public int Balas_Rifle;
        public int Balas_Subfusil;
        public int Balas_Escopeta;
        public int Balas_Lanzacohete;
        public int Granada;
        public int Espray_curativo;

        // Instancia con valores por defecto
        public static LimitesInventario Defecto => new LimitesInventario
        {
            Balas_Pistola = 48,
            Balas_Rifle = 90,
            Balas_Subfusil = 120,
            Balas_Escopeta = 24,
            Granada = 5,
            Espray_curativo = 3
        };
    }

    public struct Item
    {
        public itemType item;
        public itemIndex index;
        public bool isStackable;
        public int amount;
        public static Item empty => new Item(itemType.Empty, itemIndex.None);
        public static Item M1911 => new Item(itemType.Weapon, itemIndex.M1911, true);
        public static Item startingAmmo => new Item(itemType.Ammo, itemIndex.Balas_Pistola, false, LimitesInventario.Defecto.Balas_Pistola / 2);
        public Item(itemType item, itemIndex index, bool isWeapon = false, int amount = 1)
        {
            this.item = item;
            this.index = index;
            this.isStackable = !isWeapon;
            this.amount = amount;
        }
    }
    private Item[] actualInventory;
    



    public void AddItem(Item newItem)
    {
        if (newItem.isStackable)
        {
            for (int i = 0; i < actualInventory.Length; i++)
            {
                if (actualInventory[i].item == newItem.item && actualInventory[i].index == newItem.index)
                {
                    Item existingItem = actualInventory[i];
                    existingItem.amount += newItem.amount;
                    actualInventory[i] = existingItem;
                }
            }
        }
        for (int i = 0; i < actualInventory.Length; i++)
        {
            if (actualInventory[i].item == itemType.Empty)
            {
                actualInventory[i] = newItem;
            }
        }
    }


    public void ResetInventory()
    {
        //equippedSlot = 0;
        actualInventory = new Item[9];
        actualInventory[0] = Item.M1911;
        actualInventory[1] = Item.startingAmmo;
        for (int i = 2; i < 9; i++)
        {
            actualInventory[i] = Item.empty;
        }
    }

    private void Awake()
    {
        ResetInventory();
    }
}