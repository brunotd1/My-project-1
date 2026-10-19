using UnityEngine;

public class InimigoController : MonoBehaviour
{
    [Header("Movimentação e Perseguição")]
    public float velocidade = 4.5f;
    public float velocidadeGiro = 850f;
    public float distanciaTiro = 14f;

    [Header("Disparo da IA")]
    public GameObject balaInimigaPrefab;
    public Transform pontoDisparo;
    public float tempoEntreTiros = 1.2f;

    private Rigidbody rb;
    private Transform alvoJogador;
    private Vector3 direcaoAtual = Vector3.back;
    private float proximaDecisao = 0f;
    private float proximoTiro = 0f;
    private bool obstaculoBloqueando = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.useGravity = true;
        }

        gameObject.tag = "Enemy";

        // Localiza o jogador na cena
        BuscarJogador();
        proximaDecisao = Time.time + Random.Range(0.2f, 0.6f);
    }

    void Update()
    {
        if (alvoJogador == null)
        {
            BuscarJogador();
            return;
        }

        // 1. O cérebro da IA: recalcula a rota de perseguição periodicamente
        if (Time.time >= proximaDecisao)
        {
            proximaDecisao = Time.time + 0.4f;
            DecidirDirecaoPerseguicao();
        }

        // 2. Mira e Disparo: checa se o jogador está na mira do canhão
        ChecarDisparoNoJogador();
    }

    void FixedUpdate()
    {
        // Movimento suave em direção ao alvo
        if (direcaoAtual != Vector3.zero)
        {
            Quaternion rotacaoDesejada = Quaternion.LookRotation(direcaoAtual, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacaoDesejada, velocidadeGiro * Time.fixedDeltaTime);

            Vector3 deslocamento = direcaoAtual * velocidade * Time.fixedDeltaTime;
            if (rb != null)
            {
                rb.MovePosition(rb.position + deslocamento);
            }
            else
            {
                transform.position += deslocamento;
            }
        }
    }

    private void BuscarJogador()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null && player.activeInHierarchy)
        {
            alvoJogador = player.transform;
        }
    }

    private void DecidirDirecaoPerseguicao()
    {
        if (alvoJogador == null) return;

        Vector3 diferenca = alvoJogador.position - transform.position;
        float diffX = diferenca.x;
        float diffZ = diferenca.z;

        Vector3 direcaoEixoZ = (diffZ >= 0) ? Vector3.forward : Vector3.back;
        Vector3 direcaoEixoX = (diffX >= 0) ? Vector3.right : Vector3.left;

        // Se bateu em um obstáculo, tenta o eixo alternativo para contornar
        if (obstaculoBloqueando)
        {
            obstaculoBloqueando = false;
            // Inverte a preferência para desviar
            direcaoAtual = (Mathf.Abs(diffZ) > Mathf.Abs(diffX)) ? direcaoEixoX : direcaoEixoZ;
            return;
        }

        // Battle City: Persegue no eixo de maior distância primeiro
        if (Mathf.Abs(diffZ) > Mathf.Abs(diffX))
        {
            direcaoAtual = direcaoEixoZ;
        }
        else
        {
            direcaoAtual = direcaoEixoX;
        }
    }

    private void ChecarDisparoNoJogador()
    {
        if (alvoJogador == null) return;

        Vector3 vetorParaJogador = alvoJogador.position - transform.position;
        float distancia = vetorParaJogador.magnitude;

        if (distancia > distanciaTiro) return;

        // Verifica se o tanque já está apontando diretamente para o jogador (alinhamento)
        float alinhamento = Vector3.Dot(transform.forward, vetorParaJogador.normalized);

        // Se estiver de frente para o jogador (mais de 85% alinhado na mira)
        if (alinhamento > 0.85f && Time.time >= proximoTiro)
        {
            proximoTiro = Time.time + tempoEntreTiros;
            Disparar();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Plane")
        {
            // Bateu em algo: avisa para desviar pelo outro eixo na próxima decisão
            obstaculoBloqueando = true;
            DecidirDirecaoPerseguicao();
        }
    }

    private void Disparar()
    {
        Vector3 posDisparo = (pontoDisparo != null) ? pontoDisparo.position : (transform.position + transform.forward * 1.3f + Vector3.up * 0.35f);
        Quaternion rotDisparo = transform.rotation;

        if (balaInimigaPrefab != null)
        {
            GameObject bala = Instantiate(balaInimigaPrefab, posDisparo, rotDisparo);
            Projetil proj = bala.GetComponent<Projetil>();
            if (proj != null) proj.ehTiroInimigo = true;
        }
        else
        {
            // Fallback caso não tenha prefab conectado
            GameObject bala = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bala.name = "Bala_Inimigo";
            bala.transform.position = posDisparo;
            bala.transform.rotation = rotDisparo;
            bala.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            Material matBalaInimiga = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Color vermelhoTiro = new Color(1f, 0.2f, 0.2f);
            matBalaInimiga.color = vermelhoTiro;
            if (matBalaInimiga.HasProperty("_BaseColor")) matBalaInimiga.SetColor("_BaseColor", vermelhoTiro);
            bala.GetComponent<Renderer>().sharedMaterial = matBalaInimiga;

            Collider col = bala.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            Rigidbody rbBala = bala.AddComponent<Rigidbody>();
            rbBala.isKinematic = true;
            rbBala.useGravity = false;

            Projetil proj = bala.AddComponent<Projetil>();
            proj.ehTiroInimigo = true;
            proj.velocidade = 15f;
        }

        Debug.Log("🎯 [IA] Inimigo mirou no jogador e ATIROU!");
    }
}
