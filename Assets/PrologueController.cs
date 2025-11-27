using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueController : MonoBehaviour
{
    public FadeText text1;
    public FadeText text2;

    private int step = 0;

    void Start()
    {
        text1.gameObject.SetActive(true);
        text2.gameObject.SetActive(true);

        text1.ShowText();
        text2.text.alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            step++;

            if (step == 1)
            {
                text1.HideText();
                text2.ShowText();
            }
            else if (step == 2)
            {
                text2.HideText();
                GameManager.Instance.NextMission();
            }
        }
    }
}
