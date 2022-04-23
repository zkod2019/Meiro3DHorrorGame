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

public class Integration
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

        signupButton.onClick.Invoke();
    }

    [UnityTest]
    public IEnumerator HintObjectCollection()
    {
        GoToSignUp();
        string uniqueId = System.Guid.NewGuid().ToString();
        TestSignUp($"testplayer-{uniqueId}", $"testplayer-{uniqueId}");
        yield return new WaitForSeconds(40);
        Assert.That(Auth.idToken, Is.Not.Null);
        
        var barrel = GameObject.Find("Barrel_Math_1");
        GameObject.Find("FirstPersonPlayer").transform.position = barrel.transform.position;
        Assert.That(barrel, Is.Not.Null);
        yield return null;
        barrel.GetComponent<PressX>().ActivateQuestion();
        yield return null;
        
        var button = GameObject.Find("Choice_B").GetComponent<Button>();
        button.onClick.Invoke();
        Assert.That(button, Is.Not.Null);
        yield return new WaitForSeconds(1);
        var chainsaw = GameObject.Find("Chainsaw");
        Debug.Log(GameObject.Find("FirstPersonPlayer").GetComponent<BoxCollider>());
        Debug.Log(chainsaw.GetComponent<Collectable>());
        chainsaw.GetComponent<Collectable>().OnTriggerEnter(GameObject.Find("FirstPersonPlayer").GetComponent<BoxCollider>());
        yield return new WaitForSeconds(2);

        Debug.Log(GameObject.Find("FirstPersonPlayer").GetComponent<PlayerInventory>().inventory.Count);

        Assert.AreEqual(GameObject.Find("FirstPersonPlayer").GetComponent<PlayerInventory>().inventory.Count, 1);

    }
}
