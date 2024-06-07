using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCasting : MonoBehaviour
{
    // Helper script to place the object on the terrain level
    public float heightAboveTerrain = 1.0f; 

    void Update()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 500, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Terrain"))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + heightAboveTerrain, transform.position.z);
            }
        }
    }
}
