using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapPlayerPos : MonoBehaviour
{
    // 드래그 앤 드랍으로 넣어줄 예정
    public Transform InGamePlayerPos;

    private void Update()
    {
        if(InGamePlayerPos != null)
        {
            gameObject.transform.localPosition = new Vector3((InGamePlayerPos.position.x - 35) / 27.8f, 0, InGamePlayerPos.position.z / 27.8f);
        }
        else
        {
            Debug.Log("[WARN]MiniMapPlayerPos(Update) - 미니맵에 표현할 FreeModePlayer가 존재하지 않습니다.");
        }
    }
}
