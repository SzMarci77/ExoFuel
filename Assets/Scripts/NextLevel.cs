using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{

    public int currentLevel;
    public float levelTimeLimit = 140f;
    public bool hasBoss = false;


    private bool levelCompleted = false;
    private float timer; 

    void Start()
    {
        timer = 0f;

        if (!IsLevelUnlocked(currentLevel))
        {
            Debug.LogError("Ez a szint nincs feloldva!");
        }
    }

    void Update()
    {
        if (levelCompleted) return;

        if (!hasBoss)
        {
            timer += Time.deltaTime;
            if (timer >= levelTimeLimit)
            {
                CompleteLevel();
            }
        }
    }
    public void BossDefeated()
    {
        if (hasBoss && !levelCompleted)
        {
            CompleteLevel();
        }
    }
    void CompleteLevel()
    {
        levelCompleted = true;
        Debug.Log("Szint teljesítve: " + currentLevel);

        UnlockLevel(currentLevel + 1);
    }
    public void UnlockLevel(int level)
    {
        PlayerPrefs.SetInt($"Level_{level}_Unlock", 1);
        Debug.Log("Feloldva: Szint " + level);
    }
    public bool IsLevelUnlocked(int level)
    {
        return PlayerPrefs.GetInt($"Level_{level}_Unlock", 0) == 1;
    }
}
