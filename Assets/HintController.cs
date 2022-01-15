using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour {
    void Start() {
        // Pseudocode:
        //   1. Load all the question image textures (and answer texts)
        //   2. Get all the barrels in the level
        //   3. For every barrel, assign a question (and answer)


        // Let's try some real code now :/
        //   1. Load all the question image textures (and answer texts)
        Sprite[] iqQuestions = Array.ConvertAll(Resources.LoadAll("iq-questions", typeof(Sprite)), asset => (Sprite)asset);
        TextAsset[] iqAnswers = Array.ConvertAll(Resources.LoadAll("iq-answers", typeof(TextAsset)), asset => (TextAsset)asset);

        //   2. Get all the barrels in the level
        GameObject[] barrels = GameObject.FindGameObjectsWithTag("barrel");

        //   3. For every barrel, assign a question (and answer)
        for (int i = 0; i < barrels.Length; i++) {
            barrels[i].GetComponent<PressX>().questionSprite = iqQuestions[i];
            barrels[i].GetComponent<PressX>().answer = iqAnswers[i].text;
        }
    }
}
