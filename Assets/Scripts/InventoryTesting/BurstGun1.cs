using System.Collections;
using UnityEngine;

//==
public class BurstGun1 : GunBehaviour1
//==
{
    [Header("Burst Settings")]
    [SerializeField] private int bulletsPerBurst = 3;
    [SerializeField] private float burstInterval = 0.1f;

    private Coroutine burstCoroutine;

    //==
    public override void Unequip()
    {
        base.Unequip();
        burstCoroutine = null;
    }

    //==
    public override void Shoot()
    {
        if (burstCoroutine == null)
        {
            burstCoroutine = StartCoroutine(ShootBurstCoroutine());
        }
    }

    private IEnumerator ShootBurstCoroutine()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            base.Shoot();

            if (i < bulletsPerBurst - 1)
                yield return new WaitForSeconds(burstInterval);
        }

        burstCoroutine = null;
    }
}