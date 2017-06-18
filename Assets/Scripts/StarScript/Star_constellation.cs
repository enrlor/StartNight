using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star_constellation : MonoBehaviour
{
    public GameObject popup;
    public UnityEngine.UI.Text Constellation;
    public GameObject firingPoint;
    Vector3 rotationCenter;
    string constellation = "";

    // Use this for initialization
    void Start() { }

    public void set_constellation(string cons)
    {
        constellation = cons;
    }

    public string get_constellation()
    {
        return constellation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
        if(ThreadedTracking.gameState == ThreadedTracking.GAME_STATE.PLANETARIUM)
        {
            Constellation.text = constellation;
            popup.gameObject.SetActive(true);
            StartCoroutine("WaitSeconds");
        }
    }

    IEnumerator WaitSeconds()
    {
        yield return (new WaitForSeconds(2));
        popup.gameObject.SetActive(false);
    }
}