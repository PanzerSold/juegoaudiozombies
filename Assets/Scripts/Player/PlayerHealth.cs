using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    
    [SerializeField] private int healthMax = 6;
    [SerializeField] private float invulnTime = 0.75f;

    public int health = 6;
    private HealthBar healthBar;
    private float invulnUntil = 0f;

    public bool IsInvulnerable => Time.time < invulnUntil;

    private void Start()
    {
        health = healthMax;
        healthBar = GetComponent<HealthBar>();
    }

    /* private void Update()
    {
        //testeo de los metodos y ver si el hud cambia
       
        if (Input.GetKeyDown(KeyCode.Z))
        {
            loseHealth(1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            gainHealth(1);
        }
        
    } */
    public void GainHealth(int bonusHealth)
    {
        health += bonusHealth;
        if (health > healthMax) 
        {
            health = healthMax;
        }
        healthBar.OnHealthChange(health);
    }
    public void LoseHealth(int damage)
    {
        if (IsInvulnerable) return;

        invulnUntil = Time.time + invulnTime;
        health = Mathf.Max(0, health - damage);
        healthBar.OnHealthChange(health);
    }
}
