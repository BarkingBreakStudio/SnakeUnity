using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace NorLib.Sound
{
    public class AudioManager4Music : MonoBehaviour
    {

        public float MusicTransitionTime = 1f;
        [SerializeField]
        private int NumOfInitialTracks = 2;
        [SerializeField]
        private int NumOfMusicTracks = 0;
        [SerializeField] AudioMixerGroup audioMixerGrp;

        private class MusicTrackState
        {
            public AudioSource source;
            public bool VolumeIncreasing;
        }

        [SerializeField]
        private Queue<MusicTrackState> unusedTracks = new Queue<MusicTrackState>();
        private MusicTrackState activeTrack = null;
        [SerializeField]
        private List<MusicTrackState> unloadTracks = new List<MusicTrackState>();


        private void OnEnable()
        {
            while (unusedTracks.Count < NumOfInitialTracks)
            {
                unusedTracks.Enqueue(CreateNewTrack());
            }
        }


        public virtual void PlayMusic(AudioClip clip)
        {
            if (activeTrack != null)
            {
                if (clip == activeTrack.source.clip)
                {
                    Debug.Log("This Track is already playing");
                    return;
                }

                unloadTracks.Add(activeTrack);
            }

            MusicTrackState track = ReserveTrack();
            track.source.clip = clip;
            track.source.Play();
            track.VolumeIncreasing = true;
            activeTrack = track;
        }

        private MusicTrackState ReserveTrack()
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

        private MusicTrackState CreateNewTrack()
        {
            MusicTrackState mts = new MusicTrackState();
            var gm = new GameObject("Track_" + NumOfMusicTracks++);
            gm.transform.parent = transform;
            mts.source = gm.AddComponent<AudioSource>();
            mts.source.volume = 0;
            mts.source.loop = true;
            mts.source.playOnAwake = false;
            mts.VolumeIncreasing = false;
            mts.source.outputAudioMixerGroup = audioMixerGrp;
            return mts;
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (activeTrack != null)
            {
                if (activeTrack.VolumeIncreasing)
                {
                    float Volume = activeTrack.source.volume + Time.deltaTime / MusicTransitionTime;
                    if (Volume < 1)
                    {
                        activeTrack.source.volume = Volume;
                    }
                    else
                    {
                        activeTrack.source.volume = 1;
                        activeTrack.VolumeIncreasing = false;
                    }
                }
            }
            for (int i = unloadTracks.Count - 1; i >= 0; i--)
            {
                float step = Time.deltaTime / MusicTransitionTime;
                float volume = unloadTracks[i].source.volume;
                if (volume > step)
                {
                    unloadTracks[i].source.volume = volume - step;
                }
                else
                {
                    unloadTracks[i].source.volume = 0;
                    unloadTracks[i].source.Stop();
                    unusedTracks.Enqueue(unloadTracks[i]);
                    unloadTracks.RemoveAt(i);
                }
            }
        }

        public void SetAudioMixerGrp(AudioMixerGroup grp)
        {
            audioMixerGrp = grp;
            var allTracks = new List<MusicTrackState>();
            allTracks.AddRange(unusedTracks);
            allTracks.AddRange(unloadTracks);
            if (activeTrack != null)
            {
                allTracks.Add(activeTrack);
            }
            foreach (var track in allTracks)
            {
                track.source.outputAudioMixerGroup = grp;
            }
        }
    }
}