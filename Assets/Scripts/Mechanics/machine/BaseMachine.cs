using UnityEngine;

public abstract class BaseMachine : Interaction
{
    protected Inventory inventory;
    protected AudioSource SFX_FaltaPlata;
    protected override int PuntosRequeridos { get => base.PuntosRequeridos; set => base.PuntosRequeridos = value; }

    protected abstract void Buy();
    protected override void Start()
    {
        base.Start();
        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
        }
        //SFX_FaltaPlata = GetComponent<AudioSource>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Interactuar()
    {
        if (player.points >= PuntosRequeridos)
        {
            Buy();
            player.RemovePoints(PuntosRequeridos);
        }
        else
        {
            //SFX_FaltaPlata.Play();
        }
    }


}
