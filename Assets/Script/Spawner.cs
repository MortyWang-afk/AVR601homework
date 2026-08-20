using UnityEngine;
using System.Collections;
using TMPro;

public class Spawner : MonoBehaviour
{
    public GameObject crowPrefab;
    public float minY = -4f;
    public float maxY = 4f;
    public float bowlMinY = 2f;      // 偷食物的乌鸦最低高度
    public TMP_Text killCountText;   // 拖 KillCountText 进来


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
    int crowsKilled = 0;               // 本波已击杀数
    int killTarget = 0;                // 本波需要击杀几只
    public bool spawnPaused = false;

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

            crowsKilled = 0;               // 每波开始时归零
            killTarget = w.crowCount;      // 本波要打死这么多只
            UpdateKillUI();                // 波次开始时显示 0 / N


            GameManager.Instance.SetWave(currentWave, waves.Length);

            SFXManager.Instance.Play(SFXManager.Instance.waveStart);  

            // 开场提示：等它播完再刷乌鸦
            yield return StartCoroutine(
                GameManager.Instance.ShowMessage(w.waveName, Color.white, 0.6f));

            for (int i = 0; i < w.crowCount; i++)
            {
                // 暂停期间原地等待，不生成
                while (spawnPaused)
                    yield return null;

                SpawnCrow(w);
                if (i < w.crowCount - 1)
                    yield return new WaitForSeconds(w.spawnInterval);
            }

            // ★ 等"击杀数达标"
            while (crowsKilled < killTarget)
                yield return null;

            // 清场提示（最后一波不播，直接进胜利）
            if (currentWave < waves.Length)
            {
                yield return StartCoroutine(
                    GameManager.Instance.ShowMessage("WAVE CLEAR!", Color.green, 0.5f));
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        SFXManager.Instance.Play(SFXManager.Instance.victory);   // ← 通关音

        GameManager.Instance.Win();
    }

    void SpawnCrow(Wave w)
    {
        // 先抽签决定目标
        string targetTag = (Random.value < w.dogTargetChance) ? "Dog" : "Bowl";

        // 根据目标决定生成高度
        float y;
        if (targetTag == "Bowl")
            y = Random.Range(bowlMinY, maxY);    // 偷食物的只从高空来
        else
            y = Random.Range(minY, maxY);        // 攻击狗的不限高度

        Vector3 pos = new Vector3(transform.position.x, y, 0);
        GameObject crowObj = Instantiate(crowPrefab, pos, Quaternion.identity);

        if (crowObj.TryGetComponent<Crow>(out var crow))
            crow.Setup(w.crowSpeed, targetTag);
    }

     // 刷新击杀数 UI
    void UpdateKillUI()
    {
        if (killCountText != null)
            killCountText.text = "KILL COUNT: " + crowsKilled + " / " + killTarget;
    }

    // ★ 乌鸦被球打死时调用 → 记一个战果
    public void CrowKilled()
    {
        crowsKilled++;
        UpdateKillUI();                // 击杀后刷新数字
    }

    // 乌鸦逃出屏幕时调用 → 不记战果，补一只新的
    public void CrowEscaped()
    {
        if (crowsKilled < killTarget)
            StartCoroutine(RespawnWhenReady());
    }

    // 如果正在暂停，等暂停结束再补
    IEnumerator RespawnWhenReady()
    {
        while (spawnPaused)
        yield return null;
    
        if (crowsKilled < killTarget)          // 等待期间可能已经通关，再查一次
        SpawnCrow(waves[currentWave - 1]);
    }   
}