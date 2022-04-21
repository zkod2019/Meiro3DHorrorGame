using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PressSpace : MonoBehaviour
{
    public GameObject pressSpace;
    GameObject question;
    public QuestionStatus questionStatus;
    public String userAnswer;
    public QuestionType type;
    public GameObject[] choices;
    public int wrongAnswer = 0;
    public Text answerText;

    void Awake() {
        choices =  new GameObject[7];
    }

    void Start()
    {
        this.question = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

        pressSpace.SetActive(false);
        question.SetActive(false);
        Canvas canvas = (Canvas)FindObjectOfType(typeof(Canvas));

        for (int i = 0; i < 7; i++) {
            choices[i] = canvas.transform.GetChild(i+8).gameObject;
            choices[i].SetActive(false);
        }
    }

    void OnTriggerEnter(Collider player)
    {
        if (!this.questionStatus.answered && player.gameObject.tag == "Player")
        {
            question.GetComponent<Image>().sprite = questionStatus.question;
            pressSpace.SetActive(true);
            Debug.Log(choices.Length);
            for (int i = 0; i < 7; i++) {
                Button btn = choices[i].GetComponent<Button>();
                btn.onClick.AddListener(delegate { OnButtonPress(btn); });
            }
        }
    }

    void OnButtonPress(Button pressed) {
        Debug.Log(pressed);
        userAnswer = pressed.GetComponentInChildren<TMP_Text>().text;
        Debug.Log(userAnswer);
        Debug.Log(this.questionStatus.answer);
        if (userAnswer == this.questionStatus.answer){
            Debug.Log("Correct Answer");
            answerText.text = "CORRECT!";
            ShowAnswerText();
            Invoke("HideAnswerText", 2);
            GameObject player = GameObject.Find("FirstPersonPlayer");
            Timer timer = player.GetComponent<Timer>();
            timer.timeRemaining += 500;

            if (type == QuestionType.Puzzle){
                for (int i = 0; i < HintController.puzzleQuestions.Length; i++){
                    if (HintController.puzzleQuestions[i].question == this.questionStatus.question){
                        HintController.answerQuestion(type, i);
                    }
                }
            } 

            pressSpace.SetActive(false);
            question.SetActive(false);
            for (int i = 0; i < 7; i++) {
                    choices[i].SetActive(false);
                    choices[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }
        } else{
            wrongAnswer += 1;
            if (wrongAnswer == 1){
                answerText.text = "WRONG! LAST TRY!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
            }
            if (wrongAnswer == 2){
                answerText.text = "WRONG AGAIN!";
                ShowAnswerText();
                Invoke("HideAnswerText", 2);
                this.gameObject.SetActive(false);
                pressSpace.SetActive(false);
                question.SetActive(false);
        
                 for (int i = 0; i < 7; i++) {
                    choices[i].SetActive(false);
                    choices[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }
            }
        }
        // Debug.Log(pressed.transform.GetChild(0).GetComponent<Text>().text);
    }

    void OnTriggerExit(Collider player)
    {
        if (player.gameObject.tag == "Player")
        {
            question.GetComponent<Image>().sprite = null;

            pressSpace.SetActive(false);
            question.SetActive(false);
            for (int i = 0; i < 7; i++) {
                choices[i].SetActive(false);
                choices[i].GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    void HideAnswerText() {
        answerText.enabled = false;
    }
    void ShowAnswerText() {
        answerText.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && pressSpace.activeSelf)
        {
            question.SetActive(true);
            for (int i = 0; i < 7; i++) {
                choices[i].SetActive(true);
            }
            pressSpace.SetActive(false);
        }
    }
}