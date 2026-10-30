using UnityEngine;
using System.Collections.Generic;

public class UpgradesMachine : BaseMachine
{
    protected override void Start()
    {
        base.Start();
        fullString = "Buy Upgrade (" + PuntosRequeridos + ") (E)";
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
            player.AwardPoints(PuntosRequeridos);
            StartCoroutine(VisualPurchase("Every perk collected"));
            return;      
        }
        SFX_Comprar.Play();
    }

    private void GivePerk(List<int> missingPerks)
    {
        int randomIndex = Random.Range(0, missingPerks.Count - 1);
        int perkIndex = missingPerks[randomIndex];

        player.activePerks[perkIndex] = true;

        Player.PlayerPerks perk = (Player.PlayerPerks)perkIndex;

        if (perk == Player.PlayerPerks.Shielded) UnityEvents.shielded.Invoke(true);
        StartCoroutine(VisualPurchase("You got: " + perk));
        UnityEvents.onPerkActive.Invoke();
    }
}