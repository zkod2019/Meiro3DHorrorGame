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

public class SystemTest : MonoBehaviour
{
    [SetUp]
    public void Setup() {
        Debug.Log("hello");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        Assert.That(Auth.idToken, Is.Null);
    }

    public void GoToSignUp() {
        var button = GameObject.Find("PlayButton").GetComponent<Button>();
        button.onClick.Invoke();
        var signupButton = GameObject.Find("SignUpButton").GetComponent<Button>();
        signupButton.onClick.Invoke();
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
    public IEnumerator GameWonSucess()
    {
        GoToSignUp();
        string uniqueId = System.Guid.NewGuid().ToString();
        TestSignUp($"testplayer-{uniqueId}", $"testplayer-{uniqueId}");
        yield return new WaitForSeconds(40);
        Assert.That(Auth.idToken, Is.Not.Null);
        
        var player = GameObject.Find("FirstPersonPlayer");
        var weapons = GameObject.FindGameObjectsWithTag("weapon");
        foreach (var weapon in weapons) {
            var collectable = weapon.GetComponent<Collectable>();
            collectable.OnTriggerEnter(player.GetComponent<BoxCollider>());
            yield return null;
        }
        yield return new WaitForSeconds(2);

        Assert.AreEqual(GameObject.Find("FirstPersonPlayer").GetComponent<PlayerInventory>().inventory.Count, 8);
        
        player.GetComponent<PlayerInventory>().OnTriggerEnter(GameObject.Find("GroundExit").GetComponent<BoxCollider>());
        yield return new WaitForSeconds(2);

        var gameWonScreen = GameObject.Find("GameWon");
        Assert.That(gameWonScreen, Is.Not.Null);
    }
}
