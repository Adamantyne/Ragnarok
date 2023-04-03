public class MinoQuest : Quest 
{
    private int quantidadeNecessaria = 3; //define a quantidade necessária de inimigos que devem ser mortos
    private int InimigoID = 1; //define o inimigo que deve ser morto

    //Constroi a quest
    void Awake()
    {
        NomeQuest = "Matador de Minotauros"; //define o nome da quest
        objetivoQuest = new Objetivo (quantidadeNecessaria, InimigoID, this); //define o objetivo da quest
    }
}
