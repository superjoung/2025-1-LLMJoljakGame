using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;

public abstract class BaseNpcStatAction : MonoBehaviour
{
    protected abstract NonePlayerAction NpcAction { get; } // NPC�� ������ �� �ִ� �ൿ
    public abstract NPCAttachData npcData { get;  } // NPC ������ ����ִ� ��ũ��Ʈ
    public abstract bool IsTalkWithPlayer { get; } // NPC�� �÷��̾�� ��ȭ�ϱ� �����ߴ���
    public abstract float MoveSpeed { get; } // Npc�� �̵��ϴ� �ӵ�
    public abstract Transform SpotPos { get; } // NPC�� �̵��ؾ��ϴ� ���

    public virtual void Init() { } // �׼��� �ϱ� �� �ʱ�ȭ�ؾ��ϴ� �Լ�
    public virtual void SpotMove() // �׼��� �ϱ� �� ������ �������� �̵�
    {
        
    } 
    public virtual void ActionStart() { } // �ش� NonePlayerAction�� ���� �����ϱ��� ���ؾ��ϴ� ����
    public virtual void ActionUpdate() { } // ActionUpdate �׼��� �����ϸ鼭 ó���ؾ��ϴ� �Լ�
    public virtual void ActionEnd() { } // �� �׼��� ������ �� ������Ѿ��ϴ� �Լ�
}
