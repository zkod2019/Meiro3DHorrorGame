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

            string questionsFirestoreUrl = "https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/questions?pageSize=100";
            var questionsFirestoreResponse = await client.GetAsync(questionsFirestoreUrl);
            var questionsFirestoreResponseString = await questionsFirestoreResponse.Content.ReadAsStringAsync();
            Debug.Log(questionsFirestoreResponseString);
            var questionsJson = (JArray)JObject.Parse(questionsFirestoreResponseString).GetValue("documents");
            Debug.Log(questionsJson);

            List<Tuple<string, string>> iqQuestionsRaw = new List<Tuple<string, string>>();
            List<Tuple<string, string>> mathQuestionsRaw = new List<Tuple<string, string>>();
            List<Tuple<string, string>> readingQuestionsRaw = new List<Tuple<string, string>>();
            List<Tuple<string, string>> puzzleQuestionsRaw = new List<Tuple<string, string>>();

            Debug.Log(questionsJson.Count);
            for (int i = 0; i < questionsJson.Count; i++)
            {
                var questionJsonFields = (JObject)(((JObject)questionsJson[i]).GetValue("fields"));
                var questionFileName = (string)(((JObject)questionJsonFields.GetValue("question")).GetValue("stringValue"));
                var answer = (string)(((JObject)questionJsonFields.GetValue("answer")).GetValue("stringValue"));
                if (questionFileName.StartsWith("iq"))
                {
                    iqQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
                else if (questionFileName.StartsWith("math"))
                {
                    mathQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
                else if (questionFileName.StartsWith("reading"))
                {
                    readingQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
                else if (questionFileName.StartsWith("puzzle"))
                {
                    puzzleQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
            }

            string firestoreUrl = $"https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/users?documentId={username}";

            var initialJson = $@"
                {{
                    ""fields"": {{
                        ""loop"": {{
                            ""integerValue"": 0
                        }},
                        ""iqQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(iqQuestionsRaw.ToArray(), _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }}, 
                        ""mathQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(mathQuestionsRaw.ToArray(), _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }},
                        ""readingQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(readingQuestionsRaw.ToArray(), _ => $@"{{""booleanValue"": false}}"))}]
                            }}
                        }},
                        ""puzzleQuestions"": {{
                            ""arrayValue"": {{
                                ""values"": [{String.Join(",", Array.ConvertAll(puzzleQuestionsRaw.ToArray(), _ => $@"{{""booleanValue"": false}}"))}]
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
            Debug.Log(firestoreResponseString);

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