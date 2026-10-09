using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Player;

public abstract class Interaction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] protected int puntosRequeridos;
    [SerializeField] protected bool usoUnico = false;
    protected PlayerInputs pi;
    protected bool jugadorDentro = false;
    protected Player player;
    [SerializeField] protected TextMeshProUGUI textoInteraccion;
    protected String fullString = "";


    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        pi = player.gameObject.GetComponent<PlayerInputs>();
    }

    protected virtual void Update()
    {
        if (jugadorDentro)
        {
            if (pi.interactAction.WasPressedThisFrame()) Interactuar();
        }
    }

    protected abstract void Interactuar();

    protected virtual int PuntosRequeridos
    {
        get
        {
            if (player.CurrentPowerUp == PlayerBonus.Discount) return puntosRequeridos / 2;
            return puntosRequeridos;
        }

        set
        {
            puntosRequeridos = value;
        }
    }
    public virtual void PlayerEntered()
    {
        jugadorDentro = true;
        textoInteraccion.text = fullString;
        textoInteraccion.gameObject.SetActive(true);
    }
    public virtual void PlayerExit()
    {
        jugadorDentro = false;
        textoInteraccion.gameObject.SetActive(false);
    }
}
