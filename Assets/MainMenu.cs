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

    public string API_KEY;

    private static readonly HttpClient client = new HttpClient();

    public void PlayGame(){
        //SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
    }

    public void QuitGame(){
        Debug.Log("Game Quit.");
        Application.Quit();
    }

    public void ReturnMenu(){
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex - 1);
    }

    void Start() {
        API_KEY = Environment.GetEnvironmentVariable("FIREBASE_API_KEY");
        Button btnSign = signUpBtn.GetComponent<Button>();
		btnSign.onClick.AddListener(SignUp);

        Button btnLog = loginBtn.GetComponent<Button>();
		btnLog.onClick.AddListener(Login);

        signUpErrMsg.SetActive(false);
    }

   
    public async void SignUp(){
        signUpErrMsg.SetActive(false);

        string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}";
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

        if (response.StatusCode == HttpStatusCode.OK) {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
        } else {
            var errorMessage = ((JObject)responseJson.GetValue("error")).GetValue("message").ToObject<string>();

            Debug.Log(errorMessage);
            signUpErrMsg.SetActive(true);
            signUpErrMsg.GetComponent<Text>().text = errorMessage;
        }
    }

    public async void Login(){
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

        if (response.StatusCode == HttpStatusCode.OK) {
            SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
        } else {
            var errorMessage = ((JObject)responseJson.GetValue("error")).GetValue("message").ToObject<string>();

            Debug.Log(errorMessage);
            loginErrMsg.SetActive(true);
            loginErrMsg.GetComponent<Text>().text = errorMessage;
        }

    }



}
