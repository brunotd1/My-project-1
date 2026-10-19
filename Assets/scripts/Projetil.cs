using UnityEngine;

public class Projetil : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public float velocidade = 18f;
    public float tempoVida = 3f;

    [Tooltip("Se for verdadeiro, este tiro veio de um tanque inimigo")]
    public bool ehTiroInimigo = false;

    void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider outro)
    {
        ProcessarImpacto(outro.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ProcessarImpacto(collision.gameObject);
    }

    private void ProcessarImpacto(GameObject alvo)
    {
        // Ignora o chão e tiros colidindo entre si
        if (alvo.name == "Plane" || alvo.name.Contains("Bala")) return;

        // TIRO DO INIMIGO:
        if (ehTiroInimigo)
        {
            // Ignora outros inimigos
            if (alvo.CompareTag("Enemy") || alvo.name.Contains("Inimigo")) return;

            // Se acertou o Jogador!
            if (alvo.CompareTag("Player") || alvo.name.Contains("TanqueJogador"))
            {
                Vida vida = alvo.GetComponent<Vida>();
                if (vida == null) vida = alvo.GetComponentInParent<Vida>();

                if (vida != null)
                {
                    vida.ReceberDano(1);
                }
                else if (GameManager.Instancia != null)
                {
                    GameManager.Instancia.TomarDanoJogador();
                }

                Destroy(gameObject);
                return;
            }
        }
        // TIRO DO JOGADOR:
        else
        {
            // Ignora o próprio jogador
            if (alvo.CompareTag("Player") || alvo.name.Contains("TanqueJogador")) return;

            // Se acertou um Inimigo!
            if (alvo.CompareTag("Enemy") || alvo.name.Contains("Inimigo"))
            {
                Vida vida = alvo.GetComponent<Vida>();
                if (vida == null) vida = alvo.GetComponentInParent<Vida>();

                if (vida != null)
                {
                    vida.ReceberDano(1);
                }

                Destroy(gameObject);
                return;
            }
        }

        // Se acertou um objeto quebravel (como o Cilindro ou tijolos)
        ObjetoQuebravel quebravel = alvo.GetComponent<ObjetoQuebravel>();
        if (quebravel == null) quebravel = alvo.GetComponentInParent<ObjetoQuebravel>();

        if (quebravel != null)
        {
            quebravel.Quebrar();
        }

        // Destrói a bala ao bater em qualquer obstáculo sólido
        Destroy(gameObject);
    }
}
