using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ControladorQuest : MonoBehaviourPun
{
    public List<Quest> QuestsAtivas = new List<Quest>(); //Controla as quests que o jogador aceitou mas ainda não concluiu
    public List<string> QuestsAceitas = new List<string>(); //Controla o nome das quests que o jogador já aceitou

    [SerializeField]
    private QuestUIItem questUIItem; //item para exibir o status da quest ativa
    [SerializeField]
    private Transform questUIParent; //local para exibir o status da quest ativa

    private QuestBancoDados questBD; //controla as quests do jogador

    private void Start()
    {
        questBD = GetComponent<QuestBancoDados>();
    }

    public Quest AceitaQuest(string nomeQuest, List<GameObject> lista)
    {
        QuestsAceitas.Add(nomeQuest);
        
        Quest AdicionaQuest = (Quest)gameObject.AddComponent(System.Type.GetType(nomeQuest));
        AdicionaQuest.itemRecompensa = lista;
        if(photonView.IsMine)
        {
            QuestsAtivas.Add(AdicionaQuest);
            questBD.AdicionaQuest(AdicionaQuest);
            

            QuestUIItem questUI = Instantiate(questUIItem, questUIParent);
            questUI.Setup(AdicionaQuest);
        }            
        return AdicionaQuest;
    }

    public bool JaAceitou(string nome)
    {
        foreach(var questFeita in QuestsAceitas)
        {
            if(questFeita == nome) return true;               
        }
        return false;
    }
}
