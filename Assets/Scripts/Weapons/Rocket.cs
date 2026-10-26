using UnityEngine;

public class Rocket : MonoBehaviour
{

    private float speed;
    private float explosionRadius;

    private float gunDamage;

    private float destroyTime;

    private bool isExploding;

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
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hitCollider in colliders)
        {
            ZombieBase zombie = hitCollider.gameObject.GetComponentInParent<ZombieBase>();
            PlayerHealth player = hitCollider.gameObject.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.LoseHealth(1);
            }

            if (zombie != null)
            {

                zombie.ReceiveDamage(false, gunDamage);

            }
        }
    }
}
