using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource source;

    bool silenced = false; 

    [Header("Dog")]
    public AudioClip playerShoot;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip playerHurt;

    [Header("Crow")]
    public AudioClip crowHit;
    public AudioClip crowDeath;
    public AudioClip crowShoot;  
    public AudioClip crowEscape;

    [Header("Others")]
    public AudioClip bowlHit;

    [Header("Waves")]
    public AudioClip waveStart;
    public AudioClip waveClear;
    public AudioClip victory;

    [Header("Game State")]
    public AudioClip gameOver;
    public AudioClip loseLife;
    public AudioClip respawn;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Play(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (silenced) return; 
        if (clip == null) return;
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
        source.pitch = 1f;
    }
    // 掐掉正在播的所有音效，之后一律不再播，只放最后这一声
    public void SilenceExcept(AudioClip finalClip, float volume = 1f)
    {
        source.Stop();
        silenced = true;
        if (finalClip != null) source.PlayOneShot(finalClip, volume);
    }

    public void PlayVaried(AudioClip clip, float volume = 1f)
    {
        Play(clip, volume, Random.Range(0.92f, 1.08f));
    }
}