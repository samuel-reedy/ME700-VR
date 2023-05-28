using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Security.Cryptography;
using UnityEngine.Experimental.XR.Interaction;

public class RDSManagerTwoDisplay : MonoBehaviour
{
    [SerializeField] GameObject LeftPlane;
    [SerializeField] GameObject RightPlane;

    private Renderer leftPlaneRenderer;
    private Renderer rightPlaneRenderer;

    [Header("Properties")]
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
    [SerializeField] TMP_Text countDownTimeText;

    [SerializeField] TMP_Text HadShift;
    [SerializeField] TMP_Text ShiftPosition;


    [SerializeField] RDSObjectGenerator RDSGenerator;
    [SerializeField] List<RDS> all_RDS;


    [SerializeField] RDSObjectGenerator RDSGeneratorZero;
    [SerializeField] List<RDS> all_RDS_zero;

    [SerializeField] List<RDS> possible_RDSs;



    [SerializeField] List<SCORE> scoreList;

    bool inFront;

    private bool pressedPrimary;
    private bool pressedSecondary;

    STATE currentState;

    SCORE currentScore;

    public int amountDisplayed;

    bool displayZero;
    bool first;

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
        if (amountDisplayed == 0)
        {
            displayZero = Random.Range(0, 1) == 0;
            first = displayZero == false ? true : false;
        }
        if (amountDisplayed == 1) displayZero = !displayZero;

        possible_RDSs.Clear();

        possible_RDSs = new List<RDS>(displayZero ? all_RDS_zero : all_RDS);

        if (possible_RDSs.Count == 0) return true;

        int indexToUse = Random.Range(0, possible_RDSs.Count);

        RDS rds = possible_RDSs[indexToUse];

        inFront = !rds.left_shifted;
        leftPlaneRenderer.material.mainTexture = rds.textA;
        rightPlaneRenderer.material.mainTexture = rds.textB;


        File.AppendAllText(path, (inFront ? "1" : "0") + ", ");
        File.AppendAllText(path, (rds.percentage_shift) + ", ");


        (displayZero ? all_RDS_zero : all_RDS).Remove(rds);


        return false;
    }

    // Start is called before the first frame update

    string path;

    void Start()
    {
        // Get Sprite Renderers
        leftPlaneRenderer = LeftPlane.GetComponent<Renderer>();
        rightPlaneRenderer = RightPlane.GetComponent<Renderer>();

        remainingText.text = completed + "/" + dataSetSize;

        //GenerateRandomDotSterograms();
        countdownTimeLeft = countdownTime;
        displayTimeLeft = displayTime;

        RDSGenerator.GenerateList();
        all_RDS = RDSGenerator.RDS_List;

        RDSGeneratorZero.GenerateList();
        all_RDS_zero = RDSGeneratorZero.RDS_List;

        dataSetSize = all_RDS.Count;
        
        amountDisplayed = 0;

        path = Application.dataPath + "/Log.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }

        File.AppendAllText(path, System.DateTime.Now + ":\n");


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
    }


    STATE StartWait()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return STATE.COUNTDOWN;
        }
        return STATE.START;
    }

    STATE CountDown()
    {
        LeftPlane.SetActive(false);
        RightPlane.SetActive(false);

        HadShift.text = "";
        ShiftPosition.text = "";

        countdownTimeLeft -= Time.deltaTime;
        countDownTimeText.text = "" + (int)(countdownTimeLeft+1);
        countDownTimeText.color = new Color(1, 1, 1, countdownTimeLeft / countdownTime);

        if (countdownTimeLeft <= 0)
        {
            countDownTimeText.text = "";
            countdownTimeLeft = countdownTime;

            bool isFinished;
            isFinished = SelectRDS();

            amountDisplayed++;

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

            if (amountDisplayed == 2)
            {
                HadShift.text = "Had Shift (1 or 2): ";
                ShiftPosition.text = "Disparity Position: ";
                return STATE.GUESS;
            }

            
            return STATE.COUNTDOWN;

        }
        return STATE.DISPLAY;
    }

    bool guessInFront;
    bool guessFirst;

    STATE Guess()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            guessFirst = true;
            HadShift.text = "Had Shift (1 or 2): 1";
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            guessFirst = false;
            HadShift.text = "Had Shift (1 or 2): 2";
        } 

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            guessInFront = false;
            ShiftPosition.text = "Disparity Position: Front";
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            guessInFront = true;
            ShiftPosition.text = "Disparity Position: Back";
        }

   

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (guessFirst && first) File.AppendAllText(path, "1, ");
            else if (!guessFirst && !first) File.AppendAllText(path, "1, ");
            else File.AppendAllText(path, "0, ");

            if (guessInFront && inFront) File.AppendAllText(path, "1\n");
            else if (!guessInFront && !inFront) File.AppendAllText(path, "1\n");
            else File.AppendAllText(path, "0\n");

            completed++;
            remainingText.text = "" + completed + "/" + dataSetSize;

            amountDisplayed = 0;

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
    
