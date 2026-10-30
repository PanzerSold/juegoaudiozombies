using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Rocket : MonoBehaviour
{

    private float speed;
    private float explosionRadius;

    private float gunDamage;

    private float destroyTime;

    private bool isExploding;

    [SerializeField] private ParticleSystem Explosion;

    private MeshRenderer meshRenderer;

    public void Initialize(float speed, float explosionRadius, float gunDamage, float destroyTime)
    {
        this.speed = speed;
        this.explosionRadius = explosionRadius;
        this.gunDamage = gunDamage;
        this.destroyTime = destroyTime;
    }

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        if(!isExploding)
        {
            Move();
        } 
    }

    private void Move()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isExploding)
        {
            Explode();
            Destroy(gameObject, destroyTime);
        }
    }

    private void Explode()
    {
        meshRenderer.enabled = false;
        isExploding = true;

        if (Explosion != null)
        {
            ParticleSystem explosion = Instantiate(Explosion,transform.position,Quaternion.identity);

            explosion.Play();

            Destroy(explosion.gameObject, explosion.main.duration + explosion.main.startLifetime.constantMax);
        }

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            explosionRadius
        );

        List<ZombieBase> currentZombie = new List<ZombieBase>();
        foreach (var hitCollider in colliders)
        {
            ZombieBase zombie = hitCollider.GetComponentInParent<ZombieBase>();
            PlayerHealth player = hitCollider.GetComponent<PlayerHealth>();


            if (player != null)
            {
                player.LoseHealth(1);
            }

            if (zombie != null)
            {
                if (currentZombie.Contains(zombie)) continue;
                currentZombie.Add(zombie);
                zombie.ReceiveDamage(false, gunDamage);
            }
        }
    }
}
