using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PressX : MonoBehaviour
{
    public GameObject pressX;
    public GameObject hint;

    void Start()
    {
        pressX.SetActive(false);
        hint.SetActive(false);
    }

    void OnTriggerEnter(Collider player)
    {
        //pressX.SetActive(false);

        if(player.gameObject.tag == "Player"){
            
        //    if (player.gameObject.GetComponent<Collider>().bounds.Contains(GetComponent<Collider>().bounds.min) 
        //       && player.gameObject.GetComponent<Collider>().bounds.Contains(GetComponent<Collider>().bounds.max)) {
             pressX.SetActive(true);


            //}
        }
    }

    void OnTriggerExit(Collider player)
    {
        if(player.gameObject.tag == "Player"){
             pressX.SetActive(false);
        }
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space) && pressX.active){
            hint.SetActive(!hint.active);
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
