using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public List<Collectable> inventory;
    public int count;
    public GameObject exitMaze;
    public GameObject winnerCanvas;
    public GameObject resultCanvas;
    public Button resultsBtn;
    public Text resultText;

    public void Add(Collectable obj) {
        inventory.Add(obj);
        exitMaze.SetActive(true);
        count = 0;
        GetComponentInChildren<Text>().text = "INVENTORY: ";
        foreach (Collectable collectable in inventory) {
            GetComponentInChildren<Text>().text += collectable.collectableName + ", ";
            count++;

           if (count==1){
                winnerCanvas.SetActive(true);
                resultsBtn.onClick.AddListener(PlayerResults);
          }
            if (count==8){
                Debug.Log("All Obj Collected");
                exitMaze.SetActive(false);
                Debug.Log("Exit is open!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
            if (other.tag == "exit") { 
                winnerCanvas.SetActive(true);
                resultsBtn.onClick.AddListener(PlayerResults);
            }
    }

    void PlayerResults()
    {
        Debug.Log("heruihguerfd!");
        resultCanvas.SetActive(true);

        int correctIq = 0;
        for (int i = 0; i < HintController.iqQuestions.Length; i++){
            if (HintController.iqQuestions[i].answered)
                correctIq += 1;
        }
        int correctMath = 0;
        for (int i = 0; i < HintController.mathQuestions.Length; i++){
            if (HintController.mathQuestions[i].answered)
                correctMath += 1;
        }
        int correctReading = 0;
        for (int i = 0; i < HintController.readingQuestions.Length; i++){
            if (HintController.readingQuestions[i].answered)
                correctReading += 1;
        }
        int correctPuzzle = 0;
        for (int i = 0; i < HintController.puzzleQuestions.Length; i++){
            if (HintController.puzzleQuestions[i].answered)
                correctPuzzle += 1;
        }

        resultText.text = $"Pattern questions answered: {correctIq}/{HintController.iqQuestions.Length}\nMath questions answered: {correctMath}/{HintController.mathQuestions.Length}\nReading questions answered: {correctReading}/{HintController.readingQuestions.Length}\nPuzzle questions answered: {correctPuzzle}/{HintController.puzzleQuestions.Length}";
    }

}
