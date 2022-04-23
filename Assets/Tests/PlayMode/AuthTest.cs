using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AuthTest
{
    [SetUp]
    public void Setup() {
        Debug.Log("hello");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        Assert.That(Auth.idToken, Is.Null);
    }

    public void GoToLogin() {
        var button = GameObject.Find("PlayButton").GetComponent<Button>();
        button.onClick.Invoke();
    }

    public void GoToSignUp() {
        var button = GameObject.Find("PlayButton").GetComponent<Button>();
        button.onClick.Invoke();
        var signupButton = GameObject.Find("SignUpButton").GetComponent<Button>();
        signupButton.onClick.Invoke();
    }

    public void TestLogin(string username, string password) {

        var unameInput = GameObject.Find("Username_Input").GetComponent<TMP_InputField>();
        var pswdInput = GameObject.Find("Password_Input").GetComponent<TMP_InputField>();
        var loginButton = GameObject.Find("LoginButton").GetComponent<Button>();

        Assert.That(unameInput, Is.Not.Null);
        Assert.That(pswdInput, Is.Not.Null);

        unameInput.text = username;
        pswdInput.text = password;

        loginButton.onClick.Invoke();
    }

     public void TestSignUp(string username, string password) {

        var unameInput = GameObject.Find("Username_SignUp_Input").GetComponent<TMP_InputField>();
        var pswdInput = GameObject.Find("Password_SignUp_Input").GetComponent<TMP_InputField>();

        var signUpButtonObjs = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "SignUpButton");
        var signupButton = signUpButtonObjs.ElementAt(1).GetComponent<Button>();

        Assert.That(unameInput, Is.Not.Null);
        Assert.That(pswdInput, Is.Not.Null);
        Assert.That(signupButton, Is.Not.Null);

        unameInput.text = username;
        pswdInput.text = password;

        Debug.Log("didn't fail here");

        signupButton.onClick.Invoke();
    }
    
    [UnityTest]
    public IEnumerator LoginInvalidPassword()
    {
        GoToLogin();
        TestLogin("player23", "player");
        yield return new WaitForSeconds(2);
        Assert.That(Auth.idToken, Is.Null);
    }

    [UnityTest]
    public IEnumerator LoginInvalidUsername()
    {
        GoToLogin();
        TestLogin("player5050", "player23");
        yield return new WaitForSeconds(2);
        Assert.That(Auth.idToken, Is.Null);
    }

    [UnityTest]
    public IEnumerator SignUpInvalidPassword()
    {
        GoToSignUp();
        TestSignUp("player23", "pl");
        yield return new WaitForSeconds(2);
        Assert.That(Auth.idToken, Is.Null);
    }

    [UnityTest]
    public IEnumerator SignUpInvalidUsername()
    {
        GoToSignUp();
        TestSignUp("player23", "player23");
        yield return new WaitForSeconds(2);
        Assert.That(Auth.idToken, Is.Null);
    }

    [UnityTest]
    public IEnumerator LoginSuccessful()
    {
        GoToLogin();
        TestLogin("player23", "player23");
        yield return new WaitForSeconds(2);
        Assert.That(Auth.idToken, Is.Not.Null);
    }
    
    [UnityTest]
    public IEnumerator SignUpSuccessful()
    {
        GoToSignUp();
        string uniqueId = System.Guid.NewGuid().ToString();
        TestSignUp($"testplayer-{uniqueId}", $"testplayer-{uniqueId}");
        yield return new WaitForSeconds(2);
        Assert.That(Auth.idToken, Is.Not.Null);
    }

}
