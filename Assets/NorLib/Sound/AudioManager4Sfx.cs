using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace NorLib.Sound
{
    public class AudioManager4Sfx : MonoBehaviour
    {

        [SerializeField]
        private int NumOfInitialTracks = 4;
        [SerializeField]
        private int NumOfMusicTracks = 0;
        [SerializeField]
        private AudioMixerGroup audioMixerGrp;

        Queue<AudioSource> unusedTracks = new Queue<AudioSource>();
        List<AudioSource> activeTracks = new List<AudioSource>();

        private void OnEnable()
        {
            while (unusedTracks.Count < NumOfInitialTracks)
            {
                unusedTracks.Enqueue(CreateNewTrack());
            }
        }


        public virtual void PlayMusic(AudioClip clip)
        {
            AudioSource track = ReserveTrack();
            track.clip = clip;
            track.Play();
            activeTracks.Add(track);
        }

        private AudioSource ReserveTrack()
        {
            if (unusedTracks.Count > 0)
            {
                return unusedTracks.Dequeue();
            }
            else
            {
                return CreateNewTrack();
            }
        }

        private AudioSource CreateNewTrack()
        {
            var gm = new GameObject("Track_" + NumOfMusicTracks++);
            gm.transform.parent = transform;
            AudioSource source = gm.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = audioMixerGrp;
            return source;
        }

        void Update()
        {
            for (int i = activeTracks.Count - 1; i >= 0; i--)
            {
                if (!activeTracks[i].isPlaying)
                {
                    unusedTracks.Enqueue(activeTracks[i]);
                    activeTracks.RemoveAt(i);
                }
            }
        }

        public void SetAudioMixerGrp(AudioMixerGroup grp)
        {
            audioMixerGrp = grp;
            var allTracks = new List<AudioSource>();
            allTracks.AddRange(unusedTracks);
            allTracks.AddRange(activeTracks);
            foreach (var track in allTracks)
            {
                track.outputAudioMixerGroup = grp;
            }
        }
    }
}