using UnityEngine;
using System.Collections.Generic;

public class UpgradesMachine : BaseMachine
{
    protected override void Start()
    {
        base.Start();
        fullString = "Comprar Mejora " + PuntosRequeridos + " (E)";
    }

    protected override void Buy()
    {
        List<int> missingPerks = new List<int>();

        for (int i = 0; i < player.activePerks.Length; i++)
        {
            if (!player.activePerks[i])
            {
                missingPerks.Add(i);
            }
        }

        if (missingPerks.Count > 0)
        {
            GivePerk(missingPerks);
        }
        else
        {
            Debug.Log("Ya tenés todas las mejoras.");
        }
    }

    private void GivePerk(List<int> missingPerks)
    {
        int randomIndex = Random.Range(0, missingPerks.Count);
        int perkIndex = missingPerks[randomIndex];

        player.activePerks[perkIndex] = true;

        Player.PlayerPerks perk = (Player.PlayerPerks)perkIndex;
        Debug.Log("Obtuviste: " + perk.ToString());
    }
}