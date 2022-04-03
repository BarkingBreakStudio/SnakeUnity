using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    [SerializeField]
    NorLib.Sound.AudioCueEventChannelSO srfChannel;
    [SerializeField]
    AudioClip upClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("PlayUpSfx")]
    void PlayUpSfx()
    {
        srfChannel?.Play(upClip);
    }
}
