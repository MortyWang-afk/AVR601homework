using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 0.6f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
