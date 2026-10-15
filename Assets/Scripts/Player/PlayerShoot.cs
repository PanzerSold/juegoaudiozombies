using TMPro;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GunBehaviour startgun;

    [SerializeField] private Camera playerCamera;

    [SerializeField] private TextMeshProUGUI playerAmmoText;

    [SerializeField] private ParticleSystem shotParticle;


    private PlayerInputs pi;

    private GunBehaviour gun;

    public GunBehaviour Gun => gun;

    private float shootTimer = 0;

    private void Start()
    {
        pi = gameObject.GetComponent<PlayerInputs>();
        gun = startgun;
        gun.Initialize(playerCamera, playerAmmoText, shotParticle);

    }


    public void EquipGun(GunBehaviour newGun)
    {
        gun = newGun;
        gun.Initialize(playerCamera, playerAmmoText, shotParticle);
        shootTimer = 0;
    }


    private void Update()
    {
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
                        else if (!gun.EmptyMagSfx.isPlaying)
                        {
                            gun.EmptyMagSfx.Play();
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
