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
        player.GetComponent<PlayerShoot>().Gun.GainAmmo(20);
    }
}
