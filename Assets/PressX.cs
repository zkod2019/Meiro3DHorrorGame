using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressX : MonoBehaviour{
    public GameObject pressX;
    public GameObject inputText;
    GameObject hint;

    public Sprite hintSprite;
    public String answer;

    void Start() {
        this.hint = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        Debug.Log(this.hint);
        pressX.SetActive(false);
        hint.SetActive(false);
        inputText.SetActive(false);
    }

    void OnTriggerEnter(Collider player) {
        if(player.gameObject.tag == "Player"){
            hint.GetComponent<Image>().sprite = hintSprite;
            pressX.SetActive(true);
        }
    }

    void OnTriggerExit(Collider player) {
        if(player.gameObject.tag == "Player"){
            hint.GetComponent<Image>().sprite = null;
            pressX.SetActive(false);
            hint.SetActive(false);
            inputText.SetActive(false);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && pressX.activeSelf){
            hint.SetActive(!hint.activeSelf);
            inputText.SetActive(!inputText.activeSelf);
        }
    }
}