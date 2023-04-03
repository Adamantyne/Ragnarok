using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks //utiliza uma classe especial do photon para ter retorno de chamadas
{
    //Quantidade m�xima de jogadores na sala de jogo
    public int QtdMaximaJogadores;
    // (singleton) - garante a exist�ncia de apenas uma inst�ncia dessa classe, mantendo um ponto global de acesso ao seu objeto.
    public static PhotonManager instance;

    //Define o nome da sala de jogo
    public string nomeSala = "MMORPG";

    //Este m�todo � chamado quando o script est� sendo carregado
    void Awake()
    {
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    //Este m�todo � chamado quando o script � executado antes de de qualquer m�todo de atualiza��o ser chamado pela primeira vez
    public void Conectar()
    {
        // conecta-se ao servidor
        PhotonNetwork.ConnectUsingSettings();
    }
    //Fun��o chamada toda vez que o jogador se conecta no servidor
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao servidor!");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado ao Lobby!");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)QtdMaximaJogadores;
        EntrarSala(nomeSala);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Sala de jogo inexistente, criando sala: " + nomeSala);
        CriaSala(nomeSala);
    }
    public override void OnJoinedRoom()
    {
        foreach(var player in PhotonNetwork.PlayerList)
        {
            if(player.NickName == PlayfabLogin.PFL.NickName)
                photonView.RPC("DesconectarJogador",RpcTarget.MasterClient, player);
        }
        PhotonNetwork.NickName = PlayfabLogin.PFL.NickName;
        Debug.Log("Entrou na sala: " + PhotonNetwork.CurrentRoom.Name);
        photonView.RPC("CarregaCena",PhotonNetwork.LocalPlayer,"Mundo");
    }

    private void DesconectarJogador(Player player)
    {
        PhotonNetwork.CloseConnection(player);
        Application.Quit();
    }

    //cria a sala de jogo
    public void CriaSala(string nomeSala)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)QtdMaximaJogadores;

        PhotonNetwork.CreateRoom(nomeSala, options);
    }

    //Entra na sala selecionada
    public void EntrarSala(string nomeSala)
    {
        PhotonNetwork.JoinRoom(nomeSala);
    }
    //Muda para a cena selecionada
    [PunRPC]
    public void CarregaCena(string nomeCena)
    {
        PhotonNetwork.LoadLevel(nomeCena);
    }
}