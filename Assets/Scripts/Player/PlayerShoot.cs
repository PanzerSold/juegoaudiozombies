using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GunBehaviour startgun;
    private PlayerInputs pi;

    private GunBehaviour gun;

    public GunBehaviour Gun => gun;

    private float shootTimer = 0;

    private void Start()
    {
        pi = gameObject.GetComponent<PlayerInputs>();
        gun = startgun;
        gun.AmmoText.text = gun.AmmoCurrent + " / " + gun.AmmoQuantity;

    }


    public void EquipGun(GunBehaviour newGun)
    {
        gun = newGun;
        gun.AmmoText.text = gun.AmmoCurrent + " / " + gun.AmmoQuantity;
    }


    private void Update()
    {
        if (gun != null)
         { 
            shootTimer += Time.deltaTime;
            if (!gun.IsRecharging)
            {
                if (shootTimer > gun.ShootTime)
                {
                    if (pi.shootAction.WasPressedThisFrame())
                    {
                        if (gun.AmmoCurrent > 0)
                        {
                            gun.Shoot();
                        }
                        else gun.EmptyMagSfx.Play();
                        shootTimer = 0;
                    }
                }

                if (gun.AmmoCurrent != gun.AmmoMax || gun.AmmoQuantity == 0)
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
