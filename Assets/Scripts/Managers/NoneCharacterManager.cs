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

    public void NoneCharacterAwake()
    {
        NpcSpawn();
        FixNPCSpawn();
        FixNpcInit();
        MoveSpotSetting();
    }

    // 모든 파셜 클래스에 존재하는 Start 부분을 모아 GameManager에서 실행
    public void NoneCharacterStart()
    {
        // LLM으로 이름 & NPC 이동 장소 선정 완료 되고 데이터가 들어왔을 때 실행
        LLMMoveSpotNameSetting();
        LLMNPCMoveStart();
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

    public GameObject GetFixNpcToID(int ID)
    {
        if (ID < 0 && TalkList.Count <= ID)
        {
            Debug.LogWarning("[Warning] NoneCharacterManager - GetNpcToID 올바른 인수를 넘기지 않았습니다.");
            return null;
        }
        return FixNpcs[ID];
    }

    public Sprite GetFixNpcPortraitToID(int ID)
    {
        if (ID < 0 && TalkList.Count <= ID)
        {
            Debug.LogWarning("[Warning] NoneCharacterManager - GetNpcPortraitToID 올바른 인수를 넘기지 않았습니다.");
            return null;
        }
        return FixNpcPortraitList[ID];
    }

    public string GetNpcNameToID(int ID)
    {
        // TEMP : LLM API ID 값으로 전달해서 캐릭터 이름 받아오기
        return LLMConnectManager.Instance.GetAllSuspects()[ID].name;
        return TempLLMNpcNames[ID];
    }

    public string GetFixNpcNameToID(int ID)
    {
        // 고정 NPC 이름 받아오기
        return FixNpcNames[ID];
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
        Dictionary<string, int> meshNameCount = new();
        for (int i = 0; i < _npcCount; i++)
        {   
            // NPC 위치 조정
            int spawnInt = 0;
            do
            {
                // 임시 테스트 코드
                spawnInt = Random.Range(0, 3);
                //spawnInt = Random.Range(0, GameManager.Instance.ParentPrefabs.NpcSpawnBox.transform.childCount);
            } while (spawnList.Contains(spawnInt));
            spawnList.Add(spawnInt);
            // NPC 소환
            Transform spawnPos = GameManager.Instance.ParentPrefabs.NpcSpawnBox.transform.GetChild(spawnInt);
            GameObject npc = ResourceManager.Instance.Instantiate(NPC_PREFABS_PATH, spawnPos.position, GameManager.Instance.ParentPrefabs.NpcBox.transform);

            string meshPrefabPath = "NPC/AI_NPC/";
            string prefabName = LLMConnectManager.Instance.GetSuspectByName(GetNpcNameToID(i)).gender + "_" +
                                LLMConnectManager.Instance.GetSuspectByName(GetNpcNameToID(i)).age_group + "_";
            
            meshNameCount.TryAdd(prefabName, 0);
            meshNameCount[prefabName] += 1;
            prefabName += meshNameCount[prefabName];
            ResourceManager.Instance.Instantiate(meshPrefabPath + prefabName, npc.transform.position + Vector3.down, npc.transform);
            npc.name = "NPC_" + i; // id 연결 이후 _스플릿 후 ID만 가져올 예정
            npc.GetComponent<NPCAttachData>().ID = i; // id 연결
            // NPC 오브젝트 리스트 추가
            NpcList.Add(npc);
            NonePlayersAction.Add(i, new Queue<BaseNpcStatAction>());
        }
    }
}
