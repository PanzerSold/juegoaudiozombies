using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Hammer : MonoBehaviour
{
    [SerializeField] private float hammerRadius = 5.5f;
    [SerializeField] private float coolDown = 15.0f;
    [SerializeField] private bool isOnCooldown = false;
    [SerializeField] private RawImage hammerImage;
    [SerializeField] private TextMeshProUGUI HammerText;
    [SerializeField] private PlayerInputs pi;
    private Color originalColor;
    private float cooldownTimer;
    private int timeLeft;
    private int lastTimeLeft;
    void Awake()
    {
        cooldownTimer = coolDown;
        lastTimeLeft = -1;

        if (hammerImage != null) originalColor = hammerImage.color;
    }

    private void OnEnable()
    {
        UnityEvents.onCooldownChanged.AddListener(ChangeColor);
    }

    private void OnDisable()
    {
        UnityEvents.onCooldownChanged.RemoveListener(ChangeColor);
    }

    private void ChangeColor(bool onCooldown)
    {
        hammerImage.color = onCooldown ? Color.black : originalColor;
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;
        timeLeft = Mathf.CeilToInt(coolDown - cooldownTimer);

        if (timeLeft <= 0)
        {
            if (isOnCooldown)
            {
                isOnCooldown = false;
                UnityEvents.onCooldownChanged.Invoke(false);
            }

            HammerText.text = "Ready";
        }
        else
        {
            if (timeLeft != lastTimeLeft)
            {
                HammerText.text = timeLeft + "s";
                lastTimeLeft = timeLeft;
            }
        }

        if (pi.skillAction.WasPressedThisFrame())
        {
            if (!isOnCooldown)
            {
                HammerStun();
                isOnCooldown = true;
                cooldownTimer = 0;
                UnityEvents.onCooldownChanged.Invoke(true);
            }
            else
            {
                //acá va el reproducir sonido de fallo
            }
        }



    }

    void HammerStun()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, hammerRadius);
        foreach (Collider collider in colliders)
        {

            ZombieBase zombie = collider.GetComponent<ZombieBase>();
            if (zombie != null)
            {
                zombie.Stun();
            }
        }

    }


    void SonidoCooldown()
    {
        
    }



}
