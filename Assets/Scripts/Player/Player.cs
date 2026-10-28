using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionsAsset;
    [SerializeField] private static float moveSpeed = 5f;
    private float wingsSpeed = moveSpeed * 1.5f;
    private float superSpeed = moveSpeed * 1.7f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float friction = 5f;
    private bool hasWings = false;
    public bool shieldAvailable = true;
    [SerializeField] private float shieldCooldown = 30f;
    private float shieldCooldownTimer;
    private int timeLeft;
    private int lastTimeLeft;

    [SerializeField] private TextMeshProUGUI textPoints;
    [SerializeField] private AudioSource BarrierBuildSfx;
    private PlayerShoot playerAmmo;
    private string pointsStart;
    public int points = 0;
    public enum PlayerBonus
    {
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
    [SerializeField] private Dictionary<PlayerBonus, Coroutine> powerUpTimers = new Dictionary<PlayerBonus, Coroutine>();
    public bool HasPowerUp(PlayerBonus bonus)
    {
        return powerUpTimers.ContainsKey(bonus);
    }
    public bool[] activePerks;

    private float horizontalMovement;
    private float forwardMovement;
    private float verticalMovement = -2f;
    private CharacterController cc;

    [SerializeField] private PlayerInputs pi;
    [SerializeField] private Inventory inv;
    [SerializeField] private PlayerHealth ph;
    [SerializeField] private ParticleSystem PlayerFire;
    [SerializeField] private ParticleSystem psWings;


    private void Start()
    {
        ph = GetComponent<PlayerHealth>();
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInputs>();
        playerAmmo = GetComponent<PlayerShoot>();
        pointsStart = textPoints.text;
        textPoints.text = pointsStart + " " + points;
        shieldCooldownTimer = shieldCooldown;
        lastTimeLeft = -1;
    }

    private void Update()
    {
        shieldCooldownTimer += Time.deltaTime;
        timeLeft = Mathf.CeilToInt(shieldCooldown - shieldCooldownTimer);
        bool isGrounded = cc.isGrounded;
        Vector3 playerMovement;

        if (!shieldAvailable)
        {
            if (timeLeft != lastTimeLeft)
            {
                HealthBar healthBar = GetComponent<HealthBar>();
                healthBar.shieldTime.gameObject.SetActive(true);
                healthBar.shieldTimeText.gameObject.SetActive(true);
                healthBar.shieldTimeText.text = timeLeft + "s";
                lastTimeLeft = timeLeft;
            }
        }
        if (isGrounded)
        {
            playerMovement = pi.moveAction.ReadValue<Vector2>();
            bool hasSpeed = activePerks[(int)PlayerPerks.Speed];
            float currentSpeed;
            if (!hasSpeed && !hasWings) currentSpeed = moveSpeed;
            else if ((hasSpeed && !hasWings) || (!hasSpeed && hasWings)) currentSpeed = wingsSpeed;
            else (currentSpeed) = superSpeed;
            horizontalMovement = Mathf.MoveTowards(horizontalMovement, playerMovement.x * currentSpeed, friction * Time.deltaTime);
            forwardMovement = Mathf.MoveTowards(forwardMovement, playerMovement.y * currentSpeed, friction * Time.deltaTime); verticalMovement = -2f;
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

    public IEnumerator Shield()
    {
        shieldCooldownTimer = 0;
        yield return new WaitForSeconds(shieldCooldown);
        HealthBar healthBar = GetComponent<HealthBar>();
        healthBar.shieldTime.gameObject.SetActive(false);
        healthBar.shieldTimeText.gameObject.SetActive(false);
        shieldAvailable = true;
        UnityEvents.shielded.Invoke(true);
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
            if (pu != null)
            {
                PlayerBonus bonus = pu.pu;
                ActivatePowerUp(bonus);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Fire") && !PlayerFire.isEmitting)
        {
            ph.LoseHealth();
            PlayerFire.Play();
        }
    }

    private void ActivatePowerUp(PlayerBonus bonus)
    {
        if (bonus == PlayerBonus.MaxAmmo)
        {
            playerAmmo.Gun.RechargeAmmo();
            playerAmmo.Gun.GainAmmo(50);
            HealthBar healthBar = GetComponent<HealthBar>();
            healthBar.VisualMaxAmmoText();
            return;
        }

        if (bonus == PlayerBonus.Wings)
        {
            hasWings = true;

            if (psWings != null)
            {
                psWings.Play();
            }
        }

        bool alreadyActive = powerUpTimers.TryGetValue(bonus, out Coroutine existingTimer);

        if (alreadyActive) StopCoroutine(existingTimer);

        int index = (int)bonus;

        if (index >= activePowerUps.Length) return;

        float duration = activePowerUps[index];

        powerUpTimers[bonus] = StartCoroutine(RemovePowerUpAfterTime(bonus, duration));

        UnityEvents.onPowerUpTriggered.Invoke();
    }
    private void LateUpdate()
    {
        if (psWings != null && hasWings)
        {
            psWings.transform.rotation = transform.rotation;
        }
    }
    private IEnumerator RemovePowerUpAfterTime(PlayerBonus bonus, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (bonus == PlayerBonus.Wings)
        {
            hasWings = false;

            if (psWings != null)
            {
                psWings.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        powerUpTimers.Remove(bonus);

        UnityEvents.onPowerUpTriggered.Invoke();
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
        if (HasPowerUp(PlayerBonus.DoublePoints)) points += pts;
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
