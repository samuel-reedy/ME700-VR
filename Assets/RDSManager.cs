using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using TMPro;
namespace Autohand.Demo
{
    public class RDSManager : MonoBehaviour
    {
        // Panels
        [SerializeField] GameObject LeftPlane;
        [SerializeField] GameObject RightPlane;
        private Renderer leftPlaneRenderer;
        private Renderer rightPlaneRenderer;

        [SerializeField] Color color1;
        [SerializeField] Color color2;


        [Header("Properties: Image Size")]
        [SerializeField] int pixelCountMin;
        [SerializeField] int pixelCountMax;
        [SerializeField] PhysicsGadgetConfigurableLimitReader pixelCountSlider;
        [SerializeField] TMP_Text pixelCountSliderText;
        int pixelCount;


        [Header("Properties: Shift Size")]
        [SerializeField] int shiftSizeMin;
        [SerializeField] int shiftSizeMax;
        [SerializeField] PhysicsGadgetConfigurableLimitReader shiftSizeSlider;
        [SerializeField] TMP_Text shiftSizeSliderText;
        int shiftSize;


        [Header("Properties: Shift Amount")]
        [SerializeField] int shiftAmountMin;
        [SerializeField] int shiftAmountMax;
        [SerializeField] PhysicsGadgetConfigurableLimitReader shiftAmountSlider;
        [SerializeField] TMP_Text shiftAmountSliderText;
        int shiftAmount;


        [Header("Properties: Percentage Shifted")]
        [SerializeField] PhysicsGadgetConfigurableLimitReader percentageShiftedSlider;
        [SerializeField] TMP_Text percentageShiftedSliderText;
        float percentageShifted;


        [Header("Properties: Timer")]
        [SerializeField] float timerMin;
        [SerializeField] float timerMax;
        [SerializeField] PhysicsGadgetConfigurableLimitReader timerSlider;
        [SerializeField] TMP_Text timerSliderText;
        private float timer;
        private float timeLeft;


        [Header("Properties: Percentage With Depth")]
        [SerializeField] PhysicsGadgetConfigurableLimitReader percentageWithDepthSlider;
        [SerializeField] TMP_Text percentageWithDepthSliderText;
        private float percentageWithDepth;


        [Header("Data Set Variables")]
        [SerializeField] int dataSetSize;
        int correctCounter;
        int withDepthCounter;
        int completed = 0;

        [Header("UI")]
        private Sprite LeftPlaneSprite;
        private Sprite RightPlaneSprite;

        [SerializeField] TMP_Text remainingText;
        [SerializeField] TMP_Text correctText;
        [SerializeField] TMP_Text timerText;

        [SerializeField] Image beginImage;
        [SerializeField] Image depthButton;
        [SerializeField] Image noDepthButton;

        [Header("Audio")]
        [SerializeField] AudioClip wrongSoundClip;
        [SerializeField] AudioClip rightSoundClip;

        [Header("Animation")]
        [SerializeField] GameObject planeParent;

        [Header("OPen XR Input")]

        [SerializeField] XRNode xrNode = XRNode.RightHand;


        
        
        
        
        

        private List<InputDevice> inputDevices = new List<InputDevice>();
        private InputDevice device;

        bool hasDepth;


        void GetDevice()
        {
            InputDevices.GetDevicesAtXRNode(xrNode, inputDevices);
            if (inputDevices.Count > 0)
            {
                device = inputDevices[0];
            }

        }

        private void OnEnable()
        {
            if (!device.isValid)
            {
                GetDevice();
            }
        }



        // Generate the defulat random dot sterogram
        Texture2D generateTexture(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;          // Ensuring pixels are clear

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    texture.SetPixel(x, y, Random.Range(0, 100) > percentageWithDepth ? color1 : color2);

                }
            }

            texture.Apply();
            return texture;
        }

        Texture2D ShiftPixels(Texture2D texture)
        {
            //int shiftAmount = Random.Range(shiftAmountmin, shiftAmountmax);
            //int shiftSize = Random.Range(shiftSizeMin, shiftSizeMax);
 

            int startX = Random.Range(shiftAmount, pixelCount - shiftSize - shiftAmount);
            int startY = Random.Range(0, pixelCount - shiftSize);

            Texture2D textureNew = generateTexture(pixelCount);

            Graphics.CopyTexture(texture, textureNew);



            for (int x = startX; x < startX + shiftSize + shiftAmount; x++)
            {
                for (int y = startY; y < startY + shiftSize; y++)
                {
                    bool shiftPixel = Random.Range(0, 100) < percentageShifted;
                    if (shiftPixel) textureNew.SetPixel(x, y, texture.GetPixel(x + shiftAmount, y));
                }
            }

            textureNew.Apply();
            return textureNew;
        }

        void PlayerGuess(bool guessDepth)
        {

            timeLeft = timer;
            if ((guessDepth && hasDepth) || (!guessDepth && !hasDepth))
            {
                correctCounter++;
                this.GetComponent<AudioSource>().PlayOneShot(rightSoundClip);

            }
            else
            {
                this.GetComponent<AudioSource>().PlayOneShot(wrongSoundClip);
            }

            completed++;
            

            planeParent.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = leftPlaneRenderer.material.mainTexture;


            if (guessDepth) planeParent.GetComponent<Animator>().Play("Flip");
            else planeParent.GetComponent<Animator>().Play("FlipOpp");


            GenerateRandomDotSterograms();

            remainingText.gameObject.GetComponent<Animator>().Play("Blip");
        }


        void GenerateRandomDotSterograms()
        {
            // Texture generation
            Texture2D leftPlaneTexture = generateTexture(pixelCount);
            Texture2D rightPlaneTexture;

            // Whether has depth
            hasDepth = Random.Range(0, 100) > 50;

            if (hasDepth)
            {
                rightPlaneTexture = ShiftPixels(leftPlaneTexture);
                withDepthCounter++;
            }
            else
            {
                rightPlaneTexture = leftPlaneTexture;
            }


            // Create
            leftPlaneRenderer.material.mainTexture = leftPlaneTexture;
            rightPlaneRenderer.material.mainTexture = rightPlaneTexture;
        }

        float ScaleSlider(PhysicsGadgetConfigurableLimitReader slider, float min, float max)
        {
            float valueAdjusted = (slider.GetValue() + 1) / 2.0f;
            return (max - min) * valueAdjusted + min;
        }

        void updateSliders()
        {
            pixelCount = (int)ScaleSlider(pixelCountSlider, pixelCountMin, pixelCountMax);
            pixelCountSliderText.text = pixelCount.ToString();

            shiftSize = (int)ScaleSlider(shiftSizeSlider, shiftSizeMin, shiftSizeMax);
            shiftSizeSliderText.text = shiftSize.ToString();

            shiftAmount = (int)ScaleSlider(shiftAmountSlider, shiftAmountMin, shiftAmountMax);
            shiftAmountSliderText.text = shiftAmount.ToString();

            percentageShifted = (int)ScaleSlider(percentageShiftedSlider, 0, 100);
            percentageShiftedSliderText.text = percentageShifted.ToString() + "%";

            timer = (int)ScaleSlider(timerSlider, timerMin, timerMax);
            timerSliderText.text = timer.ToString() + "s";

            percentageWithDepth = (int)ScaleSlider(percentageWithDepthSlider, 0, 100);
            percentageWithDepthSliderText.text = percentageWithDepth.ToString() + "%";
        }

        void Start()
        {
            // Get Sprite Renderers
            leftPlaneRenderer = LeftPlane.GetComponent<Renderer>();
            rightPlaneRenderer = RightPlane.GetComponent<Renderer>();

            remainingText.text = completed + "/" + dataSetSize;
            correctText.text = "100%";

            //GenerateRandomDotSterograms();

            timeLeft = timerMax;


        }

        bool primaryButton = false;
        bool prevPrimaryButton = false;

        bool secondaryButton = false;
        bool prevSecondaryButton = false;

        bool countingDown = false;
        float pressAgainTime = 0.5f;

        private void Update()
        {
            if (!device.isValid)
            {
                GetDevice();
            }

            if (completed < dataSetSize)
            {
                InputFeatureUsage<bool> usagePrimary = CommonUsages.primaryButton;
                if ((device.TryGetFeatureValue(usagePrimary, out primaryButton) && primaryButton && !prevPrimaryButton) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (countingDown)
                    {
                        Debug.Log("Primary Button");
                        PlayerGuess(true);
                        depthButton.GetComponent<Animator>().Play("Blip");
                    }
                    else
                    {
                        countingDown = true;
                        beginImage.enabled = false;
                        GenerateRandomDotSterograms();
                    }


                }
                prevPrimaryButton = primaryButton;

                InputFeatureUsage<bool> usageSecondary = CommonUsages.secondaryButton;
                if ((device.TryGetFeatureValue(usageSecondary, out secondaryButton) && secondaryButton && !prevSecondaryButton) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (countingDown)
                    {
                        Debug.Log("Secondary Button");
                        PlayerGuess(false);
                        noDepthButton.GetComponent<Animator>().Play("Blip");
                    }


                }
                prevSecondaryButton = secondaryButton;

                if (countingDown)
                {
                    if (timeLeft <= 0)
                    {

                        PlayerGuess(!hasDepth);
                    }

                    timeLeft -= Time.deltaTime;
                }
            }
            else
            {
                timeLeft = timer;
            }
            timerText.text = (Mathf.RoundToInt(timeLeft * 10.0f) / 10.0f).ToString("0.0") + " / " + (Mathf.RoundToInt(timer * 10.0f) / 10.0f).ToString("0.0");
            remainingText.text = completed + "/" + dataSetSize;
            if (completed == 0)
            {
                correctText.text = "100%";
            }
            else
            {
                correctText.text = (int)(correctCounter / (float)completed * 100) + "%";
            }
            

            updateSliders();
        }

        public void GuessDepthButton()
        {
            if (countingDown)
            {
                if (completed < dataSetSize)
                {
                    PlayerGuess(true);
                }
                    
            }
            else
            {
                countingDown = true;
                beginImage.enabled = false;
                GenerateRandomDotSterograms();
            }

        }
        public void GuessNoDepthButton()
        {
            if (countingDown)
            {
                if (completed < dataSetSize)
                {
                    PlayerGuess(false);
                }
            }
            else
            {
                countingDown = true;
                beginImage.enabled = false;
                GenerateRandomDotSterograms();
            }

        }

        public void Reset()
        {
            countingDown = false;
            completed = 0;
            correctCounter = 0;
            withDepthCounter = 0;

            beginImage.enabled = true;
            timeLeft = timer;
        }
    }

    
}

