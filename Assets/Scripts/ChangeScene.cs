using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ChangeScene : MonoBehaviour
{
    [Header("UI Stuff")]
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject NameMenu;
    [SerializeField] private GameObject LeaderBoard;
    [SerializeField] private GameObject elementWrapper;
    [SerializeField] private GameObject HighScoreElementPrefab;

    List<GameObject> uiElements = new List<GameObject>();
    List<HighscoreElement> highscoreList = new List<HighscoreElement>();

    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("CurrentMission"))
        {
            int index = PlayerPrefs.GetInt("CurrentMission");
            if (index > 0) SceneManager.LoadScene(index);
        }
    }
    public void ShowLeaderboard()
    {

        for (int i = 0; i < highscoreList.Count; i++)
        {
            HighscoreElement el = highscoreList[i];

            if (el != null && el.points > 0)
            {
                if (i >= uiElements.Count)
                {
                    var inst = Instantiate(HighScoreElementPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(elementWrapper.transform, false);

                    uiElements.Add(inst);
                }

                var texts = uiElements[i].GetComponentsInChildren<Text>();
                texts[0].text = el.playerName;
                texts[1].text = el.points.ToString();
            }
        }
        LeaderBoard.SetActive(true);
    }

    private void Start()
    {
        highscoreList = FileHandler.ReadListFromJSON<HighscoreElement>("scores.json");
        if(highscoreList.Count > 0) PlayerPrefs.SetInt("HighScore", highscoreList[0].points);
        else PlayerPrefs.SetInt("HighScore", 0);
    }

    public void HideLeaderboard()
    {
        LeaderBoard.SetActive(false);
    }
    public void NewGame()
    {
        MainMenu.SetActive(false);
        NameMenu.SetActive(true);
    }
    public void SubmitName()
    {
        var inputField = GameObject.Find("NameInputField");
        var name = inputField.GetComponent<InputField>().text;
        
        if (name == null || name == "") return;

        PlayerPrefs.SetString("Name", name);
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Level", 1);

        SceneManager.LoadScene(1);
    }
    public void Cancel()
    {
        var inputField = GameObject.Find("NameInputField");
        inputField.GetComponent<InputField>().text = "";
        MainMenu.SetActive(true);
        NameMenu.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
}