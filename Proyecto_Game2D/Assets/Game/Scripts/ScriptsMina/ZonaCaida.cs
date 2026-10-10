using UnityEngine;

public class ZonaCaida : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public class Zonacaida : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            MinaController mina = FindFirstObjectByType<MinaController>();

            if (mina != null)
            {
                mina.RecibirDano(1);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
