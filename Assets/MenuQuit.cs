using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuQuit : MonoBehaviour
{
void Start() {

}

void Update() {}

    public void QuitGame() {
        Application.Quit();
    }

    public void GoMenu() {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex - 1);
    }
}
