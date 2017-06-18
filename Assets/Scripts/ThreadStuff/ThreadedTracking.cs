using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadedTracking : MonoBehaviour
{

    public static AndroidJavaObject currentActivity;

    //Main Stuff
    public GameObject mainPanel;
    public GameObject ARCamera;
    public GameObject[] descriptionLables;
    public GameObject cloud;
    public GameObject starHaruko;
    public Transform starPlaceholder;
    //EndGame Stuff
    public GameObject endPanel;
    public GameObject introCamera;
    //Game Stuff
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text timer;
    public GameObject bonusHolder;
    public static GAME_STATE gameState;

    int textIndex = 0;

    public UnityEngine.UI.Text ConsoleText;
    public UnityEngine.UI.Button start;
    public UnityEngine.UI.Button pause;
    public GameObject areUSure;

    public GameObject runningStar;
    WalkingData dataRetreiver = null;

    int caughtStars = 0;
    int totStars = 5;
    float timeLeft;
    float time = 240;
    float lastSprint = 60;

    Vector3 previousPos;
    Vector3 playerPhysicalPosition;
    Vector3 playerLogicalPosition;
    float carburationTime = 1;

    void Start()
    {
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        gameState = GAME_STATE.INTRO;
        ARCamera.GetComponent<AudioController>().AudioinDome();
    }

    void Update()
    {
        //Chech for termination
        if (ThreadedTracking.gameState == ThreadedTracking.GAME_STATE.GAME)
        {
            timeLeft -= Time.deltaTime;
            carburationTime -= Time.deltaTime;
            int minutes = (int)timeLeft / 60;
            int secs = (int)timeLeft % 60;

            timer.text = ((("" + minutes).Length == 1) ? "0" : "") + minutes + ":" + ((("" + secs).Length == 1) ? "0" : "") + secs;

            if (timeLeft <= 0)
            {
                StopTracking();
                return;
            }

            if (timeLeft <= lastSprint)
                LastSprint();
        }
    }

    public void UpdateScore()
    {
        caughtStars++;
        if (caughtStars == totStars)
            StopTracking();
        else
        {
            MakeStarFall();
        }
        scoreText.text = caughtStars + "/" + totStars;
    }

    public bool GetPlayerDeltaPosition(out Vector3 delta, out Vector3 playerLogicalPos)
    {
        playerLogicalPos = delta = Vector3.zero;
        Vector3 relativePos;

        if ((dataRetreiver != null && dataRetreiver.isPlaying()) && dataRetreiver.getPlayerRelativePosition(out relativePos))
        {
            //Debug.Log("PLAYER LOGICAL POS " + relativePos);
            if (previousPos != relativePos && (carburationTime > 0))
            {
                playerPhysicalPosition = relativePos;
                Debug.Log("PLAYER New Physical " + relativePos);
            }

            previousPos = relativePos;
            //ConsoleText.text = "Is Walking =  " + dataRetreiver.isWalking() + "{" + relativePos.x + ", " + relativePos.y + "}";

            delta = playerPhysicalPosition - relativePos;
            playerLogicalPosition = playerLogicalPos = relativePos;

            //Debug.Log("CHANGEEEEED  prev " + previousPos + " delta " + delta + " walking? " + dataRetreiver.isWalking());

            return true;
        }
        else
        {
            delta = Vector3.zero;
            return false;
        }
    }

    public void StartTracking()
    {
        //Debug.Log("Start");
        if (ThreadedTracking.gameState != ThreadedTracking.GAME_STATE.PLANETARIUM)
            return;

        ARCamera.GetComponent<AudioController>().AudioinGame();

        scoreText.text = "0/" + totStars;
        caughtStars = 0;
        timeLeft = time;
        carburationTime = 1;
        if (dataRetreiver == null)
            dataRetreiver = new WalkingData();
        playerLogicalPosition = playerPhysicalPosition = GameObject.FindGameObjectWithTag("PlayerPhysicalPosition").transform.position;
        //Debug.Log("PLAYER START POS " + playerPosition);
        previousPos = playerPhysicalPosition;
        dataRetreiver.Start(currentActivity, playerPhysicalPosition);

        start.gameObject.SetActive(false);
        pause.gameObject.SetActive(true);
        timer.transform.parent.GetChild(0).GetComponent<Animator>().enabled = false;

        bonusHolder.SetActive(true);
        foreach (Bonus bonus in bonusHolder.GetComponentsInChildren<Bonus>())
        {
            bonus.SetInitialPos();
            bonus.Restore();
        }
        //Debug.Log("Bonus START POS " + bonusHolder.transform.GetChild(0).transform.position);

        MakeStarFall();

        ThreadedTracking.gameState = ThreadedTracking.GAME_STATE.GAME;
    }

    void LastSprint()
    {
        timer.transform.parent.GetChild(0).GetComponent<Animator>().enabled = true;
    }

    public void ShowPopup()
    {
        areUSure.SetActive(true);
        gameState = GAME_STATE.INTRO;
    }

    public void NoPopup()
    {
        areUSure.SetActive(false);
        gameState = GAME_STATE.GAME;
    }

    public void IncreaseTime()
    {
        timeLeft += 60;
    }

    public void StopTracking()
    {
        areUSure.SetActive(false);
        runningStar.SetActive(false);
        bonusHolder.SetActive(false);
        if (dataRetreiver != null)
            dataRetreiver.Quit();
        start.gameObject.SetActive(true);
        pause.gameObject.SetActive(false);
        ThreadedTracking.gameState = ThreadedTracking.GAME_STATE.PLANETARIUM;

        ShowEndGamePanel();
    }

    void MakeStarFall()
    {
        Vector3 position = starPlaceholder.position;
        runningStar.SetActive(true);
        runningStar.transform.position = position;
        runningStar.GetComponent<Star>().manager = this;
        runningStar.GetComponent<Star>().SetLogicalPos(position - (playerPhysicalPosition - playerLogicalPosition));
        //runningStar.GetComponent<Star>().SetInitialPos();
        runningStar.GetComponent<Star>().FallAndRun();
    }

    void ShowEndGamePanel()
    {
        ARCamera.GetComponent<AudioController>().AudioinScore();

        mainPanel.SetActive(false);
        introCamera.SetActive(true);
        endPanel.SetActive(true);

        endPanel.GetComponent<Animator>().enabled = true;
        Invoke("ColorStars", endPanel.GetComponent<Animation>().clip.length);
    }

    void ColorStars()
    {
        endPanel.GetComponent<Animator>().enabled = false;
        Transform stars = endPanel.transform.GetChild(0).GetChild(0);
        for (int i = 0; i < caughtStars; i++)
            stars.GetChild(i).GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
        for (int i = caughtStars; i < totStars; i++)
            stars.GetChild(i).GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.55f);
    }

    public void GoBack()
    {
        endPanel.GetComponent<Animator>().enabled = false;

        mainPanel.SetActive(true);
        introCamera.SetActive(false);
        endPanel.SetActive(false);
        
        mainPanel.SetActive(true);
        timeLeft = time;
        caughtStars = 0;
        int minutes = (int)timeLeft / 60;
        int secs = (int)timeLeft % 60;
        timer.text = ((("" + minutes).Length == 1) ? "0" : "") + minutes + ":" + ((("" + secs).Length == 1) ? "0" : "") + secs;
        timer.transform.parent.GetChild(0).GetComponent<Animator>().enabled = false;
        timer.transform.parent.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = Color.white;
        scoreText.text = "0/5";

        ARCamera.GetComponent<AudioController>().AudioinDome();

        GoToPlanetarium();
    }

    public void GoAheadPresentation()
    {
        descriptionLables[textIndex].SetActive(false);
        textIndex++;
        if (textIndex < descriptionLables.Length)
            descriptionLables[textIndex].SetActive(true);
        else
        {
            starHaruko.SetActive(false);
            cloud.SetActive(false);
            GoToPlanetarium();
        }
    }

    void GoToPlanetarium()
    {
        gameState = GAME_STATE.PLANETARIUM;
        ARCamera.GetComponent<GyroController>().enabled = true;
    }

    public enum GAME_STATE
    {
        INTRO,
        PLANETARIUM,
        GAME
    }

    void OnApplicationQuit()
    {
        if (dataRetreiver != null)
            dataRetreiver.DefiniteQuit();
    }
}
