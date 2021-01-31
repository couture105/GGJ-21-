using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource atractSource;
    public AudioSource scareSource;

    public AudioSource ambientMusicSource;
    public AudioSource startMusicSource;
    public AudioSource endMusicSource;

    public AudioSource effectsSource;

    public AudioClip hitEffectClip;
    public AudioClip enterEffectClip;
    public AudioClip fireworkEffectClip;

    public void StopAll()
    {
        if (atractSource != null && atractSource.isPlaying)
        {
            atractSource.Stop();
        }
        if (scareSource != null && scareSource.isPlaying)
        {
            scareSource.Stop();
        }

        if (ambientMusicSource != null && ambientMusicSource.isPlaying)
        {
            ambientMusicSource.Stop();
        }
        if (endMusicSource != null && endMusicSource.isPlaying)
        {
            endMusicSource.Stop();
        }
        if (startMusicSource != null && startMusicSource.isPlaying)
        {
            startMusicSource.Stop();
        }

        if (effectsSource != null && effectsSource.isPlaying)
        {
            effectsSource.Stop();
        }
    }

    public void StopSounds()
    {
        if (atractSource != null && atractSource.isPlaying)
        {
            atractSource.Stop();
        }
        if (scareSource != null && scareSource.isPlaying)
        {
            scareSource.Stop();
        }

        if (effectsSource != null && effectsSource.isPlaying)
        {
            effectsSource.Stop();
        }
    }

    public void StopMusic()
    {
        if (ambientMusicSource != null && ambientMusicSource.isPlaying)
        {
            ambientMusicSource.Stop();
        }
        if (endMusicSource != null && endMusicSource.isPlaying)
        {
            endMusicSource.Stop();
        }
        if (startMusicSource != null && startMusicSource.isPlaying)
        {
            startMusicSource.Stop();
        }
    }

    public void PlayAttractSound()
    {
        if (atractSource != null && !atractSource.isPlaying)
        {
            atractSource.Play();
        }
    }
    public void StopAttractSound()
    {
        if (atractSource != null && atractSource.isPlaying)
        {
            atractSource.Stop();
        }
    }

    public void PlayScareSound()
    {
        if (scareSource != null && !scareSource.isPlaying)
        {
            scareSource.Play();
        }
    }
    public void StopScareSound()
    {
        if (scareSource != null && scareSource.isPlaying)
        {
            scareSource.Stop();
        }
    }

    public void PlayAmbientMusic()
    {
        if (ambientMusicSource != null && !ambientMusicSource.isPlaying)
        {
            ambientMusicSource.Play();
        }
    }
    public void StopAmbientMusic()
    {
        if (ambientMusicSource != null && ambientMusicSource.isPlaying)
        {
            ambientMusicSource.Stop();
        }
    }

    public void PlayStartMusic()
    {
        if (startMusicSource != null && !startMusicSource.isPlaying)
        {
            startMusicSource.Play();
        }
    }
    public void StopStartMusic()
    {
        if (startMusicSource != null && startMusicSource.isPlaying)
        {
            startMusicSource.Stop();
        }
    }

    public void PlayEndMusic()
    {
        if (endMusicSource != null && !endMusicSource.isPlaying)
        {
            endMusicSource.Play();
        }
    }
    public void StopEndMusic()
    {
        if (endMusicSource != null && endMusicSource.isPlaying)
        {
            endMusicSource.Stop();
        }
    }

    public void PlayHitEffect()
    {
        if (effectsSource != null && hitEffectClip != null)
        {
            effectsSource.PlayOneShot(hitEffectClip);
        }
    }

    public void PlayEnterEffect()
    {
        if (effectsSource != null && enterEffectClip != null)
        {
            effectsSource.PlayOneShot(enterEffectClip);
        }
    }

    public void PlayFireworkEffect()
    {
        if (effectsSource != null && fireworkEffectClip != null)
        {
            effectsSource.PlayOneShot(fireworkEffectClip);
        }
    }
}
