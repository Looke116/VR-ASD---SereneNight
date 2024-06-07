using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private GameObject barrierPrefab;
    [SerializeField] private List<GameObject> barriers;
    [SerializeField] private float radius = 5f;

    [ContextMenu("Place Barrier")]
    private void PlaceBarriers()
    {
        foreach (var barrier in barriers)
        {
            if (barrier != null)
                DestroyImmediate(barrier);
        }
        barriers.Clear();

        float angleStep = 360f / 6;
        float angle = 0f;

        for (int i = 0; i < 6; i++)
        {
            Vector3 spawnPosition = transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
            GameObject barrier = Instantiate(barrierPrefab, spawnPosition, Quaternion.Euler(0, angle + 90, 0), transform);
            barriers.Add(barrier);

            angle += angleStep;
        }
    }

    [ContextMenu("Set Barrier Scale")]
    private void SetBarrierScale(float scaleWidth, float scaleHeight)
    {
        foreach (var barrier in barriers)
        {
            barrier.transform.localScale = new Vector3(scaleWidth, scaleHeight, 1f);
        }
    }

}
