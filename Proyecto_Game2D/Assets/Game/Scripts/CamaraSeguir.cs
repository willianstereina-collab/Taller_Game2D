using UnityEngine;

public class CamaraSeguir : MonoBehaviour
{
    [SerializeField] Transform objetivo;
    [SerializeField] Vector3 desplazamiento = new Vector3(0f, 1.5f, -10f);
    [SerializeField] float suavizado = 8f;

    [Header("Límites (opcional)")]
    [SerializeField] bool limitar;
    [SerializeField] float minX, maxX, minY, maxY;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 destino = objetivo.position + desplazamiento;

        if (limitar)
        {
            destino.x = Mathf.Clamp(destino.x, minX, maxX);
            destino.y = Mathf.Clamp(destino.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, destino, suavizado * Time.deltaTime);
    }
}