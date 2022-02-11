using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressSpace : MonoBehaviour
{
    public GameObject pressSpace;
    public GameObject inputText;
    GameObject question;

    public QuestionStatus questionStatus;
    public String userAnswer;

    public QuestionType type;

    void Start()
    {
        this.question = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

        pressSpace.SetActive(false);
        question.SetActive(false);
        inputText.SetActive(false);
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            inputText.GetComponent<InputField>().text = "";
            question.GetComponent<Image>().sprite = questionStatus.question;

            pressSpace.SetActive(true);
            inputText.GetComponent<InputField>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        }
    }

    void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            question.GetComponent<Image>().sprite = null;
            inputText.GetComponent<InputField>().text = "";

            pressSpace.SetActive(false);
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
            GameObject player = GameObject.Find("FirstPersonPlayer");
            Timer timer = player.GetComponent<Timer>();
            timer.timeRemaining += 500;
            inputText.GetComponent<InputField>().onValueChanged.RemoveAllListeners();
            this.gameObject.SetActive(false);
            pressSpace.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Space) && pressSpace.activeSelf)
        {
            question.SetActive(true);
            inputText.SetActive(true);
            pressSpace.SetActive(false);
        }
    }
}