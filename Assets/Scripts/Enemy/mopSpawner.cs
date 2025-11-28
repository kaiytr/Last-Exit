using UnityEngine;
using System.Collections; // Coroutine 사용을 위해 필요

public class MobSpawner : MonoBehaviour
{
    // [Inspector에서 설정할 변수]
    [Header("Spawn Settings")]
    public float spawnInterval = 5f; // 적 생성 주기 (5초)

    [Header("Enemy Prefabs")]
    // Inspector 창에 4가지 적 프리팹을 끌어다 놓을 수 있습니다.
    public GameObject wingbatPrefab;
    public GameObject mushroomPrefab;
    public GameObject goblinPrefab;
    public GameObject skeletonPrefab;

    // 생성 위치를 다양하게 하고 싶다면 여러 Transform을 배열로 설정할 수 있습니다.
    [Header("Spawn Point")]
    public Transform spawnPoint;
    // 현재 스크립트가 붙은 mobSpawner의 Transform을 사용해도 되지만, 명시적으로 하나 지정하는 것이 좋습니다.

    void Start()
    {
        // Coroutine을 시작하여 주기적인 스폰을 처리합니다.
        StartCoroutine(SpawnMobsRoutine());
    }

    // Coroutine: 일정 시간 간격으로 반복 작업을 수행할 때 유용합니다.
    IEnumerator SpawnMobsRoutine()
    {
        // 무한 루프를 사용하여 게임이 끝날 때까지 반복합니다.
        while (true)
        {
            // 1. 설정된 시간(spawnInterval)만큼 기다립니다.
            yield return new WaitForSeconds(spawnInterval);

            // 2. 적 생성 함수 호출
            SpawnRandomMob();
        }
    }

    void SpawnRandomMob()
    {
        // 4가지 적 프리팹을 배열에 담습니다.
        GameObject[] mobPrefabs = new GameObject[]
        {
            wingbatPrefab,
            mushroomPrefab,
            goblinPrefab,
            skeletonPrefab
        };

        // 0부터 3까지의 난수를 생성하여 무작위로 적을 선택합니다.
        int randomIndex = Random.Range(0, mobPrefabs.Length);
        GameObject mobToSpawn = mobPrefabs[randomIndex];

        // 선택된 적 프리팹이 null이 아닌지 확인합니다. (Inspector 설정 오류 방지)
        if (mobToSpawn != null)
        {
            // 적을 생성 위치(spawnPoint.position)에 회전 없이(Quaternion.identity) 생성합니다.
            // 생성된 적은 앞서 구현한 EnemyController 스크립트에 의해 즉시 Player를 추적합니다.
            Instantiate(mobToSpawn, spawnPoint.position, Quaternion.identity);

            Debug.Log($"적을 생성했습니다: {mobToSpawn.name} (태그: {mobToSpawn.tag})");
        }
        else
        {
            Debug.LogError("적 프리팹 중 하나가 Inspector에 할당되지 않았습니다! 확인해주세요.");
        }
    }
}