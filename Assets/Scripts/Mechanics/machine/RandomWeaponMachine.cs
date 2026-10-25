using UnityEngine;

public class RandomWeaponMachine: BaseMachine
{
    [SerializeField] private Inventory.itemIndex[] availableWeapons;

    protected override void Start()
    {
        base.Start();
        fullString = "Maquina Aleatoria " + PuntosRequeridos + " (E)";
    }

    protected override void Buy()
    {
        EntregarArmaAleatoria();
        Debug.Log("Te tocó el " + availableWeapons);
    }

    private void EntregarArmaAleatoria()
    {
        if (inventory == null)
        {
            inventory = player.GetComponent<Inventory>();
        }

        if (inventory == null)
        {
            GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
            if (jugadorObj != null) inventory = jugadorObj.GetComponent<Inventory>();
        }
        if (inventory != null && availableWeapons != null && availableWeapons.Length > 0)
        {
            Inventory.itemIndex chosenWeapon = availableWeapons[Random.Range(0, availableWeapons.Length)];

            inventory.AddItem(new Inventory.Item(
                Inventory.itemType.Weapon,
                chosenWeapon,
                amount: 1
            ));
            Debug.Log("Te toco el " + chosenWeapon);
        }
    }
}