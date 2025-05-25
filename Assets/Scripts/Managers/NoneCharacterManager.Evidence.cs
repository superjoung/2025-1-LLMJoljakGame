using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.SpotNameDefine;

public partial class NoneCharacterManager
{
    public Dictionary<int, List<SpotName>> LLMNPCMoveSpots = new Dictionary<int, List<SpotName>>();

    // 초기 LLM NPC 움직이는 장소 넣어두기 추후 삭제될 함수
    private void MoveSpotSetting()
    {
        LLMNPCMoveSpots.Add(0, new List<SpotName>());
        LLMNPCMoveSpots.Add(1, new List<SpotName>());
        LLMNPCMoveSpots.Add(2, new List<SpotName>());
        LLMNPCMoveSpots.Add(3, new List<SpotName>());
        LLMNPCMoveSpots.Add(4, new List<SpotName>());
    }
}
