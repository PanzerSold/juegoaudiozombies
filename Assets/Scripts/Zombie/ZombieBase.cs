using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ZombieBase : MonoBehaviour
{
    [SerializeField] private float health = 10f;

    [Header("Probabilidad de crítico")]
    [SerializeField, Range(0f, 1f)]

    private float criticalChance = 0.33f;

    [Header("Probabilidad de curación")]
    [SerializeField, Range(0f, 1f)]

    private float healChance = 0.0667f;
    private bool isStunned = false;
    private Player player;
    [SerializeField] private PowerUpSpawner pus;
    private PlayerShoot playerAmmo;
    private Transform playerTransform;

    //----- Codigo borrado ----- El movimiento por waypoint ahora lo maneja ZombieNavmesh
    // private bool scriptedMovement = false;
    // private Vector3 scriptedDir = Vector3.zero;
    //----- Codigo borrado -----

    private Rigidbody rb;
    
    //----- NUEVO CODIGO ----- Referencia al script que mueve al zombie con el NavMesh
    private ZombieNavmesh nav;
    //----- NUEVO CODIGO -----


    //----- NUEVO CODIGO ----- Awake porque RoundManager llama a goTo justo después del Instantiate, antes del Start
    private void Awake()
    {
        nav = GetComponent<ZombieNavmesh>();
    }
    //----- NUEVO CODIGO -----

    //----- NUEVO CODIGO ----- Distancia a la que el zombie le pega al jugador (tiene que ser mayor al Stopping Distance del NavMeshAgent)
    [SerializeField] private float attackRange = 1.5f;
    private PlayerHealth playerHealth;
    //----- NUEVO CODIGO -----
    private Barricade ultimabarricada;

    // [SerializeField] private float rotationSpeed = 10f; la rotacion depende del agente, no del rigidbody.

    [SerializeField] private int zombieDamage = 1;
    [SerializeField] private ParticleSystem ps;

    [SerializeField] private float[] speedValues = new float[3]{1.6f, 1.8f, 2.4f};

    private int speedIndex = 0;
    private bool destroyingBarricade = false;
    private RoundManager rm;

    private Animator anim;

    private bool isAttacking = false;

    // private float timerPos = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();
        playerAmmo = playerObject.GetComponent<PlayerShoot>();
        playerTransform = playerObject.transform;
        //----- NUEVO CODIGO -----
        playerHealth = playerObject.GetComponent<PlayerHealth>();
        //----- NUEVO CODIGO -----
        rm = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        health = rm.SetHealthBasedOnRound();

        //----- NUEVO CODIGO ----- Le pasa la velocidad al agente
        nav.SetSpeed(speedValues[speedIndex]);
        //----- NUEVO CODIGO -----
    }

    //----- Codigo borrado ----- Con el Rigidbody kinematic esta colisión ya no se dispara
    // private void OnCollisionStay(Collision collision) => HitStay(collision);
    //----- Codigo borrado -----

    //----- NUEVO CODIGO ----- Frena al zombie si está en una barricada cerrada y ataca al jugador si está al alcance
    private void Update()
    {
        bool blockedByBarricade = destroyingBarricade && ultimabarricada != null && !ultimabarricada.isOpen;
        nav.SetHolding(blockedByBarricade);

        if (blockedByBarricade || isStunned || isAttacking) return;

        Vector3 diff = playerTransform.position - transform.position;
        diff.y = 0f;

        if (diff.magnitude <= attackRange)
        {
            StartCoroutine(Attack(playerHealth));
        }
    }
    //----- NUEVO CODIGO -----

    private void FixedUpdate()
    {
        //----- Codigo borrado ----- El movimiento y la rotación ahora los hace el NavMeshAgent
        /*
        if (!isStunned)
        {
            Vector3 direction = playerTransform.position - transform.position; 
            if (scriptedMovement)
            {
                if((scriptedDir - transform.position).magnitude < 1f) scriptedMovement = false;
                else direction = scriptedDir - transform.position;
            }
             

            direction.y = 0;
            direction.Normalize();

            Vector3 position = transform.position + direction * speedValues[speedIndex] * Time.fixedDeltaTime;
            rb.MovePosition(position);

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime));
            }
        } 
        else
        {
            rb.linearVelocity = Vector3.zero;

        }
        */
        //----- Codigo borrado -----

        float actualSpeed = isStunned ? 0 : speedValues[speedIndex];
        
        anim.SetFloat("Speed", actualSpeed);

    }


    public async void Stun()
    {
        isStunned = true;
        //----- NUEVO CODIGO ----- Frena al agente mientras está aturdido
        nav.SetStunned(true);
        //----- NUEVO CODIGO -----
        await Awaitable.WaitForSecondsAsync(2f);
        if (this == null) return;
        isStunned = false;
        //----- NUEVO CODIGO ----- Lo vuelve a mover
        nav.SetStunned(false);
        //----- NUEVO CODIGO -----
    }
    public void ReceiveDamage(bool isHeadshot, float damage)
    {
        if (isHeadshot)
        {
            if (player.HasPowerUp(Player.PlayerBonus.HeadShots))
            {
                health = 0;
            }
            else
            {
                if (player.activePerks[6])
                {
                    if (Random.value >= criticalChance) health -= damage * 5;
                    else health -= damage * 10;
                }
                else health -= damage * 5;
            }
        }
        if (player.activePerks[6])
        {
            if (Random.value >= criticalChance) health -= damage;
            else health -= damage * 2;
        }
        else health -= damage;
        if (health <= 0)
        {
            player.AwardPoints(60);
            if (isHeadshot)
            {
                player.AwardPoints(40);
            }
            Despawn();
            if (player.activePerks[4] && playerAmmo.Gun != null)
            {
                int bonusAmmo = playerAmmo.Gun.AmmoMax * 10 / 100;
                playerAmmo.Gun.GainAmmo(bonusAmmo);
            }
            if (player.activePerks[5])
            {
                PlayerHealth ph = GameObject.Find("Player").GetComponent<PlayerHealth>();
                if (Random.value >= healChance || ph.health >= 6) return;
                else ph.GainHealth(1);
            }
        }
        else
        {
            player.AwardPoints(10);
        }
    }

    private void Despawn()
    {
        if(destroyingBarricade) ultimabarricada.zombiesClose--;
        rm.RemoveZombie(this);
        pus.Spawn();
        Destroy(gameObject);
    }

    private void HitStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || isStunned || isAttacking) return;

        PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            StartCoroutine(Attack(ph));
        }
    }

    private IEnumerator Attack(PlayerHealth player)
    {
        isAttacking = true;

        anim.Play("attack", 0, 0f);

        yield return new WaitForSeconds(0.3f);

        player.LoseHealth(zombieDamage);

        yield return new WaitForSeconds(0.45f);

        isAttacking = false;
        
    }

/*private void OnCollisionEnter(Collision collision)
{
    PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

    if (player != null)
    {
        player.LoseHealth(zombieDamage);
    }
}*/
public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieBarricade")){ 
            ultimabarricada=other.GetComponentInParent<Barricade>();
            ultimabarricada.zombiesClose++;
            destroyingBarricade = true;
        }
        if (other.CompareTag("Fire"))
        {
            StartCoroutine(Burn());
        }
    }

    IEnumerator Burn()
    {
        isStunned = true;
        //----- NUEVO CODIGO ----- Frena al agente mientras se quema
        nav.SetStunned(true);
        //----- NUEVO CODIGO -----
        ps.Play();
        yield return new WaitForSeconds(1f);
        Despawn();
    }
    
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ZombieBarricade"))
        {
            ultimabarricada.zombiesClose--;
            destroyingBarricade = false;
        }
    }
    public void goTo( Vector3 position)
    {
        //----- Codigo borrado -----
        // scriptedMovement = true;
        // scriptedDir = position;
        //----- Codigo borrado -----

        //----- NUEVO CODIGO ----- Ahora el punto de la barricada lo maneja ZombieNavmesh
        nav.SetWaypoint(position);
        //----- NUEVO CODIGO -----
    }

}
