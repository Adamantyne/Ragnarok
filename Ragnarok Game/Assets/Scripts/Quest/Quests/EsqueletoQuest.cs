public class EsqueletoQuest : Quest 
{
    private int quantidadeNecessaria = 10; //define a quantidade necessária de inimigos que devem ser mortos
    private int InimigoID = 2; //define o inimigo que deve ser morto

    //Constroi a quest
    void Awake()
    {
        NomeQuest = "Matador de Esqueletos"; //define o nome da quest
        objetivoQuest = new Objetivo (quantidadeNecessaria, InimigoID, this); //define o objetivo da quest
    }
}
