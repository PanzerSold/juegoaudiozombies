using TMPro;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GunBehaviour startgun;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private TextMeshProUGUI playerAmmoText;

    [SerializeField] private ParticleSystem shotParticle;

    [Header("Sonido")]
    [SerializeField] protected AudioSource shootSfx;
    [SerializeField] private AudioSource emptyMagSfx;
    [SerializeField] private AudioSource reloadSfx;

    private PlayerInputs pi;

    private GunBehaviour gun;

    public GunBehaviour Gun => gun;

    private float shootTimer = 0;

    private void Start()
    {
        pi = gameObject.GetComponent<PlayerInputs>();
        gun = startgun;
        gun.Initialize(playerCamera, playerAmmoText, shotParticle, shootSfx, reloadSfx);

    }


    public void EquipGun(GunBehaviour newGun)
    {
        gun = newGun;
        gun.Initialize(playerCamera, playerAmmoText, shotParticle, shootSfx, reloadSfx);
        shootTimer = 0;
    }


    private void Update()
    {
        if (startgun != gun)
        {
            gun = startgun;
            gun.Initialize(playerCamera, playerAmmoText, shotParticle, shootSfx, reloadSfx);
        }
        if (gun != null)
         {
            bool inputShoot;
            shootTimer += Time.deltaTime;
            if (gun.IsAutomatic)
            {
                inputShoot = pi.shootAction.IsPressed();
            } else
            {
                inputShoot = pi.shootAction.WasPressedThisFrame();
            }

            if (!gun.IsRecharging)
            {
                if (shootTimer >= gun.ShootTime)
                {
                    if (inputShoot)
                    {
                        if (gun.AmmoCurrent > 0)
                        {
                            gun.Shoot();
                        }
                        else if (!emptyMagSfx.isPlaying)
                        {
                            emptyMagSfx.Play();
                        }
                        shootTimer = 0;
                    }
                }

                if (gun.AmmoCurrent != gun.AmmoMax && gun.AmmoQuantity > 0)
                {
                    if (pi.reloadAction.WasPressedThisFrame())
                    {
                        gun.RechargeAmmo();
                    }

                }
            }
        }
    }
}
