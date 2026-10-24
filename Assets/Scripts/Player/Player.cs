using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionsAsset;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float friction = 5f;

    [SerializeField] private TextMeshProUGUI textPoints;
    [SerializeField] private TextMeshProUGUI textPUandP;
    [SerializeField] private AudioSource BarrierBuildSfx;
    private HealthBar healthBar;
    private PlayerShoot playerAmmo;
    private string pointsStart;
    public int points = 0;
    public enum PlayerBonus
    {
        None,
        DoublePoints,
        HeadShots,
        Discount,
        Wings,
        MaxAmmo
    }

    public enum PlayerPerks
    {
        Shielded,
        Speed,
        Haste,
        Rebirth,
        Scavenger,
        Vampirism,
        CriticalShots,
        ExplosiveBullets
    }

    public float[] activePowerUps;
    public bool[] activePerks;

    private float horizontalMovement;
    private float forwardMovement;
    [SerializeField] private PlayerBonus currentPowerUp = PlayerBonus.None;
    public PlayerBonus CurrentPowerUp => currentPowerUp;
    [SerializeField] private PlayerPerks currentPerk;
    public PlayerPerks CurrentPerk => currentPerk;
    private float verticalMovement = -2f;
    private CharacterController cc;

    [SerializeField] private PlayerInputs pi;
    [SerializeField] private Inventory inv;
    [SerializeField] private PlayerHealth ph;
    [SerializeField] private ParticleSystem PlayerFire;

    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
        ph = GetComponent<PlayerHealth>();
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInputs>();
        playerAmmo = GetComponent<PlayerShoot>();
        pointsStart = textPoints.text;
        textPoints.text = pointsStart + " " + points;
    }

    private void Update()
    {
        bool isGrounded = cc.isGrounded;
        Vector3 playerMovement;


        if (currentPerk == PlayerPerks.Speed) IncreaseSpeed(1.5f);


        if (isGrounded)
        {
            playerMovement = pi.moveAction.ReadValue<Vector2>();
            horizontalMovement = Mathf.MoveTowards(horizontalMovement, playerMovement.x * moveSpeed, friction * Time.deltaTime);
            forwardMovement = Mathf.MoveTowards(forwardMovement, playerMovement.y * moveSpeed, friction * Time.deltaTime);
            verticalMovement = -2f;
            playerMovement = new Vector3(horizontalMovement, verticalMovement, forwardMovement);
            Quaternion rotacion = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            playerMovement = rotacion*playerMovement;
        } 
        else
        {
            verticalMovement += gravity * Time.deltaTime;
            playerMovement = new Vector3(0, verticalMovement, 0);
        }

        cc.Move(playerMovement * Time.deltaTime);
        
    }

    private async void Shield()
    {
        await Awaitable.WaitForSecondsAsync(30f);
        if (this == null) return;
        if (currentPerk == PlayerPerks.Shielded)
        {
            UnityEvents.shielded.Invoke(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interaction"))
        {
            other.GetComponent<Interaction>().PlayerEntered();
        }
        if (other.CompareTag("PowerUp"))
        {
            PowerUp pu = other.GetComponent<PowerUp>();
            if (pu.pu != PlayerBonus.None)
            {
                currentPowerUp = pu.pu;
                asyncRemovePowerUp();
            }
            else currentPowerUp = PlayerBonus.MaxAmmo;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Fire") && !PlayerFire.isEmitting)
        {
            ph.LoseHealth();
            PlayerFire.Play();
        }
    }

    private void IncreaseSpeed(float speed)
    {
        moveSpeed *= speed;
    }

    private void DecreaseSpeed(float speed)
    {
        moveSpeed /= speed;
    }
    private async void asyncRemovePowerUp()
    {
        await Awaitable.WaitForSecondsAsync(20f);
        if (this == null) return;
        if (currentPowerUp == PlayerBonus.Wings) DecreaseSpeed(1.5f);
        currentPowerUp = PlayerBonus.None;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interaction"))
        {
            other.GetComponent<Interaction>().PlayerExit();
        }
    }

    public void AwardPoints(int pts)
    {
        if (currentPowerUp == PlayerBonus.DoublePoints) points += pts;
        points += pts;
        textPoints.text = pointsStart + " " + points;
    }
    public void RemovePoints(int pts)
    {
        points -= pts;
        textPoints.text = pointsStart + " " + points;
    }
    public void PlayBarrierSound()
    {
        BarrierBuildSfx.Play();
    }


    
}
