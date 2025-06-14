﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum SoundType
{
    BGM,
    EFFECT,
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioMixer audioMixer;
    private float currentBGMVolume, currentEffectVolume;
    private Dictionary<string, AudioClip> clipsDic;
    [SerializeField] private AudioClip[] preLoadClips;
    private List<TemporarySoundPlayer> instantiatedSounds;

    private void Start()
    {
        clipsDic = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in preLoadClips)
        {
            clipsDic.Add(clip.name, clip);
        }
        instantiatedSounds = new List<TemporarySoundPlayer>();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        InitVolumes(-2, -2);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {        
        Debug.Log("Load");
        instantiatedSounds.Clear();
        switch (scene.name)
        {
            case "ChiefScene":
                Instance.PlaySound2D("ChiefBGM", 0, true, SoundType.BGM);
                break;
            case "MainScene":
                Instance.PlaySound2D("MainBGM", 0, true, SoundType.BGM);
                break;
            case "TrialScene":
                Instance.PlaySound2D("TrialBGM", 0, true, SoundType.BGM);
                break;
        }
    }

    private AudioClip GetClip(string clipName)
    {
        AudioClip clip = clipsDic[clipName];

        if (clip == null)
        {
            Debug.LogError(clipName + "is not find");
            return null;
        }

        return clip;
    }

    private void AddToList(TemporarySoundPlayer soundPlayer)
    {
        instantiatedSounds.Add(soundPlayer);
    }

    public void StopLoopSound(string clipName)
    {
        foreach(TemporarySoundPlayer audioPlayer in instantiatedSounds)
        {
            if(audioPlayer.ClipName == clipName)
            {
                instantiatedSounds.Remove(audioPlayer);
                Destroy(audioPlayer.gameObject);
                return;
            }
        }
        Debug.LogError(clipName + "is not find (StopLoopSound)");
    }

    public void PlaySound2D(string clipName, float delay = 0f, bool isLoop = false, SoundType type = SoundType.EFFECT)
    {
        GameObject soundObj = new GameObject("TemporarySoundPlayer 2D");
        TemporarySoundPlayer soundPlayer = soundObj.AddComponent<TemporarySoundPlayer>();

        if(isLoop)
        {
            AddToList(soundPlayer);
        }
        soundPlayer.InitSound2D(GetClip(clipName));
        soundPlayer.Play(audioMixer.FindMatchingGroups(type.ToString())[0], delay, isLoop);
    }

    public void InitVolumes(float bgm, float effect)
    {
        SetVolumes(SoundType.BGM, bgm);
        SetVolumes(SoundType.EFFECT, effect);
    }

    public void SetVolumes(SoundType type, float value)
    {
        audioMixer.SetFloat(type.ToString(), value);
    }
}