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
    public int dogLives = 3;        //狗的命数

    public Image dogHPBar;
    public Image foodHPBar;
    public Text waveText;
    public TMP_Text messageText; 
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    public GameObject[] lifeIcons;      //三个命图标
    public AudioSource bgmSource;
    public GameObject[] hideOnGameOver;   // 结算时要隐藏的 UI（kill count、wave text 等）

    int maxDogHP, maxFoodHP;
    bool gameOver = false;                    // 结算标记

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        maxDogHP = dogHP;
        maxFoodHP = foodHP;
        Time.timeScale = 1f;                  // 保险,防止场景重载后仍冻结
        if (victoryPanel != null) victoryPanel.SetActive(false);   // 开局强制隐藏
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateBars();
        UpdateLives(); 
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
        if (gameOver) return;                 // 结算后不再扣血
        dogHP = Mathf.Max(0, dogHP - amount); // 血量不会变负数
        Debug.Log("Ouch! Dog HP: " + dogHP);
        UpdateBars();
        if (dogHP <= 0)
        {
            dogLives--;
            UpdateLives();  

            SFXManager.Instance.Play(SFXManager.Instance.loseLife); 

            if (dogLives > 0)
                StartCoroutine(RespawnRoutine());     // 还有命，复活
            else
                Lose();         // 没命了，真正结束
        }
    }

     IEnumerator RespawnRoutine()
    {
        // 先停一下，让玩家看到空血条
        yield return new WaitForSecondsRealtime(1f);

        dogHP = maxDogHP;
        UpdateBars();

        SFXManager.Instance.Play(SFXManager.Instance.respawn, 0.5f); 

        DogController dog = FindAnyObjectByType<DogController>();
        if (dog != null)
            dog.StartInvincible();

        Debug.Log("Respawn! Lives left: " + dogLives);
    }

    public void DamageFood(int amount)
    {
        if (gameOver) return;                 
        foodHP = Mathf.Max(0, foodHP - amount); 

        UpdateBars();
        if (foodHP <= 0) Lose();
    }

    public void SetWave(int current, int total)
    {   
        if (gameOver) return;

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
        if (gameOver) return;                 // 防止先输后赢同时触发
        gameOver = true;

        if (bgmSource != null) bgmSource.Stop();
        SFXManager.Instance.SilenceExcept(SFXManager.Instance.victory);

        HideHUD();

        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Lose()
    {
        gameOver = true; 

        if (bgmSource != null) bgmSource.Stop();
        SFXManager.Instance.SilenceExcept(SFXManager.Instance.gameOver, 0.4f);


        HideHUD();

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

     void UpdateLives()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].SetActive(i < dogLives);
        }
    }

    void HideHUD()
    {
        foreach (GameObject go in hideOnGameOver)
            if (go != null) go.SetActive(false);

        foreach (GameObject lifeIcon in lifeIcons)
            if (lifeIcon != null) lifeIcon.SetActive(false);
    }
}