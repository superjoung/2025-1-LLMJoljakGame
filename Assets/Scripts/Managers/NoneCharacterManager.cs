using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;
using System.Linq;

public class NoneCharacterManager : Singleton<NoneCharacterManager>
{
    public Dictionary<int, Queue<BaseNpcStatAction>> NonePlayersAction = new Dictionary<int, Queue<BaseNpcStatAction>>();
    public List<GameObject> NpcList = new List<GameObject>();
    public List<int> CanStartTalkNpcs // 대화할 수 있는 NPC 선정 리스트로 전달
    {
        get
        {
            _canStartTalkNpcs = new List<int>();
            foreach(GameObject child in NpcList)
            {
                NPCAttachData data = child.GetComponent<NPCAttachData>();
                if (data.CanTalkStart)
                {
                    _canStartTalkNpcs.Add(data.ID);
                }
            }
            return _canStartTalkNpcs;
        }
    }

    public int CanTalkNpcCount // 대화 가능 NPC 변수로 설정
    {
        get
        {
            int count = 0;
            foreach (GameObject child in NpcList)
            {
                NPCAttachData data = child.GetComponent<NPCAttachData>();
                if (data.CanTalkStart)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public bool TalkStart = false; // TEMP : 어떤 NPC들이든 대화를 시작했을 때 사용 추후 수정될 예정

    private string NPC_PREFABS_PATH = "NPC/NPC";
    private int _npcCount = 3;
    private List<int> _canStartTalkNpcs;

    public void Update()
    {
        
    }
    // NPC 움직임 선택
    private void UpdateNpcAction()
    {

    }

    public void TalkStartWithPlayer(int NpcId)
    {
        // 플레이어랑 대화중에는 이동 중이던 행동 멈추기
        // NonePlayersAction[NpcId].Peek().IsTalkWithPlayer = true;
        TalkStart = true; // TEMP : 플레이어 대화 시작은 전부 NPCAttachData로 수정될 예정
        GameObject npc = GetNpcToID(NpcId);
        GameObject player = GameObject.FindWithTag("Player");
        npc.transform.LookAt(player.transform.position);

        UIManager.Instance.ShowNPCUI<NPCTalkPanelUI>(npc.GetComponent<NPCAttachData>().UIPos);
    }

    public GameObject GetNpcToID(int ID)
    {
        if(ID < 0 && NpcList.Count <= ID)
        {
            Debug.LogWarning("[Warning] NoneCharacterManager - GetNpcToID 올바른 인수를 넘기지 않았습니다.");
            return null;
        }
        return NpcList[ID];
    }

    public string GetNpcNameToID(int ID)
    {
        // TEMP : LLM API ID 값으로 전달해서 캐릭터 이름 받아오기
        string name = "프리아";
        return name;
    }

    public NonePlayerAction GetNpcActionType(int npcId)
    {
        // LLM에게 ID 값을 넘겨주면 원하는 행동 얻기 임의로 랜덤으로 생성
        int randomAction = Random.Range(1, System.Enum.GetValues(typeof(NonePlayerAction)).Length);
        return (NonePlayerAction)randomAction;
    }

    // 초기 NPC 세팅
    public void NpcSpawn()
    {
        List<int> spawnList = new List<int>();
        for (int i = 0; i < _npcCount; i++)
        {   
            // NPC 위치 조정
            int spawnInt = 0;
            do
            {
                spawnInt = Random.Range(0, GameManager.Instance.ParentPrefabs.NpcSpawnBox.transform.childCount);
            } while (spawnList.Contains(spawnInt));
            spawnList.Add(spawnInt);
            // NPC 소환
            Transform spawnPos = GameManager.Instance.ParentPrefabs.NpcSpawnBox.transform.GetChild(spawnInt);
            GameObject npc = ResourceManager.Instance.Instantiate(NPC_PREFABS_PATH, spawnPos.position, GameManager.Instance.ParentPrefabs.NpcBox.transform);
            npc.name = "NPC_" + i; // id 연결 이후 _스플릿 후 ID만 가져올 예정
            // NPC 오브젝트 리스트 추가
            NpcList.Add(npc);
            NonePlayersAction.Add(i, new Queue<BaseNpcStatAction>());
        }
    }
}
