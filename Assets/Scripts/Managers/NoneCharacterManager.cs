using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnum.NonePlayerDefine;
using System.Linq;

public partial class NoneCharacterManager : Singleton<NoneCharacterManager>
{
    public Dictionary<int, Queue<BaseNpcStatAction>> NonePlayersAction = new Dictionary<int, Queue<BaseNpcStatAction>>();
    public List<GameObject> NpcList = new List<GameObject>(); // 소환된 NPC 게임오브젝트로 저장
    public GameObject InteractiveNPC // 상호작용 NPC
    {
        get
        {
            if (CanStartTalkNpcs.Count == 0)
            {
                Debug.LogWarning("[WARN]NoneCharacterManager(InteractiveNPC) - 상호작용 가능한 NPC가 존재하지않습니다.");
                return null;
            }
            return GetNpcToID(CanStartTalkNpcs[0]);
        }
    }

    private string NPC_PREFABS_PATH = "NPC/NPC";
    private int _npcCount = 3;

    public void Update()
    {
        
    }
    // NPC 움직임 선택
    public void UpdateNpcAction()
    {

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
