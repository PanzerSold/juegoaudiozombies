using System.Collections.Generic;
using UnityEngine;

public class RandomWeaponMachine : BaseMachine
{
    [SerializeField] private Inventory.itemIndex[] availableWeapons;

    protected override void Start()
    {
        base.Start();
        fullString = "Randon weapon machine " + PuntosRequeridos + " (E)";
    }

    protected override void Buy()
    {
        GiveRandomWeapon();
    }

    private void GiveRandomWeapon()
    {
        // Dar arma

        if (inventory == null)
        {
            inventory = player.GetComponent<Inventory>();

            GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
            if (jugadorObj != null)
                inventory = jugadorObj.GetComponent<Inventory>();
        }

        List<Inventory.itemIndex> missingWeapons = new List<Inventory.itemIndex>();

        foreach (Inventory.itemIndex weapon in availableWeapons)
        {
            bool alreadyOwned = false;

            for (int i = 0; i < 9 && alreadyOwned == false; i++)
            {
                Inventory.Item slot = inventory.GetSlot(i);

                if (slot.item == Inventory.itemType.Weapon && slot.index == weapon)
                {
                    alreadyOwned = true;
                }
            }

            if (alreadyOwned == false)
            {
                missingWeapons.Add(weapon);
            }
        }

        if (missingWeapons.Count == 0)
        {
            player.AwardPoints(PuntosRequeridos);
            StartCoroutine(VisualPurchase("Tenes todas las armas de la caja!"));
            return;
        }

        Inventory.itemIndex chosenWeapon =
            missingWeapons[Random.Range(0, missingWeapons.Count)];

        bool added = inventory.AddItem(
            new Inventory.Item(
                Inventory.itemType.Weapon,
                chosenWeapon,
                isWeapon: true,
                amount: 1
            )
        );

        // Dar balas

        Inventory.itemIndex ammo = Inventory.GetAmmoFor(chosenWeapon);
        int ammoAmount = GetAmmoAmount(chosenWeapon);

        if (ammo != Inventory.itemIndex.None && ammoAmount > 0)
        {
            inventory.AddItem(
                new Inventory.Item(
                    Inventory.itemType.Ammo,
                    ammo,
                    amount: ammoAmount
                )
            );
        }
        

        StartCoroutine(VisualPurchase("te tocó el: " + chosenWeapon));
    }

    // una verguenza esto
    private int GetAmmoAmount(Inventory.itemIndex weapon)
    {
        switch (weapon)
        {
            case Inventory.itemIndex.MAC10:
                return 120;

            case Inventory.itemIndex.AK47:
                return 90;

            case Inventory.itemIndex.AWP:
                return 10;

            case Inventory.itemIndex.M4:
                return 90;

            case Inventory.itemIndex.RPG:
                return 3;

            default:
                return 0;
        }
    }

}

