using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Dados do Jogo")]
    public int pontuacao = 0;
    public int vidasJogador = 3;
    public bool jogoIniciado = false;
    public bool jogoFinalizado = false;

    [Header("Painéis de Interface (UI)")]
    public GameObject painelMenuInicial;
    public GameObject painelHUD;
    public GameObject painelGameOver;

    [Header("Botões")]
    public Button botaoIniciar;
    public Button botaoReiniciar;

    [Header("Textos")]
    public TextMeshProUGUI textoPontos;
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoPontosFinal;

    void Awake()
    {
        // ⚠️ Garante que o EventSystem existe na cena para os botões clicarem!
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    void Start()
    {
        // Conecta os botões via código automaticamente!
        if (botaoIniciar != null)
        {
            botaoIniciar.onClick.RemoveAllListeners();
            botaoIniciar.onClick.AddListener(IniciarPartida);
        }

        if (botaoReiniciar != null)
        {
            botaoReiniciar.onClick.RemoveAllListeners();
            botaoReiniciar.onClick.AddListener(ReiniciarJogo);
        }

        AtualizarUI();

        // Se houver Menu Inicial, começa com ele na tela e o tanque pausado
        if (painelMenuInicial != null)
        {
            painelMenuInicial.SetActive(true);
            if (painelHUD != null) painelHUD.SetActive(false);
            if (painelGameOver != null) painelGameOver.SetActive(false);
            ConfigurarControles(false);
        }
        else
        {
            IniciarPartida();
        }
    }

    void Update()
    {
        // Atalho: apertar Enter ou Espaço no teclado também inicia o jogo!
        if (!jogoIniciado && !jogoFinalizado)
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame ||
                    UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    IniciarPartida();
                }
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                IniciarPartida();
            }
#endif
        }
        else if (jogoFinalizado)
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame ||
                    UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
                {
                    ReiniciarJogo();
                }
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.R))
            {
                ReiniciarJogo();
            }
#endif
        }
    }

    // Chamado quando o jogador clica no botão "INICIAR JOGO"
    public void IniciarPartida()
    {
        jogoIniciado = true;
        jogoFinalizado = false;

        if (painelMenuInicial != null) painelMenuInicial.SetActive(false);
        if (painelHUD != null) painelHUD.SetActive(true);
        if (painelGameOver != null) painelGameOver.SetActive(false);

        // Ativa o controle do tanque
        ConfigurarControles(true);

        Debug.Log("🎮 Partida iniciada! Boa sorte comandante!");
    }

    public void AdicionarPontos(int valor)
    {
        if (jogoFinalizado || !jogoIniciado) return;

        pontuacao += valor;
        AtualizarUI();
        Debug.Log("⭐ + " + valor + " Pontos! Total: " + pontuacao);
    }

    public void TomarDanoJogador()
    {
        if (jogoFinalizado || !jogoIniciado) return;

        vidasJogador--;
        AtualizarUI();
        Debug.Log("💔 Jogador atingido! Vidas restantes: " + vidasJogador);

        if (vidasJogador <= 0)
        {
            GameOver();
        }
    }

    public void AtualizarUI()
    {
        if (textoPontos != null)
        {
            textoPontos.text = "PONTOS: " + pontuacao.ToString("D4");
        }

        if (textoVidas != null)
        {
            string coracoes = "";
            for (int i = 0; i < vidasJogador; i++) coracoes += "❤️ ";
            textoVidas.text = "VIDAS: " + coracoes;
        }
    }

    public void GameOver()
    {
        jogoFinalizado = true;
        Debug.Log("💀 GAME OVER! O tanque foi destruído.");

        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
        }

        if (textoPontosFinal != null)
        {
            textoPontosFinal.text = "PONTUAÇÃO FINAL: " + pontuacao.ToString("D4");
        }

        ConfigurarControles(false);
    }

    // Chamado pelo botão "JOGAR NOVAMENTE"
    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ConfigurarControles(bool ativo)
    {
        // Jogador
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            TanqueController controller = player.GetComponent<TanqueController>();
            if (controller != null) controller.enabled = ativo;
        }

        // Spawner
        SpawnerInimigos spawner = Object.FindAnyObjectByType<SpawnerInimigos>();
        if (spawner != null) spawner.enabled = ativo;
    }
}
