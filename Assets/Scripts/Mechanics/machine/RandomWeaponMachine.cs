using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using System.Linq;

public class RandomWeaponMachine : BaseMachine
{
    private static Inventory.itemIndex[] armasPosibles = {
        Inventory.itemIndex.M1911,
        Inventory.itemIndex.M93R,
        Inventory.itemIndex.MP5,
        Inventory.itemIndex.UZI,
        Inventory.itemIndex.RPG,
        Inventory.itemIndex.Remington,
        Inventory.itemIndex.AK47,
        Inventory.itemIndex.M4,
        Inventory.itemIndex.AWP,
        };
    protected override void Start()
    {
        base.Start();
        fullString = "Random Gun Machine (" + PuntosRequeridos + ") (E)";
    }

    protected override void Buy()
    {
        GiveRandomWeapon();
    }

    private void GiveRandomWeapon()
    {
        // Dar arma


        if (inventory.CheckFull())
        {
            player.AwardPoints(PuntosRequeridos);
            StartCoroutine(VisualPurchase("Your Inventory is Full"));
            return;
        }
        
        Inventory.itemIndex[] weaponPool = armasPosibles;
        Inventory.itemIndex[] currentWeapons = inventory.getAllWeapons(); 
        Inventory.itemIndex[] result = weaponPool.Except(currentWeapons).ToArray();
        Inventory.itemIndex chosenWeapon = result[Random.Range(0,result.Length-1)];

        inventory.AddItem(
            new Inventory.Item(
                Inventory.itemType.Weapon,
                chosenWeapon,
                isWeapon: true,
                amount: 1
            )
        );

        SFX_Comprar.Play();
        StartCoroutine(VisualPurchase("You got: " + chosenWeapon));

        // si ni te puede dar las balas porque tenes el inventario lleno, sale

        if (inventory.CheckFull()) return;

        Inventory.itemIndex ammo = Inventory.GetAmmoFor(chosenWeapon);
        int ammoAmount = Inventory.GetLimit(ammo)/2;
        inventory.AddItem(
            new Inventory.Item(
                Inventory.itemType.Ammo,
                ammo,
                amount: ammoAmount
            )
        );
        
    }

    // una verguenza esto
    // confirmo, tenes un switch en el propio inventory :P
    // confirmo que lo mio tambien puede ser mucho mejor


}

