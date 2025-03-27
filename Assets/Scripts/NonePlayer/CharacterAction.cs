using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;

public abstract class CharacterAction : MonoBehaviour
{
    public abstract NonePlayerAction NpcAction { get; } // NPC�� ������ �� �ִ� �ൿ
    public abstract int ID { get; } // NPC ID - ���� ������Ʈ�� ID�� ������ ����
    public abstract bool IsTalkWithPlayer { get; } // NPC�� �÷��̾�� ��ȭ�ϱ� �����ߴ���

    public virtual void Init() { } // �׼��� �ϱ� �� �ʱ�ȭ�ؾ��ϴ� �Լ�
    public virtual void ActionStart() { } // �ش� NonePlayerAction�� ���� �����ϱ��� ���ؾ��ϴ� ����
    public virtual void ActionUpdate() { } // ActionUpdate �׼��� �����ϸ鼭 ó���ؾ��ϴ� �Լ�
    public virtual void ActionEnd() { } // �� �׼��� ������ �� ������Ѿ��ϴ� �Լ�
}
