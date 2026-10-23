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

    [SerializeField] private AudioSource BarrierBuildSfx;
    private HealthBar healthBar;
    private PlayerShoot playerAmmo;
    private string pointsStart;
    public int points = 0;
    private bool wingsIsCollected = false;
    private bool speedIsCollected = false;
    public bool isShielded = false;
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
        None,
        Shielded,
        Speed,
        Haste,
        Rebirth,
        Scavenger,
        Vampirism,
        CriticalShots,
        ExplosiveBullets
    }

    private float horizontalMovement;
    private float forwardMovement;
    [SerializeField] private PlayerBonus currentPowerUp = PlayerBonus.None;
    public PlayerBonus CurrentPowerUp => currentPowerUp;
    [SerializeField] private PlayerPerks currentPerk = PlayerPerks.None;
    public PlayerPerks CurrentPerk => currentPerk;
    private float verticalMovement = -2f;
    private CharacterController cc;

    [SerializeField] private PlayerInputs pi;
    [SerializeField] private Inventory inv;

    private void Start()
    {
        healthBar = GetComponent<HealthBar>();
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInputs>();
        playerAmmo = GetComponent<PlayerShoot>();
        pointsStart = textPoints.text;
        textPoints.text = pointsStart + " " + points;
        MeCagoEnLaPuta();
    }

    private void Update()
    {
        bool isGrounded = cc.isGrounded;
        Vector3 playerMovement;

        if (currentPowerUp == PlayerBonus.Wings) IncreaseSpeed(1.5f);

        if (currentPowerUp == PlayerBonus.MaxAmmo)
        {
            playerAmmo.Gun.RechargeAmmo();
            playerAmmo.Gun.GainAmmo(50);
            currentPowerUp = PlayerBonus.None;
        }

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

    private void MeCagoEnLaPuta()
    {
        if (currentPerk == PlayerPerks.Shielded)
        {
            if (isShielded) return;
            isShielded = true;
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
    }

    public void IncreaseSpeed(float speed)
    {
        if (wingsIsCollected) return;
        moveSpeed *= speed;
        wingsIsCollected = true;

        if (speedIsCollected) return;
        moveSpeed *= speed;
        speedIsCollected = true;
    }
    private async void asyncRemovePowerUp()
    {
        await Awaitable.WaitForSecondsAsync(20f);
        if (this == null) return;
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
