using System.Collections.Generic;

namespace LLM
{
    using System;

    [Serializable]
    public class UserInput
    {
        public string input;
        public string npc;
    }

    [Serializable]
    public class ResponseData
    {
        public string npc;
        public string response;
        public bool allowed;
    }

    [Serializable]
    public class Personality
    {
        public string behavior;
        public string emotion;
    }

    [Serializable]
    public class Suspect
    {
        public string name;
        public Personality personality;
        public string occupation;
        public string[] relationships;
        public string statement;
        public bool is_witch;
    }

    [Serializable]
    public class Setup
    {
        public string village;
        public string @event; // 'event'는 예약어라 escape 처리
        public Suspect[] suspects;
    }

    [Serializable]
    public class SetupResponse
    {
        public string message;
        public Setup setup;
    }
    
    [System.Serializable]
	public class Clue
	{
   		public string id;
    	public string name;
    	public string location;
    	public int importance;
	}

    [System.Serializable]
    public class GameDataResponse
    {
        public Dictionary<string, List<string>> npc_routes;
        public List<Clue> clues;
    }
    
    [System.Serializable]
    public class FinalStatementResponse
    {
        public string[] final_statements;
    }

}