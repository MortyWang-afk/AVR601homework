using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject crowPrefab;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Waves setting")]
    public int totalWaves = 3;             // 一共几波
    public int firstWaveCrows = 3;         // 第1波几只
    public int extraCrowsPerWave = 2;      // 每波比上一波多几只(难度递增,对应PANEL 5)
    public float spawnInterval = 2f;       // 同一波内,每隔几秒出一只
    public float timeBetweenWaves = 3f;    // 波与波之间休息几秒

    int currentWave = 0;
    int crowsAlive = 0;                    // 场上还剩几只

    void Start()
    {
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;
            GameManager.Instance.SetWave(currentWave, totalWaves);

            // 这一波的乌鸦数量:3, 5, 7...
            int crowCount = firstWaveCrows + (currentWave - 1) * extraCrowsPerWave;

            // 逐只刷出
            for (int i = 0; i < crowCount; i++)
            {
                SpawnCrow();
                yield return new WaitForSeconds(spawnInterval);
            }

            // 等这一波全部被消灭
            while (crowsAlive > 0)
                yield return null;

            // 波间休息(最后一波后不用休息)
            if (currentWave < totalWaves)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        // 所有波次清完 → 胜利!
        GameManager.Instance.Win();
    }

    void SpawnCrow()
    {
        float y = Random.Range(minY, maxY);
        Vector3 pos = new Vector3(transform.position.x, y, 0);
        Instantiate(crowPrefab, pos, Quaternion.identity);
        crowsAlive++;
    }

    // 乌鸦消失时(被球打死/咬完食物跑了)调用这个
    public void CrowRemoved()
    {
        crowsAlive = Mathf.Max(0, crowsAlive - 1);
    }
}