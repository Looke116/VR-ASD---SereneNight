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
    }

    float crtLat, crtLon;
    DateTime crtTime, newTime;
    
    public void Update()
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

    ParticleSystem.Particle[] particleStars;
    private float alt, az, dist;
    string[] components;
    public void LoadParticles(DateTime time, float latitude, float longitude)
    {
        String[] lines = fileContent.Split('\n');
        if (particleStars == null)
        {
            particleStars = new ParticleSystem.Particle[maxParticles];
        }
        particleSystem.GetParticles(particleStars);

        for (int i = 1; i < maxParticles; i++) // first line is the header
        {
            components = lines[i].Split(',');
            /*particleStars[i].position = new Vector3(-float.Parse(components[10], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                   float.Parse(components[11], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                    float.Parse(components[12], NumberStyles.Any, CultureInfo.InvariantCulture));
            particleStars[i].position = particleStars[i].position.normalized;
            particleStars[i].position *= 1000;// (Camera.main.farClipPlane-1);*/

            /*transform.eulerAngles = new Vector3(StarPosition.getAltitude(float.Parse(components[4], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             float.Parse(components[5], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             51, 0,
                                                                             DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour),
                                                    180 + StarPosition.getAzimuth(float.Parse(components[4], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             float.Parse(components[5], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             51, 0,
                                                                             DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour),
                                                    0);*/

            alt = StarPosition.getAltitude(float.Parse(components[4], NumberStyles.Any, CultureInfo.InvariantCulture) * 15,
                                                                             float.Parse(components[5], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             latitude, longitude,
                                                                             time) * Mathf.Deg2Rad;

            // added 90 to align it with the north of the scene
            az = (StarPosition.getAzimuth(float.Parse(components[4], NumberStyles.Any, CultureInfo.InvariantCulture) * 15,
                                                                             float.Parse(components[5], NumberStyles.Any, CultureInfo.InvariantCulture),
                                                                             latitude, longitude,
                                                                             time) + 90) * Mathf.Deg2Rad;
            dist = float.Parse(components[6], NumberStyles.Any, CultureInfo.InvariantCulture);
            //  https://math.stackexchange.com/questions/15323/how-do-i-calculate-the-cartesian-coordinates-of-stars
            particleStars[i].position = new Vector3(dist * Mathf.Cos(alt) * Mathf.Cos(az),
                                                    dist * Mathf.Cos(alt) * Mathf.Sin(az),
                                                    dist * Mathf.Sin(alt)
                                                    );
            particleStars[i].position = particleStars[i].position.normalized;
            particleStars[i].position *= 1500;

            particleStars[i].remainingLifetime = Mathf.Infinity;

            Stars.addStar(components[3].Trim() == "" ? components[0] : components[3], 
                            float.Parse(components[4]), 
                            float.Parse(components[5]), 
                            alt * Mathf.Rad2Deg, 
                            az * Mathf.Rad2Deg, 
                            float.Parse(components[7]), 
                            dist,
                            dist * Mathf.Cos(alt) * Mathf.Cos(az),
                            dist * Mathf.Cos(alt) * Mathf.Sin(az),
                             dist * Mathf.Sin(alt)
                            );
            
            particleStars[i].startSize = 200 * Mathf.Pow(10, (-1.44f - Stars.getVisualMagnitudeAfterExtinction(i-1)) / 5);
            //Debug.Log(Stars.GetStar(i - 1).getVisualMagnitude() + " " + Stars.getVisualMagnitudeAfterExtinction(i - 1) + " " + particleStars[i].startSize + " " + Stars.GetStar(i-1).getAltitude());

            switch (components[9][0])
            {
                case 'O':
                    particleStars[i].startColor = new Color(155f / 255f, 176f / 255f, 255f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '5':
                            particleStars[i].startColor = new Color(157f / 255f, 180f / 255f, 255f / 255f);
                            break;
                    }
                    break;
                case 'B':
                    particleStars[i].startColor = new Color(170f / 255f, 191f / 255f, 255f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '1':
                            particleStars[i].startColor = new Color(162f / 255f, 185f / 255f, 255f / 255f);
                            break;
                        case '3':
                            particleStars[i].startColor = new Color(167f / 255f, 188f / 255f, 255f / 255f);
                            break;
                        case '5':
                            particleStars[i].startColor = new Color(170f / 255f, 191f / 255f, 255f / 255f);
                            break;
                        case '8':
                            particleStars[i].startColor = new Color(175f / 255f, 195f / 255f, 255f / 255f);
                            break;
                    }
                    break;
                case 'A':
                    particleStars[i].startColor = new Color(202f / 255f, 215f / 255f, 255f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '1':
                            particleStars[i].startColor = new Color(186f / 255f, 204f / 255f, 255f / 255f);
                            break;
                        case '3':
                            particleStars[i].startColor = new Color(192f / 255f, 209f / 255f, 255f / 255f);
                            break;
                        case '5':
                            particleStars[i].startColor = new Color(202f / 255f, 216f / 255f, 255f / 255f);
                            break;
                    }
                    break;
                case 'F':
                    particleStars[i].startColor = new Color(248f / 255f, 247f / 255f, 255f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '0':
                            particleStars[i].startColor = new Color(228f / 255f, 232f / 255f, 255f / 255f);
                            break;
                        case '2':
                            particleStars[i].startColor = new Color(237f / 255f, 238f / 255f, 255f / 255f);
                            break;
                        case '5':
                            particleStars[i].startColor = new Color(251f / 255f, 248f / 255f, 255f / 255f);
                            break;
                        case '8':
                            particleStars[i].startColor = new Color(255f / 255f, 249f / 255f, 249f / 255f);
                            break;
                    }
                    break;

                case 'G':
                    particleStars[i].startColor = new Color(255f / 255f, 244f / 255f, 234f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '2':
                            particleStars[i].startColor = new Color(255f / 255f, 245f / 255f, 236f / 255f);
                            break;
                        case '5':
                            particleStars[i].startColor = new Color(255f / 255f, 244f / 255f, 232f / 255f);
                            break;
                        case '8':
                            particleStars[i].startColor = new Color(255f / 255f, 241f / 255f, 223f / 255f);
                            break;
                    }
                    break;
                case 'K':
                    particleStars[i].startColor = new Color(255f / 255f, 210f / 255f, 161f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '0':
                            particleStars[i].startColor = new Color(255f / 255f, 235f / 255f, 209f / 255f);
                            break;
                        case '4':
                            particleStars[i].startColor = new Color(255f / 255f, 215f / 255f, 174f / 255f);
                            break;
                        case '7':
                            particleStars[i].startColor = new Color(255f / 255f, 198f / 255f, 144f / 255f);
                            break;
                    }
                    break;
                case 'M':
                    particleStars[i].startColor = new Color(255f / 255f, 204f / 255f, 111f / 255f);
                    if (components[9].Length < 2)
                    {
                        break;
                    }
                    switch (components[9][1])
                    {
                        case '2':
                            particleStars[i].startColor = new Color(255f / 255f, 190f / 255f, 127f / 255f);
                            break;
                        case '4':
                        case '6':
                            particleStars[i].startColor = new Color(255f / 255f, 187f / 255f, 123f / 255f);
                            break;
                    }
                    break;
            }
        }
        particleSystem.SetParticles(particleStars, maxParticles);
 
    }

    // particles are drawn in the getSunPosition class
    public void UpdateStars(DateTime time, float latitude, float longitude)
    {
        /*if (particleStars == null)
        {
            particleStars = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
        }

        GetComponent<ParticleSystem>().GetParticles(particleStars);
        particleSystem.GetParticles(particleStars);
        */

        for (int i = 1; i < Stars.getNumberOfStars(); i++) // go through each Star and update its position
        {
            alt = StarPosition.getAltitude(Stars.GetStar(i).getRA() * 15,
                                                                                 Stars.GetStar(i).getDec(),
                                                                                 latitude, longitude,
                                                                                 time) * Mathf.Deg2Rad;
            // added 90 to align it with the north of the scene
            az = (StarPosition.getAzimuth(Stars.GetStar(i).getRA() * 15,
                                                                             Stars.GetStar(i).getDec(),
                                                                             latitude, longitude,
                                                                             time) + 90) * Mathf.Deg2Rad;
            dist = Stars.GetStar(i).getDistance();

            Stars.UpdateStar(i,
                alt * Mathf.Rad2Deg,
                az * Mathf.Rad2Deg,
                dist * Mathf.Cos(alt) * Mathf.Cos(az),
                            dist * Mathf.Cos(alt) * Mathf.Sin(az),
                             dist * Mathf.Sin(alt));

            /*particleStars[i].position = new Vector3(dist * Mathf.Cos(alt) * Mathf.Cos(az),
                                        dist * Mathf.Cos(alt) * Mathf.Sin(az),
                                        dist * Mathf.Sin(alt)
                                        );
            particleStars[i].position = particleStars[i].position.normalized;
            particleStars[i].position *= 1500;
            */
        }

        //particleSystem.SetParticles(particleStars, maxParticles);

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
