using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestPlacer : MonoBehaviour
{
    // Script Inspiration https://www.youtube.com/watch?v=Bv1foWFahM4

    [SerializeField] private List<GameObject> forestPrefabs;

    [SerializeField] private List<GameObject> forests;

    [SerializeField] private float scaleWidth, scaleHeight, distance;

    [SerializeField] private LayerMask terrainLayer;

    [ContextMenu("Place Forest")]
    private void PlaceForest()
    {
        foreach(var forest in forests)
        {
            if (forest != null)
                DestroyImmediate(forest);
        }
        forests.Clear();

        int prefabIndex = 0;
        float angle = 0f;

        for(int i = 0; i < 8; i++)
        {
            Vector3 spawnPosition = transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition + Vector3.up * 50, Vector3.down, out hit, 100, terrainLayer))
            {
                GameObject forest = Instantiate(forestPrefabs[prefabIndex], hit.point, Quaternion.Euler(0, angle, 0), transform);
                forests.Add(forest);
            }

            angle += 45f;
            prefabIndex++;
            if(prefabIndex >= forestPrefabs.Count)
                prefabIndex = 0;
        }
    }


    [ContextMenu("Set Forest Scale")]
    private void SetForestScale()
    {
        foreach(var forest in forests)
        {
            forest.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1f);
            float spriteLength = forests[0].GetComponent<SpriteRenderer>().bounds.size.x;
            distance = spriteLength / 2f + (Mathf.Sqrt(2) / 2) * spriteLength;
            forest.transform.position = distance * forest.transform.forward;
        }
    }

    private void OnValidate()
    {
        if (forests.Count == 8)
        {
            SetForestScale();
        }
    }
}
