using UnityEngine;

/*  public class UpgradesMachine : BaseMachine
{
    [SerializeField] private Inventory.itemIndex givenUpgrade;
    protected override void Start()
    {
        base.Start();
        fullString = "Comprar Mejora " + PuntosRequeridos + " (E)";
    }
    protected override void Buy()
    {
        EntregarMejora();
        Debug.Log("Obtuviste " + givenUpgrade);
    }
    private void EntregarMejora()
    {
        Inventory.Item nuevaMejora = new Inventory.Item(
            Inventory.itemType.Upgrade,
            givenUpgrade,
            amount: 1
        );
        if (inventory != null)
        {
            inventory.AddUpgrade(nuevaMejora);

        }
    }
} 
*/

// Filo ahi les puse los nombres para el metodo, fijate que tiene que ser "upgrade", nose si es un itemType el upgrade o aparte, ahi ni idea.