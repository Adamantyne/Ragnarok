using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreatePlayer : MonoBehaviourPun
{
    public GameObject item;
    public GameObject JogadorPrefab; //prefab do personagem do jogador
    public Transform[] spawnPoints; //possíveis locais para o jogador entrar no jogo

    public List<PlayerController> jogadores;
    public static CreatePlayer instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        photonView.RPC("EntrarJogo", PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void EntrarJogo()
    {
        CriaJogador();
    }

    private void CriaJogador()
    {
        GameObject jogadorObj = PhotonNetwork.Instantiate(JogadorPrefab.name, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        // inicializa o jogador
        jogadorObj.GetComponent<PhotonView>().RPC("InicializaJogador", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
    }
}
