using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePlayerSoundManager : MonoBehaviour
{
    [SerializeField] AudioSource jumpSound;
    [SerializeField] AudioSource moveSound;
    [SerializeField] AudioSource splashSound;
    [SerializeField] AudioSource eatingSound;

    public void PlayJumpsound()
    {
        jumpSound.Play();
    }
    public void PlayMoveSound()
    {
        moveSound.Play();
    }
    public void PlaySplashSound()
    {
        splashSound.Play();
    }
    public void PlayEatingSound()
    {
        eatingSound.Play();
    }

}
