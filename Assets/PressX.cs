using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressX : MonoBehaviour{
    public GameObject pressX;
    public GameObject inputText;
    GameObject question;

    public Sprite questionSprite;
    public String answer;
    public String userAnswer;

    void Start() {
        this.question = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        Debug.Log(this.question);
        inputText.GetComponent<InputField>().onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        pressX.SetActive(false);
        question.SetActive(false);
        inputText.SetActive(false);
    }

    void OnTriggerEnter(Collider player) {
       
        if(this.gameObject.activeSelf){
           if(player.gameObject.tag == "Player"){
            question.GetComponent<Image>().sprite = questionSprite;
            pressX.SetActive(true);
        }
        }
    }

    void OnTriggerExit(Collider player) {
        if(player.gameObject.tag == "Player"){
            question.GetComponent<Image>().sprite = null;
            pressX.SetActive(false);
            question.SetActive(false);
            inputText.SetActive(false);
        }
    }

    public void ValueChangeCheck()
    {
        Debug.Log("Value Changed");
         Debug.Log(inputText);
            userAnswer = inputText.GetComponent<InputField>().text;
            Debug.Log(userAnswer);
            if (userAnswer.Equals(answer)){
                Debug.Log("Correct Answer");
                this.gameObject.SetActive(false);
                pressX.SetActive(false);
                question.SetActive(false);
                inputText.SetActive(false);
            }else{
                Debug.Log("Wrong Answer! Try Again!");
            }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && pressX.activeSelf){
            question.SetActive(!question.activeSelf);
            inputText.SetActive(!inputText.activeSelf);
        }
    }
}