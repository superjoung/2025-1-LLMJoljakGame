using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefineEnum
{
    namespace NonePlayerDefine
    {
        public enum NonePlayerAction
        {
            None = -1,
            Move,
            Talk,
            Hearing,
            Cooking,
        }
    }

    namespace GameModeDefine
    {
        public enum GameFlowMode
        {
            None = -1,
            FreeMoveMode,   // 자유 이동 모드
            TalkMode,       // 대화 모드
            HearingMode,    // 심문 모드
            EvidenceMode    // 증거 수집 모드
        }
    }
}
