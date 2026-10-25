//==
using System.Collections.Generic;
//==
using TMPro;
using UnityEngine;

//==
public class PlayerShoot1 : MonoBehaviour
//==
{
    [SerializeField] private Camera playerCamera;

    [SerializeField] private TextMeshProUGUI playerAmmoText;

    [SerializeField] private ParticleSystem shotParticle;

    //==
    [SerializeField] private GameObject inventoryPanel;
    //==

    [Header("Sonido")]
    [SerializeField] protected AudioSource shootSfx;
    [SerializeField] private AudioSource emptyMagSfx;
    [SerializeField] private AudioSource reloadSfx;

    //==
    private PlayerInputs1 pi;
    private Inventory1 inventory;

    private GunBehaviour1 gun;

    public GunBehaviour1 Gun => gun;

    private readonly Dictionary<Inventory1.itemIndex, GunBehaviour1> guns = new Dictionary<Inventory1.itemIndex, GunBehaviour1>();
    //==

    private float shootTimer = 0;
    //==
    private bool inventoryOpen = false;

    private void Awake()
    {
        inventory = GetComponent<Inventory1>();

        foreach (GunBehaviour1 g in GetComponentsInChildren<GunBehaviour1>())
        {
            if (g.WeaponIndex == Inventory1.itemIndex.None)
            {
                Debug.LogWarning("El arma '" + g.name + "' no tiene Weapon Index asignado.", g);
                continue;
            }
            if (!guns.TryAdd(g.WeaponIndex, g))
            {
                Debug.LogWarning("Dos armas comparten el Weapon Index " + g.WeaponIndex + " ('" + g.name + "').", g);
            }
        }
    }
    //==

    private void Start()
    {
        //==
        pi = gameObject.GetComponent<PlayerInputs1>();
        EquipWeapon(inventory.EquippedWeapon);
        //==
    }

    //==
    private void EquipWeapon(Inventory1.itemIndex weapon)
    {
        if (!guns.TryGetValue(weapon, out GunBehaviour1 newGun))
        {
            Debug.LogWarning("No hay ningun arma en el Player con Weapon Index " + weapon);
            return;
        }
        EquipGun(newGun);
    }

    public void EquipGun(GunBehaviour1 newGun)
    {
        if (newGun == gun) return;
        if (gun != null) gun.Unequip();

        gun = newGun;
        gun.Initialize(playerCamera, playerAmmoText, shotParticle, shootSfx, reloadSfx, inventory);
        shootTimer = 0;
    }

    public void EquipFromSlot(int slot)
    {
        if (inventory.TrySelectWeaponSlot(slot, out Inventory1.itemIndex weapon))
        {
            EquipWeapon(weapon);
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;
        if (inventoryPanel != null) inventoryPanel.SetActive(inventoryOpen);

        Time.timeScale = inventoryOpen ? 0f : 1f;
        Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inventoryOpen;

        if (inventoryOpen && gun != null) gun.CancelReload();
    }

    private void ThrowGrenade()
    {
        if (inventory.TakeAmount(Inventory1.itemIndex.Granada, 1) == 0) return;
    }
    //==


    private void Update()
    {
        //==
        if (pi.inventoryAction.WasPressedThisFrame())
        {
            ToggleInventory();
        }

        if (inventoryOpen) return;
        //==

        if (gun != null)
        {
            bool inputShoot;
            shootTimer += Time.deltaTime;
            if (gun.IsAutomatic)
            {
                inputShoot = pi.shootAction.IsPressed();
            }
            else
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