using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackingHandler : MonoBehaviour
{

    public UnityEngine.UI.Text ConsoleText;

    public bool walking = false;
    public bool release = true;

    //public UnityEngine.UI.Text Angle;
    public PathRenderer path;
    public float teta = 0;

    //considering a velocity of 2 km/h
    private const float METERS_PER_MILLISECOND = 0.0005f;
    public const int UNIT_PER_METER = 170;
    private const float MAGIC_NUMBER = 57.2958f;

    AndroidJavaObject currentActivity;
    Vector3 pathPosition;
    Vector3 playerPosition;


    long lasttime = 0;

    void Start()
    {
        if (release)
        {
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            //InvokeRepeating("CheckDirection", 0f, 1f);
        }

        pathPosition = path.gameObject.transform.position + Vector3.forward*100;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        path.Draw(pathPosition);
            
    }

    void Update()
    {
        if (release)
        {
            string s = currentActivity.Call<string>("getPeriod");

            if (s != null)
            {
                string[] values = s.Split(',');

                if (lasttime == 0)
                    lasttime = long.Parse(values[2]);

                float interval = long.Parse(values[2]) - lasttime;
                lasttime = long.Parse(values[2]);

                if (values[0].StartsWith("Walking"))
                {
                    Vector3 delta = CalculatePosition(interval);
                    pathPosition += delta;
                    playerPosition += delta;
                }
                ConsoleText.text = "Activity " + values[0] + " direction " + values[1] + " deltatime " + interval + "\n"
                    + "{" + pathPosition.x + ", " + pathPosition.y + "}";
            }
            teta = currentActivity.Call<float>("getDegreeFromNorth");
            //smarthphone in landscape mode.
            teta = (teta + 90) % 360;
        }
        else
        {
            if (!walking)
                return;
            Vector3 delta = CalculatePosition(Time.deltaTime * 100);
            pathPosition += delta;
            playerPosition += delta;
         //   Angle.text = teta + "";
        }
        
        path.Draw(pathPosition);
        //Debug.Log(playerPosition.x + " " + playerPosition.y);

    }

    Vector3 CalculatePosition(float interval)
    {
        //velocity 5km/h 
        float distance = ((interval * METERS_PER_MILLISECOND) * UNIT_PER_METER);

        float x = 0;
        float y = 0;
        
        if (teta == 90)
            x = distance;
        else if (teta == 180)
            y = -distance;
        else if (teta == 270)
            x = -distance;
        else if (teta == 0 || teta == 360)
            y = distance;
        else if(teta < 90)
        {
            x = distance * Mathf.Sin((teta) / MAGIC_NUMBER);
            y = distance * Mathf.Cos((teta) / MAGIC_NUMBER);
        }
        else if(teta > 270)
        {
            x = -(distance * Mathf.Sin((360 - teta) / MAGIC_NUMBER));
            y = distance * Mathf.Cos((360 - teta) / MAGIC_NUMBER);

        }
        else if(teta > 90 && teta < 180)
        {
            x = distance * Mathf.Sin((180 - teta) / MAGIC_NUMBER);
            y = -(distance * Mathf.Cos((180 - teta) / MAGIC_NUMBER));
        }
        else //if(direction > 180 && direction < 270)
        {
            x = -(distance * Mathf.Sin((teta - 180) / MAGIC_NUMBER));
            y = -(distance * Mathf.Cos((teta - 180) / MAGIC_NUMBER));
        }
                       
        return new Vector3(x, y, 0);
    }

    public Vector3 getPlayerRelativePosition() {
        return playerPosition;
    }

}
