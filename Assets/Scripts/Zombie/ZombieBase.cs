using UnityEngine;

public class ZombieBase : MonoBehaviour
{
    private float health = 10f;
    // public bool Active = true;
    private bool isStunned = false;
    // private RoundManager rm;
    private Player player;
    private Transform playerTransform;
    private bool scriptedMovement = false;
    private Vector3 scriptedDir = Vector3.zero;
    private Rigidbody rb;

    private Barricade ultimabarricada;

    [SerializeField] private float speed = 5f;

    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private int zombieDamage = 1;
    private bool destroyingBarricade = false;
    private RoundManager rm;

    // private float timerPos = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerTransform = GameObject.Find("Player").GetComponentInParent<Transform>();
        rm = GameObject.FindWithTag("RoundManager").GetComponent<RoundManager>();
        health = rm.SetHealthBasedOnRound();
    }


    private void FixedUpdate()
    {
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

            Vector3 position = transform.position + direction * speed * Time.fixedDeltaTime;
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

    }

    public async void Stun()
    {
        isStunned = true;
        await Awaitable.WaitForSecondsAsync(2f);
        if (this == null) return;
        isStunned = false;
    }
    public void ReceiveDamage(bool isHeadshot, float damage)
    {
        if (isHeadshot)
        {
            if (player.CurrentPowerUp == Player.PlayerBonus.HeadShots)
            {
                health = 0;
            }
            else
            {
                health -= damage * 5;
            }
        }
        health -= damage;
        if (health <= 0)
        {
            player.AwardPoints(60);
            if (isHeadshot)
            {
                player.AwardPoints(40);
            }
            Despawn();
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
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.LoseHealth(zombieDamage);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieBarricade")){ 
            ultimabarricada=other.GetComponentInParent<Barricade>();
            ultimabarricada.zombiesClose++;
            destroyingBarricade = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ZombieBarricade"))
        {
            ultimabarricada.zombiesClose--;
            destroyingBarricade = false;
        }
    }


    /*
    void Update()
    {
        
        if (Active){
            if (!isStunned) return;
            if (TimedCheck())
            {
                
            }

        }
        
    }
    

    
    private bool TimedCheck()
    {
        if (timerPos < 0.2f)
        {
            timerPos += Time.deltaTime;
            return false;
        }
        timerPos = 0f;
        return true;
    }
    
    private void Respawn()
    {
        SetHealthBasedOnRound(rm.currentRound);

    }
    */


    public void goTo( Vector3 position)
    {
        scriptedMovement = true;
        scriptedDir = position;

    }

}
