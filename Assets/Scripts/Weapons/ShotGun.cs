using UnityEngine;

public class ShotGun : GunBehaviour
{
    [Header("Shotgun Settings")]
    [SerializeField] private int raycastCount = 8;
    [SerializeField] private float spreadAngle = 6f;
    [SerializeField] private float rotationPerShot = 15f;

    [Header("Impact Particles")]
    [SerializeField] private ParticleSystem impactPrefab;

    private float currentRotation = 0f;

    public override void Shoot()
    {
        shootSfx.pitch = Random.Range(minPitchShift, maxPitchShift);
        shootSfx.PlayOneShot(shootClip);
        FireAmmo();

        for (int i = 0; i < raycastCount; i++)
        {

            float angle = currentRotation + i * (360 / raycastCount);

            float radians = angle * Mathf.Deg2Rad;

            float radius = Mathf.Tan(spreadAngle * Mathf.Deg2Rad);

            Vector3 offset = cameraPos.transform.right * Mathf.Cos(radians) + cameraPos.transform.up * Mathf.Sin(radians);

            Vector3 direction = (cameraPos.transform.forward + offset * radius).normalized;

            ShootRaycast(direction);

        }

        currentRotation = Mathf.Repeat(currentRotation + rotationPerShot, 360f);

    }



    private void ShootRaycast(Vector3 direction)
    {
        Vector3 origin = cameraPos.transform.position;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, raycastDistance, coll))
        {
            ParticleSystem impact = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));

            ParticleSystemRenderer renderer = impact.GetComponent<ParticleSystemRenderer>();

            renderer.material = materialGenerico;

            if (hit.collider.CompareTag("ZombieBody"))
            {
                ZombieBase zombie = hit.collider.GetComponent<ZombieBase>();
                zombie.ReceiveDamage(false, gunDamage);

                renderer.material = materialZombieHit;

            }

            if (hit.collider.CompareTag("ZombieHead"))
            {
                ZombieBase zombie = hit.collider.GetComponentInParent<ZombieBase>();
                zombie.ReceiveDamage(true, gunDamage);

                renderer.material = materialZombieHit;
            }

            impact.Play();

            ParticleSystem.MainModule main = impact.main;

            float lifetime = main.duration +
                             main.startLifetime.constantMax;

            Destroy(impact.gameObject, lifetime);
        }
    }
}