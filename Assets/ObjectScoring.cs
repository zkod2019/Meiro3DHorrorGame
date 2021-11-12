using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //allows the script to reference the UI

public class ObjectScoring : MonoBehaviour
{
    public GameObject objectText;
    public int currentScore;
    public AudioSource collectSound;

    void OnTriggerEnter(Collider other){
        collectSound.Play();
        currentScore = currentScore + 1;
        objectText.GetComponent<Text>().text = "OBJECT: " + currentScore;
        Destroy(gameObject);
    }
}
