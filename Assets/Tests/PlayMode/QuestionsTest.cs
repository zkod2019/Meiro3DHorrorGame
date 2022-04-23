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

public class BarrelsTest : MonoBehaviour
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
    public IEnumerator WrongBarrelInput()
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
        
        var buttonA = GameObject.Find("Choice_A").GetComponent<Button>();
        barrel.GetComponent<PressX>().OnButtonPress(buttonA);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(GameObject.Find("Answer").GetComponent<Text>().text, "WRONG!");
        yield return null;
    }

    [UnityTest]
    public IEnumerator CorrectBarrelInput()
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
        
        var buttonB = GameObject.Find("Choice_B").GetComponent<Button>();
        barrel.GetComponent<PressX>().OnButtonPress(buttonB);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(GameObject.Find("Answer").GetComponent<Text>().text, "CORRECT!");
    }

    [UnityTest]
    public IEnumerator CorrectCrateInput(){
        GoToSignUp();
        string uniqueId = System.Guid.NewGuid().ToString();
        TestSignUp($"testplayer-{uniqueId}", $"testplayer-{uniqueId}");
        yield return new WaitForSeconds(40);
        Assert.That(Auth.idToken, Is.Not.Null);
        
        var crate = GameObject.Find("Crate_1");
        GameObject.Find("FirstPersonPlayer").transform.position = crate.transform.position;
        Assert.That(crate, Is.Not.Null);
        yield return null;
        crate.GetComponent<PressSpace>().ActivateQuestion();
        yield return null;
        
        var buttonD = GameObject.Find("Choice_D").GetComponent<Button>();
        crate.GetComponent<PressSpace>().OnButtonPress(buttonD);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(GameObject.Find("Answer").GetComponent<Text>().text, "CORRECT!");
    }

    [UnityTest]
    public IEnumerator WrongCrateInput(){
        GoToSignUp();
        string uniqueId = System.Guid.NewGuid().ToString();
        TestSignUp($"testplayer-{uniqueId}", $"testplayer-{uniqueId}");
        yield return new WaitForSeconds(40);
        Assert.That(Auth.idToken, Is.Not.Null);
        
        var crate = GameObject.Find("Crate_1");
        GameObject.Find("FirstPersonPlayer").transform.position = crate.transform.position;
        Assert.That(crate, Is.Not.Null);
        yield return null;
        crate.GetComponent<PressSpace>().ActivateQuestion();
        yield return null;
        
        var buttonC = GameObject.Find("Choice_C").GetComponent<Button>();
        crate.GetComponent<PressSpace>().OnButtonPress(buttonC);
        yield return new WaitForSeconds(1);
        Assert.AreEqual(GameObject.Find("Answer").GetComponent<Text>().text, "WRONG! LAST TRY!");
    }
}
