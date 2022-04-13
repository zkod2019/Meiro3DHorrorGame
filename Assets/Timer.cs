using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Net.Http;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 0;
    public bool timerIsRunning = false;
    public Text timeText;
    public Text loopText;
    System.Random random = new System.Random();
    public static int loopCount = 0;
    public GameObject cam1;
    public GameObject cam2;

    private static readonly HttpClient client = new HttpClient();


    // Start is called before the first frame update
    async void Start()
    {
        cam1.SetActive(true);
        cam2.SetActive(false);
        timeRemaining = 60; //random.Next(600,1800);
        timerIsRunning = true;
        string firestoreUrl = $"https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/users/{Auth.username}";

        var firestoreResponse = await client.GetAsync(firestoreUrl);
        var firestoreResponseString = await firestoreResponse.Content.ReadAsStringAsync();
        var json = JObject.Parse(firestoreResponseString);
        Debug.Log(json);


        var fields = (JObject)json.GetValue("fields");
        Debug.Log(fields);


        loopCount = (int)((JObject)fields.GetValue("loop")).GetValue("integerValue");
        loopText.text =  "Loop: " + loopCount.ToString();

        Debug.Log("Timer iniitliazed!!");
        Debug.Log(loopText);
    }

    // Update is called once per frame
    async void Update()
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
                cam2.SetActive(true);
                cam1.SetActive(false);
                Invoke("DeactivateCam2", 10);
                await DisplayLoop();
                SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
            }
        }
    }

    void DeactivateCam2() {
        cam1.SetActive(true);
        cam2.SetActive(false);
    }


    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public async Task DisplayLoop(){
        loopCount = loopCount + 1;
        
        string firestoreUrl = $"https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/users/{Auth.username}";
        Debug.Log(firestoreUrl);

        var json = $@"
            {{
                ""fields"": {{
                    ""loop"": {{
                        ""integerValue"": {Timer.loopCount}
                    }},
                    ""iqQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.iqQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }}, 
                    ""mathQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.mathQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }},
                    ""readingQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.readingQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }},
                    ""puzzleQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.puzzleQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }}
                }}
            }}";

        var firestoreSetReq = new HttpRequestMessage
        {
            Method = new HttpMethod("PATCH"),
            RequestUri = new Uri(firestoreUrl),
            Headers = {
                { HttpRequestHeader.Authorization.ToString(), $"Bearer {Auth.idToken}" },
                { HttpRequestHeader.Accept.ToString(), "application/json" },
            },
            Content = new StringContent(json)
        };

        var firestoreResponse = await client.SendAsync(firestoreSetReq);
        var firestoreResponseString = await firestoreResponse.Content.ReadAsStringAsync();
        Debug.Log(json);
        Debug.Log(firestoreResponseString);

        loopText.text =  "Loop: " + loopCount.ToString();
    }

}
