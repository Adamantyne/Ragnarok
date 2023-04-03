using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DoadorQuest : MonoBehaviourPun
{
    public string NomeScriptQuest; //Armazena o ome do Script da quest que você criou
    public string TituloQuest; //Título a ser exibido
    public string Descricao; //Descrição da quest
    public string Recompensa; //Descrição da recompensa

    public GameObject CanvasQuest; //Janela de aceitação da quest
    public TextMeshProUGUI TituloTXT; //Objeto responsável em exibir o título da quest
    public TextMeshProUGUI DescricaoQuestTXT; //Objeto responsável em exibir a descrição da quest
    public TextMeshProUGUI RecompensaTXT; //Objeto responsável em exibir a recompensa da quest
    public List<GameObject> recompensas; //Item(ns) que será(ão) entregues ao concluir a quest
    private ControladorQuest controladorQuest; //Controla as quests do jogador

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            controladorQuest =  col.GetComponent<ControladorQuest>();

            if(controladorQuest.JaAceitou(NomeScriptQuest))
                return;
            
            photonView.RPC("ConstruirQuest",col.GetComponent<PlayerController>().photonPlayer);
            photonView.RPC("ExibirCanvasQuest",col.GetComponent<PlayerController>().photonPlayer,true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        
        if(col.CompareTag("Player"))
        {
            photonView.RPC("ExibirCanvasQuest",col.GetComponent<PlayerController>().photonPlayer,false);
        }
    }

    [PunRPC]
    public void ConstruirQuest()
    {
        TituloTXT.text = TituloQuest;
        DescricaoQuestTXT.text = Descricao;
        RecompensaTXT.text = Recompensa;
    }

    [PunRPC]
    public void ExibirCanvasQuest(bool exibir)
    {
        CanvasQuest.SetActive(exibir);
    }

    public void AceitaQuest()
    {
        photonView.RPC("AdicionaQuest",RpcTarget.All);
    }

    [PunRPC]
    void AdicionaQuest()
    {
        controladorQuest.AceitaQuest(NomeScriptQuest, recompensas);
    }
}
