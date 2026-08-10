using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; 
using TMPro;   

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int dogHP = 5;
    public int foodHP = 5;

    public Image dogHPBar;
    public Image foodHPBar;
    public Text waveText;
    public TMP_Text messageText; 
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    int maxDogHP, maxFoodHP;
    bool gameOver = false;                    // ← 新增:结算标记

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        maxDogHP = dogHP;
        maxFoodHP = foodHP;
        Time.timeScale = 1f;                  // ← 新增:保险,防止场景重载后仍冻结
        if (victoryPanel != null) victoryPanel.SetActive(false);   // ← 新增:开局强制隐藏
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateBars();
    }

    void Update()
    {
        if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void DamageDog(int amount)
    {
        if (gameOver) return;                 // ← 新增:结算后不再扣血
        dogHP = Mathf.Max(0, dogHP - amount); // ← 修改:血量不会变负数
        Debug.Log("Ouch! Dog HP: " + dogHP);
        UpdateBars();
        if (dogHP <= 0) Lose();
    }

    public void DamageFood(int amount)
    {
        if (gameOver) return;                 // ← 新增
        foodHP = Mathf.Max(0, foodHP - amount); // ← 修改
        Debug.Log("Food took damage! Food HP: " + foodHP);
        UpdateBars();
        if (foodHP <= 0) Lose();
    }

    public void SetWave(int current, int total)
    {
        if (waveText != null)
            waveText.text = "WAVE: " + current + "/" + total;
    }

    public IEnumerator ShowMessage(string msg, Color color, float duration)
    {
        if (messageText == null) yield break;

        messageText.text = msg;
        messageText.color = color;

        // 闪烁三次
        for (int i = 0; i < 3; i++)
        {
            messageText.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.25f);
            messageText.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(0.15f);
        }

        // 最后停留一会儿再消失
        messageText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        messageText.gameObject.SetActive(false);
    }

    public void Win()
    {
        if (gameOver) return;                 // ← 新增:防止先输后赢同时触发
        gameOver = true;
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        gameOver = true;                      // ← 新增
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void UpdateBars()
    {
        if (dogHPBar != null)
            dogHPBar.fillAmount = (float)dogHP / maxDogHP;
        if (foodHPBar != null)
            foodHPBar.fillAmount = (float)foodHP / maxFoodHP;
    }
}