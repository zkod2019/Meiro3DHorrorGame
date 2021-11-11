using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //allows the script to reference the UI

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject objectText;
    public int currentScore;
    public AudioSource collectSound;

    void OnTriggerEnter(Collider other){
        collectSound.Play();
        currentScore += 1;
        objectText.GetComponent<Text>().Text = "OBJECT: " + currentScore;
        Destroy(gameObject);
    }
}
