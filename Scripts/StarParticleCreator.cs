using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using stardata;
using UnityEngine.UI;

// Based on https://thomaskole.nl/2016/07/01/star-field-generator/
// For realistic Sun https://www.youtube.com/watch?v=BUi2PpR4u6o
// For star colors http://www.vendian.org/mncharity/dir3/starcolor/
public class StarParticleCreator : MonoBehaviour
{

    private ParticleSystem particleSystem;
    [SerializeField] public int maxParticles; //= 8905;
    [SerializeField] public GameObject timeGameObject;
    private XRTime xrTime;
    private float latitude;
    private float longitude;

    private float fadeLevel;
    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        var main = particleSystem.main; main.maxParticles = maxParticles;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
        bursts[0].minCount = (short)maxParticles;
        bursts[0].maxCount = (short)maxParticles;
        bursts[0].time = 0.0f;
        particleSystem.emission.SetBursts(bursts, 1);
    }

    String fileContent;
    
    void Start()
    {
        xrTime = timeGameObject.GetComponent<XRTime>();
        latitude = xrTime.getLatitude();
        longitude = xrTime.getLongitude();

        fileContent = Resources.Load<TextAsset>("stars").text;

        LoadParticles(xrTime.getTime(), latitude, longitude);
        crtLat = latitude;
        crtLon = longitude;
        crtTime = xrTime.getTime();

        UpdateStars(newTime, latitude, longitude);
    }

    float crtLat, crtLon;
    DateTime crtTime, newTime;
    
    public void LateUpdate()
    {
        newTime = xrTime.getTime();
        // only update when the lat/lon have changed
        if (crtLat != latitude || crtLon != longitude || (Math.Abs((crtTime - newTime).TotalSeconds) > 1))
        {
            UpdateStars(newTime, latitude, longitude);
            crtTime = newTime;
            crtLat = latitude;
            crtLon = longitude;
        }
    }

    private float alt, az, dist, r, g, b;
    string[] components;
    public void LoadParticles(DateTime time, float latitude, float longitude)
    {
        String[] lines = fileContent.Split('\n');

        for (int i = 1; i < maxParticles; i++) // first line is the header
        {
            components = lines[i].Split(',');

            alt = StarPosition.getAltitude(float.Parse(components[4], NumberStyles.Any, CultureInfo.InvariantCulture) * 15,
                                                                             float.Parse(components[5], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             latitude, longitude,
                                                                             time) * Mathf.Deg2Rad;
            az = StarPosition.getAzimuth(float.Parse(components[4], NumberStyles.Any, CultureInfo.InvariantCulture) * 15,
                                                                             float.Parse(components[5], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             latitude, longitude,
                                                                             time) * Mathf.Deg2Rad;
            dist = float.Parse(components[6], NumberStyles.Any, CultureInfo.InvariantCulture);
            //  https://math.stackexchange.com/questions/15323/how-do-i-calculate-the-cartesian-coordinates-of-stars
            
            switch (components[9][0])
            {
                case 'O':
                    r = 155f / 255f;
                    g = 176f / 255f;
                    b = 255f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '5':
                            r = 157f / 255f;
                            g = 180f / 255f;
                            b = 255f / 255f;
                            break;
                    }
                    break;
                case 'B':
                    r = 170f / 255f;
                    g = 191f / 255f;
                    b = 255f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '1':
                            r = 162f / 255f;
                            g = 185f / 255f;
                            b = 255f / 255f;
                            break;
                        case '3':
                            r = 167f / 255f;
                            g = 188f / 255f;
                            b = 255f / 255f;
                            break;
                        case '5':
                            r = 170f / 255f;
                            g = 191f / 255f;
                            b = 255f / 255f;
                            break;
                        case '8':
                            r = 175f / 255f;
                            g = 195f / 255f;
                            b = 255f / 255f;
                            break;
                    }
                    break;
                case 'A':
                    r = 202f / 255f;
                    g = 215f / 255f;
                    b = 255f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '1':
                            r = 186f / 255f;
                            g = 204f / 255f;
                            b = 255f / 255f;
                            break;
                        case '3':
                            r = 192f / 255f;
                            g = 209f / 255f;
                            b = 255f / 255f;
                            break;
                        case '5':
                            r = 202f / 255f;
                            g = 216f / 255f;
                            b = 255f / 255f;
                            break;
                    }
                    break;
                case 'F':
                    r = 248f / 255f;
                    g = 247f / 255f;
                    b = 255f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '0':
                            r = 228f / 255f;
                            g = 232f / 255f;
                            b = 255f / 255f;
                            break;
                        case '2':
                            r = 237f / 255f;
                            g = 238f / 255f;
                            b = 255f / 255f;
                            break;
                        case '5':
                            r = 251f / 255f;
                            g = 248f / 255f;
                            b = 255f / 255f;
                            break;
                        case '8':
                            r = 255f / 255f;
                            g = 249f / 255f;
                            b = 249f / 255f;
                            break;
                    }
                    break;

                case 'G':
                    r = 255f / 255f;
                    g = 244f / 255f;
                    b = 234f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '2':
                            r = 255f / 255f;
                            g = 245f / 255f;
                            b = 236f / 255f;
                            break;
                        case '5':
                            r = 255f / 255f;
                            g = 244f / 255f;
                            b = 232f / 255f;
                            break;
                        case '8':
                            r = 255f / 255f;
                            g = 241f / 255f;
                            b = 223f / 255f;
                            break;
                    }
                    break;
                case 'K':
                    r = 255f / 255f;
                    g = 210f / 255f;
                    b = 161f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '0':
                            r = 255f / 255f;
                            g = 235f / 255f;
                            b = 209f / 255f;
                            break;
                        case '4':
                            r = 255f / 255f;
                            g = 215f / 255f;
                            b = 174f / 255f;
                            break;
                        case '7':
                            r = 255f / 255f;
                            g = 198f / 255f;
                            b = 144f / 255f;
                            break;
                    }
                    break;
                case 'M':
                    r = 255f / 255f;
                    g = 204f / 255f;
                    b = 111f / 255f;
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '2':
                            r = 255f / 255f;
                            g = 190f / 255f;
                            b = 127f / 255f;
                            break;
                        case '4':
                        case '6':
                            r = 255f / 255f;
                            g = 187f / 255f;
                            b = 123f / 255f;
                            break;
                    }
                    break;
            }
            Stars.addStar(components[3].Trim() == "" ? components[0] : components[3],
                            float.Parse(components[4]),
                            float.Parse(components[5]),
                            alt * Mathf.Rad2Deg,
                            az * Mathf.Rad2Deg,
                            float.Parse(components[7]),
                            dist,
                            dist * Mathf.Cos(alt) * Mathf.Cos(az),
                            dist * Mathf.Cos(alt) * Mathf.Sin(az),
                             dist * Mathf.Sin(alt),
                             r, g, b
                            );
        } 
    }

    Star star;

    // particles are drawn in the getSunPosition class
    public void UpdateStars(DateTime time, float latitude, float longitude)
    {
        Stars.resetNumberVisibleStars();
        for (int i = 1; i < Stars.getNumberOfStars(); i++) // go through each Star and update its position
        {
            star = Stars.GetStar(i);
            alt = StarPosition.getAltitude(star.getRA() * 15,
                                                                                 star.getDec(),
                                                                                 latitude, longitude,
                                                                                 time) * Mathf.Deg2Rad;
            az = (StarPosition.getAzimuth(star.getRA() * 15,
                                                                             star.getDec(),
                                                                             latitude, longitude,
                                                                             time) + 90) * Mathf.Deg2Rad; // +90 to correct for scene orientation
            dist = star.getDistance();

            Stars.UpdateStar(i,
                alt * Mathf.Rad2Deg,
                az * Mathf.Rad2Deg,
                dist * Mathf.Cos(alt) * Mathf.Cos(az),
                            dist * Mathf.Cos(alt) * Mathf.Sin(az),
                             dist * Mathf.Sin(alt));

            if (star.getAltitude() > 0)
            {
                Stars.incrementVisibleStars();
            }
        }
    }

    String ReadTextFile(string file_path)
    {
        String text = "";
        StreamReader inp_stm = new StreamReader(file_path);

        int i = 0;
        while (!inp_stm.EndOfStream)
        {
            text += inp_stm.ReadLine() + "\n";
        }

        inp_stm.Close();
        return text;
    }
}
