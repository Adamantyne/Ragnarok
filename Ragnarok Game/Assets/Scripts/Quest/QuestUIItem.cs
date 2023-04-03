using UnityEngine;
using TMPro;

public class QuestUIItem : MonoBehaviour 
{
    
    [SerializeField] private TextMeshProUGUI nomeQuest; //Objeto responsável em exibir o nome da quest ativa
    [SerializeField] private TextMeshProUGUI progressoQuest; //objeto responsável em exibir o progresso da quest ativa
    private Quest quest;

    private void Start()
    {
        ControladorEventos.OnQuestConcluida += QuestConcluida;
        ControladorEventos.OnQuestProgressoAlterado += AtualizaProgresso;
    }

    public void Setup(Quest questToSetup)
    {
        quest = questToSetup;
        nomeQuest.text = questToSetup.NomeQuest;
        progressoQuest.text = questToSetup.objetivoQuest.qtdAtual + "/" + questToSetup.objetivoQuest.qtdNecessaria;
    }

    public void AtualizaProgresso(Quest quest)
    {
        if (this.quest == quest)
        {
            progressoQuest.text = quest.objetivoQuest.qtdAtual + "/" + quest.objetivoQuest.qtdNecessaria;
        }
    }

    public void QuestConcluida(Quest quest)
    {
        if (this.quest == quest)
        {
            Destroy(this.gameObject, 1f);
        }
    }
}
