using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Xml.Linq;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private bool hasEndBoss;
    [SerializeField] private bool isLastMission = false;

    public float missionTime;

    public GameObject invaderExplosion;
    public GameObject playerExplosion;

    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private float gameOverVolume = 1.0f;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private float enemyDeathVolume = 1.0f;

    [SerializeField] private Upgrade upgradePrefab;
    [SerializeField] private int upgradeDropRate;
    [SerializeField] private Repair repairkitPrefab;
    [SerializeField] private int repairkitDropRate;
    [SerializeField] private int maxCount = 7;

    private Player player;
    private UiManager uiManager;   // now optional
    private int maxHealth;
    private string playerName;
    public int score { get; private set; } = 0;
    private int sceneIndex;

    public void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        uiManager = FindObjectOfType<UiManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("UiManager not found. Skipping UI updates.");
        }

        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene. Player-specific features will be disabled.");
            maxHealth = 0;
        }
        else
        {
            maxHealth = player.health;
        }

        if (PlayerPrefs.HasKey("Score"))
            score = PlayerPrefs.GetInt("Score");

        if (PlayerPrefs.HasKey("Name"))
            playerName = PlayerPrefs.GetString("Name");

        InvokeRepeating("SpawnRepairKit", 0f, 1f);

        if (!hasEndBoss)
            StartCoroutine(MissionTimeCountdown());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameOver();
        }
    }

    private IEnumerator MissionTimeCountdown()
    {
        yield return new WaitForSeconds(missionTime);

        if (!this.hasEndBoss)
        {
            EndOfMission();
        }
    }

    private void SpawnRepairKit()
    {
        int spawnRepairkit = Random.Range(0, repairkitDropRate);
        if (spawnRepairkit == 0)
        {
            if (GameObject.FindGameObjectsWithTag("RepairKit").Length < 1)
            {
                Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
                Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
                Vector3 upperEdge = Camera.main.ViewportToWorldPoint(Vector3.up);
                Vector3 where = new Vector3(Random.Range(leftEdge.x + 2, rightEdge.x - 2), upperEdge.y);
                Instantiate(repairkitPrefab, where, Quaternion.identity);
            }
        }
    }

    public void RestartMission()
    {
        HandleHighScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void StopGame()
    {
        foreach (var backg in GameObject.FindGameObjectsWithTag("Background"))
            backg.gameObject.GetComponent<BackgroundScroll>().beginScroll = false;

        foreach (var invader in GameObject.FindGameObjectsWithTag("Invader"))
            invader.gameObject.SetActive(false);

        foreach (var boss in GameObject.FindGameObjectsWithTag("Boss"))
            boss.gameObject.SetActive(false);

        foreach (var spawnp in GameObject.FindGameObjectsWithTag("SpawnPoints"))
            spawnp.GetComponent<SpawnPoint>().turnOff();

        uiManager?.StopGameUI();

        CancelInvoke("SpawnRepairKit");
        player.gameObject.SetActive(false);
    }

    private void GameOver()
    {
        StopGame();

        uiManager?.HandleGameOverUI(score);

        var nextbtn = GameObject.Find("NextButton");
        if (nextbtn) nextbtn.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        uiManager?.UpdateScoreUI(score);
    }

    public void OnPlayerKilled()
    {
        int health = Mathf.Max(player.health - 1, 0);
        player.health = health;
        uiManager?.UpdatePlayerHealthUI(health);

        if (health > 0)
        {
            player.beUnkillable(1.0f);
            player.ActivateShieldBubble();
        }
        else
        {
            if (playerExplosion != null)
                Instantiate(playerExplosion, player.transform.position, Quaternion.identity);

            OnGameOverSounds();
            GameOver();
        }
    }

    public void HealPlayer()
    {
        if (player.health < maxHealth)
        {
            player.health++;
            uiManager?.UpdatePlayerHealthUI(player.health);
        }
    }

    public void UpgradeWeapons()
    {
        int currentWpnIndex = player.currentTemplate;

        if (currentWpnIndex < player.upgradeTemplates.Count - 1)
        {
            player.speed += 1;
            currentWpnIndex++;

            foreach (var gun in player.guns)
                gun.gameObject.SetActive(false);

            player.guns.Clear();

            player.currentTemplate = currentWpnIndex;

            foreach (var gun in player.upgradeTemplates[currentWpnIndex].guns)
            {
                Gun newGun = new GameObject(gun.name).AddComponent<Gun>();
                newGun.timeBetweenShoots = gun.timeBetweenShoots;
                newGun.projectilePrefab = gun.projectilePrefab;
                newGun.missileSpeed = gun.missileSpeed;
                newGun.pewPewSFX = gun.pewPewSFX;
                newGun.pewpewVolume = gun.pewpewVolume;
                newGun.layerIndex = LayerMask.NameToLayer("PlayerMissile");
                newGun.transform.SetParent(player.transform, false);
                newGun.transform.localPosition = gun.transform.localPosition;
                newGun.transform.localRotation = gun.transform.localRotation;
                player.guns.Add(newGun);
            }
        }
    }

    public void OnInvaderKilled(Invader invader)
    {
        invader.health = Mathf.Max(invader.health - 1, 0);
        GameObject explosion = null;

        if (invader.health <= 0)
        {
            if (invaderExplosion != null)
                explosion = Instantiate(invaderExplosion, invader.transform.position, Quaternion.identity);

            if (enemyDeathSound != null)
            {
                AudioSource aud = GameObject.Find("SFXPlayer").GetComponent<AudioSource>();
                aud.PlayOneShot(enemyDeathSound, enemyDeathVolume);
            }

            int spawnUpgrade = Random.Range(0, upgradeDropRate);
            if (spawnUpgrade == 0 && GameObject.FindGameObjectsWithTag("Upgrade").Length < 1)
                if (upgradePrefab != null)
                    Instantiate(upgradePrefab, invader.transform.position, Quaternion.identity);

            Destroy(invader.gameObject);

            SetScore(score + invader.score);
            if (explosion) Destroy(explosion, 3);

            if (invader.gameObject.tag == "Boss")
                EndOfMission();
        }
    }

    public void EndOfMission()
    {
        StopGame();
        PlayerPrefs.SetInt("Score", score);

        if (isLastMission)
        {
            LoadEndScene();
            PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);
            uiManager?.HandleEndOfMissionUI(true, score);
        }
        else
        {
            PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex + 1);
            uiManager?.HandleEndOfMissionUI(false, score);
        }
    }

    public void ExitMission()
    {
        HandleHighScore();
        PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

    public void NextMission()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        HandleHighScore();

        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings && !isLastMission)
        {
            currentSceneIndex += 1;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    private void HandleHighScore()
    {
        HighscoreElement element = new HighscoreElement(playerName, score);
        List<HighscoreElement> highscoreList = FileHandler.ReadListFromJSON<HighscoreElement>("scores.json");

        while (highscoreList.Count > maxCount)
            highscoreList.RemoveAt(maxCount);

        for (int i = 0; i < maxCount; i++)
        {
            if (i >= highscoreList.Count || element.points > highscoreList[i].points)
            {
                highscoreList.Insert(i, element);

                while (highscoreList.Count > maxCount)
                    highscoreList.RemoveAt(maxCount);

                FileHandler.SaveToJSON<HighscoreElement>(highscoreList, "scores.json");
                break;
            }
        }
    }

    public void OnGameOverSounds()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in allAudioSources)
            if (audioSource.clip != gameOverSound)
                audioSource.Stop();

        AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position, gameOverVolume);
    }

    private void LoadEndScene()
    {
        SceneManager.LoadScene("EndCredit");
    }
}
