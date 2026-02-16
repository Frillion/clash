using System;
using Clash.Utillities;
using UnityEngine;

namespace Clash.Features.Audio
{
    public class AudioPlayer : Spawnable
    { 
        public AudioSource source;

        public void PlayOneShot(AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }

        public override void Despawn()
        {
            source.Pause();
            source.Stop();
            base.Despawn();
        }

        public bool IsPlaying()
        {
            return source.isPlaying;
        }
    }
}

