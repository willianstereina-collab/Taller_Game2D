using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activado || !other.CompareTag("Player"))
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarCheckpoint(transform.position);
            activado = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
