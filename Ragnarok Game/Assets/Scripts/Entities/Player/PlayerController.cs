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
    [HideInInspector] public bool Morto = false; //controla se o jogador está morto
    private float velMover = 4f; //velocidade de movimento atual do personagem
    private float MoveX;  //valor de movimento no eixos x
    private float MoveY; //valor de movimento no eixo y
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
    public TextMeshProUGUI nomeJogadorTxt; //Nome de identificação do jogador
    private Animator animator; //componente responsável pela animação do jogador
    private Rigidbody2D corpo; //controla a física do jogador

    public GameObject minhaCamera; //Objeto da camera que deverá ser desligado se não for localplayer

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

        photonView.RPC ("AtualizaBarraVida", RpcTarget.All, VidaAtual);

        if (player.IsLocal)
            meuHeroi = this;
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

        if (!Morto) photonView.RPC ("Mover", RpcTarget.All); // Executa o método responsável por mover o personagem
    }

    public void Entradas ()
    {
        MoveX = Input.GetAxisRaw ("Horizontal"); //Define o quanto de movimento o jogador irá executar no eixo horizontal        
        MoveY = Input.GetAxisRaw ("Vertical"); //Define o quanto de movimento o jogador irá executar no eixo vertical
        if (Input.GetMouseButtonDown (0)) //Verifica se o jogador clicou com o botão esquerdo mouse para atacar
        {
            photonView.RPC("Atacar",RpcTarget.All);
        }
    }

    [PunRPC]
    public void Atacar ()
    {
        animator.SetTrigger ("Attack"); //Executa a animação de ataque

        // Calcula a direção do ataque
        Vector3 direcao = (Input.mousePosition - Camera.main.WorldToScreenPoint (transform.position)).normalized;

        // dispara um raio na direção do ponteiro do mouse
        RaycastHit2D hit = Physics2D.Raycast (transform.position + direcao, direcao, alcanceAtaque);

        // verifica se acertou um inimigo
        //VAMOS TRATAR ESSA PARTE DO CÓDIGO QUANDO O JOGADOR ACERTAR O INIMIGO (Continuação na próxima etapa)

    }

    [PunRPC]
    public void Mover ()
    {
        //Executa a animação de Andar caso velocidade seja diferente de zero
        animator.SetBool ("Running", (corpo.velocity.x != 0 || corpo.velocity.y != 0));

        //Ajusta o visual conforme a direção do personagem
        if (MoveX > 0)
        {
            photonView.RPC ("AjustaUI", RpcTarget.All, 1f, 0.01f);
        }
        else if (MoveX < 0)
        {
            photonView.RPC ("AjustaUI", RpcTarget.All, -1f, -0.01f);
        }

        //efetua o movimento do personagem
        corpo.velocity = new Vector2 (MoveX, MoveY) * velMover;
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

}