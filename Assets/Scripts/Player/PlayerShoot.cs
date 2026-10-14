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
    }


    public void EquipGun(GunBehaviour newGun)
    {
        gun = newGun;
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
                        gun.FireAmmo();
                        shootTimer = 0;
                    }
                }

                if (gun.AmmoCurrent != gun.AmmoMax)
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
