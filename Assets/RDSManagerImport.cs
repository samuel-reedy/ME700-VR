using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Security.Cryptography;

public class RDSManagerImport : MonoBehaviour
{
    [SerializeField] GameObject LeftPlane;
    [SerializeField] GameObject RightPlane;

    private Renderer leftPlaneRenderer;
    private Renderer rightPlaneRenderer;

    [Header("Properties")]
    [SerializeField] float percentageShifted;
    [SerializeField] float countdownTime;
    [SerializeField] float displayTime;


    private float countdownTimeLeft;
    private float displayTimeLeft;


    [Header("Data Set Variables")]
    [SerializeField] int dataSetSize;
    [SerializeField] int correctCounter;
    [SerializeField] int completed = 0;

    [Header("UI")]
    [SerializeField] TMP_Text remainingText;
    [SerializeField] TMP_Text correctText;
    [SerializeField] TMP_Text countDownTimeText;



    [SerializeField] RDSObjectGenerator RDSGenerator;
    [SerializeField] List<RDS> all_RDS;
    [SerializeField] List<RDS> possible_RDSs;

    [SerializeField] bool usePercentageShifted;

    [SerializeField] List<SCORE> scoreList;

    bool inFront;

    private bool pressedPrimary;
    private bool pressedSecondary;

    STATE currentState;

    SCORE currentScore;

    

    enum STATE
    {
        START,
        COUNTDOWN,
        DISPLAY,
        GUESS,
        FINISH,
    }

    bool SelectRDS()
    {
        possible_RDSs.Clear();

        if (usePercentageShifted)
        {
            for (int i = 0; i < all_RDS.Count; i++)
            {
                RDS rds = this.all_RDS[i];
                if (rds.percentage_shift == percentageShifted) possible_RDSs.Add(rds);
            }
        }
        else
        {
            possible_RDSs = new List<RDS>(all_RDS);
        }
        

        if (possible_RDSs.Count == 0) return true;

        int indexToUse = Random.Range(0, possible_RDSs.Count);
        inFront = !possible_RDSs[indexToUse].left_shifted;
        leftPlaneRenderer.material.mainTexture = possible_RDSs[indexToUse].textA;
        rightPlaneRenderer.material.mainTexture = possible_RDSs[indexToUse].textB;

        bool foundScore = false;
        foreach(SCORE score in scoreList)
        {
            if (score.percentage_shift == possible_RDSs[indexToUse].percentage_shift)
            {
                currentScore = score;
                foundScore = true;
                break;
            }
            
        }

        if (!foundScore)
        {
            SCORE score = ScriptableObject.CreateInstance<SCORE>();
            score.percentage_shift = possible_RDSs[indexToUse].percentage_shift;
            scoreList.Add(score);
            currentScore = score;
        }

        all_RDS.Remove(possible_RDSs[indexToUse]);

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get Sprite Renderers
        leftPlaneRenderer = LeftPlane.GetComponent<Renderer>();
        rightPlaneRenderer = RightPlane.GetComponent<Renderer>();

        remainingText.text = completed + "/" + dataSetSize;
        correctText.text = "100%";

        //GenerateRandomDotSterograms();
        countdownTimeLeft = countdownTime;
        displayTimeLeft = displayTime;

        RDSGenerator.GenerateList();
        all_RDS = RDSGenerator.RDS_List;

        dataSetSize = all_RDS.Count;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case STATE.START:
                currentState = StartWait();
                break;
            case STATE.COUNTDOWN:
                currentState = CountDown();
                break;
            case STATE.DISPLAY:
                currentState = Display();
                break;
            case STATE.GUESS:
                currentState = Guess();
                break;
            case STATE.FINISH:
                currentState = Finish();
                break;
        }



        if (Input.GetKeyDown(KeyCode.LeftArrow)) pressedPrimary = true;
        else pressedPrimary = false;


        if (Input.GetKeyDown(KeyCode.RightArrow)) pressedSecondary = true;
        else pressedSecondary = false;
    }


    STATE StartWait()
    {
        if (pressedPrimary)
        {
            return STATE.COUNTDOWN;
        }
        return STATE.START;
    }

    STATE CountDown()
    {
        LeftPlane.SetActive(false);
        RightPlane.SetActive(false);

        countdownTimeLeft -= Time.deltaTime;
        countDownTimeText.text = "" + (int)(countdownTimeLeft+1);
        countDownTimeText.color = new Color(1, 1, 1, countdownTimeLeft / countdownTime);

        if (countdownTimeLeft <= 0)
        {
            countDownTimeText.text = "";
            countdownTimeLeft = countdownTime;

            bool isFinished;
            isFinished = SelectRDS();

            if (isFinished) return STATE.FINISH;
            return STATE.DISPLAY;
        }
        return STATE.COUNTDOWN;
    }

    STATE Display()
    {
        LeftPlane.SetActive(true);
        RightPlane.SetActive(true);

        displayTimeLeft -= Time.deltaTime;

        if (displayTimeLeft <= 0)
        {
            displayTimeLeft = displayTime;

            LeftPlane.SetActive(false);
            RightPlane.SetActive(false);


            return STATE.GUESS;
        }
        return STATE.DISPLAY;
    }

    STATE Guess()
    {
        if (pressedPrimary || pressedSecondary)
        {
            bool guessInFront = pressedPrimary ? false : true;
            if (inFront)
            {
                currentScore.count_right++;
                if (guessInFront) currentScore.correct_right++;
            }

            if (!inFront)
            {
                currentScore.count_left++;
                if (!guessInFront) currentScore.correct_left++;
            }

            completed++;

            correctText.text = "" + (int)(correctCounter / (float)completed * 100.0) + "%";
            remainingText.text = "" + completed + "/" + dataSetSize;

            if (completed == dataSetSize) return STATE.FINISH;
            return STATE.COUNTDOWN;

        }
        return STATE.GUESS;
    }

    bool displayedResults = false;
    STATE Finish()
    {
        if (!displayedResults)
        {
            displayedResults = true;
            scoreList.Sort(SortByScore);
            CreateText();
        }
        Debug.Log("Finish");
        return STATE.FINISH;
    }

    static int SortByScore(SCORE s1, SCORE s2)
    {
        return s1.percentage_shift.CompareTo(s2.percentage_shift);
    }

    void CreateText()
    {
        string path = Application.dataPath + "/Log.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }

        string content = System.DateTime.Now + ":\n";
        foreach(SCORE score in scoreList)
        {
            content = content + score.percentage_shift + ", " + score.correct_right + ", " + score.count_right
                + ", " + score.correct_left + ", " + score.count_left + "\n";
        }

        File.AppendAllText(path, content);
    }

}
    
