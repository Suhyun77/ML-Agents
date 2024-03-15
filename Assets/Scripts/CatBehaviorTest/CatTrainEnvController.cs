namespace Anipen.AI_NPC_Demo
{
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public enum TrainMode { OneTarget, RandomOneTarget, AllTarget}

    public class CatTrainEnvController : MonoBehaviour
    {
        #region Members
        [Header("Agent")]
        [SerializeField] CatAgent agent;
        [SerializeField] Collider agentSpawnArea;

        [Header("Targets")]
        [SerializeField] Transform targetRoot;
        [SerializeField] Transform targetSpawnAreaRoot;

        [SerializeField] private TrainMode trainMode;
        [SerializeField] private int oneTargetIdx;

        private List<Transform> targetList = new List<Transform>();
        private List<Bounds> targetSpawnAreaBoundList = new List<Bounds>();
        private int trainTargetCount;
        

        #region Properties
        public IEnumerable<Transform> TargetList => targetList;
        public int TrainTargetCount => trainTargetCount;
        #endregion
        #endregion

        #region Method
        public void InitializeEnv()
        {
            switch (trainMode)
            {
                case TrainMode.OneTarget:
                    targetList.Add(targetRoot.GetChild(oneTargetIdx));
                    break;
                case TrainMode.RandomOneTarget:
                case TrainMode.AllTarget:
                    foreach (Transform target in targetRoot)
                        targetList.Add(target);
                    break;
            }

            foreach (Transform spawnArea in targetSpawnAreaRoot)
                targetSpawnAreaBoundList.Add(spawnArea.GetComponent<Collider>().bounds);
        }

        public void ResetEnv()
        {
            ResetPlayer();
            ResetTarget();
        }

        private void ResetPlayer()
        {
            agent.transform.position = GetRandomSpawnPos(agentSpawnArea.bounds, agent.transform.localPosition.y);
        }

        private void ResetTarget()
        {
            var targetY = targetList[0].localPosition.y;

            switch (trainMode)
            {
                case TrainMode.OneTarget:
                    targetList[0].position = GetRandomSpawnPos(targetSpawnAreaBoundList[oneTargetIdx], targetY);
                    targetList[0].gameObject.SetActive(true);
                    trainTargetCount = 1;
                    break;
                case TrainMode.RandomOneTarget:
                    for (int i = 0; i < targetList.Count; i++)
                    {
                        targetList[i].position = GetRandomSpawnPos(targetSpawnAreaBoundList[i], targetY);
                    }
                    ActiveRandomTarget();
                    trainTargetCount = 1;
                    break;
                case TrainMode.AllTarget:
                    for (int i = 0; i < targetList.Count; i++)
                    {
                        targetList[i].position = GetRandomSpawnPos(targetSpawnAreaBoundList[i], targetY);
                        targetList[i].gameObject.SetActive(true);
                    }
                    trainTargetCount = targetList.Count;
                    break;
            }

            
        }

        public void ActiveRandomTarget()
        {
            var randIdx = Random.Range(0, targetList.Count);
            targetList[randIdx].gameObject.SetActive(true);  
        }

        private Vector3 GetRandomSpawnPos(Bounds bounds, float yValue)
        {
            var randomPosX = Random.Range(-bounds.extents.x, bounds.extents.x);
            var randomPosZ = Random.Range(-bounds.extents.z, bounds.extents.z);

            var randomSpawnPos = bounds.center + new Vector3(randomPosX, 0, randomPosZ);
            randomSpawnPos.y = yValue;
            return randomSpawnPos;
        }
        #endregion
    }
}