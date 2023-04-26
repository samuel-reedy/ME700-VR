using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using TMPro;


namespace Autohand.Demo
{
    public class RDSManagerDualShift : MonoBehaviour
    {
        [SerializeField] GameObject LeftPlane;
        [SerializeField] GameObject RightPlane;

        [SerializeField] GameObject LeftPlaneShowInFront;
        [SerializeField] GameObject RightPlaneShowInFront;

        [SerializeField] GameObject LeftPlaneShowBehind;
        [SerializeField] GameObject RightPlaneShowBehind;
        private Renderer leftPlaneRenderer;
        private Renderer rightPlaneRenderer;

        [Header("Properties")]
        [SerializeField] int pixelCount;
        [SerializeField] int shiftSize;
        [SerializeField] int shiftAmount;
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


        [Header("Open XR Input")]
        private bool pressedPrimary;
        private bool pressedSecondary;

        bool isDeciding;
        bool countingDown;
        bool inFront;

        

        enum STATE
        {
            START,
            COUNTDOWN,
            DISPLAY,
            GUESS,
            FINISH,
        }

        STATE currentState;

        // Generate the defulat random dot sterogram
        Texture2D generateTexture(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;          // Ensuring pixels are clear

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    texture.SetPixel(x, y, Random.Range(0, 100) > 50 ? Color.black : Color.white);

                }
            }

            texture.Apply();
            return texture;
        }

        Texture2D ShiftPixels(Texture2D texture, bool shiftInFront)
        {
            int startX = (pixelCount / 2) - (shiftSize / 2);
            int startY = (pixelCount / 2) - (shiftSize / 2);

            Texture2D textureNew = generateTexture(pixelCount);

            Graphics.CopyTexture(texture, textureNew);



            for (int x = startX; x < startX + shiftSize + shiftAmount; x++)
            {
                for (int y = startY; y < startY + shiftSize; y++)
                {
                    bool shiftPixel = Random.Range(0, 100) < percentageShifted;
                    if (shiftPixel) textureNew.SetPixel(x, y, texture.GetPixel(x + (shiftInFront ? -shiftAmount : shiftAmount), y));
                }
            }

            textureNew.Apply();
            return textureNew;
        }

        
        void GenerateRandomDotSterograms()
        {
            // Texture generation
            Texture2D leftPlaneTexture = generateTexture(pixelCount);
            Texture2D rightPlaneTexture;

            // Whether In Front
            inFront = Random.Range(0, 100) > 50;

            rightPlaneTexture = ShiftPixels(leftPlaneTexture, inFront);

            // Create
            leftPlaneRenderer.material.mainTexture = leftPlaneTexture;
            rightPlaneRenderer.material.mainTexture = rightPlaneTexture;
        }
        
        void GenerateShows()
        {
            // Texture generation
            Texture2D leftPlaneTexture = generateTexture(pixelCount);
            Texture2D rightPlaneTexture;

            rightPlaneTexture = ShiftPixels(leftPlaneTexture, true);

            // Create
            LeftPlaneShowInFront.GetComponent<Renderer>().material.mainTexture = leftPlaneTexture;
            RightPlaneShowInFront.GetComponent<Renderer>().material.mainTexture = rightPlaneTexture;

            rightPlaneTexture = ShiftPixels(leftPlaneTexture, false);

            // Create
            LeftPlaneShowBehind.GetComponent<Renderer>().material.mainTexture = leftPlaneTexture;
            RightPlaneShowBehind.GetComponent<Renderer>().material.mainTexture = rightPlaneTexture;

        }




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

            isDeciding = false;
            countingDown = false;

            GenerateShows();
        }






        private void Update()
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
                LeftPlaneShowInFront.SetActive(false);
                RightPlaneShowInFront.SetActive(false);
                LeftPlaneShowBehind.SetActive(false);
                RightPlaneShowBehind.SetActive(false);

                return STATE.COUNTDOWN;
            }
            return STATE.START;
                
        }

        STATE CountDown()
        {
            LeftPlane.SetActive(false);
            RightPlane.SetActive(false);

            countdownTimeLeft -= Time.deltaTime;
            countDownTimeText.text = "" + (int)countdownTimeLeft;
            countDownTimeText.color = new Color(1, 1, 1, countdownTimeLeft / countdownTime);

            if (countdownTimeLeft <= 0)
            {
                countDownTimeText.text = "";
                countdownTimeLeft = countdownTime;
                GenerateRandomDotSterograms();
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
                bool guessInFront = pressedPrimary ? true : false;
                if ((guessInFront && inFront) || (!guessInFront && !inFront)) correctCounter++;
                completed++;

                

                correctText.text = "" + (int)(correctCounter / (float)completed * 100.0) + "%";
                remainingText.text = "" + completed + "/" + dataSetSize;

                if (completed == dataSetSize) return STATE.FINISH;
                return STATE.COUNTDOWN;

            }
            return STATE.GUESS;
        }

        STATE Finish()
        {
            Debug.Log("Finish");
            return STATE.FINISH;
        }
    }


    


    
}

