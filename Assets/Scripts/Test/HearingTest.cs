using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HearingTest : MonoBehaviour
{
    public GameObject World;
    public GameObject NPC;
    public GameObject Head;

    private void Start()
    {
        World.transform.DOMoveY(-10f, 1f).SetEase(Ease.InCubic);
        Head.transform.DOShakePosition(3f, 1, 10, 1, false, true, ShakeRandomnessMode.Harmonic);
    }

    private void Update()
    {

    }
}
