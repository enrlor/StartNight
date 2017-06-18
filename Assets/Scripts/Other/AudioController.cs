using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    public AudioClip dome;
    public AudioClip game;
    public AudioClip score;

    AudioSource source;

	// Use this for initialization
	void Awake () {
        source = this.GetComponent<AudioSource>();
	}

    public void AudioinDome()
    {
        source.enabled = false;
        source.loop = true;
        source.clip = dome;
        source.enabled = true;
        source.Play();
    }

    public void AudioinGame()
    {
        source.enabled = false;
        source.loop = true;
        source.clip = game;
        source.enabled = true;
        source.Play();
    }

    public void AudioinScore()
    {
        source.enabled = false;
        source.loop = false;
        source.clip = score;
        source.enabled = true;
        source.Play();
    }

}
