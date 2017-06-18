using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : PositioningHandler
{
    bool recharged = true;

    void Update()
    {
        Vector3 position;
        float distance;

        if (CalculatePositionAndDistance(out position, out distance))
        {
            //Debug.Log("FROM " + this.transform.position + " TO " + position);

            position.y = logicalPosition.y;
            this.transform.position = position;

            Color color = this.transform.GetComponentInChildren<Renderer>().material.color;
            if (distance <= 0)
                color.a = 1f;
            else if (distance >= DISAPPEARING_DISTANCE)
                color.a = 0f;
            else
                color.a = 1 - (0.5f * distance / (DISAPPEARING_DISTANCE / 2));
            this.transform.GetComponent<Renderer>().material.color = color;

        }

    }

    void OnMouseDown()
    {
        if ((ThreadedTracking.gameState == ThreadedTracking.GAME_STATE.GAME) && recharged)
        {
            manager.IncreaseTime();
            this.transform.GetComponent<Renderer>().material.color = Color.red;
            recharged = false;
            Invoke("Restore", 60);
        }
    }

    public void Restore()
    {
        recharged = true;
        this.transform.GetComponent<Renderer>().material.color = Color.green;
    }

}
