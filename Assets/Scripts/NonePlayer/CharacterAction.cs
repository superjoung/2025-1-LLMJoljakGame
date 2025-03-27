using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;

public abstract class CharacterAction : MonoBehaviour
{
    public abstract NonePlayerAction NpcAction { get; } // NPC가 선택할 수 있는 행동
    public abstract int ID { get; } // NPC ID - 게임 오브젝트는 ID로 구별할 예정
    public abstract bool IsTalkWithPlayer { get; } // NPC가 플레이어와 대화하기 시작했는지

    public virtual void Init() { } // 액션을 하기 전 초기화해야하는 함수
    public virtual void ActionStart() { } // 해당 NonePlayerAction에 의해 시작하기전 정해야하는 값들
    public virtual void ActionUpdate() { } // ActionUpdate 액션을 진행하면서 처리해야하는 함수
    public virtual void ActionEnd() { } // 한 액션이 끝났을 때 실행시켜야하는 함수
}
