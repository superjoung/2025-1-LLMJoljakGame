using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HearingTest : MonoBehaviour
{
    private GameObject _world;
    private GameObject _npc;
    private GameObject _playerHead;
    private float _timer = 1.5f;

    [SerializeField] private GameObject _hearingFloor;
    [SerializeField] private GameObject _hearingCeiling;
    [SerializeField] private Transform[] _wallPoints = new Transform[4];
    [SerializeField] private GameObject[] _walls = new GameObject[4];

    private void Start()
    {
        Init();
        // 회전각 조정
        gameObject.transform.Rotate(0, _npc.transform.rotation.y, 0);

        Sequence haeringSeq = DOTween.Sequence();

        haeringSeq.Append(_world.transform.DOMoveY(-10f, 1f).SetEase(Ease.InCubic));
        haeringSeq.Append(_hearingFloor.transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutElastic));
        for(int i = 0; i < 4; i++)
        {
            haeringSeq.Insert(_timer, _walls[i].transform.DOMove(_wallPoints[i].transform.position, 1f).SetEase(Ease.OutQuart));
            haeringSeq.Join(_walls[i].transform.GetChild(0).DOShakePosition(2f, 2, 10, 1, false, true, ShakeRandomnessMode.Full));
            _timer += Random.Range(0.1f, 0.6f);
        }
        haeringSeq.Insert(_timer, _hearingCeiling.transform.DOScale(new Vector3(10f, 0.2f, 10f), 1f).SetEase(Ease.OutElastic));

        haeringSeq.Play();
    }

    private void Update()
    {

    }

    private void Init()
    {
        _world = GameObject.FindWithTag("WorldMap");
        _playerHead = GameObject.FindWithTag("MainCamera");
        
        // 레거시코드
        _npc = GameObject.Find("Npc");
    }
}
