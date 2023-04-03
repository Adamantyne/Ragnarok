using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Quest : MonoBehaviourPun
{
    [HideInInspector] public string NomeQuest; //Nome da Quest - Nome a ser exibido ao aceitar a quest
    public Objetivo objetivoQuest; //Controla o objetivo da quest (qtd necessaria, qtd já coletada e se já completou)
    [HideInInspector] public bool Concluida; //Controla se a quest está ou não concluída
    [HideInInspector] public List<GameObject> itemRecompensa; //Controla os itens de recompensa ao jogador por completar a quest

    public virtual void ConcluirQuest() //Conclui a quest 
    {
        Debug.Log("Quest concluída!");
        ControladorEventos.QuestConcluida(this);
        photonView.RPC("ConcederRecompensa",RpcTarget.MasterClient);
    }

    [PunRPC]
    public void ConcederRecompensa() //Concede recompensa ao jogador
    {
        Debug.Log("Quest concluída... conceder recompensa.");
        if(itemRecompensa == null)
            return;

        foreach(GameObject item in itemRecompensa)
        {
            Debug.Log("Recompensado com: " + item.name);
            PhotonNetwork.Instantiate(item.name,transform.position,transform.rotation);
        }
    }
}