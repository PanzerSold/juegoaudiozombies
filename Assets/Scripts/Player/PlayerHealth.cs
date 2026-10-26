using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    
    [SerializeField] private int healthMax = 6;
    [SerializeField] private float invulnTime = 0.75f;

    public int health = 6;
    private HealthBar healthBar;
    private Player player;
    private float invulnUntil = 0f;

    public bool IsInvulnerable => 0 < invulnUntil;

    private void Start()
    {
        health = healthMax;
        healthBar = GetComponent<HealthBar>();
        player = GetComponent<Player>();
    }

    public void GainHealth(int bonusHealth)
    {
        health += bonusHealth;
        if (health > healthMax) 
        {
            health = healthMax;
        }
        healthBar.OnHealthChange(health);
    }
    private void Update()
    {
        if (invulnUntil > 0) invulnUntil -= Time.deltaTime;
    }

    public void LoseHealth(int damage = 1)
    {
        if (IsInvulnerable) return;
        invulnUntil = invulnTime;

        if (player.activePerks[0] && player.shieldAvailable)
        {
            player.shieldAvailable = false;
            UnityEvents.shielded.Invoke(false);
            player.StartCoroutine(player.Shield());
            return;
        }

        health = Mathf.Max(0, health - damage);
        healthBar.OnHealthChange(health);
    }
}

