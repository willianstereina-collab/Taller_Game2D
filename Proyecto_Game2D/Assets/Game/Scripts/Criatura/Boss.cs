using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss : MonoBehaviour
{
    [SerializeField] Transform[] puntos; 
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Color[] coloresFase = { Color.white, new Color(1f, 0.7f, 0.4f), new Color(1f, 0.35f, 0.35f) };
    [SerializeField] float pausaEnPunto = 0.8f;

    JefeConfig config;
    int vidaActual;
    int faseAplicada;
    int faseSolicitada;
    bool activo;
    bool muerto;
    int incidentesVistos;
    int indicePunto = 1;
    int sentido = 1;
    float pausaHasta;
    Transform jugador;
    float minX, maxX;

    public event System.Action AlMorir;
    public int Fase => faseAplicada;
    public int FaseSolicitada => faseSolicitada;
    public float VidaNormalizada => config == null ? 1f : (float)vidaActual / config.vida;

    void Awake()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        config = GameManager.Instance.Config.jefe; 
        vidaActual = config.vida;

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) jugador = pc.transform;

        minX = float.MaxValue; maxX = float.MinValue;
        foreach (Transform p in puntos)
        {
            minX = Mathf.Min(minX, p.position.x);
            maxX = Mathf.Max(maxX, p.position.x);
        }

        indicePunto = Mathf.Min(1, puntos.Length - 1);
        AplicarFase(0);
    }

    public void Iniciar()
    {
        if (muerto) return;
        activo = true;
        incidentesVistos = GameManager.Instance.Incidentes.Count;
    }

    void Update()
    {
        if (!activo || muerto) return;
        VigilarMuertesDelJugador();
        Mover();
    }
    void Mover()
    {
        float objetivoX;
        bool persigue = faseAplicada >= 2 && jugador != null;

        if (persigue)
        {
            objetivoX = Mathf.Clamp(jugador.position.x, minX, maxX); 
        }
        else
        {
            if (Time.time < pausaHasta) return;
            objetivoX = puntos[indicePunto].position.x;
        }

        Vector3 pos = transform.position;
        float nuevaX = Mathf.MoveTowards(pos.x, objetivoX, VelocidadActual() * Time.deltaTime);

        if (sprite != null && !Mathf.Approximately(nuevaX, pos.x))
            sprite.flipX = nuevaX < pos.x;

        pos.x = nuevaX;
        transform.position = pos;

        if (!persigue && Mathf.Approximately(nuevaX, objetivoX))
        {
            SiguientePunto();
            pausaHasta = Time.time + (faseAplicada == 0 ? pausaEnPunto : pausaEnPunto * 0.4f);
        }
    }

    void SiguientePunto()
    {
        if (faseAplicada == 0)
        {
            indicePunto += sentido;
            if (indicePunto >= puntos.Length - 1) { indicePunto = puntos.Length - 1; sentido = -1; }
            else if (indicePunto <= 0) { indicePunto = 0; sentido = 1; }
        }
        else
        {
            int nuevo;
            do { nuevo = Random.Range(0, puntos.Length); }
            while (nuevo == indicePunto && puntos.Length > 1);
            indicePunto = nuevo;
        }
    }
    float VelocidadActual()
    {
        return config.velocidadPorFase[Mathf.Min(faseAplicada, config.velocidadPorFase.Count - 1)];
    }

    int DanoActual()
    {
        return config.danoPorFase[Mathf.Min(faseAplicada, config.danoPorFase.Count - 1)];
    }

    int CalcularFase()
    {
        float porcentaje = VidaNormalizada * 100f;
        int fase = 0;
        for (int i = 0; i < config.umbralesFase.Count; i++)
            if (porcentaje <= config.umbralesFase[i]) fase = i;
        return fase;
    }
    public void RecibirDano(int dano)
    {
        if (!activo || muerto) return;

        vidaActual = Mathf.Max(0, vidaActual - dano);

        if (vidaActual <= 0)
        {
            muerto = true;
            activo = false;
            AlMorir?.Invoke();
            gameObject.SetActive(false);
            return;
        }

        int fase = CalcularFase();
        if (fase != faseSolicitada)
        {
            faseSolicitada = fase;
            GameManager.Instance.EncolarEvento(TipoEvento.CambioFase, fase.ToString());
        }
    }

    public void AplicarFase(int fase)
    {
        faseAplicada = fase;
        faseSolicitada = fase;
        if (sprite != null && coloresFase.Length > 0)
            sprite.color = coloresFase[Mathf.Min(fase, coloresFase.Length - 1)];
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!activo) return;
        PlayerController j = other.GetComponentInParent<PlayerController>();
        if (j != null) j.RecibirGolpe("jefe", DanoActual());
    }

    void VigilarMuertesDelJugador()
    {
        var incidentes = GameManager.Instance.Incidentes;
        bool murio = false;
        for (int i = incidentesVistos; i < incidentes.Count; i++)
            if (incidentes[i].tipo == "muerte") murio = true;
        incidentesVistos = incidentes.Count;

        if (murio) ReiniciarCombate();
    }

    void ReiniciarCombate()
    {
        vidaActual = config.vida;
        AplicarFase(0);
        indicePunto = Mathf.Min(1, puntos.Length - 1);
        sentido = 1;
        pausaHasta = Time.time + 1f;
        Vector3 pos = transform.position;
        pos.x = puntos[indicePunto].position.x;
        transform.position = pos;
    }
}
