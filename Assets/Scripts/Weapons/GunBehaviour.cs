using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] protected float raycastDistance = 100f;
    [SerializeField] protected float gunDamage = 1f;
    protected Camera cameraPos;
    [SerializeField] protected LayerMask coll;
    protected ParticleSystem ps;

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
    [SerializeField] protected AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;

    [SerializeField] protected float minPitchShift = 0.85f;
    [SerializeField] protected float maxPitchShift = 1.15f;

    protected AudioSource shootSfx;
    private AudioSource reloadSfx;

    private TextMeshProUGUI ammoText;

    private string rechargeText = "Reloading...";

    [Header("Automatica")]

    [SerializeField] private bool isAutomatic = false;

    [Header("Balas explosivas")]

    [SerializeField] private float boomRadius = 1f;

    public bool IsAutomatic => isAutomatic;

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
    private Player player;

    private void Awake()
    {
        ammoCurrent = ammoMax;
    }

    public void Initialize(Camera playerCamera, TextMeshProUGUI playerAmmoText, ParticleSystem shotParticle, AudioSource shootSource, AudioSource reloadSource, Player playerScript)
    {
        cameraPos = playerCamera;
        ammoText = playerAmmoText;
        ps = shotParticle;
        shootSfx = shootSource;
        reloadSfx = reloadSource;
        player = playerScript;

        if (ps != null)
        {
            psR = ps.GetComponent<ParticleSystemRenderer>();
        }

        if (ammoText != null)
        {
            ammoText.text = ammoCurrent + " / " + ammoQuantity;
        }
    }


    virtual public void Shoot()
    {
        RaycastHit hit;
        Vector3 origin = cameraPos.transform.position;
        Vector3 direction = cameraPos.transform.forward;
        Material impactMaterial = materialGenerico;
        shootSfx.pitch = Random.Range(minPitchShift, maxPitchShift);
        shootSfx.PlayOneShot(shootClip);
        FireAmmo();
        if (Physics.Raycast(origin, direction, out hit, raycastDistance, coll))
        {
            ps.transform.position = hit.point;

            if (player.activePerks[7])
            {
                float explosiveDamage = gunDamage * 2;
                Collider[] colliders = Physics.OverlapSphere(hit.point, boomRadius);

                foreach (Collider hitCollider in colliders)
                {
                    if (hitCollider.CompareTag("ZombieBody"))
                    {

                        ZombieBase zombie = hitCollider.GetComponent<ZombieBase>();
                        zombie.ReceiveDamage(false, explosiveDamage);
                        impactMaterial = materialZombieHit;
                    }

                    if (hitCollider.CompareTag("ZombieHead"))
                    {

                        ZombieBase zombie = hitCollider.GetComponentInParent<ZombieBase>();

                        zombie.ReceiveDamage(true, explosiveDamage);
                        impactMaterial = materialZombieHit;

                    }
                }
            }

            else
            {
                if (hit.collider.CompareTag("ZombieBody"))
                {

                    ZombieBase zombie = hit.collider.GetComponent<ZombieBase>();
                    zombie.ReceiveDamage(false, gunDamage);
                    impactMaterial = materialZombieHit;
                }

                if (hit.collider.CompareTag("ZombieHead"))
                {

                    ZombieBase zombie = hit.collider.GetComponentInParent<ZombieBase>();

                    zombie.ReceiveDamage(true, gunDamage);
                    impactMaterial = materialZombieHit;

                }
            }

            psR.material = impactMaterial;
            ps.Play();

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

    private IEnumerator Recharge()
    {
        isRecharging = true;
        reloadSfx.PlayOneShot(reloadClip);
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
