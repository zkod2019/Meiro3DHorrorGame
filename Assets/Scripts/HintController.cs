using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Net.Http;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine.Networking;


public enum QuestionType
{
    Logic,
    Reading,
    Math,
    Puzzle,
}

public class QuestionStatus
{
    public Sprite question;
    public String answer;
    public bool answered;

    public QuestionStatus(Sprite question, String answer, bool answered)
    {
        this.question = question;
        this.answer = answer;
        this.answered = answered;
    }
}

/*
[userName]/
    {
        loops: 4,
        xQuestions: [false, false, false, true, false],
    }
*/

public class UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation asyncOp;
    private Action continuation;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
    {
        this.asyncOp = asyncOp;
        asyncOp.completed += OnRequestCompleted;
    }

    public bool IsCompleted { get { return asyncOp.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
    }

    private void OnRequestCompleted(AsyncOperation obj)
    {
        continuation();
    }
}

public static class ExtensionMethods
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new UnityWebRequestAwaiter(asyncOp);
    }

    public static string SubstringIdx(this string value, int startIndex, int endIndex)
    {
        return value.Substring(startIndex, (endIndex - startIndex + 1));
    }
}

public class HintController : MonoBehaviour
{
    public static QuestionStatus[] iqQuestions;
    public static QuestionStatus[] mathQuestions;
    public static QuestionStatus[] readingQuestions;
    public static QuestionStatus[] puzzleQuestions;
    public static GameObject loadingScreen;

    static bool initial = true;
    private static readonly HttpClient client = new HttpClient();

    public static async void answerQuestion(QuestionType type, int index)
    {
        switch (type)
        {
            case QuestionType.Logic:
                iqQuestions[index].answered = true;
                break;
            case QuestionType.Math:
                mathQuestions[index].answered = true;
                break;
            case QuestionType.Reading:
                readingQuestions[index].answered = true;
                break;
            case QuestionType.Puzzle:
                puzzleQuestions[index].answered = true;
                break;
        }

        string firestoreUrl = $"https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/users/{Auth.username}";
        Debug.Log(firestoreUrl);

        await HintController.InitializeQuestions();
        var json = $@"
            {{
                ""fields"": {{
                    ""loop"": {{
                        ""integerValue"": {Timer.loopCount}
                    }},
                    ""iqQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.iqQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }}, 
                    ""mathQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.mathQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }},
                    ""readingQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.readingQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }},
                    ""puzzleQuestions"": {{
                        ""arrayValue"": {{
                            ""values"": [{String.Join(",", Array.ConvertAll(HintController.puzzleQuestions, q => $@"{{""booleanValue"": {q.answered.ToString().ToLower()} }}"))}]
                        }}
                    }}
                }}
            }}";

        var firestoreSetReq = new HttpRequestMessage
        {
            Method = new HttpMethod("PATCH"),
            RequestUri = new Uri(firestoreUrl),
            Headers = {
                { HttpRequestHeader.Authorization.ToString(), $"Bearer {Auth.idToken}" },
                { HttpRequestHeader.Accept.ToString(), "application/json" },
            },
            Content = new StringContent(json)
        };

        var firestoreResponse = await client.SendAsync(firestoreSetReq);
        var firestoreResponseString = await firestoreResponse.Content.ReadAsStringAsync();
        Debug.Log(json);
        Debug.Log(firestoreResponseString);

        // if (firestoreResponse.StatusCode == HttpStatusCode.OK) {
        //     SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
        // }
    }

    static async Task<Sprite> spriteFromURL(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            return null;
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
    }

    public static async Task InitializeQuestions()
    {   
        if (initial)
        {
            loadingScreen.SetActive(true);
            Debug.Log("HintController trigerred");
            initial = false;

            // Pseudocode:
            //   1. Load all the question image textures (and answer texts)
            //   2. Get all the barrels in the level
            //   3. For every barrel, assign a question (and answer)


            //   1. Load all the question image textures (and answer texts)
            string questionsFirestoreUrl = "https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/questions?pageSize=100";
            var questionsFirestoreResponse = await client.GetAsync(questionsFirestoreUrl);
            var questionsFirestoreResponseString = await questionsFirestoreResponse.Content.ReadAsStringAsync();
            Debug.Log(questionsFirestoreResponseString);
            var questionsJson = (JArray)JObject.Parse(questionsFirestoreResponseString).GetValue("documents");
            Debug.Log(questionsJson);
            /*
                {
                    documents: [
                        {
                            fields: {
                                answer: {
                                    stringValue: '',
                                },
                                question: {
                                    stringValue: '',
                                },
                            }
                        }
                    ]   
                }
            */

            List<Tuple<string, string>> iqQuestionsRaw = new List<Tuple<string, string>>();
            List<Tuple<string, string>> mathQuestionsRaw = new List<Tuple<string, string>>();
            List<Tuple<string, string>> readingQuestionsRaw = new List<Tuple<string, string>>();
            List<Tuple<string, string>> puzzleQuestionsRaw = new List<Tuple<string, string>>();

            Debug.Log(questionsJson.Count);
            for (int i = 0; i < questionsJson.Count; i++)
            {
                var questionJsonFields = (JObject)(((JObject)questionsJson[i]).GetValue("fields"));
                var questionFileName = (string)(((JObject)questionJsonFields.GetValue("question")).GetValue("stringValue"));
                var answer = (string)(((JObject)questionJsonFields.GetValue("answer")).GetValue("stringValue"));
                if (questionFileName.StartsWith("iq"))
                {
                    iqQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
                else if (questionFileName.StartsWith("math"))
                {
                    mathQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
                else if (questionFileName.StartsWith("reading"))
                {
                    readingQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
                else if (questionFileName.StartsWith("puzzle"))
                {
                    puzzleQuestionsRaw.Add(Tuple.Create(questionFileName, answer));
                }
            }

            iqQuestionsRaw.Sort((a, b) => Int32.Parse(a.Item1.SubstringIdx("iq-questions/".Length, a.Item1.Length - 5)) - Int32.Parse(b.Item1.SubstringIdx("iq-questions/".Length, b.Item1.Length - 5)));
            mathQuestionsRaw.Sort((a, b) => Int32.Parse(a.Item1.SubstringIdx("math-questions/".Length, a.Item1.Length - 5)) - Int32.Parse(b.Item1.SubstringIdx("math-questions/".Length, b.Item1.Length - 5)));
            readingQuestionsRaw.Sort((a, b) => Int32.Parse(a.Item1.SubstringIdx("reading-questions/".Length, a.Item1.Length - 5)) - Int32.Parse(b.Item1.SubstringIdx("reading-questions/".Length, b.Item1.Length - 5)));
            puzzleQuestionsRaw.Sort((a, b) => Int32.Parse(a.Item1.SubstringIdx("puzzle-questions/".Length, a.Item1.Length - 5)) - Int32.Parse(b.Item1.SubstringIdx("puzzle-questions/".Length, b.Item1.Length - 5)));

            Debug.Log(iqQuestionsRaw.Count);
            Debug.Log(mathQuestionsRaw.Count);
            Debug.Log(readingQuestionsRaw.Count);
            Debug.Log(puzzleQuestionsRaw.Count);

            HintController.iqQuestions = new QuestionStatus[iqQuestionsRaw.Count];
            for (int i = 0; i < iqQuestionsRaw.Count; i++)
            {
                Debug.Log($"https://storage.googleapis.com/meiro-ip.appspot.com/{iqQuestionsRaw[i].Item1}");
                HintController.iqQuestions[i] = new QuestionStatus(await spriteFromURL($"https://storage.googleapis.com/meiro-ip.appspot.com/{iqQuestionsRaw[i].Item1}"), iqQuestionsRaw[i].Item2, false);
            }
            HintController.mathQuestions = new QuestionStatus[mathQuestionsRaw.Count];
            for (int i = 0; i < mathQuestionsRaw.Count; i++)
            {
                Debug.Log($"https://storage.googleapis.com/meiro-ip.appspot.com/{mathQuestionsRaw[i].Item1}");
                HintController.mathQuestions[i] = new QuestionStatus(await spriteFromURL($"https://storage.googleapis.com/meiro-ip.appspot.com/{mathQuestionsRaw[i].Item1}"), mathQuestionsRaw[i].Item2, false);
            }
            HintController.readingQuestions = new QuestionStatus[readingQuestionsRaw.Count];
            for (int i = 0; i < readingQuestionsRaw.Count; i++)
            {
                Debug.Log($"https://storage.googleapis.com/meiro-ip.appspot.com/{readingQuestionsRaw[i].Item1}");
                HintController.readingQuestions[i] = new QuestionStatus(await spriteFromURL($"https://storage.googleapis.com/meiro-ip.appspot.com/{readingQuestionsRaw[i].Item1}"), readingQuestionsRaw[i].Item2, false);
            }
            HintController.puzzleQuestions = new QuestionStatus[puzzleQuestionsRaw.Count];
            for (int i = 0; i < puzzleQuestionsRaw.Count; i++)
            {
                Debug.Log($"https://storage.googleapis.com/meiro-ip.appspot.com/{puzzleQuestionsRaw[i].Item1}");
                HintController.puzzleQuestions[i] = new QuestionStatus(await spriteFromURL($"https://storage.googleapis.com/meiro-ip.appspot.com/{puzzleQuestionsRaw[i].Item1}"), puzzleQuestionsRaw[i].Item2, false);
            }

            Debug.Log("all questions loaded");

            string firestoreUrl = $"https://firestore.googleapis.com/v1/projects/meiro-ip/databases/(default)/documents/users/{Auth.username}";

            var firestoreResponse = await client.GetAsync(firestoreUrl);
            var firestoreResponseString = await firestoreResponse.Content.ReadAsStringAsync();
            var json = JObject.Parse(firestoreResponseString);
            Debug.Log(json);

            /*

            {
                fields: {
                    loops: {
                        numberValue: 42,
                    },
                    iqQuestions: {
                        values: [
                            {
                                booleanValue: false
                            }
                        ]
                    }
                }
            }

            */

            var fields = (JObject)json.GetValue("fields");
            Debug.Log(fields);
            Debug.Log((JObject)fields.GetValue("readingQuestions"));

            var readingQuestionsJson =
                (JArray)
                (
                    ((JObject)(
                    (
                        (
                          (JObject)
                          fields.GetValue("readingQuestions")
                        )
                        .GetValue("arrayValue")
                    )
                    )).GetValue("values")
                );
            Debug.Log(readingQuestionsJson);

            var mathQuestionsJson =
                (JArray)
                (
                    ((JObject)(
                    (
                        (
                          (JObject)
                          fields.GetValue("mathQuestions")
                        )
                        .GetValue("arrayValue")
                    )
                    )).GetValue("values")
                );


            var iqQuestionsJson =
                (JArray)
                (
                    ((JObject)(
                    (
                        (
                          (JObject)
                          fields.GetValue("iqQuestions")
                        )
                        .GetValue("arrayValue")
                    )
                    )).GetValue("values")
                );


            var puzzleQuestionsJson =
                (JArray)
                (
                    ((JObject)(
                    (
                        (
                          (JObject)
                          fields.GetValue("puzzleQuestions")
                        )
                        .GetValue("arrayValue")
                    )
                    )).GetValue("values")
                );

            for (int i = 0; i < readingQuestionsJson.Count; i++)
            {
                var answered = (bool)(((JObject)readingQuestionsJson[i]).GetValue("booleanValue"));
                readingQuestions[i].answered = answered;
            }
            for (int i = 0; i < mathQuestionsJson.Count; i++)
            {
                var answered = (bool)(((JObject)mathQuestionsJson[i]).GetValue("booleanValue"));
                mathQuestions[i].answered = answered;
            }
            for (int i = 0; i < iqQuestionsJson.Count; i++)
            {
                var answered = (bool)(((JObject)iqQuestionsJson[i]).GetValue("booleanValue"));
                iqQuestions[i].answered = answered;
            }
            for (int i = 0; i < puzzleQuestionsJson.Count; i++)
            {
                var answered = (bool)(((JObject)puzzleQuestionsJson[i]).GetValue("booleanValue"));
                puzzleQuestions[i].answered = answered;
            }
            loadingScreen.SetActive(false);
            Debug.Log("closed loading screen");
        }
    }

    public static void InitializeBarrels()
    {
        Debug.Log("initalizing barrels...");

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

    public static void InitializeCrates()
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("crate");
        GameObject[] puzzleCrates = crates.Where(b => b.GetComponent<PressSpace>().type == QuestionType.Puzzle).ToArray();

        for (int i = 0; i < puzzleCrates.Length; i++)
        {
            var crateScript = puzzleCrates[i].GetComponent<PressSpace>();
            crateScript.questionStatus = HintController.puzzleQuestions[i];
        }
    }

    async void Start()
    {
        Debug.Log("helooo?");
        HintController.loadingScreen = GameObject.Find("Canvas").transform.GetChild(6).transform.GetChild(4).gameObject;
        Debug.Log(loadingScreen);
        await HintController.InitializeQuestions();
        Debug.Log("finished initializing questions");
        HintController.InitializeBarrels();
        HintController.InitializeCrates();
    }
}
