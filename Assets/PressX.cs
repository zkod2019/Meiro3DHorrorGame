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

    void Start()
    {
        this.question = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

        pressX.SetActive(false);
        question.SetActive(false);
        inputText.SetActive(false);
    }

    void OnTriggerEnter(Collider player)
    {
        if (!this.questionStatus.answered && player.gameObject.tag == "Player")
        {
            inputText.GetComponent<InputField>().text = "";
            question.GetComponent<Image>().sprite = questionStatus.question;

            pressX.SetActive(true);
            inputText.GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
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

    public void ValueChangeCheck()
    {
        userAnswer = inputText.GetComponent<InputField>().text;
        Debug.Log(userAnswer);
        Debug.Log(this.questionStatus.answer);
        if (userAnswer == this.questionStatus.answer)
        {
            Debug.Log("Correct Answer");

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
            this.gameObject.SetActive(false);
            pressX.SetActive(false);
            question.SetActive(false);
            inputText.SetActive(false);
        }
        else
        {
            Debug.Log("Wrong Answer! Try Again!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pressX.activeSelf)
        {
            question.SetActive(true);
            inputText.SetActive(true);
            pressX.SetActive(false);
            // question.SetActive(!question.activeSelf);
            // inputText.SetActive(!inputText.activeSelf);
        }
    }
}