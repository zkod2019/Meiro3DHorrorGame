using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public enum QuestionType
{
    Logic,
    Reading,
    Math,
}

public class HintController : MonoBehaviour
{
    void Start()
    {
        // Pseudocode:
        //   1. Load all the question image textures (and answer texts)
        //   2. Get all the barrels in the level
        //   3. For every barrel, assign a question (and answer)


        //   1. Load all the question image textures (and answer texts)
        Sprite[] iqQuestions = Array.ConvertAll(Resources.LoadAll("iq/questions", typeof(Sprite)), asset => (Sprite)asset);
        TextAsset[] iqAnswers = Array.ConvertAll(Resources.LoadAll("iq/answers", typeof(TextAsset)), asset => (TextAsset)asset);
        Sprite[] mathQuestions = Array.ConvertAll(Resources.LoadAll("math/questions", typeof(Sprite)), asset => (Sprite)asset);
        TextAsset[] mathAnswers = Array.ConvertAll(Resources.LoadAll("math/answers", typeof(TextAsset)), asset => (TextAsset)asset);
        Sprite[] readingQuestions = Array.ConvertAll(Resources.LoadAll("reading/questions", typeof(Sprite)), asset => (Sprite)asset);
        TextAsset[] readingAnswers = Array.ConvertAll(Resources.LoadAll("reading/answers", typeof(TextAsset)), asset => (TextAsset)asset);

        Array.Sort(iqQuestions, (a, b) =>
            Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
        Array.Sort(iqAnswers, (a, b) =>
            Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));

        Array.Sort(mathQuestions, (a, b) =>
            Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
        Array.Sort(mathAnswers, (a, b) =>
            Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));

        Array.Sort(readingQuestions, (a, b) =>
            Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
        Array.Sort(readingAnswers, (a, b) =>
            Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));

        //   2. Get all the barrels in the level
        GameObject[] barrels = GameObject.FindGameObjectsWithTag("barrel");

        //   3. For every barrel, assign a question (and answer)
        for (int i = 0; i < barrels.Length; i++)
        {
            var barrelScript = barrels[i].GetComponent<PressX>();
            switch (barrelScript.type)
            {
                case QuestionType.Logic:
                    barrelScript.questionSprite = iqQuestions[i];
                    barrelScript.answer = iqAnswers[i].text;
                    break;
                case QuestionType.Math:
                    barrelScript.questionSprite = mathQuestions[i];
                    barrelScript.answer = mathAnswers[i].text;
                    break;
                case QuestionType.Reading:
                    barrelScript.questionSprite = readingQuestions[i];
                    barrelScript.answer = readingAnswers[i].text;
                    break;
            }
        }
    }
}
