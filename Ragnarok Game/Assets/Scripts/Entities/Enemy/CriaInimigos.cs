using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CriaInimigos : MonoBehaviourPun
{
    public GameObject inimigoPrefab; //Objeto pré-fabricado do Inimigo
    public float qtdMaximaInimigos; //quantidade máxima de Inimigos que terá neste ponto de criação
    public float RaioSpawn; //define raio da área de criação dos Inimigos
    public float tempoEntreSpawn; //define o tempo entre a criação dos inimigos

    private float ultimoSpawn; //controla quando foi a última criação
    public List<int> InimigosAtuais = new List<int>(); //controla os inimigos vivos

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (Time.time - ultimoSpawn > tempoEntreSpawn)
        {
            ultimoSpawn = Time.time;
            CriaInimigo();
        }
    }

    [PunRPC]
    void CriaInimigo()
    {

        //Lista temporária para armazenar os inimigos que já morreram
        List<int> inimigosMortos = new List<int>();

        // Procura por qualquer inimigo morto da lista de inimigosAtuais
        foreach (var inimigoID in InimigosAtuais)
        {
            if (PhotonView.Find(inimigoID) == null)
            {
                inimigosMortos.Add(inimigoID);
            }
        }

        //Atualiza a lista de Inimigos atuais retirando os inimigos mortos
        foreach (var inimigoID in inimigosMortos)
        {
            InimigosAtuais.Remove(inimigoID);
            //photonView.RPC("RetirarInimigo", RpcTarget.AllBuffered, inimigoID);
        }

        // se alcançou o maximo de inimigos, para por aqui
        if (InimigosAtuais.Count >= qtdMaximaInimigos)
            return;

        // caso contrário, cria inimigo
        Vector3 posAleatoria = Random.insideUnitCircle * RaioSpawn;

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject inimigo = PhotonNetwork.InstantiateRoomObject(inimigoPrefab.name, transform.position + posAleatoria, Quaternion.identity);
            int id = inimigo.GetComponent<PhotonView>().ViewID;
     
            InimigosAtuais.Add(id);
            //photonView.RPC("AdicionarInimigo", RpcTarget.AllBuffered, id);
        }

    }

    [PunRPC]
    public void RetirarInimigo(int inimigoID)
    {
        InimigosAtuais.Remove(inimigoID);
    }

    [PunRPC]
    public void AdicionarInimigo(int inimigoID)
    {
        InimigosAtuais.Add(inimigoID);
    }
}
