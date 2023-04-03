using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objetivo 
{
    public int ID_Inimigo; //Define qual inimigo deve ser o objetivo
    public int qtdNecessaria; //Quantidade necessária do objetivo
    public int qtdAtual; //Quantidade atual coletada do objetivo
    public bool Concluido; //controla se o objetivo já foi concluído
    public Quest quest; //Controla a quest
    
    public void Somar(int amount)
    {
        qtdAtual = Mathf.Min(qtdAtual + 1, qtdNecessaria);
        if (qtdAtual >= qtdNecessaria && !Concluido)
        {
            this.Concluido = true;
            Debug.Log("Objetivo concluído!");
            quest.ConcluirQuest();
        }
        
        ControladorEventos.QuestProgressoAlterado(quest);
    }

    public Objetivo(int quantidadeNecessaria, int InimigoID, Quest quest)
    {
        qtdAtual = 0;
        qtdNecessaria = quantidadeNecessaria;
        Concluido = false;
        this.quest = quest;
        this.ID_Inimigo = InimigoID;
        ControladorEventos.OnInimigoMorto += InimigoMorto;
    }

    void InimigoMorto(int InimigoID)
    {
        if (this.ID_Inimigo == InimigoID)
        {
            Somar(1);
            if (this.Concluido)
            {
                ControladorEventos.OnInimigoMorto -= InimigoMorto;
            }
        }
    }
}
