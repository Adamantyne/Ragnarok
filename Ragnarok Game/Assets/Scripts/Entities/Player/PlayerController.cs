using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header ("Dados do jogador")]
    [HideInInspector] public bool Morto = false; //controla se o jogador est� morto
    private float velMover = 4f; //velocidade de movimento atual do personagem
    private float MoveX;  //valor de movimento no eixos x
    private float MoveY; //valor de movimento no eixo y
    private int Xp = 0;
    public int Level;
    public float VidaAtual; //controla a vida atual do jogador
    public float VidaMaxima; //controla a vida maxima do jogador
    public Player photonPlayer;

    [Header ("Combate")]
    public float poderAtaque; //controla o poder de ataque do jogador
    public float alcanceAtaque; //define o alcance do ataque

    [Header ("Objetos e Componentes")]
    public GameObject barraVida; //objeto da interface para mostrar a vida do jogador
    public GameObject CanvaUI; //canvas com nome e barra de vida do jogador
    public SpriteRenderer imgJogador;
    public TextMeshProUGUI nomeJogadorTxt; //Nome de identifica��o do jogador
    public TextMeshProUGUI LevelJogadorTxt; //Nome de identifica��o do jogador
    private Animator animator; //componente respons�vel pela anima��o do jogador
    private Rigidbody2D corpo; //controla a f�sica do jogador
    public AudioSource steps1;
    public AudioSource atack1;
    
    public GameObject InventarioPrefab;
    public Inventario InventarioJogador;

    private bool audioPlaing = false;

    public GameObject minhaCamera; //Objeto da camera que dever� ser desligado se n�o for localplayer

    public static PlayerController meuHeroi; //Pega o heroi do jogador


    void Awake ()
    {
        corpo = GetComponent<Rigidbody2D> ();
        animator = GetComponent<Animator> ();

        minhaCamera.SetActive (photonView.IsMine);
    }

    [PunRPC]
    public void InicializaJogador (Player player)
    {
        photonPlayer = player;
        CreatePlayer.instance.jogadores.Add (this);

        VidaAtual = VidaMaxima = 100;
        nomeJogadorTxt.text = player.NickName;
        LevelJogadorTxt.text = "Lv "+this.Level.ToString();

        photonView.RPC ("AtualizaBarraVida", RpcTarget.All, VidaAtual);
    
       if(player.IsLocal)
        {
            meuHeroi = this;          
            GameObject inv = Instantiate(InventarioPrefab);
            InventarioJogador = inv.GetComponentInChildren<Inventario>();
        }            
        else
            corpo.isKinematic = true;
    }

    void Update ()
    {
        if (photonView.IsMine)
        {
            Entradas ();
            animator.SetBool ("Dead", Morto);
        }
    }


    void FixedUpdate ()
    {
        if (!photonView.IsMine) return;

        if (!Morto) photonView.RPC ("Mover", RpcTarget.All); // Executa o m�todo respons�vel por mover o personagem
    }

    public void Entradas ()
    {
        MoveX = Input.GetAxisRaw ("Horizontal"); //Define o quanto de movimento o jogador ir� executar no eixo horizontal        
        MoveY = Input.GetAxisRaw ("Vertical"); //Define o quanto de movimento o jogador ir� executar no eixo vertical
        if (Input.GetMouseButtonDown (0)) //Verifica se o jogador clicou com o bot�o esquerdo mouse para atacar
        {
            photonView.RPC("Atacar",RpcTarget.All);
        }
    }

    [PunRPC]
    public void Atacar()
    {
        int ataqueAdicional = Level*5;
        float AtaqueTotal = poderAtaque + ataqueAdicional;
        animator.SetTrigger("Attack"); //Executa a anima��o de ataque
        photonView.RPC("ExecutarAudio",RpcTarget.All,"atack1");

        // Calcula a dire��o do ataque
        Vector3 direcao = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

        // dispara um raio na dire��o do ponteiro do mouse
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direcao, direcao, alcanceAtaque);

        // verifica se acertou um inimigo
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Inimigo"))
        {
            // causa dano ao inimigo
            Inimigo inimigo = hit.collider.gameObject.GetComponent<Inimigo>();
            if (!inimigo.Morto)
                inimigo.photonView.RPC("RecebeDano", RpcTarget.MasterClient, AtaqueTotal, PhotonNetwork.LocalPlayer);
        }

    }

    public void GanharXp(int XpGanho)
    {
        this.Xp += XpGanho;
        if (this.Xp >= 100)
        {
            this.Xp -= 100;
            Evoluir();
        }
    }

    public void Evoluir()
    {
        this.Level += 1;
        LevelJogadorTxt.text = "Lv "+this.Level.ToString();
    }

    [PunRPC]
    public void Mover ()
    {
        //Executa a anima��o de Andar caso velocidade seja diferente de zero
        animator.SetBool ("Running", (corpo.velocity.x != 0 || corpo.velocity.y != 0));

        //Ajusta o visual conforme a dire��o do personagem
        if (MoveX > 0)
        {
            photonView.RPC ("AjustaUI", RpcTarget.All, 1f, 0.003f);
        }
        else if (MoveX < 0)
        {
            photonView.RPC ("AjustaUI", RpcTarget.All, -1f, -0.003f);
        }

        //efetua o movimento do personagem
        corpo.velocity = new Vector2 (MoveX, MoveY) * velMover;

        if(corpo.velocity.x != 0 || corpo.velocity.y != 0){
            if(this.audioPlaing == false){
                photonView.RPC("ExecutarAudio",RpcTarget.All,"steps1");
                this.audioPlaing = true;
            }    
        }else{
            if(this.audioPlaing == true){
                photonView.RPC("PararAudio",RpcTarget.All,"steps1");
                this.audioPlaing = false;
            }
        }
    }

    [PunRPC]
    void AjustaUI (float ScaleX, float CanvaScaleX)
    {
        transform.localScale = new Vector3 (ScaleX, 1, 1);
        CanvaUI.transform.localScale = new Vector3 (CanvaScaleX, CanvaUI.transform.localScale.y, CanvaUI.transform.localScale.z);
    }

    [PunRPC]
    private void AtualizaBarraVida (float vida)
    {
        barraVida.transform.localScale = new Vector3 (vida / VidaMaxima, 1, 1);
    }

    [PunRPC]
    void RecebeDano(float dano)
    {
        VidaAtual -= dano;

        if (VidaAtual > 0)
        {
            photonView.RPC("GerarEfeitoDanoRecebido", RpcTarget.All);
            photonView.RPC("AtualizaBarraVida", RpcTarget.All, VidaAtual);
        }
        else
        {
            VidaAtual = 0;
            photonView.RPC("AtualizaBarraVida", RpcTarget.All, VidaAtual);
            Morte();
        }
    }

    [PunRPC]
    void GerarEfeitoDanoRecebido()
    {
        StartCoroutine(EfeitoDano());

        IEnumerator EfeitoDano()
        {
            imgJogador.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            imgJogador.color = Color.white;
        }
    }

    private void Morte()
    {
        if(this.Level>1)this.Level-=1;
        Morto = true;
        //desativa os efeitos de f�sica para o jogador impedindo de se mover
        corpo.isKinematic = true;
        corpo.velocity = Vector3.zero;
        StartCoroutine("Reviver");

    }

    IEnumerator Reviver()
    {
        LevelJogadorTxt.text = "Lv "+this.Level.ToString();
        yield return new WaitForSeconds(2);

        Morto = false;
        corpo.isKinematic = false;
        corpo.position = CreatePlayer.instance.spawnPoints[Random.Range(0, CreatePlayer.instance.spawnPoints.Length)].position;

        VidaAtual = VidaMaxima;

        photonView.RPC("AtualizaBarraVida", RpcTarget.All, VidaAtual);
    }

    [PunRPC]
    public void Curar(int valorCura)
    {
        VidaAtual = Mathf.Clamp(VidaAtual + valorCura,0,VidaMaxima);
        PlayfabLogin.PFL.SalvaDadosJogador("VidaAtual",VidaAtual.ToString());
        PlayerPrefs.SetString("VidaAtual",VidaAtual.ToString());
        photonView.RPC("AtualizaBarraVida",RpcTarget.All,VidaAtual);
    }
    
    [PunRPC]
    public void ExecutarAudio(string som)
    {
        if(som == "steps1") steps1.Play();
        if(som == "atack1") atack1.Play();
    } 

    [PunRPC]
    public void PararAudio(string som)
    {
        if(som == "steps1") steps1.Pause();
    } 
}