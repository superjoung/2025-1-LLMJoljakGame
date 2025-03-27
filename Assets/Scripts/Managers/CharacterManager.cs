using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    public Dictionary<int, Queue<CharacterAction>> NonePlayersAction = new Dictionary<int, Queue<CharacterAction>>();
}
