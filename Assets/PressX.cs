using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressX : MonoBehaviour
{
    public GameObject pressX;
    public GameObject inputText;
    GameObject question;
    public QuestionStatus questionStatus;
    public String userAnswer;
    public QuestionType type;
    public int wrongAnswer = 0;
    public Text answerText;
    public GameObject weapon;
    public LineRenderer lineRenderer;
    public Button choiceA;
    public GameObject choiceB;
    public GameObject choiceC;
    public GameObject choiceD;
    public GameObject choiceE;
    public GameObject choiceF;
    public GameObject choiceG;

    void Start()
    {
        this.question = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

        pressX.SetActive(false);
        question.SetActive(false);
        inputText.SetActive(false);
        //choiceA.SetActive(false); choiceB.SetActive(false); choiceC.SetActive(false); choiceD.SetActive(false); choiceE.SetActive(false); choiceF.SetActive(false); choiceG.SetActive(false);
    }

    void OnTriggerEnter(Collider player)
    {
        if (!this.questionStatus.answered && player.gameObject.tag == "Player")
        {
            //Button optA = choiceA.GetComponent<Button>();
            inputText.GetComponent<InputField>().text = "";
            question.GetComponent<Image>().sprite = questionStatus.question;
            pressX.SetActive(true);
            inputText.GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
            //optA.GetComponent<Button>().onClick.AddListener(delegate { ValueChangeCheck(); });
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            question.GetComponent<Image>().sprite = null;
            inputText.GetComponent<InputField>().text = "";

            pressX.SetActive(false);
            question.SetActive(false);
            inputText.SetActive(false);

            inputText.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
        }
    }

    void HideAnswerText() {
        answerText.enabled = false;
    }
    void ShowAnswerText() {
        answerText.enabled = true;
    }

    public void ValueChangeCheck()
    {
        userAnswer = inputText.GetComponent<InputField>().text;
        Debug.Log(userAnswer);
        Debug.Log(this.questionStatus.answer);
        if (userAnswer == this.questionStatus.answer)
        {
            Debug.Log("Correct Answer");
            answerText.text = "CORRECT!";
            Invoke("HideAnswerText", 2);
             
            if (type == QuestionType.Logic){
                for (int i = 0; i < HintController.iqQuestions.Length; i++){
                    if (HintController.iqQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            } else if (type == QuestionType.Math){
                for (int i = 0; i < HintController.mathQuestions.Length; i++){
                    if (HintController.mathQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            }else if (type == QuestionType.Reading){
                for (int i = 0; i < HintController.readingQuestions.Length; i++){
                    if (HintController.readingQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            }

            HintController.InitializeBarrels();
            
            inputText.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
            //this.gameObject.SetActive(false);
            pressX.SetActive(false);
            question.SetActive(false);
            inputText.SetActive(false);

            if (weapon != null)
        {
            var path = new UnityEngine.AI.NavMeshPath();

            // this is the little hack i added at the end
            // it wasn't working between barrel->weapon before cuz their Y positions were different
            // so we just set both Y's to the same thing
            var selfPosition = gameObject.transform.position;
            selfPosition.y = -9;
            var weaponPosition = weapon.transform.position;
            weaponPosition.y = -9;

            if (UnityEngine.AI.NavMesh.CalculatePath(selfPosition, weaponPosition, UnityEngine.AI.NavMesh.AllAreas, path))
            {
                lineRenderer.positionCount = path.corners.Length;
                for (int i = 0; i < path.corners.Length; i++)
                    lineRenderer.SetPosition(i, path.corners[i] + Vector3.up * 5);
            }
            else
            {
                // idk what you wanna do here
                Debug.Log("could not find path");
            }
        }
        }
        else
        {
            wrongAnswer += 1;
            if (wrongAnswer == 1){
                answerText.text = "WRONG!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
            }
            Debug.Log("Wrong Answer! Try Again!");
            if (wrongAnswer == 2){
                answerText.text = "WRONG! LAST TRY!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
            }
            if (wrongAnswer == 3){
                inputText.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
                this.gameObject.SetActive(false);
                pressX.SetActive(false);
                question.SetActive(false);
                inputText.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pressX.activeSelf)
        {
            question.SetActive(true);
            inputText.SetActive(true);
            pressX.SetActive(false);
        }
    }
}