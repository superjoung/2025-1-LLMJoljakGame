using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingRoomAction : MonoBehaviour
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
        gameObject.transform.rotation = Quaternion.Euler(0, _npc.transform.eulerAngles.y, 0);
        _playerHead.transform.DOShakePosition(3f, 2, 10, 1, false, true, ShakeRandomnessMode.Full);

        Sequence haeringSeq = DOTween.Sequence();

        haeringSeq.Append(_world.transform.DOScale(new Vector3(0, 0, 0), 1f).SetEase(Ease.InCubic));
        haeringSeq.Append(_hearingFloor.transform.DOScale(new Vector3(1, 1, 1), 1.5f).SetEase(Ease.OutElastic));
        for (int i = 0; i < 4; i++)
        {
            haeringSeq.Insert(_timer, _walls[i].transform.DOMove(_wallPoints[i].transform.position, 1f).SetEase(Ease.OutQuart));
            haeringSeq.Join(_walls[i].transform.GetChild(0).DOShakePosition(2f, 2, 10, 1, false, true, ShakeRandomnessMode.Full));
            _timer += Random.Range(0.1f, 0.6f);
        }
        haeringSeq.Insert(_timer, _hearingCeiling.transform.DOScale(new Vector3(10f, 0.2f, 10f), 1f).SetEase(Ease.OutElastic));

        haeringSeq.Play();
    }

    private void Init()
    {
        _world = GameObject.FindWithTag("WorldMap");
        _playerHead = GameObject.FindWithTag("PlayerHead");

        // 레거시코드
        _npc = NoneCharacterManager.Instance.InteractiveNPC;
    }
}
