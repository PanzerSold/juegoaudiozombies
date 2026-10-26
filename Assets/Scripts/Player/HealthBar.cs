using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private Player player;
    [SerializeField] private RawImage image1;
    [SerializeField] private RawImage image2;
    [SerializeField] private RawImage image3;
    [SerializeField] private RawImage image4;
    [SerializeField] private RawImage image5;
    [SerializeField] private RawImage image6;
    private RawImage[] images;
    [SerializeField] private Gradient colorByHealth;
    [SerializeField] private RawImage shield;
    private void Start()
    {
        images = new RawImage[] { image1, image2, image3, image4, image5, image6 };
        playerHealth = GetComponent<PlayerHealth>();
        player = GetComponent<Player>();
        UnityEvents.onHealthChanged.AddListener(OnHealthChange);
        UnityEvents.shielded.AddListener(Shielded);
    }

    private void CasoMuerte()
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
    }
    public void OnHealthChange(int health)
    {
        playerHealth.health = health;
        float healthPercent = health / 6f;
        if (health <= 0)
        {
            CasoMuerte();
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
        if (shield != null)
        {
            shield.gameObject.SetActive(isShielded);
        }
    }
}
