using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuQuit : MonoBehaviour
{
void Start() {

}

void Update() {}

    public async void QuitGame() {
        if (Auth.idToken != null)
        await GameObject.Find("FirstPersonPlayer").GetComponent<Timer>().DisplayLoop();
        Application.Quit();
    }

    public void GoMenu() {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex - 1);
    }
}
