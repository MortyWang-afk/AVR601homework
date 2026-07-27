using UnityEngine;

public class GameManager : MonoBehaviour
{
     public static GameManager Instance; // 全局唯一入口,别人用 GameManager.Instance 就能找到我

    public int dogHP = 5;
    public int foodHP = 5;

    void Awake()
    {
        Instance = this; // 游戏一启动就把自己登记进去
    }

    public void DamageDog(int amount)
    {
        dogHP -= amount;
        Debug.Log("Ouch! Dog HP: " + dogHP);
        if (dogHP <= 0) Lose();
    }

    public void DamageFood(int amount)
    {
        foodHP -= amount;
        Debug.Log("Food took damage! Food HP: " + foodHP);
        if (foodHP <= 0) Lose();
    }

    void Lose()
    {
        Debug.Log("GAME OVER");
        Time.timeScale = 0f; // 时间暂停,全场定格,最简单的结束方式
    }
}
