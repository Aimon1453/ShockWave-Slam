using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource SfxAudio;
    [SerializeField] AudioSource BgmAudio;

    public AudioClip bgmMain;

    public AudioClip footstep;
    public AudioClip keyPickup;
    public AudioClip doorOpen;
    public AudioClip weaponPickup;
    public AudioClip shockwaveLaunch;
    public AudioClip shockwaveHit;
    public AudioClip checkPoint;
    public AudioClip playerDeath;

    void Start()
    {
        BgmAudio.clip = bgmMain;
        BgmAudio.Play();
    }


    public void PlaySfx(AudioClip clip)
    {
        SfxAudio.PlayOneShot(clip);
    }
}

