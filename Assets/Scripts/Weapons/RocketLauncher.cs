using System;
using UnityEngine;

public class RocketLauncher : GunBehaviour
{
    [Header("Rocket")]
    [SerializeField] private Rocket rocketPrefab;

    [SerializeField] private GameObject rocketSpawn;

    [SerializeField] private float rocketSpeed = 20f;
    [SerializeField] private float explosionRadius = 10f;

    [SerializeField] private float destroyTime = 0.5f;

    public override void Shoot()
    {
        Vector3 origin = rocketSpawn.transform.position;
        Quaternion rotation = cameraPos.transform.rotation;
        Rocket rocket = Instantiate(rocketPrefab, origin, rotation);
        ShootSfx.Play();
        FireAmmo();
        rocket.Initialize(rocketSpeed, explosionRadius, gunDamage,destroyTime);
        
    }


}
