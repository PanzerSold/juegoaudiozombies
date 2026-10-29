using UnityEngine;

public class WeaponMachine : BaseMachine
{
    [SerializeField] private Inventory.itemIndex givenWeapon;
    protected override void Start()
    {
        base.Start();
        fullString = "Buy gun " + PuntosRequeridos + " (E)";
    }
    protected override void Buy()
    {
        EntregarArma();
        StartCoroutine(VisualPurchase("Now you have: " + givenWeapon));
    }
    private void EntregarArma()
    {
        Inventory.Item nuevaArma = new Inventory.Item(Inventory.itemType.Weapon, givenWeapon, amount: 1);
        if (inventory != null)
        {
            inventory.AddItem(nuevaArma);
        }
        else if (inventory == null)
        {

        }
    }
}