namespace Anipen.AI_NPC_Demo
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum ActorType { None, Player, Friend }
    public enum PropType { None, Plate, Cup }

    public class ScenarioManager : ManagerBase<ScenarioManager>
    {
        private Dictionary<ActorType, Actor> actors = new Dictionary<ActorType, Actor>();
        private Dictionary<PropType, Prop> props = new Dictionary<PropType, Prop>();


    }
}
