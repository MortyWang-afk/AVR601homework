using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    public GameObject crowPrefab;
    public float spawnInterval = 2f;  // 每 2 秒一只
    public float minY = -4f;
    public float maxY = 4f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            float y = Random.Range(minY, maxY);
            Vector3 pos = new Vector3(transform.position.x, y, 0);
            Instantiate(crowPrefab, pos, Quaternion.identity);
        }
    }
}
