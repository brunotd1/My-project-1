using UnityEngine;

public class Projetil : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public float velocidade = 18f;
    public float tempoVida = 3f;

    void Start()
    {
        // Garante que a bala é destruída após 3 segundos se voar no vazio
        Destroy(gameObject, tempoVida);
    }

    void Update()
    {
        // Voa em linha reta na direção do cano
        transform.Translate(Vector3.forward * velocidade * Time.deltaTime);
    }

    // DISPARADO QUANDO A BALA ATRAVESSA UM COLISOR (TRIGGER)
    private void OnTriggerEnter(Collider outro)
    {
        // 1. Ignora o próprio tanque que atirou e o chão
        if (outro.CompareTag("Player") || outro.name == "Plane" || outro.name.Contains("Tanque"))
        {
            return;
        }

        // 2. Se acertar um objeto quebravel (como o Cilindro ou tijolos)
        ObjetoQuebravel quebravel = outro.GetComponent<ObjetoQuebravel>();
        if (quebravel == null)
        {
            // Tenta pegar no objeto pai caso tenha colidido com um colisor filho
            quebravel = outro.GetComponentInParent<ObjetoQuebravel>();
        }

        if (quebravel != null)
        {
            quebravel.Quebrar(); // Destrói o objeto com estilhaços!
            Debug.Log("💥 Bala destruiu: " + outro.gameObject.name);
        }
        else
        {
            // Acertou parede de aço ou obstáculo indestrutível
            Debug.Log("🛡️ Bala acertou obstáculo rígido: " + outro.gameObject.name);
        }

        // 3. A bala se destrói no impacto
        Destroy(gameObject);
    }
}
