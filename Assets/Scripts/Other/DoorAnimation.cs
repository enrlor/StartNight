using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour
{

    bool animate = false;

    public Transform doorSin;
    public Transform doorDx;
    public GameObject introPanel;
    public GameObject mainPanel;
    public GameObject introBackground;
    public GameObject introCamera;
    public GameObject StarHaruko;
    public GameObject ARCamera;
    public GameObject skydome;

    float rotationY;
    float speed = 20f;
    bool alreadyClicked = false;

    // Use this for initialization
    void Start()
    {
        rotationY = 0;
    }

    //IEnumerator AnimateCoroutine()
    //{
    //    while (animate)
    //    {
    //        if (rotationY <= 100)
    //        {
    //            float delta = speed * Time.deltaTime;
    //            rotationY += delta;

    //            doorSin.Rotate(new Vector3(0, -delta, 0));
    //            doorDx.Rotate(new Vector3(0, delta, 0));
    //        }
    //        else
    //        {
    //            animate = false;
    //            //Enable main game stuff
    //            ARCamera.SetActive(true);
    //            mainPanel.SetActive(true);
    //            StarHaruko.SetActive(true);

    //            //Disable intro stuff
    //            introCamera.SetActive(false);
    //            introPanel.SetActive(false);
    //            introBackground.SetActive(false);
    //            doorDx.transform.parent.transform.gameObject.SetActive(false);
    //        }

    //        if ((rotationY >= 25) && (introCamera.GetComponent<Camera>().fieldOfView >= 0))
    //            introCamera.GetComponent<Camera>().fieldOfView -= speed * Time.deltaTime;

    //        yield return null;
    //    }

    //}

    public void Animate()
    {
        doorDx.transform.parent.GetComponent<Animator>().enabled = true;
        float animTime = doorDx.transform.parent.GetComponent<Animation>().clip.length;
        //animTime /= 0.65f;
        doorDx.transform.parent.GetComponent<Animation>().Play();
        skydome.SetActive(true);
        Invoke("AnimateCamera", animTime / 2);
        //StartCoroutine("AnimateCoroutine");

    }

    void AnimateCamera()
    {
        introCamera.GetComponent<Animator>().enabled = true;
        float animTime = introCamera.GetComponent<Animation>().clip.length;
        //animTime /= 0.5f;
        introCamera.GetComponent<Animation>().Play();
        Invoke("GoAhead", animTime);
    }

    void GoAhead()
    {
        animate = false;
        //Enable main game stuff
        ARCamera.SetActive(true);
        mainPanel.SetActive(true);
        StarHaruko.SetActive(true);

        //Disable intro stuff
        introCamera.SetActive(false);
        introCamera.GetComponent<Camera>().fieldOfView = 60;
        introPanel.SetActive(false);
        introBackground.SetActive(false);
        doorDx.transform.parent.transform.gameObject.SetActive(false);
    }
}
