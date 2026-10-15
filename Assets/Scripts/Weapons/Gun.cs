using System.Collections;
using TMPro;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] protected float raycastDistance = 100f;
    [SerializeField] protected float gunDamage = 1f;
    [SerializeField] protected Camera cameraPos;
    [SerializeField] protected LayerMask coll;
    [SerializeField] protected ParticleSystem ps;

    [SerializeField] private float shootTime = 0.5f;
    protected ParticleSystemRenderer psR;

    [Header("Municion")]
    [SerializeField] private int ammoMax = 10;
    [SerializeField] private int ammoQuantity = 50;

    [SerializeField] private float delayRecharge = 3f;

    [Header("Materiales")]
    [SerializeField] protected Material materialGenerico;
    [SerializeField] protected Material materialZombieHit;

    [Header("Sonido")]
    [SerializeField] protected AudioSource ShootSfx;
    [SerializeField] private AudioSource emptyMagSfx;
    [SerializeField] private AudioSource ReloadSfx;

    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private string rechargeText = "Reloading...";

    [Header("Automatica")]

    [SerializeField] private bool isAutomatic = false;

    public bool IsAutomatic => isAutomatic;
    public AudioSource EmptyMagSfx => emptyMagSfx;

    public int AmmoCurrent => ammoCurrent;

    public float ShootTime => shootTime;
    public int AmmoMax => ammoMax;

    private int ammoUsed;
    private int ammoCurrent;

    private bool isRecharging;

    public bool IsRecharging => isRecharging;

    public TextMeshProUGUI AmmoText => ammoText;

    public int AmmoQuantity => ammoQuantity;
    private Coroutine rechargeCoroutine;

    private void Awake()
    {
        ammoCurrent = ammoMax;
    }

    private void Start()
    {
        psR = ps.GetComponent<ParticleSystemRenderer>();
    }

    virtual public void Shoot()
    {
        RaycastHit hit;
        Vector3 origin = cameraPos.transform.position;
        Vector3 direction = cameraPos.transform.forward;
        psR.material = materialGenerico;
        ShootSfx.Play();
        FireAmmo();
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

            if (hit.collider.CompareTag("ZombieHead"))
            {

                ZombieBase zombie = hit.collider.GetComponentInParent<ZombieBase>();

                zombie.ReceiveDamage(true, gunDamage);
                psR.material = materialZombieHit;

            }

        }

    }


    public void FireAmmo()
    {
        ammoCurrent--;
        if (ammoCurrent < 0)
        {
            ammoCurrent = 0;
        }
        ammoText.text = ammoCurrent + " / " + ammoQuantity;
    }
    public void RechargeAmmo()
    {
        if(rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
        }
        rechargeCoroutine = StartCoroutine(Recharge());
        
    }

    public void GainAmmo(int gainedAmmo)
    {
        ammoQuantity += gainedAmmo;
        ammoText.text = ammoCurrent + " / " + ammoQuantity;
    }

    IEnumerator Recharge()
    {
        isRecharging = true;
        ReloadSfx.Play();
        ammoText.text = rechargeText;

        yield return new WaitForSeconds(delayRecharge);

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
        ammoText.text = ammoCurrent + " / " + ammoQuantity;
        isRecharging = false;
        rechargeCoroutine = null;
    }
}
