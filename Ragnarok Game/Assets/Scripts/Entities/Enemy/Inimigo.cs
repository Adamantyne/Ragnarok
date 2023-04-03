using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Inimigo : MonoBehaviourPun
{
    [Header("Dados do Inimigo")]
    public string nomeInimgo; //Nome que ser� exibido em cima do inimigo
    public float moveSpeed; //Define a velocidade de movimento do inimigo
    public float vidaAtual; //Define o quanto de vida o inimigo est� no momento
    public float vidaMaxima; //Define a quantidade m�xima de vida que o inimigo pode chegar
    public int Level;
    public float distAgro; //Define a dist�ncia que o inimigo inicia a persegui��o do jogador
    public float alcanceAtaque; //Define qual o alcance do ataque do inimigo
    private PlayerController JogadorAlvo; //Armazena o c�digo do jogador alvo (PlayerController)
    public float tempoProcurarJogador = 0.2f; //Intervalo entre uma busca e outra (visando impactar menos no processamento)
    private float tempoUltimaProcura; //Vari�vel de controle de tempo para busca do jogador
    private bool Atacando; //Controla se o inimigo est� atacando ou n�o no momento
    public bool Morto; //Controla se o inimigo est� morto ou n�o no momento
    private Vector3 posInicio; //Define a posicao inicial do inimigo
    public int inimigoID;

    [Header("Ataque")]
    public float poderAtaque; //Define o quanto de dano o inimigo ir� causar no jogador
    public float tempoEntreAtaques; //Define a cad�ncia de ataque do inimigo
    private float ultimoAtaque; //Vari�vel de Controle de tempo para o sistema de ataque do inimigo

    [Header("Components")]
    public TextMeshProUGUI nomeInimigoTXT; //Componente respons�vel por exibir o nome do inimigo na tela
    public TextMeshProUGUI levelInimigoTXT;
    public SpriteRenderer sr; //Componente respons�vel pela imagem do inimigo
    public Rigidbody2D corpoInimigo; //Componente respons�vel pela f�sica do inimigo
    public GameObject barraVidaInimigo; //Objeto respons�vel por exibir a vida atual do inimigo
    public GameObject CanvaUI; //Objeto respons�vel em exibir o nome e a vida do inimigo (Dever� ser "espelhado" (scale -1/ +1) quando o inimigo virar)
    public Animator animInimigo; //Componente respons�vel pelas anima��es do inimigo


    [Header("Drop")]
public List<GameObject> ObjetoSoltarMorte;


    void Start()
    {
        photonView.RPC("InicializaInimigo", RpcTarget.All);
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (JogadorAlvo != null && !Morto)
        {

            // calcula a dist�ncia
            float distancia = Vector2.Distance(transform.position, JogadorAlvo.transform.position);

            // Se est� proximo o suficiente para atacar, ataca
            if (distancia < alcanceAtaque && Time.time - ultimoAtaque >= tempoEntreAtaques && !JogadorAlvo.Morto)
                StartCoroutine("Atacar");
            // Caso contr�rio, continua seguindo o jogador
            else if (distancia > alcanceAtaque && !Atacando)
            {
                Vector3 dir = JogadorAlvo.transform.position - transform.position;
                corpoInimigo.velocity = dir.normalized * moveSpeed;
            }
            else //se est� longe demais, para de seguir o jogador.
            {
                corpoInimigo.velocity = Vector2.zero;
            }
        }
        if (!Morto && !Atacando)
            ProcuraJogador();
        else
            corpoInimigo.velocity = Vector3.zero;

        AnimaInimigo();
        if (corpoInimigo.velocity.x != 0 || corpoInimigo.velocity.y != 0) ViraInimigo();
    }


    void AnimaInimigo()
    {
        animInimigo.SetBool("Running", (corpoInimigo.velocity.magnitude != 0));
        animInimigo.SetBool("Dead", Morto);
    }

    [PunRPC]
    void AnimaAtaqueInimigo()
    {
        animInimigo.SetTrigger("Attack"); //Executa a anima��o de ataque
    }
    [PunRPC]
    public void InicializaInimigo()
    {
        posInicio = transform.position;
        Morto = false;
        vidaAtual = vidaMaxima;
        nomeInimigoTXT.text = nomeInimgo;
        levelInimigoTXT.text = "Lv "+this.Level.ToString();

        photonView.RPC("AtualizaVidaInimigo", RpcTarget.All, vidaAtual);
    }

    // ataca o jogador alvo
    IEnumerator Atacar()
    {
        photonView.RPC("AnimaAtaqueInimigo", RpcTarget.All);
        Atacando = true;
        ultimoAtaque = Time.time;
        JogadorAlvo.photonView.RPC("RecebeDano", JogadorAlvo.gameObject.GetComponent<PhotonView>().Owner, poderAtaque);
        yield return new WaitForSeconds(0.3f);
        Atacando = false;
    }


    // atualiza o alvo do jogador
    void ProcuraJogador()
    {
        if (Time.time - tempoUltimaProcura > tempoProcurarJogador)
        {
            tempoUltimaProcura = Time.time;


            // olha para todos os jogadores na cena
            foreach (PlayerController player in CreatePlayer.instance.jogadores)
            {
                if (player != null)
                {
                    // calcula a distancia at� o jogador
                    float dist = Vector2.Distance(transform.position, player.transform.position);

                    if (player == JogadorAlvo)
                    {
                        if (dist > distAgro)
                            JogadorAlvo = null;
                    }
                    else if (dist < distAgro)
                    {
                        if (JogadorAlvo != null)
                        {
                            float distAlvoAtual = Vector2.Distance(transform.position, JogadorAlvo.transform.position);
                            if (dist < distAlvoAtual)
                                JogadorAlvo = player;
                        }
                        else
                        {
                            JogadorAlvo = player;
                        }
                    }
                }
            }

            if (JogadorAlvo == null)
                corpoInimigo.velocity = Vector3.zero;
        }
    }

    [PunRPC]
    public void RecebeDano (float dano, Photon.Realtime.Player player)
	{
		if(Morto) return;

		vidaAtual -= dano;

		photonView.RPC("AtualizaVidaInimigo", RpcTarget.All, vidaAtual);

		if(vidaAtual <= 0)
		{
			vidaAtual = 0;
			photonView.RPC("AnunciaMorte",player);
			StartCoroutine("Morte");
		}                
		else
		{
			photonView.RPC("GerarEfeitoDano", RpcTarget.All);
		}
	}


    [PunRPC]
    void GerarEfeitoDano()
    {
        StartCoroutine(EfeitoDano());

        IEnumerator EfeitoDano()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }

    [PunRPC]
    public void AtualizaVidaInimigo(float vida)
    {
        vidaAtual = vida;
        barraVidaInimigo.transform.localScale = new Vector3(vidaAtual / vidaMaxima, 1, 1);
    }

    [PunRPC]
    public void MorteRede()
    {
        JogadorAlvo.GanharXp(this.Level);
        StartCoroutine("Morte");
    }

    	IEnumerator Morte ()
    {
        Morto = true;
        corpoInimigo.isKinematic = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        CanvaUI.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        if(ObjetoSoltarMorte.Count > 0)
        {
            int i = Random.Range(0,ObjetoSoltarMorte.Count-1);
            if(ObjetoSoltarMorte[i].name != string.Empty)
                PhotonNetwork.Instantiate(ObjetoSoltarMorte[i].name, transform.position, Quaternion.identity);
        }     
        yield return new WaitForSeconds(2.0f);
 
        // destroi o objeto na rede
        PhotonNetwork.Destroy(gameObject);
    }



    void ViraInimigo()
    {
        Vector2 Velocidade = corpoInimigo.velocity;
        var localvel = transform.InverseTransformDirection(Velocidade);

        //ajusta visual do inimigo
        if (localvel.x < 0 && transform.localScale.x != -1f)
        {
            photonView.RPC("AjustaInimigo", RpcTarget.AllBuffered, -1f, -0.003f);
        }
        else if (localvel.x > 0 && transform.localScale.x != 1f)
        {
            photonView.RPC("AjustaInimigo", RpcTarget.AllBuffered, 1f, 0.003f);
        }
    }

    [PunRPC]
    void AjustaInimigo(float ScaleX, float CanvaScaleX)
    {
        transform.localScale = new Vector3(ScaleX, 1, 1);
        CanvaUI.transform.localScale = new Vector3(CanvaScaleX, CanvaUI.transform.localScale.y, CanvaUI.transform.localScale.z);
    }

        [PunRPC]
    public void AnunciaMorte()
    {
        ControladorEventos.InimigoMorto(inimigoID);
    }
}

