using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    
    [SerializeField] private int healthMax = 6;

    public int health = 6;
    private HealthBar healthBar;

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
        health -= damage;

        if (health < 0)
        {
            health = 0;
        }
        healthBar.OnHealthChange(health);
    }
}
