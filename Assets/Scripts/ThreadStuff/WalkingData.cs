using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingData
{

    AndroidJavaObject currentActivity;

    private object handle = new object();
    System.Threading.Thread Thread = null;

    Vector3 playerPosition;
    volatile bool walking = false;
    volatile bool playing = false;
    volatile bool quit = false;

    //considering a velocity of 2 km/h
    const float METERS_PER_SECOND = 0.5f;
    const int UNIT_PER_METER = 4;
    const float MAGIC_NUMBER = 57.2958f;
    
    public void Start(AndroidJavaObject act, Vector3 startPosition)
    {
        currentActivity = act;
        walking = false;
        lock (handle)
            playerPosition = startPosition;

        if (Thread == null)
        {
            Thread = new System.Threading.Thread(Run);
            Thread.Start();
        }
        else
            playing = true;

    }

    public void Quit()
    {
        playing = false;
    }

    public void DefiniteQuit()
    {
        quit = true;
    }

    protected void Job()
    {
        playing = true;
        while (!quit)
        {
            AndroidJNI.AttachCurrentThread();

            long lasttime = 0;
            //playing = true;
            float i = 0;

            while (playing)
            {
                string s = currentActivity.Call<string>("getPeriod");
                //string s = "No,0," + i;
                //i += 0.5f;

                if (s != null)
                {
                    string[] values = s.Split(',');

                    if (lasttime == 0)
                    {
                        lasttime = long.Parse(values[2]);
                        continue;
                    }

                    float interval = long.Parse(values[2]) - lasttime;
                    //Debug.Log("INTERVAL " + interval + " lasttime " + lasttime + " current " + long.Parse(values[2]));
                    lasttime = long.Parse(values[2]);

                    float teta = currentActivity.Call<float>("getDegreeFromNorth");
                    //float teta = 0;
                    //smarthphone in landscape mode.
                    teta = (teta + 90) % 360;

                    if (values[0].StartsWith("Walking"))
                    {
                        walking = true;
                        Vector3 delta = CalculatePosition(teta, interval);
                        lock (handle)
                            playerPosition += delta;
                    }
                    else
                        walking = false;

                }
            }//end while
            AndroidJNI.DetachCurrentThread();
        }

        //OnFinished();
    }

    protected void OnFinished()
    {
        Thread.Abort();
    }

    private void Run()
    {
        Job();
        playing = true;
    }

    public bool getPlayerRelativePosition(out Vector3 position)
    {
        position = Vector3.zero;
        if (playing)
        {
            position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
            return true;
        }
        else
            return false;
    }

    public bool isWalking()
    {
        if (playing)
            return walking;
        else
            return false;
    }

    public bool isPlaying()
    {
        return (playing);
    }

    Vector3 CalculatePosition(float teta, float interval)
    {
        //velocity 5km/h 
        float distance = (METERS_PER_SECOND / (interval * 0.001f) ) * UNIT_PER_METER;

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
        else if (teta < 90)
        {
            x = distance * Mathf.Sin((teta) / MAGIC_NUMBER);
            y = distance * Mathf.Cos((teta) / MAGIC_NUMBER);
        }
        else if (teta > 270)
        {
            x = -(distance * Mathf.Sin((360 - teta) / MAGIC_NUMBER));
            y = distance * Mathf.Cos((360 - teta) / MAGIC_NUMBER);

        }
        else if (teta > 90 && teta < 180)
        {
            x = distance * Mathf.Sin((180 - teta) / MAGIC_NUMBER);
            y = -(distance * Mathf.Cos((180 - teta) / MAGIC_NUMBER));
        }
        else //if(direction > 180 && direction < 270)
        {
            x = -(distance * Mathf.Sin((teta - 180) / MAGIC_NUMBER));
            y = -(distance * Mathf.Cos((teta - 180) / MAGIC_NUMBER));
        }

        return new Vector3(x, 0, y);
    }
}
