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
            FreeMoveMode,
            HearingMode,
            EvidenceMode
        }
    }
}
