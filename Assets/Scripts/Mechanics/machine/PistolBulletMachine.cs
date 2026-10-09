using UnityEngine;

public class PistolBulletMachine : BaseMachine
{
    protected override void Start()
    {
        base.Start();
        fullString = "Comprar Balas " + PuntosRequeridos + " (E)";
    }
        protected override void Buy()
    {
        player.GetComponent<PlayerAmmo>().GainAmmo(20);
    }
}
