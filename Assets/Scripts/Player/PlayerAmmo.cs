using TMPro;
using UnityEngine;
using static Player;

public class PlayerAmmo : MonoBehaviour
{
    [Header("Disparo")]

    [SerializeField] private float raycastDistance = 100f;

    [SerializeField] private Camera cameraPos;

    [SerializeField] private float gunDamage = 1f;

    [Header("Municion")]
    [SerializeField] private int ammoMax = 10;
    [SerializeField] private int ammoQuantity = 50;
    [SerializeField] private TextMeshProUGUI ammoText;
    private PlayerInputs pi;
    [SerializeField] private LayerMask coll;
    [SerializeField] private ParticleSystem ps;
    private ParticleSystemRenderer psR;

    [Header("Materiales")]
    [SerializeField] private Material materialGenerico;
    [SerializeField] private Material materialZombieHit;

    [Header("Sonido")]
    [SerializeField] private AudioSource ShootSfx;
    [SerializeField] private AudioSource EmptyMagSfx;
    [SerializeField] private AudioSource ReloadSfx;

    private int ammoUsed;
    private int ammoCurrent;
    private string ammoTextStart;
    private void Awake()
    {
        ammoCurrent = ammoMax;
        ammoTextStart = ammoText.text;
        ammoText.text = ammoText.text + " " + ammoMax + " / " + ammoQuantity;
    }
    private void Start()
    {
        pi = gameObject.GetComponent<PlayerInputs>();
        psR = ps.GetComponent<ParticleSystemRenderer>();
    }


    private void Update()
    {
        if (pi.shootAction.WasPressedThisFrame())
        {
            if(ammoCurrent > 0)
            {
                Shoot();
            }
            else EmptyMagSfx.Play();
            fireAmmo();
        }
        if(ammoCurrent != ammoMax)
        {
            
            if (pi.reloadAction.WasPressedThisFrame())
            {
                rechargeAmmo();
                ReloadSfx.Play();
            }
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        Vector3 origin = cameraPos.transform.position;
        Vector3 direction = cameraPos.transform.forward;
        psR.material = materialGenerico;
        ShootSfx.Play();
        if (Physics.Raycast(origin, direction, out hit, raycastDistance, coll))
        {
            ps.transform.position = hit.point;
            ps.Play();
            if (hit.collider.CompareTag("ZombieBody"))
            {
                
                ZombieBase zombie = hit.collider.GetComponent<ZombieBase>();
                zombie.ReceiveDamage(false, gunDamage);
                psR.material = materialZombieHit;
            }

            if(hit.collider.CompareTag("ZombieHead"))
            {

                ZombieBase zombie = hit.collider.GetComponentInParent<ZombieBase>();

                zombie.ReceiveDamage(true, gunDamage);
                psR.material = materialZombieHit;

            }

        }

    }


    public void fireAmmo()
    {
        ammoCurrent--;
        if (ammoCurrent < 0)
        {
            ammoCurrent = 0;
        }
        ammoText.text = ammoTextStart + " " + ammoCurrent + " / " + ammoQuantity;
    }
    public void rechargeAmmo()
    {
        ammoUsed = ammoMax - ammoCurrent;
        if (ammoQuantity < ammoUsed)
        {
            ammoUsed = ammoQuantity;
        }
        ammoCurrent += ammoUsed;

        ammoQuantity -= ammoUsed;
        if (ammoQuantity < 0)
        {
            ammoQuantity = 0;
        }
        ammoText.text = ammoTextStart + " " + ammoCurrent + " / " + ammoQuantity;
    }

    public void GainAmmo(int gainedAmmo)
    {
        ammoQuantity += gainedAmmo;
        ammoText.text = ammoTextStart + " " + ammoCurrent + " / " + ammoQuantity;
    }
}
