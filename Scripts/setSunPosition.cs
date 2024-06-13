using UnityEngine;
using SunCalcNet;
using SunCalcNet.Model;
using System;
using stardata;
using static UnityEngine.ParticleSystem;
using UnityEngine.UI;

public class setSunPosition : MonoBehaviour
{
    [SerializeField] private Transform stars;

    [SerializeField] public GameObject timeGameObject;

    [SerializeField] public GameObject meteor;

    private XRTime xrTime;
    private float latitude;
    private float longitude;

    DateTime time;
    SunPosition sunPos;
    ParticleSystem starParticles;

    // Start is called before the first frame update
    void Start()
    {
        if (meteor != null)
        {
            meteor.SetActive(false);
        }
		xrTime = timeGameObject.GetComponent<XRTime>();
        latitude = xrTime.getLatitude();
        longitude = xrTime.getLongitude();
        starParticles = stars.GetComponent<ParticleSystem>();
    }

    float crtLat, crtLon;
    DateTime crtTime, newTime;
    private float updateInterval = 1f / 30f; // Interval for 30 FPS
    private float lastUpdateTime = 0f;

    void LateUpdate()
    {
        newTime = xrTime.getTime();

        // Check if enough time has passed t o update
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            // only update when the lat/lon have changed or enough time has passed
            if (crtLat != latitude || crtLon != longitude || (Math.Abs((newTime - crtTime).TotalSeconds) > 1))
            {
                UpdateSunPosition();
                crtTime = newTime;
                crtLat = latitude;
                crtLon = longitude;
            }
            lastUpdateTime = Time.time;
        }
    }

    ParticleSystem.Particle[] particles;
    float fadeStarsValue;
    Star star;
    bool firstPass = true;
    MainModule main;
    private void UpdateSunPosition()
    {
        time = xrTime.getTime();

        sunPos = SunCalc.GetSunPosition(time, latitude, longitude);

        if (sunPos.Altitude < 0) // remove Terrain from culling mask (lights do not affect the environment)
        {
            GetComponent<Light>().cullingMask &= ~(1 << LayerMask.NameToLayer("Terrain"));
            GetComponent<Light>().cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

            if (meteor != null)
            {
                meteor.SetActive(true);
            }
        }
        else // add Terrain to culling mask (lights affect the envrionment)
        {
            GetComponent<Light>().cullingMask |= 1 << LayerMask.NameToLayer("Terrain");
            GetComponent<Light>().cullingMask |= 1 << LayerMask.NameToLayer("Player");

        }

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

        // Render stars
        particles = new ParticleSystem.Particle[Stars.getNumberVisibleStars()];
        //starParticles.GetParticles(particles);
        int p = 0;
        for (int i = 0; i < Stars.getNumberOfStars(); i++)
        {
            if (p >= particles.Length)
                continue;
            star = Stars.GetStar(i);
            if (star.isVisible() || firstPass)
            {
                particles[p].position = new Vector3(star.getX(), star.getY(), star.getZ());
                particles[p].position = particles[p].position.normalized;
                particles[p].position *= 1500;

                particles[p].startSize = 200 * Mathf.Pow(10, (-1.44f - Stars.getVisualMagnitudeAfterExtinction(p)) / 5);
            }
            particles[p].startColor = new Color(star.getR(), star.getG(), star.getB(), fadeStarsValue * particles[p].startSize /*brighest star become visible first*/);//where a = the alpha
            p++;
        }
        firstPass = false;
        main = starParticles.main;
        main.maxParticles = particles.Length;
        starParticles.SetParticles(particles, particles.Length);
    }
}
