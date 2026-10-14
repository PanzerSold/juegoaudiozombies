using System.Collections;
using UnityEngine;

public class Barricade : Interaction
{
    [Header("Arrastra las barricadas en orden de aparición:")]
    [SerializeField] private GameObject[] barricades;
    [SerializeField] private Collider ColliderPrincipal;
    [SerializeField] private float delayBarricada = 3f;

    public bool isOpen { get; private set; } = false;
    private bool cycle = false;
    private int barricadasRestantes = 5;
    public int zombiesClose = 0;

    protected override void Start()
    {
        base.Start();
        fullString = "Reparar Barricada (E)";
    }
    protected override void Update()
    {
        if(jugadorDentro)
        {
            if (barricadasRestantes < barricades.Length)
            {
                textoInteraccion.gameObject.SetActive(true);

                if (pi.interactAction.IsPressed())
                {
                    Interactuar();
                }
            }
            else
            {
                textoInteraccion.gameObject.SetActive(false);
            }
        }

        if (zombiesClose > 0)
        {
            SacarBarricada();
        }
    }
    protected override void Interactuar()
    {
        if (!cycle && barricadasRestantes < barricades.Length)
        {
            player.AwardPoints(10);

            StartCoroutine(BarricadeCorrutine());
        }
    }

    private void SacarBarricada()
    {
        if (!cycle && barricadasRestantes > 0)
        {
            
            StartCoroutine(ZombieCorrutine());
        }
    }

    IEnumerator BarricadeCorrutine()
    {
        cycle = true;
        player.PlayBarrierSound();
        for (int i = 0; i < barricades.Length; i++)
        {
            if (barricades[i] != null && !barricades[i].activeSelf)
            {
                barricades[i].SetActive(true);
                barricadasRestantes++;
                break; 
            }
        }

        ColliderPrincipal.enabled = true;
        isOpen = false;

        yield return new WaitForSeconds(delayBarricada);
        cycle = false;
    }

    IEnumerator ZombieCorrutine()
    {
        cycle = true;
        for (int i = barricades.Length - 1; i >= 0; i--)
        {
            if (barricades[i] != null && barricades[i].activeSelf)
            {
                barricades[i].SetActive(false);
                barricadasRestantes--;
                break; 
            }
        }
        if (barricadasRestantes <= 0 && ColliderPrincipal != null)
        {
            ColliderPrincipal.enabled = false;
            isOpen = true;
        }
        yield return new WaitForSeconds(delayBarricada/2f);


        cycle = false;
    }

    override public void PlayerEntered()
    {
        jugadorDentro = true;
        if (barricadasRestantes < barricades.Length)
        {
            textoInteraccion.text = fullString;
            textoInteraccion.gameObject.SetActive(true);
        }
    }
}