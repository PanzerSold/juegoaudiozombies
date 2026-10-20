public class Door : Interaction
{
    protected override int PuntosRequeridos { get => base.PuntosRequeridos; set => base.PuntosRequeridos = value; }
    protected override void Start()
    {
        base.Start();
        fullString = "Abrir Puerta (" + PuntosRequeridos + ") (E)";
    }

    protected override void Interactuar()
    {
        if (player.points >= PuntosRequeridos)
        {
            player.RemovePoints(PuntosRequeridos);
            Destroy(gameObject);
            textoInteraccion.gameObject.SetActive(false);
        }
        else
        {
            //SFX_FaltaPlata.Play();
        }
    }

}