using System.Collections.Generic;
using UnityEngine;

public class QuestBancoDados : MonoBehaviour 
{
    public Dictionary<string, int[]> Quests = new Dictionary<string, int[]>(); //Controla as quests

    private void Awake()
    {
        ControladorEventos.OnQuestProgressoAlterado += AtualizaDadosQuest;
    }

    public bool Concluida(string questName)
    {
        if (Quests.ContainsKey(questName))
        {
            return System.Convert.ToBoolean(Quests[questName][0]);
        }
        return false;
    }

    public void AdicionaQuest(Quest quest)
    {
        Quests.Add(quest.NomeQuest, new int[] { 0, 0 });
    }

    public void AtualizaDadosQuest(Quest quest)
    {
        Quests[quest.NomeQuest] = new int[] { System.Convert.ToInt32(quest.Concluida), quest.objetivoQuest.qtdAtual};
        Debug.Log("Dados atualizados para: " + quest.NomeQuest);
    }
}
