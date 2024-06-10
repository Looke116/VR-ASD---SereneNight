using Normal.Realtime;
using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    private void Awake()
    {
        var realtimeView = GetComponent<RealtimeView>();
        
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        transform.localScale = scale;
    }
    
    
}