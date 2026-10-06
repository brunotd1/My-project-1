using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TanqueController : MonoBehaviour
{
    [Header("Movimentação Fluida")]
    public float velocidade = 7f;
    public float velocidadeGiro = 950f;

    [Header("Disparo")]
    [Tooltip("Arraste o Prefab da Bala aqui (Assets/Prefabs/BalaTanque)")]
    public GameObject balaPrefab;
    public Transform pontoDisparo;
    public float tempoEntreTiros = 0.3f;
    private float proximoTiroDisponivel = 0f;

    private Rigidbody rb;
    private Vector3 direcaoEntrada = Vector3.zero;
    private Vector3 direcaoAtualOlhar = Vector3.forward;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.useGravity = true;
        }

        gameObject.tag = "Player";
    }

    void Update()
    {
        LerEntrada();
        ProcessarDisparo();
    }

    void FixedUpdate()
    {
        MoverTanque();
    }

    private void LerEntrada()
    {
        float h = 0f;
        float v = 0f;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v += 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v -= 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h -= 1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h += 1f;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (h == 0f && v == 0f)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
#endif

        // Battle City: Prioriza o eixo de maior intensidade ou vertical
        if (v != 0f)
        {
            direcaoEntrada = new Vector3(0f, 0f, v).normalized;
        }
        else if (h != 0f)
        {
            direcaoEntrada = new Vector3(h, 0f, 0f).normalized;
        }
        else
        {
            direcaoEntrada = Vector3.zero;
        }
    }

    private void MoverTanque()
    {
        if (direcaoEntrada != Vector3.zero)
        {
            direcaoAtualOlhar = direcaoEntrada;

            // Rotação fluida e suave do chassi
            Quaternion rotacaoDesejada = Quaternion.LookRotation(direcaoAtualOlhar, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacaoDesejada, velocidadeGiro * Time.fixedDeltaTime);

            // Movimentação física sólida sem tremer contra paredes
            Vector3 deslocamento = direcaoEntrada * velocidade * Time.fixedDeltaTime;
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

    private void ProcessarDisparo()
    {
        bool apertouEspaco = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.jKey.wasPressedThisFrame))
        {
            apertouEspaco = true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Space))
        {
            apertouEspaco = true;
        }
#endif

        if (apertouEspaco && Time.time >= proximoTiroDisponivel)
        {
            proximoTiroDisponivel = Time.time + tempoEntreTiros;
            Disparar();
        }
    }

    private void Disparar()
    {
        Vector3 posDisparo = (pontoDisparo != null) ? pontoDisparo.position : (transform.position + transform.forward * 1.3f + Vector3.up * 0.35f);
        Quaternion rotDisparo = transform.rotation;

        // 🚀 O COMANDO PROFISSIONAL DA UNITY: INSTANTIATE!
        if (balaPrefab != null)
        {
            Instantiate(balaPrefab, posDisparo, rotDisparo);
            Debug.Log("🎯 POU! Tiro disparado via PREFAB!");
        }
        else
        {
            // Criação procedural de emergência se nenhum prefab for arrastado
            GameObject bala = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bala.name = "Bala_Tanque";
            bala.transform.position = posDisparo;
            bala.transform.rotation = rotDisparo;
            bala.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            Material matBala = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Color amareloTiro = new Color(1f, 0.85f, 0.1f);
            matBala.color = amareloTiro;
            if (matBala.HasProperty("_BaseColor")) matBala.SetColor("_BaseColor", amareloTiro);
            bala.GetComponent<Renderer>().sharedMaterial = matBala;

            Collider col = bala.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            Rigidbody rbBala = bala.AddComponent<Rigidbody>();
            rbBala.isKinematic = true;
            rbBala.useGravity = false;

            bala.AddComponent<Projetil>();
            Debug.Log("🎯 POU! Tiro disparado via Código!");
        }
    }
}
