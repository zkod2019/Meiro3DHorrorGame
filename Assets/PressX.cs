using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PressX : MonoBehaviour
{
    public GameObject pressX;

    void Start()
    {
        pressX.SetActive(false);
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.gameObject.tag == "Player"){
            pressX.SetActive(true);
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
