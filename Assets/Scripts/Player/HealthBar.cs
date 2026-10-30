using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Player player;

    
    [Header("Barra de vida")]
    private RawImage[] images;
    [SerializeField] private RawImage image1;
    [SerializeField] private RawImage image2;
    [SerializeField] private RawImage image3;
    [SerializeField] private RawImage image4;
    [SerializeField] private RawImage image5;
    [SerializeField] private RawImage image6;
    [SerializeField] private Gradient colorByHealth;

    [Header("Power Ups")]
    private RawImage[] powerUps;
    [SerializeField] private RawImage doublePoints;
    [SerializeField] private RawImage headShots;
    [SerializeField] private RawImage discount;
    [SerializeField] private RawImage wings;

    [Header("Perks")]
    private RawImage[] perks;
    [SerializeField] private RawImage shielded;
    [SerializeField] private RawImage speed;
    [SerializeField] private RawImage haste;
    [SerializeField] private RawImage rebirth;
    [SerializeField] private RawImage scavenger;
    [SerializeField] private RawImage vampirism;
    [SerializeField] private RawImage criticalShots;
    [SerializeField] private RawImage explosiveBullets;

    [Header("Otros visuales")]
    [SerializeField] private TextMeshProUGUI textMaxAmmo;
    [SerializeField] private RawImage shield;
    public RawImage shieldTime;
    public TextMeshProUGUI shieldTimeText;
    private void Start()
    {
        perks = new RawImage[] { shielded, speed, haste, rebirth, scavenger, vampirism, criticalShots, explosiveBullets };
        powerUps = new RawImage[] { doublePoints, headShots, discount, wings };
        images = new RawImage[] { image1, image2, image3, image4, image5, image6 };
        playerHealth = GetComponent<PlayerHealth>();
        player = GetComponent<Player>();
        UnityEvents.onHealthChanged.AddListener(OnHealthChange);
        UnityEvents.shielded.AddListener(Shielded);
        UnityEvents.onPowerUpTriggered.AddListener(VisualPowerUps);
        UnityEvents.onPerkActive.AddListener(VisualPerks);
    }

    private void OnDeath()
    {
        if (player.activePerks[3])
        {
            playerHealth.GainHealth(6);
            player.activePerks[3] = false;
            for (int i = 0; i < images.Length; i++)
            {
                images[i].gameObject.SetActive(true);
            }
        }
        UnityEvents.onPerkActive.Invoke();
    }
    public void OnHealthChange(int health)
    {
        playerHealth.health = health;
        float healthPercent = health / 6f;
        if (health <= 0)
        {
            OnDeath();
            return;
        }

        for (int i = 0; i < images.Length; i++)
        {
            bool check = health > i;
            if (images[i] != null)
            {
                images[i].gameObject.SetActive(check);
                images[i].color = colorByHealth.Evaluate(healthPercent);
            }
        }    
    }

    public void Shielded(bool isShielded)
    {
        if (shield != null) shield.gameObject.SetActive(isShielded);
    }

    public void VisualPowerUps()
    {
        for (int i = 0; i < powerUps.Length; i++)
        {
            if (powerUps[i] == null) continue;
            PlayerBonus bonus = (PlayerBonus)i;
            powerUps[i].gameObject.SetActive(player.HasPowerUp(bonus));
        }
    }

    public void VisualPerks()
    {
        for (int i = 0; i < perks.Length; i++)
        {
            if(perks[i] == null) continue;
            PlayerPerks perk = (PlayerPerks)i;
            perks[i].gameObject.SetActive(player.activePerks[(int)perk]);
        }
    }

    public void VisualMaxAmmoText()
    {
        StartCoroutine(VisualMaxAmmo());
    }

    private IEnumerator VisualMaxAmmo()
    {
        textMaxAmmo.text = "+ 50";
        textMaxAmmo.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        textMaxAmmo.gameObject.SetActive(false);
    }
}
