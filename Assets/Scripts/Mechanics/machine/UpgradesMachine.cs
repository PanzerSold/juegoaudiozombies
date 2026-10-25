using UnityEngine;

public class UpgradesMachine : BaseMachine
{
    [SerializeField] private Player.PlayerPerks givenPerks;
    protected override void Start()
    {
        base.Start();
        fullString = "Comprar Mejora " + PuntosRequeridos + " (E)";
    }
    protected override void Buy()
    {
        GivePerk();
        Debug.Log("Obtuviste " + givenPerks);
    }

    private void GivePerk()
    {

    }
} 
