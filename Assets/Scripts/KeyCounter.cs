using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyCounter : MonoBehaviour
{
    public UnityEvent OnFirstPress;
    public UnityEvent OnSecondPress;

    private int pressCount = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pressCount++;

            switch (pressCount)
            {
                case 1:
                    OnFirstPress?.Invoke();
                    break;

                case 2:
                    OnSecondPress?.Invoke();
                    break;
            }
        }
    }
}
