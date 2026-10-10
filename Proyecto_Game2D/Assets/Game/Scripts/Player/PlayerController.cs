using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public Transform origenSuelo;
    public float distanciaSuelo = 0.15f;
    public LayerMask capaSuelo;

    public Transform origenFrontal;
    public float distanciaFrontal = 0.6f;
    public LayerMask capaInteractuable;
    public KeyCode teclaInteractuar = KeyCode.E;

    public SpriteRenderer sprite;
    public Animator animator;

    Rigidbody2D rb;
    JugadorConfig jugador;
    float movimientoHorizontal;
    bool enSuelo;
    bool invulnerable;

    float multiplicadorVelocidad = 1;
    float multiplicadorSalto = 1;

    readonly WaitForSeconds esperaParpadeo = new WaitForSeconds(0.1f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (GameManager.Instance.EscenaActual == null)
        {
            GameManager.Instance.IniciarPartida();                       //gameManager temporal
            GameManager.Instance.IniciarEscena("Mina");                  //gameManager temporal
        }
        jugador = GameManager.Instance.Config.jugador;
    }

    void OnDisable()
    {
        if (sprite != null) sprite.enabled = true;
        invulnerable = false;
    }

    void Update()
    {
        LeerInput();
        DetectarSuelo();
        ActualizarAnimator();

        if (Input.GetKeyDown(teclaInteractuar))
            DetectarInteraccion();
    }

    void FixedUpdate()
    {
        float velocidad = jugador.velocidad * multiplicadorVelocidad;
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidad, rb.linearVelocity.y);
    }

    void LeerInput()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        if (movimientoHorizontal != 0)
        {
            if (sprite != null)
                sprite.flipX = movimientoHorizontal < 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (enSuelo)
            {
                float fuerza = jugador.fuerzaSalto * multiplicadorSalto;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerza);
            }
        }
    }

    void DetectarSuelo()
    {
        enSuelo = Physics2D.Raycast(origenSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);
    }

    void DetectarInteraccion()
    {
        Vector2 direccion = Vector2.right;

        if (sprite != null)
        {
            if (sprite.flipX)
                direccion = Vector2.left;
        }

        RaycastHit2D hit = Physics2D.Raycast(origenFrontal.position, direccion, distanciaFrontal, capaInteractuable);

        if (hit.collider == null) return;

        if (hit.collider.TryGetComponent(out IInteractable interactuable))
            interactuable.Interactuar();
    }

    void ActualizarAnimator()
    {
        if (animator == null) return;

        animator.SetFloat("VelocidadX", Mathf.Abs(movimientoHorizontal));
        animator.SetBool("EnSuelo", enSuelo);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vacio"))
        {
            GameManager.Instance.RegistrarMuerte("caida");
            Respawn();
        }
    }

    public void RecibirGolpe(string causa, int dano)
    {
        if (invulnerable) return;

        bool murio = GameManager.Instance.RegistrarGolpe(causa, dano);

        if (murio)
        {
            Respawn();
        }
        else
        {
            StartCoroutine(VentanaInvulnerabilidad());
        }
    }

    System.Collections.IEnumerator VentanaInvulnerabilidad()
    {
        invulnerable = true;
        float duracion = jugador.invulnerabilidad;
        float cronometro = 0;

        while (cronometro < duracion)
        {
            if (sprite != null) sprite.enabled = !sprite.enabled;
            yield return esperaParpadeo;
            cronometro += 0.1f;
        }

        if (sprite != null) sprite.enabled = true;
        invulnerable = false;
    }

    void Respawn()
    {
        if (GameManager.Instance.TieneCheckpoint(out Vector3 posicion))
            transform.position = posicion;

        rb.linearVelocity = Vector2.zero;
    }

    public void AplicarEfecto(RecursoConfig recurso)
    {
        switch (recurso.efecto)
        {
            case "velocidad":
                StartCoroutine(EfectoTemporal(() => multiplicadorVelocidad = recurso.valor, () => multiplicadorVelocidad = 1, recurso.duracion));
                break;
            case "salto":
                StartCoroutine(EfectoTemporal(() => multiplicadorSalto = recurso.valor, () => multiplicadorSalto = 1, recurso.duracion));
                break;
        }
    }

    System.Collections.IEnumerator EfectoTemporal(System.Action aplicar, System.Action revertir, float duracion)
    {
        aplicar();
        yield return new WaitForSeconds(duracion);
        revertir();
    }
}
