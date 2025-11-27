using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private Text scoreIndicator;
    [SerializeField] private Text scoreText;

    [SerializeField] private Text highScoreIndicator;
    [SerializeField] private Text highScoreText;

    [SerializeField] private Text livesText;

    [SerializeField] private GameObject infoUI;


    [SerializeField] private GameObject pauseUI;


    [SerializeField] private GameObject endUI;


    [SerializeField] private Image progressBarFill;


    private float elapsedTime = 0f;
    private float missionTime;
    private int flashCount = 0;
    private bool isFlashing = false;
    private int highScore = 0;
    private int maxHealth;
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
        {
            Instance = null;
        }
    }

    private void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        missionTime = GameObject.Find("GameManager").GetComponent<GameManager>().missionTime;

        maxHealth = player.health;
        if(livesText != null) livesText.text = maxHealth.ToString();
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        PlayerPrefs.SetInt("CurrentMission", sceneIndex);
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
            highScoreIndicator.text = highScore.ToString().PadLeft(4, '0');
        }
        if (PlayerPrefs.HasKey("Score"))
        {
            int score = PlayerPrefs.GetInt("Score");
            scoreIndicator.text = score.ToString().PadLeft(4, '0');
        }

        InvokeRepeating("UpdateProgressBar", 0f, 0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetInt("HighScore", 0);
            highScoreIndicator.text = "".PadLeft(4, '0');
        }
    }
    private void UpdateProgressBar()
    {
        if (progressBarFill == null) return;

        elapsedTime += 0.5f;
        float progress = Mathf.Clamp01(elapsedTime / missionTime);

        progressBarFill.fillAmount = progress;
    }

    public void StopGameUI()
    {
        CancelInvoke("UpdateProgressBar");
        infoUI.SetActive(false);
        endUI.SetActive(true);
    }

    public void HandleGameOverUI(int score)
    {
        GameObject.Find("levelText").GetComponent<Text>().text = "Level " + sceneIndex + " failed!";
        GameObject.Find("scoresText").GetComponent<Text>().text = "Scores: " + score;
        pauseUI.SetActive(false);
    }

    public void UpdateScoreUI(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            if (highScoreIndicator != null) highScoreIndicator.text = highScore.ToString().PadLeft(4, '0');
            NewRecord();
        }
        if (scoreIndicator != null) scoreIndicator.text = score.ToString().PadLeft(4, '0');
    }
    private void NewRecord()
    {
        if (!isFlashing)
        {
            StartCoroutine(BlinkHighScoreText());
        }
    }

    private IEnumerator BlinkHighScoreText()
    {
        isFlashing = true;

        while (flashCount < 3)
        {
            ColorUtility.TryParseHtmlString("#0A940F", out Color highlightColor);
            highScoreText.color = highlightColor;
            highScoreText.text = "New Record";

            yield return new WaitForSeconds(1);

            ColorUtility.TryParseHtmlString("#C57C04", out Color normalColor);
            highScoreText.color = normalColor;
            highScoreText.text = "High Score";

            yield return new WaitForSeconds(1);

            flashCount++;
        }

        flashCount = 0;
        isFlashing = false;
    }

    public void UpdatePlayerHealthUI(int health)
    {
        if (livesText != null) livesText.text = health.ToString();
    }

    public void HandleEndOfMissionUI(bool isLastMission, int score)
    {
        if (isLastMission)
        {
            GameObject.Find("NextButton").SetActive(false);
            GameObject.Find("levelText").GetComponent<Text>().text = "You win the game!";
            GameObject.Find("scoresText").GetComponent<Text>().text = "Total score: " + score;
        }
        else
        {
        GameObject.Find("levelText").GetComponent<Text>().text = "Level " + sceneIndex + " completed!";
        GameObject.Find("scoresText").GetComponent<Text>().text = "Scores: " + score;
        }
    }
}
