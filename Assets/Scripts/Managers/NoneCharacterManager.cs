using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;
using System.Linq;

public class NoneCharacterManager : Singleton<NoneCharacterManager>
{
    public Dictionary<int, Queue<BaseNpcStatAction>> NonePlayersAction = new Dictionary<int, Queue<BaseNpcStatAction>>();
    public List<GameObject> NpcList = new List<GameObject>();

    private string NPC_PREFABS_PATH = "NPC/NPC";
    private int _npcCount = 3;

    public void Update()
    {
        
    }
    // NPC ������ ����
    private void UpdateNpcAction()
    {

    }

    public GameObject GetNpcToID(int ID)
    {
        if(ID < 0 && NpcList.Count <= ID)
        {
            Debug.LogWarning("[Warning] NoneCharacterManager - GetNpcToID �ùٸ� �μ��� �ѱ��� �ʾҽ��ϴ�.");
            return null;
        }
        return NpcList[ID];
    }

    public NonePlayerAction GetNpcActionType(int npcId)
    {
        // LLM���� ID ���� �Ѱ��ָ� ���ϴ� �ൿ ��� ���Ƿ� �������� ����
        int randomAction = Random.Range(1, System.Enum.GetValues(typeof(NonePlayerAction)).Length);
        return (NonePlayerAction)randomAction;
    }

    // �ʱ� NPC ����
    public void NpcSpawn()
    {
        List<int> spawnList = new List<int>();
        for (int i = 0; i < _npcCount; i++)
        {   
            // NPC ��ġ ����
            int spawnInt = 0;
            do
            {
                spawnInt = Random.Range(0, GameManager.Instance.ParentPrefabs.NpcSpawnBox.transform.childCount);
            } while (spawnList.Contains(spawnInt));
            spawnList.Add(spawnInt);
            // NPC ��ȯ
            Transform spawnPos = GameManager.Instance.ParentPrefabs.NpcSpawnBox.transform.GetChild(spawnInt);
            GameObject npc = ResourceManager.Instance.Instantiate(NPC_PREFABS_PATH, spawnPos.position, GameManager.Instance.ParentPrefabs.NpcBox.transform);
            npc.name = "NPC_" + i; // id ���� ���� _���ø� �� ID�� ������ ����
            // NPC ������Ʈ ����Ʈ �߰�
            NpcList.Add(npc);
            NonePlayersAction.Add(i, new Queue<BaseNpcStatAction>());
        }
    }
}
