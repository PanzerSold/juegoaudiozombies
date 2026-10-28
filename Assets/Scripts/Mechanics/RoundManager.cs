using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [SerializeField] AudioSource RoundPassSFX;
    [SerializeField] GameObject ZombieSpawnerMain;
    [SerializeField] private TextMeshProUGUI roundText;

    private bool restartingRound = false;
    private List<SpawnerZombie> SZSpawners = new List<SpawnerZombie>();
    private List<ZombieBase> zombieList = new List<ZombieBase>();
    private int zombiesForCurrentRound = 4;
    private int currentRound = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in ZombieSpawnerMain.transform)
        {
            SZSpawners.Add(child.GetComponent<SpawnerZombie>());
        }
    }
    public void AddZombie(SpawnerZombie sz, Transform spawnTransform, Vector3 nearestBarricadePos, GameObject zombieObj)
    {
        if (zombieList.Count < 24 && zombiesForCurrentRound > 0)
        {
            GameObject newZombie = Instantiate(zombieObj, spawnTransform.position, spawnTransform.rotation);
            ZombieBase newZombieScript = newZombie.GetComponent<ZombieBase>();
            newZombieScript.goTo(nearestBarricadePos);
            zombieList.Add(newZombieScript);
            zombiesForCurrentRound--;
            sz.zombieSpawnTimer = 0;   
        }
    }

    public async void RestartRound()
    {
        if (restartingRound) return;
        restartingRound = true;
        RoundPassSFX.Play();
        await Awaitable.WaitForSecondsAsync(1f);
        currentRound++;
        roundText.text = "Round " + currentRound;
        await Awaitable.WaitForSecondsAsync(3f);
        if (this == null) return;
        zombiesForCurrentRound = currentRound*4;
        foreach (SpawnerZombie sz in SZSpawners)
        {
            sz.SetSpawnTimer();
        }
        restartingRound = false;
    }
    public int SetHealthBasedOnRound()
    {
        return 8 + 2 * currentRound + currentRound - 1;
    }
    public void RemoveZombie(ZombieBase zombie)
    {
        zombieList.Remove(zombie);
        if (zombieList.Count == 0 && zombiesForCurrentRound == 0)
        {
            RestartRound();
        }
    }
    
}
