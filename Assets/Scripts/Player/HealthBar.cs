using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    [SerializeField] private RawImage image1;
    [SerializeField] private RawImage image2;
    [SerializeField] private RawImage image3;
    [SerializeField] private RawImage image4;
    [SerializeField] private RawImage image5;
    [SerializeField] private RawImage image6;
    private RawImage[] images;
    [SerializeField] private Gradient colorByHealth;

    private void Start()
    {
        images = new RawImage[] { image1, image2, image3, image4, image5, image6 };
        playerHealth = GetComponent<PlayerHealth>();
        UnityEvents.onHealthChanged.AddListener(OnHealthChange);
    }

    private void CasoMuerte()
    {
        
    }
    public void OnHealthChange(int health)
    {
        playerHealth.health = health;
        float healthPercent = health / 6f;

        for (int i = 0; i < images.Length; i++)
        {
            if (health == 0) CasoMuerte();

            bool check = health > i;
            if (images[i] != null)
            {
                images[i].gameObject.SetActive(check);
                images[i].color = colorByHealth.Evaluate(healthPercent);
            }
        }    
    }
}
