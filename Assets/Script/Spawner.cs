using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject crowPrefab;
    public float minY = -4f;
    public float maxY = 4f;

    [System.Serializable]
    public struct Wave
    {
        public string waveName;        // 波次名，会显示给玩家
        public int crowCount;          // 这波放几只
        public float spawnInterval;    // 每隔几秒出一只
        public float crowSpeed;        // 这波乌鸦的飞行速度
        public float dogTargetChance;  // 盯上狗的概率 (0~1)
    }

    [Header("Waves setting")]
    public Wave[] waves;               // 波次数组，在 Inspector 里配
    public float timeBetweenWaves = 3f;

    int currentWave = 0;
    int crowsAlive = 0;

    void Start()
    {
        StartCoroutine(RunWaves());
    }

     IEnumerator RunWaves()
    {
        while (currentWave < waves.Length)
        {
            Wave w = waves[currentWave];
            currentWave++;

            GameManager.Instance.SetWave(currentWave, waves.Length);

            // ↓ 开场提示：等它播完再刷乌鸦
            yield return StartCoroutine(
                GameManager.Instance.ShowMessage(w.waveName, Color.white, 0.6f));
            // ↑

            for (int i = 0; i < w.crowCount; i++)
            {
                SpawnCrow(w);
                if (i < w.crowCount - 1)
                    yield return new WaitForSeconds(w.spawnInterval);
            }

            while (crowsAlive > 0)
                yield return null;

            // ↓ 清场提示（最后一波不播，直接进胜利）
            if (currentWave < waves.Length)
            {
                yield return StartCoroutine(
                    GameManager.Instance.ShowMessage("WAVE CLEAR!", Color.green, 0.5f));
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            // ↑
        }

        GameManager.Instance.Win();
    }

    void SpawnCrow(Wave w)
    {
        float y = Random.Range(minY, maxY);
        Vector3 pos = new Vector3(transform.position.x, y, 0);

        GameObject crowObj = Instantiate(crowPrefab, pos, Quaternion.identity);

        // 把这一波的配置传给乌鸦
        if (crowObj.TryGetComponent<Crow>(out var crow))
            crow.Setup(w.crowSpeed, w.dogTargetChance);

        crowsAlive++;
    }
    // 乌鸦消失时(被球打死/叼完食物跑了)调用这个
    public void CrowRemoved()
    {
        crowsAlive = Mathf.Max(0, crowsAlive - 1);
    }
}