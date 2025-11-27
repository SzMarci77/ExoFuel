using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLoadOn : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("CurrentMission"))
        {
            if(PlayerPrefs.GetInt("CurrentMission") > 0)
            {
                var continueTxt = GameObject.Find("ContinueText");
                
                ColorUtility.TryParseHtmlString("#D1F8FF", out Color lightColor);
                continueTxt.GetComponent<Text>().color = lightColor;                
            }
        }

        float volume = 1f;
        if (PlayerPrefs.HasKey("Volume")) volume = PlayerPrefs.GetFloat("Volume");
        gameObject.GetComponent<AudioSource>().volume = volume;

        List<HighscoreElement> highscoreList = new List<HighscoreElement>();
        highscoreList = FileHandler.ReadListFromJSON<HighscoreElement>("scores.json");
        if (highscoreList.Count > 0) PlayerPrefs.SetInt("HighScore", highscoreList[0].points);
    }
}
