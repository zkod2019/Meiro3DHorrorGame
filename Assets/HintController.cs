using System;
using System.Linq;
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
    Puzzle,
}

public class QuestionStatus {
    public Sprite question;
    public String answer;
    public bool answered;

    public QuestionStatus(Sprite question, String answer, bool answered) {
        this.question = question;
        this.answer = answer;
        this.answered = answered;
    }
}

public class HintController : MonoBehaviour
{
    public static QuestionStatus[] iqQuestions;
    public static QuestionStatus[] mathQuestions;
    public static QuestionStatus[] readingQuestions;
    public static QuestionStatus[] puzzleQuestions;

    static bool initial = true;

    public static void InitializeQuestions() {
        if (initial) {
            Debug.Log("HintController trigerred");
            initial = false;

            // Pseudocode:
            //   1. Load all the question image textures (and answer texts)
            //   2. Get all the barrels in the level
            //   3. For every barrel, assign a question (and answer)


            //   1. Load all the question image textures (and answer texts)
            Sprite[] iqQuestionSprites = Array.ConvertAll(Resources.LoadAll("iq/questions", typeof(Sprite)), asset => (Sprite)asset);
            TextAsset[] iqAnswers = Array.ConvertAll(Resources.LoadAll("iq/answers", typeof(TextAsset)), asset => (TextAsset)asset);
            Sprite[] mathQuestionSprites = Array.ConvertAll(Resources.LoadAll("math/questions", typeof(Sprite)), asset => (Sprite)asset);
            TextAsset[] mathAnswers = Array.ConvertAll(Resources.LoadAll("math/answers", typeof(TextAsset)), asset => (TextAsset)asset);
            Sprite[] readingQuestionSprites = Array.ConvertAll(Resources.LoadAll("reading/questions", typeof(Sprite)), asset => (Sprite)asset);
            TextAsset[] readingAnswers = Array.ConvertAll(Resources.LoadAll("reading/answers", typeof(TextAsset)), asset => (TextAsset)asset);

            Sprite[] puzzleQuestionSprites = Array.ConvertAll(Resources.LoadAll("tPuzzles/questions", typeof(Sprite)), asset => (Sprite)asset);
            TextAsset[] puzzleAnswers = Array.ConvertAll(Resources.LoadAll("tPuzzles/answers", typeof(TextAsset)), asset => (TextAsset)asset);

            Array.Sort(iqQuestionSprites, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
            Array.Sort(iqAnswers, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));

            Array.Sort(mathQuestionSprites, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
            Array.Sort(mathAnswers, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));

            Array.Sort(readingQuestionSprites, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
            Array.Sort(readingAnswers, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));

            Array.Sort(puzzleQuestionSprites, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
            Array.Sort(puzzleAnswers, (a, b) =>
                Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(a))) - Int32.Parse(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(b))));
            
            iqQuestions = new QuestionStatus[iqQuestionSprites.Length];
            for (int i = 0; i < iqQuestionSprites.Length; i++)
            {
                iqQuestions[i] = new QuestionStatus(iqQuestionSprites[i], iqAnswers[i].text, false);
            }
            mathQuestions = new QuestionStatus[mathQuestionSprites.Length];
            for (int i = 0; i < mathQuestionSprites.Length; i++)
            {
                mathQuestions[i] = new QuestionStatus(mathQuestionSprites[i], mathAnswers[i].text, false);
            }
            readingQuestions = new QuestionStatus[readingQuestionSprites.Length];
            for (int i = 0; i < readingQuestionSprites.Length; i++)
            {
                readingQuestions[i] = new QuestionStatus(readingQuestionSprites[i], readingAnswers[i].text, false);
            }
            puzzleQuestions = new QuestionStatus[puzzleQuestionSprites.Length];
            for (int i = 0; i < puzzleQuestionSprites.Length; i++)
            {
                puzzleQuestions[i] = new QuestionStatus(puzzleQuestionSprites[i], puzzleAnswers[i].text, false);
            }

        }
    }

    public static void InitializeBarrels() {
        GameObject[] barrels = GameObject.FindGameObjectsWithTag("barrel");
        GameObject[] logicBarrels = barrels.Where(b => b.GetComponent<PressX>().type == QuestionType.Logic).ToArray();
        GameObject[] mathBarrels = barrels.Where(b => b.GetComponent<PressX>().type == QuestionType.Math).ToArray();
        GameObject[] readingBarrels = barrels.Where(b => b.GetComponent<PressX>().type == QuestionType.Reading).ToArray();

        //   3. For every barrel, assign a question (and answer)
        for (int i = 0; i < logicBarrels.Length; i++)
        {
            var barrelScript = logicBarrels[i].GetComponent<PressX>();
            barrelScript.questionStatus = HintController.iqQuestions[i];
        }

        for (int i = 0; i < mathBarrels.Length; i++)
        {
            var barrelScript = mathBarrels[i].GetComponent<PressX>();
            barrelScript.questionStatus = HintController.mathQuestions[i];
        }

        for (int i = 0; i < readingBarrels.Length; i++)
        {
            var barrelScript = readingBarrels[i].GetComponent<PressX>();
            barrelScript.questionStatus = HintController.readingQuestions[i];
        }
    }

    public static void InitializeCrates() {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("crate");
        GameObject[] puzzleCrates = crates.Where(b => b.GetComponent<PressSpace>().type == QuestionType.Puzzle).ToArray();

        for (int i = 0; i < puzzleCrates.Length; i++)
        {
            var crateScript = puzzleCrates[i].GetComponent<PressSpace>();
            crateScript.questionStatus = HintController.puzzleQuestions[i];
        }
    }
    
    void Start()
    {
        HintController.InitializeQuestions();
        HintController.InitializeBarrels();
        HintController.InitializeCrates();
    }
}
