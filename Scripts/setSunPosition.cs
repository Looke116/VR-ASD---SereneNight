using UnityEngine;
using SunCalcNet;
using SunCalcNet.Model;
using System;
using stardata;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;

public class setSunPosition : MonoBehaviour
{
    [SerializeField] int hour = DateTime.Now.Hour;
    [SerializeField] int minute = DateTime.Now.Minute;
    [SerializeField] private Transform stars;
    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;

    // set the main camera in the inspector
    [SerializeField] public Camera MainCamera;

    [SerializeField] public GameObject timeGameObject;

    private XRTime xrTime;
    private float latitude;
    private float longitude;

    DateTime time;
    SunPosition sunPos;
    // Start is called before the first frame update
    void Start()
    {
		xrTime = timeGameObject.GetComponent<XRTime>();
        latitude = xrTime.getLatitude();
        longitude = xrTime.getLongitude();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSunPosition();
    }

    ParticleSystem.Particle[] particles;
    float fadeStarsValue;
    float z;
    private void UpdateSunPosition()
    {
        time = xrTime.getTime();

        sunPos = SunCalc.GetSunPosition(time, latitude, longitude);

        transform.eulerAngles = new Vector3((float)(sunPos.Altitude) * Mathf.Rad2Deg, 180 + (float)sunPos.Azimuth * Mathf.Rad2Deg, 0);

        // fade stars according to the Sun's altitude
        if (sunPos.Altitude * Mathf.Rad2Deg <= 0 && sunPos.Altitude * Mathf.Rad2Deg >= -12f)
        {
            fadeStarsValue = (Mathf.Abs((float)sunPos.Altitude) * Mathf.Rad2Deg) / 12f;
        }
        if (sunPos.Altitude * Mathf.Rad2Deg > 0)
        {
            fadeStarsValue = 0f;
        }
        if (sunPos.Altitude * Mathf.Rad2Deg < -12f)
        {
            fadeStarsValue = 1f;
        }

        if (particles == null)
        {
            particles = new ParticleSystem.Particle[stars.GetComponent<ParticleSystem>().particleCount];
        }

        // Render stars
        stars.GetComponent<ParticleSystem>().GetParticles(particles);
        for (int p = 0; p < particles.Length; p++)
        {
            if (p < Stars.getNumberOfStars())
            {
                particles[p].position = new Vector3(Stars.GetStar(p).getX(), Stars.GetStar(p).getY(), Stars.GetStar(p).getZ());
                particles[p].position = particles[p].position.normalized;
                particles[p].position *= 1500;

                particles[p].startSize = 200 * Mathf.Pow(10, (-1.44f - Stars.getVisualMagnitudeAfterExtinction(p)) / 5);

            }
            particles[p].startColor = new Color(particles[p].startColor.r / 255f, particles[p].startColor.g / 255f, particles[p].startColor.b / 255f, fadeStarsValue);//where a = the alpha
        }
        stars.GetComponent<ParticleSystem>().SetParticles(particles, particles.Length);
    }
}
