using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PressX : MonoBehaviour{
    public GameObject pressX;
    public GameObject inputText;
    public GameObject hint;

    //public GameObject hh ; 

    void Start(){
        //hh= (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IQ");
        pressX.SetActive(false);
        hint.SetActive(false);
        inputText.SetActive(false);
    }

    void OnTriggerEnter(Collider player){
        if(player.gameObject.tag == "Player"){
            pressX.SetActive(true);
        }
    }

    void OnTriggerExit(Collider player){
        if(player.gameObject.tag == "Player"){
            pressX.SetActive(false);
            hint.SetActive(false);
            inputText.SetActive(false);
        }
    }

    void Update(){
        Texture2D[] results = Array.ConvertAll(Resources.LoadAll("IQ", typeof(Texture2D)), asset => (Texture2D)asset);
        // string path = "Assets/IQ";
	    // test = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject> (path);

        // for(int i = 0; i < gameobject.transform.GetChildCount(); i++)
        // {
        //     Transform Children = gameobject.transform.GetChild(i);
        // }

    //    for(int i = 0; i < results.si; i++)
    //     {
    //         Transform Children = gameobject.transform.GetChild(i);
    //     }

        if (Input.GetKeyDown(KeyCode.Space) && pressX.active){
            hint.SetActive(!hint.active);
            inputText.SetActive(!inputText.active);
        }
    }


    /*
   // public TextMesh pressX;
    
    
    public Text pressX;
   
   void Start()
    {
        //Text sets your text to say this message
        pressX.text = "This is my text";
    }

    void Update()
    {
        //Press the space key to change the Text message
        if (Input.GetKey(KeyCode.Space))
        {
            pressX.text = "My text has now changed.";
        }
    }
   
    void OnCollisionEnter (Collision collisionInfo)
    {
       Debug.Log (collisionInfo.collider.name);
        pressX.text = "Press X" + collisionInfo.collider.name;


        //GetComponentInChildren<Text>().text = "Press X";
    }
    
    */
}
