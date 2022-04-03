using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NorLib.Sound
{

    [CreateAssetMenu(menuName = "BasicTools/Audio/Audio Cue Channel")]
    public class AudioCueEventChannelSO : ScriptableObject
    {

        public event UnityAction<AudioCueReqest> AudioCueRequested = delegate { };

        public void Play(AudioClip clip)
        {
            AudioCueRequested.Invoke(new AudioCueReqest { clip = clip });
        }

        [System.Serializable]
        public struct AudioCueReqest
        {
            public AudioClip clip;
        }
    }
}