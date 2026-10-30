using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Player.PlayerBonus pu;

    private void Start()
    {
        Destroy(gameObject, 7f);
    }
}
