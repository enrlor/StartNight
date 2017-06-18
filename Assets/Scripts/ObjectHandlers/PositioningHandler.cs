using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PositioningHandler : MonoBehaviour {

    public ThreadedTracking manager;
    //public UnityEngine.UI.Text Distance;

    //value in meters
    protected const float DISAPPEARING_DISTANCE = 7f;
    protected Vector3 logicalPosition;
    protected float distance = 0f;
    // Use this for initialization
    void Awake () {
        //logicalPosition = this.transform.position;
        logicalPosition = transform.position;
    }

    public void SetLogicalPos(Vector3 p)
    {
        logicalPosition = p;
    }

    public void SetInitialPos()
    {
        this.transform.position = logicalPosition;
    }

    public bool CalculatePositionAndDistance(out Vector3 position, out float distance)
    {
        position = Vector3.zero;
        distance = 0f;
        //we don't use hight which in my calculations is z, while in the world coordinates system is y
        //so SystemY = myZ and SystemZ = myY
        Vector3 delta;
        Vector3 playerLogicalPos;

        if (manager.GetPlayerDeltaPosition(out delta, out playerLogicalPos))
        {
            
            //objectPhysicalPos = objLogicalPos + (playerPhysicalPos - playerLogicalPos);
            position.x = logicalPosition.x + delta.x;
            position.z = logicalPosition.z + delta.z;
            
            distance = Mathf.Sqrt(Mathf.Pow((position.x - playerLogicalPos.x), 2) + Mathf.Pow((position.z - playerLogicalPos.z), 2)) / TrackingHandler.UNIT_PER_METER;

            return true;
        }
        return false;
    }

}
