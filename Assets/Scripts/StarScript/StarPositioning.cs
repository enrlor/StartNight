using UnityEngine;
using System.Collections;
using System;
using System.Globalization;
using System.IO;

public class StarPositioning : MonoBehaviour
{
    int count = 0;
    int threshold = 400;
    double latitude;
    double longitude;

    public GameObject star;
    public GameObject firingPoint;
    //Vector3 rotationCenter;
    public GameObject popup;
    public UnityEngine.UI.Text Constellation;

    // Use this for initialization
    IEnumerator Start()
    {
        //rotationCenter = firingPoint.transform.rotation.eulerAngles;

        //geolocalization
        if (!Input.location.isEnabledByUser)// First, check if user has location service enabled
        {
            SetDefaultLatitudeLongitude();
        }
        else
        {
            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }
            if (maxWait < 1)
            {
                SetDefaultLatitudeLongitude();
            }
            else
            {
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    SetDefaultLatitudeLongitude();
                }
                else
                {
                    latitude = Input.location.lastData.latitude;
                    longitude = Input.location.lastData.longitude;
                }
            }
            Input.location.Stop();
        }
        /*
        //Set longitude and latitude
        latitude = 40;
        longitude = 13;*/

        //visible stars in the sky given a lat, long
        DateTime local_solar_time = DateTime.Now;
        //DateTime local_solar_time = new DateTime(2017, 3, 8, 14, 14, 0);

        double julian_date = 367 * local_solar_time.Year -
           (int)((7.0 / 4.0) * (local_solar_time.Year + (int)((local_solar_time.Month + 9.0) / 12.0))) +
           (int)((275.0 * local_solar_time.Month) / 9.0) +
           local_solar_time.Day + 1721013.5 +
           (int)local_solar_time.TimeOfDay.TotalHours / 24
           - 0.5 * Math.Sign(100 * local_solar_time.Year + local_solar_time.Month - 190002.5) + 0.5;

        double d = julian_date - 2451545.0;
        double LST = 100.46 + 0.985647 * d + longitude + 15 * local_solar_time.TimeOfDay.TotalHours;
        //sidereal hours
        LST = ((LST % 360) / 15) - 1;

        //double RA = 3.663106 * 15;
        double RA;
        double DEC_rad;
        double H;
        double MAG;
        double sinALT, ALT, cosA, A, AZ;

        TextAsset textAsset = Resources.Load("db_star_rand") as TextAsset;

        String[] lines = textAsset.text.Split('\n');
        int linesNumber = lines.Length;
        String line;
        for (var i = 1; i < linesNumber - 1; i++)
        {
            line = lines[i];
            var values = line.Split(',');

            DEC_rad = double.Parse(values[2], CultureInfo.InvariantCulture);
            //RA = double.Parse(values[1], CultureInfo.InvariantCulture);
            RA = double.Parse(values[0], CultureInfo.InvariantCulture) * 15;
            MAG = double.Parse(values[1], CultureInfo.InvariantCulture);

            //Debug.Log("DEC " + DEC_rad + " RA " + RA + " MAG " + MAG  + " Costellazione : " + values[3]);
            if (MAG >= 4)
                if (count >= threshold || UnityEngine.Random.Range(0, 100) % 2 == 0)
                {
                    continue;
                }
            H = LST * 15 - RA;
            if (Math.Sign(H) == -1)
                H = 360 + H;
            //rad
            H = H / 57.2958;

            sinALT = Math.Sin(DEC_rad) * Math.Sin(latitude / 57.2958) + Math.Cos(DEC_rad) * Math.Cos(latitude / 57.2958) * Math.Cos(H);
            ALT = Math.Asin(sinALT);

            //if ((ALT * 57.2958) >= 0 && (ALT * 57.2958) <= 180)
            if ((ALT * 57.2958) >= 0)
            {
                cosA = (Math.Sin(DEC_rad) - sinALT * Math.Sin(latitude / 57.2958)) / (Math.Cos(ALT) * Math.Cos(latitude / 57.2958));
                A = Math.Acos(cosA) * 57.2958;

                if (Math.Sign(Math.Sin(H)) == 1)
                    AZ = 360 - A;
                else AZ = A;
                ALT = ALT * 57.2958;

                //star positioning on dome

                RaycastHit hit;

                firingPoint.transform.rotation = Quaternion.identity;
                //firingPoint.transform.Rotate((float)AZ, (float)ALT,0);
                firingPoint.transform.Rotate(-(float)ALT, (float)AZ, 0);

                if (Physics.Raycast(firingPoint.transform.position, firingPoint.transform.forward, out hit))
                {
                    GameObject starClone = Instantiate(star, hit.point, Quaternion.identity);
                    starClone.GetComponent<Star_constellation>().firingPoint = firingPoint;
                    starClone.GetComponent<Star_constellation>().popup = popup;
                    starClone.GetComponent<Star_constellation>().Constellation = Constellation;
                    if (MAG < 0) //sun
                    {
                        starClone.transform.localScale += new Vector3(10, 10, 10);
                        starClone.transform.GetChild(0).transform.localScale += new Vector3(1000, 1000, 1000);
                        starClone.transform.LookAt(firingPoint.transform);
                        starClone.transform.position += starClone.transform.forward * 100;
                    }
                    else if (MAG >= 0 && MAG < 4)
                    {

                        starClone.transform.GetChild(0).transform.localScale += new Vector3(150, 150, 150);
                        starClone.transform.LookAt(firingPoint.transform);
                        starClone.transform.position += starClone.transform.forward * 2;
                    }
                    else
                    {
                        starClone.transform.GetChild(0).transform.localScale += new Vector3(5, 5, 5);
                        starClone.transform.LookAt(firingPoint.transform);
                        starClone.transform.position += starClone.transform.forward * 2;
                        count++;
                    }
                    starClone.transform.localScale += new Vector3(5, 5, 5);
                    starClone.GetComponent<Star_constellation>().set_constellation(values[3]);
                }
                //firingPoint.transform.rotation.eulerAngles.Set(0,0,0);

            }
        }
        //Debug.Log(count);

    }

    void SetDefaultLatitudeLongitude()
    {
        latitude = 30;
        longitude = 30;
    }
    // Update is called once per frame
    void Update()
    {

    }


}