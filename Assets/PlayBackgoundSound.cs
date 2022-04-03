using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBackgoundSound : MonoBehaviour
{
    [SerializeField]
    NorLib.Sound.AudioCueEventChannelSO musicChannel;
    [SerializeField]
    AudioClip backgroundMusic;


    // Start is called before the first frame update
    void Start()
    {
        musicChannel.Play(backgroundMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
