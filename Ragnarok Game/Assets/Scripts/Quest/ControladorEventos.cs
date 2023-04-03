using UnityEngine;

public class ControladorEventos : MonoBehaviour 
{
    public static event System.Action<int> OnInimigoMorto = delegate { }; //Evento que controla o momento que o inimigo morre
    public static event System.Action<Quest> OnQuestProgressoAlterado = delegate { }; //Evento que controla o momento de mudança de status da quest
    public static event System.Action<Quest> OnQuestConcluida = delegate { }; //Evento que controla o momento em que a quest é concluída


    public static void InimigoMorto(int enemyID)
    {
        OnInimigoMorto(enemyID);
    }

    public static void QuestProgressoAlterado(Quest quest)
    {
        OnQuestProgressoAlterado(quest);
    }

    public static void QuestConcluida(Quest quest)
    {
        OnQuestConcluida(quest);
    }
}

