using System.Collections;
using UnityEngine;

public class BurstGun : GunBehaviour
{
    [Header("Burst Settings")]
    [SerializeField] private int bulletsPerBurst = 3;
    [SerializeField] private float burstInterval = 0.1f;

    private Coroutine burstCoroutine;
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

