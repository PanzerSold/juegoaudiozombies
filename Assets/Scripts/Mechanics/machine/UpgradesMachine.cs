using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class UpgradesMachine : BaseMachine
{
    [SerializeField] private TextMeshProUGUI perkText;
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
            player.AwardPoints(PuntosRequeridos);
            StartCoroutine(OutOfPerks());
        }
    }

    private void GivePerk(List<int> missingPerks)
    {
        int randomIndex = Random.Range(0, missingPerks.Count - 1);
        int perkIndex = missingPerks[randomIndex];

        player.activePerks[perkIndex] = true;

        Player.PlayerPerks perk = (Player.PlayerPerks)perkIndex;

        if (perk == Player.PlayerPerks.Shielded) UnityEvents.shielded.Invoke(true);
        StartCoroutine(PerkText(perk.ToString()));
    }

    private IEnumerator OutOfPerks()
    {
        perkText.gameObject.SetActive(true);
        perkText.text = "Ya tenés todas las mejoras";
        yield return new WaitForSeconds(2f);
        perkText.gameObject.SetActive(false);
    }

    private IEnumerator PerkText(string perk)
    {
        perkText.gameObject.SetActive(true);
        perkText.text = "Obtuviste: " + perk;
        yield return new WaitForSeconds(2f);
        perkText.gameObject.SetActive(false);
    }
}