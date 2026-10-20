public class FireTrap : Interaction
{
    protected override int PuntosRequeridos { get => base.PuntosRequeridos; set => base.PuntosRequeridos = value; }
    protected override void Start()
    {
        base.Start();
        fullString = "Activar Trampa (" + PuntosRequeridos + ") (E)";
    }

    protected override void Interactuar()
    {
        ;
    }
}
