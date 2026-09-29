using UnityEngine;

public class Colisao : MonoBehaviour
{
    private Renderer rend;
    private Color corOriginal;

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Pega a cor original tanto no padrão quanto no URP (_BaseColor)
        if (rend.material.HasProperty("_BaseColor"))
        {
            corOriginal = rend.material.GetColor("_BaseColor");
        }
        else
        {
            corOriginal = rend.material.color;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ignora se for o chão
        if (collision.gameObject.name == "Plane") return;

        // Detecta se for tag Parede OU se for o "cubo 2"
        if (collision.gameObject.CompareTag("Parede") || collision.gameObject.name == "cubo 2")
        {
            MudarCor(Color.yellow);
            Debug.Log("🟡 Bateu no obstáculo: Ficou Amarelo!");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Ignora se for o chão
        if (collision.gameObject.name == "Plane") return;

        if (collision.gameObject.CompareTag("Parede") || collision.gameObject.name == "cubo 2")
        {
            MudarCor(corOriginal);
            Debug.Log("🔴 Desencostou: Voltou para a cor original!");
        }
    }

    // Função que aplica a cor corretamente no shader do URP
    private void MudarCor(Color novaCor)
    {
        rend.material.color = novaCor;

        if (rend.material.HasProperty("_BaseColor"))
        {
            rend.material.SetColor("_BaseColor", novaCor);
        }
    }
}