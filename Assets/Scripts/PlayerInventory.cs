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
    public Text totalAnswered;
    public Text improvement;
    public Text puzzleImprovement;

    public void Add(Collectable obj) {
        inventory.Add(obj);
        exitMaze.SetActive(true);
        count = 0;
        GetComponentInChildren<Text>().text = "INVENTORY: ";
        foreach (Collectable collectable in inventory) {
            GetComponentInChildren<Text>().text += collectable.collectableName + ", ";
            count++;

            // if (count==1){
            //     winnerCanvas.SetActive(true);
            //     resultsBtn.onClick.AddListener(PlayerResults);
            // }
            if (count==1 || count==2 || count==3){
                Debug.Log("All Obj Collected");
                exitMaze.SetActive(false);
                Debug.Log("Exit is open!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
            if (other.tag == "exit") { 
                Debug.Log("THIS ISNT WORKING?!");
                winnerCanvas.SetActive(true);
                resultsBtn.onClick.AddListener(PlayerResults);
                Debug.Log("OR IS IT?");
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

        int totalScore = correctIq + correctMath + correctReading;
        
        if (totalScore >= 27){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Very High Exceptional";
        }else if(totalScore >= 24 && totalScore <= 26){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: High Expert";
        }else if(totalScore >= 21 && totalScore <= 23){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Expert";
        }else if(totalScore >= 19 && totalScore <= 20){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Very High Average";
        }else if(totalScore >= 17 && totalScore <= 18){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: High Average";
        }else if(totalScore >= 13 && totalScore <= 16){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Middle Average";
        }else if(totalScore >= 10 && totalScore <= 12){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Low Average";
        }else if(totalScore >= 6 && totalScore <= 9){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Borderline Low";
        }else if(totalScore >= 3 && totalScore <= 5){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Low";
        }else if(totalScore >= 0 && totalScore <= 2){
            totalAnswered.text = $"Out of all 30 IQ questions, you answered: {totalScore}\nYour rating is: Very Low";
        }

        if (correctPuzzle >= 8 && correctPuzzle <= 10){
            puzzleImprovement.text = $"Out of all 10 puzzle questions, you answered: {correctPuzzle}\nAmazing Work!";
        }else if (correctPuzzle >= 4 && correctPuzzle <= 7){
            puzzleImprovement.text = $"Out of all 10 puzzle questions, you answered: {correctPuzzle}\nGood Job!";
        }else if (correctPuzzle >= 0 && correctPuzzle <= 3){
            puzzleImprovement.text = $"Out of all 10 puzzle questions, you answered: {correctPuzzle}\nTry Harder!";
        }

        if (correctIq < correctMath && correctIq < correctReading){
            improvement.text = $"The score on your pattern questions is low! Practice them more!";
        }else if (correctMath < correctIq && correctMath < correctReading){
            improvement.text = $"The score on your math questions is low! Practice them more!";
        }else if (correctReading < correctMath && correctReading < correctIq){
            improvement.text = $"The score on your English questions is low! Practice them more!";
        }else if (correctReading == correctMath && correctReading < correctIq){
            improvement.text = $"The score on your English and math questions are low! Practice them more!";
        }else if (correctMath == correctIq && correctMath < correctReading){
            improvement.text = $"The score on your math and pattern questions are low! Practice them more!";
        }else if (correctReading == correctIq && correctIq < correctMath){
            improvement.text = $"The score on your English and pattern questions are low! Practice them more!";
        }

        resultText.text = $"Pattern questions answered: {correctIq}/{HintController.iqQuestions.Length}\nMath questions answered: {correctMath}/{HintController.mathQuestions.Length}\nReading questions answered: {correctReading}/{HintController.readingQuestions.Length}\nPuzzle questions answered: {correctPuzzle}/{HintController.puzzleQuestions.Length}\nLoops Taken: {Timer.loopCount}";
    }

}
