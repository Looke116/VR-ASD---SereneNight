using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;

public class AvatarProcessor : MonoBehaviour
{
    private AudioSource _audioSource;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        var avatarManage = GetComponent<RealtimeAvatarManager>();
        avatarManage.avatarCreated += ProcessAvatar;
    }

    private void ProcessAvatar(RealtimeAvatarManager avatarManager, RealtimeAvatar avatar, bool isLocalAvatar)
    {
        if (isLocalAvatar) 
        {
            avatar.head.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            _audioSource.Play();
        }
    }
}