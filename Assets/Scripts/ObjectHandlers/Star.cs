using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : PositioningHandler
{

    public Transform ARCamera;

    bool haveToRun = false;
    bool hiding = false;
    bool grounded = false;

    float groundY = 0;

    void Update()
    {
        Vector3 position;
        float distance;

        if (CalculatePositionAndDistance(out position, out distance))
        {
            position.y = logicalPosition.y;
            this.transform.position = position;
            if(hiding)
                this.transform.LookAt(ARCamera);
        }
        
    }

    public void FallAndRun()
    {
        hiding = false;
        groundY = 0;
        haveToRun = true;
        grounded = false;
        StartCoroutine("Fall");
    }

    void OnMouseDown()
    {
        Debug.Log("Click");
        if (ThreadedTracking.gameState == ThreadedTracking.GAME_STATE.GAME) // && !havetorun)
        {
            //check distance
            hiding = false;
            manager.UpdateScore();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Ground") && haveToRun)
        {
            grounded = true;
            groundY = collision.gameObject.transform.position.y;

            Vector3 position = this.transform.position;

            //position.x += player.x + (Random.Range(1, 1.5f) * TrackingHandler.UNIT_PER_METER);
            position.x += (Random.Range(0.1f, 1f) * TrackingHandler.UNIT_PER_METER);
            //position.z += player.y + (Random.Range(1, 1.5f) * TrackingHandler.UNIT_PER_METER);
            position.z += (Random.Range(0.1f, 1f) * TrackingHandler.UNIT_PER_METER);

            haveToRun = false;
            StartCoroutine(Run(this.transform.position, position, 2));

        }
    }

    IEnumerator Fall()
    {
        float speed = 0.4f;
        
        while (!grounded)
        {
            transform.position = transform.position + (Vector3.down * speed);
            logicalPosition = logicalPosition + (Vector3.down * speed);
            yield return null;
        }
        
    }

    IEnumerator Run(Vector3 source, Vector3 target, float time)
    {
        float startTime = Time.time;
        float speed = 0.25f;

        float elapsedTime = 0;

        Vector3 sourceLogical = logicalPosition;
        Vector3 targetLogical = sourceLogical + (target - source); ;

        this.transform.LookAt(target);
        this.GetComponent<Animator>().SetBool("run", true);

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(source, target, (elapsedTime / time) * speed);
            logicalPosition = Vector3.Lerp(sourceLogical, targetLogical, (elapsedTime / time) * speed);

            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(0.05f);
        }

        Vector3 logicalNew = base.logicalPosition + (this.transform.position - source);
        logicalNew.y = groundY;

        //Debug.Log("STAR physical " + this.transform.position + " from " + source + " logical old " + logicalPosition + "logical new " + logicalNew);
        Debug.Log("STAR ARRIVED " + this.transform.position);
        logicalPosition = logicalNew;

        this.GetComponent<Animator>().SetBool("run", false);

        hiding = true;

    }

}
