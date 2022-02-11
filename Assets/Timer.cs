using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 0;
    public bool timerIsRunning = false;
    public Text timeText;
    public Text loopText;
    System.Random random = new System.Random();
    public static int loopCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        DisplayLoop();
        timeRemaining = 30; //random.Next(600,1800);
        timerIsRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
         if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                timerIsRunning = false;
                // Application.LoadLevel(Application.loadedLevel); this works too
                //DisplayLoop();
                SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DisplayLoop(){
        loopCount = loopCount + 1;
        loopText.text =  "Loop: " + loopCount.ToString();
    }
}
