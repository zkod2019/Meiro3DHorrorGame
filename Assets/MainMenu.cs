using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;


public class MainMenu : MonoBehaviour
{
    public Button signUpBtn;
    public Button loginBtn;
    public GameObject signUpUser;
    public GameObject signUpPassword;
    public GameObject loginUser;
    public GameObject loginPassword;

    public GameObject signUpErrMsg;
    public GameObject loginErrMsg;

    public GameObject loginMenuCanvas;

    public string API_KEY;

    private static readonly HttpClient client = new HttpClient();

    public void PlayGame()
    {
        if (Auth.idToken != null) {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
        } else {
             loginMenuCanvas.SetActive(true);
             this.gameObject.SetActive(false);
        }
        
    }

    public async void QuitGame()
    {
        Debug.Log("Game Quit.");
        if (Auth.idToken != null)
        await GameObject.Find("FirstPersonPlayer").GetComponent<Timer>().DisplayLoop();
        Application.Quit();
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void Start()
    {
        API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        Button btnSign = signUpBtn.GetComponent<Button>();
        btnSign.onClick.AddListener(SignUp);

        Button btnLog = loginBtn.GetComponent<Button>();
        btnLog.onClick.AddListener(Login);

        signUpErrMsg.SetActive(false);
    }


    public async void SignUp()
    {
        signUpErrMsg.SetActive(false);

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}";

        /*
            var n = 42;
            var x = "hello " + n.toString();
            var xButBetter = $"hello {n}";

            // in js
            var xButBetter = `hello ${n}`

            // in python
            xButButter = f'hello {n}'
        */
        string username = signUpUser.GetComponent<TMP_InputField>().text.ToLower();
        string password = signUpPassword.GetComponent<TMP_InputField>().text.ToLower();
        Debug.Log(username);

        var requestData = new Dictionary<string, string>
        {
            { "email", $"{username}@player.com" },
            { "password", password },
            { "returnSecureToken", "true" }
        };

        var content = new FormUrlEncodedContent(requestData);
        var response = await client.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseJson = JObject.Parse(responseString);
        Debug.Log(responseJson);
        Debug.Log(response.StatusCode);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Auth.username = username;
            Auth.idToken = (String)responseJson.GetValue("idToken");

            string firestoreUrl = $"https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/users?documentId={username}";

            await HintController.InitializeQuestions();
            var initialJson = $@"
                {{
                    ""fields"": {{
                        ""loop"": {{
                            ""integerValue"": 0
                        }},
                        ""iqQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(HintController.iqQuestions, _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }}, 
                        ""mathQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(HintController.mathQuestions, _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }},
                        ""readingQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(HintController.readingQuestions, _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }},
                        ""puzzleQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(HintController.puzzleQuestions, _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }}
                    }}
                }}";

            var firestoreSetReq = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(firestoreUrl),
                Headers = {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {Auth.idToken}" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                },
                Content = new StringContent(initialJson)
            };

            var firestoreResponse = await client.SendAsync(firestoreSetReq);
            var firestoreResponseString = await firestoreResponse.Content.ReadAsStringAsync();

            if (firestoreResponse.StatusCode == HttpStatusCode.OK)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
        else
        {
            var errorMessage = ((JObject)responseJson.GetValue("error")).GetValue("message").ToObject<string>();

            Debug.Log(errorMessage);
            signUpErrMsg.SetActive(true);
            signUpErrMsg.GetComponent<Text>().text = errorMessage;
        }
    }

    public async void Login()
    {
        loginErrMsg.SetActive(false);

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={API_KEY}";
        string username = loginUser.GetComponent<TMP_InputField>().text.ToLower();
        string password = loginPassword.GetComponent<TMP_InputField>().text.ToLower();
        Debug.Log(username);

        var requestData = new Dictionary<string, string>
        {
            { "email", $"{username}@player.com" },
            { "password", password },
            { "returnSecureToken", "true" }
        };

        var content = new FormUrlEncodedContent(requestData);
        var response = await client.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        var responseJson = JObject.Parse(responseString);
        Debug.Log(responseJson);
        Debug.Log(response.StatusCode);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Auth.username = username;
            Auth.idToken = (String)responseJson.GetValue("idToken");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            var errorMessage = ((JObject)responseJson.GetValue("error")).GetValue("message").ToObject<string>();

            Debug.Log(errorMessage);
            loginErrMsg.SetActive(true);
            loginErrMsg.GetComponent<Text>().text = errorMessage;
        }
    }
}