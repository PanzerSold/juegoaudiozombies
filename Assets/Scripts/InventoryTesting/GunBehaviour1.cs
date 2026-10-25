using System.Collections;
using TMPro;
using UnityEngine;

//==
public class GunBehaviour1 : MonoBehaviour
//==
{
    [Header("Disparo")]
    [SerializeField] protected float raycastDistance = 100f;
    [SerializeField] protected float gunDamage = 1f;
    protected Camera cameraPos;
    [SerializeField] protected LayerMask coll;
    protected ParticleSystem ps;

    [SerializeField] private float shootTime = 0.5f;
    protected ParticleSystemRenderer psR;

    //==
    [Header("Inventario")]
    [SerializeField] private Inventory1.itemIndex weaponIndex = Inventory1.itemIndex.None;
    //==

    [Header("Municion")]
    [SerializeField] private int ammoMax = 10;

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
    //==
    private Inventory1 inventory;
    //==

    private string rechargeText = "Reloading...";

    [Header("Automatica")]

    [SerializeField] private bool isAutomatic = false;

    public bool IsAutomatic => isAutomatic;

    //==
    public Inventory1.itemIndex WeaponIndex => weaponIndex;
    protected Inventory1.itemIndex AmmoType => Inventory1.GetAmmoFor(weaponIndex);
    //==

    public int AmmoCurrent => ammoCurrent;

    public float ShootTime => shootTime;
    public int AmmoMax => ammoMax;

    private int ammoCurrent;

    private bool isRecharging;

    public bool IsRecharging => isRecharging;

    public TextMeshProUGUI AmmoText => ammoText;

    //==
    public int AmmoQuantity => inventory != null ? inventory.GetAmount(AmmoType) : 0;
    //==
    private Coroutine rechargeCoroutine;

    private void Awake()
    {
        ammoCurrent = ammoMax;
    }

    //==
    public void Initialize(Camera playerCamera, TextMeshProUGUI playerAmmoText, ParticleSystem shotParticle, AudioSource shootSource, AudioSource reloadSource, Inventory1 playerInventory)
    //==
    {
        cameraPos = playerCamera;
        ammoText = playerAmmoText;
        ps = shotParticle;
        shootSfx = shootSource;
        reloadSfx = reloadSource;

        //==
        if (inventory != null) inventory.OnInventoryChanged -= RefreshAmmoText;
        inventory = playerInventory;
        inventory.OnInventoryChanged += RefreshAmmoText;
        //==

        if (ps != null)
        {
            psR = ps.GetComponent<ParticleSystemRenderer>();
        }

        //==
        RefreshAmmoText();
        //==
    }

    //==
    public virtual void Unequip()
    {
        StopAllCoroutines();
        CancelReload();
        if (inventory != null) inventory.OnInventoryChanged -= RefreshAmmoText;
    }

    public void CancelReload()
    {
        if (rechargeCoroutine != null) StopCoroutine(rechargeCoroutine);
        rechargeCoroutine = null;
        isRecharging = false;
        RefreshAmmoText();
    }

    private void RefreshAmmoText()
    {
        if (ammoText == null) return;
        ammoText.text = isRecharging ? rechargeText : ammoCurrent + " / " + AmmoQuantity;
    }
    //==


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

            psR.material = impactMaterial;
            ps.Play();

        }

    }


    //==
    protected void FireAmmo()
    //==
    {
        ammoCurrent--;
        if (ammoCurrent < 0)
        {
            ammoCurrent = 0;
        }
        //==
        RefreshAmmoText();
        //==
    }
    public void RechargeAmmo()
    {
        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
        }
        rechargeCoroutine = StartCoroutine(Recharge());

    }

    public void GainAmmo(int gainedAmmo)
    {
        //==
        inventory.AddItem(Inventory1.CreateItem(AmmoType, gainedAmmo));
        RefreshAmmoText();
        //==
    }

    private IEnumerator Recharge()
    {
        isRecharging = true;
        reloadSfx.PlayOneShot(reloadClip);
        //==
        RefreshAmmoText();
        //==

        yield return new WaitForSeconds(delayRecharge);

        //==
        isRecharging = false;
        rechargeCoroutine = null;

        int needed = ammoMax - ammoCurrent;
        ammoCurrent += inventory.TakeAmount(AmmoType, needed);

        RefreshAmmoText();
        //==
    }
}