using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData
    {
        [Header("스폰 위치")]
        public Transform spawnPoint;

        [Header("적 순찰 경로")]
        public Transform[] patrolRoute;
    }

    [Header("적 프리팹")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("적 생성 데이터")]
    [SerializeField] private SpawnData[] spawnData;

    void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < spawnData.Length; i++)
        {
            SpawnData data = spawnData[i];
            if (data.spawnPoint == null) continue;

            GameObject enemy = Instantiate(enemyPrefab, data.spawnPoint.position, data.spawnPoint.rotation);
            EnemyController controller = enemy.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.SetPatrolRoute(data.patrolRoute);
            }
        }
    }
}
