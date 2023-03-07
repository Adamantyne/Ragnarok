using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
 
public class PlayfabLogin : MonoBehaviour
{
    public static string PlayFabID; //armazena o ID do Playfab do jogador
    public string NickName;

    private string userEmail; //armazena o email do jogador
    private string userPassword; //armazena a senha do jogador
    private string username; //armazena o nome de usuario de jogador

    //Campos utilizados para efetuar o login do jogador
    public TMP_InputField inputUserEmailLogin;
    public TMP_InputField inputUserSenhaLogin;

    //Campos utilizados para criar uma nova conta para o jogador
    public TMP_InputField inputUsername;
    public TMP_InputField inputEmail;
    public TMP_InputField inputSenha;

    //Texto utilizado para passar informa��o ao jogador
    public TextMeshProUGUI Mensagem;

    //Painel de login que ser� inativado quando efetuado o login.
    public GameObject loginPanel;

    //Singleton
    public static PlayfabLogin PFL;

    private void Awake()
    {
        if (PFL != null && PFL != this)
        {
            Destroy(this.gameObject);
        }
        PFL = this;
        DontDestroyOnLoad(this.gameObject);
    }

    #region Login

    //M�todo para efetuar o Login do jogador
    public void EfetuarLogin()
    {
        if (string.IsNullOrEmpty(inputUserEmailLogin.text) || string.IsNullOrEmpty(inputUserSenhaLogin.text))
        {
            //Caso ao menos um dos campos esteja em branco, � solicitado ao jogador que os preencha.
            MostrarMensagem("Preencha todos os campos!");
        }
        else
        {
            //Caso os campos estejam preenchidos, � efetuada a chamada de login com os dados informados
            userEmail = inputUserEmailLogin.text;
            userPassword = inputUserSenhaLogin.text;
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
    }

    //M�todo para criar uma conta de usu�rio
    public void CriarConta()
    {
        if (string.IsNullOrEmpty(inputUsername.text) || string.IsNullOrEmpty(inputEmail.text) || string.IsNullOrEmpty(inputSenha.text))
        {
            //Caso ao menos um dos campos esteja em branco, � solicitado ao jogador que os preencha.
            MostrarMensagem("Preencha todos os campos!");
        }
        else
        {
            //Caso os campos estejam preenchidos, � efetuada a chamada de cria��o da conta com os dados informados
            username = inputUsername.text;
            userEmail = inputEmail.text;
            userPassword = inputSenha.text;
            var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        }
    }

    //M�todo para tratamento em caso de sucesso com a chamada de Login
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login efetuado com sucesso!");
        //Salva o PlayFabID do jogador
        PlayFabID = result.PlayFabId;
        //Salva informa��o do NickName
        PegaDisplayName(PlayFabID);
        loginPanel.SetActive(false);
        //Conectando ao servidor
        PhotonManager.instance.Conectar();
    }

    //M�todo para tratamento em caso de sucesso com a chamada de Cria��o da conta de usu�rio
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Conta criada com sucesso!");

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = username },
        sucesso =>
        {
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        , falha =>
        {
            Debug.Log(falha.ErrorMessage);
        });

    }

    //M�todo para tratamento em caso de falha com a chamada de Login
    private void OnLoginFailure(PlayFabError error)
    {
        //Tratamento para alguns tipos de falha. Retornando com a mensagem ao jogador
        switch (error.Error)
        {
            case PlayFabErrorCode.AccountNotFound:
                // Conta n�o encontrada
                MostrarMensagem("N�o foi poss�vel efetuar o login!\nConta n�o encontrada!");
                break;

            case PlayFabErrorCode.InvalidEmailOrPassword:
                // Email ou senha inv�lidos
                MostrarMensagem("N�o foi poss�vel efetuar o login!\nE-mail ou senha inv�lidos");
                break;

            default:
                // erro inesperado!
                MostrarMensagem("N�o foi poss�vel efetuar o login!\nVerifique os dados informados!");
                break;
        }
    }

    //M�todo para tratamento em caso de falha com a chamada de Cria��o de conta
    private void OnRegisterFailure(PlayFabError error)
    {
        //Tratamento para alguns tipos de falha. Retornando com a mensagem ao jogador
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidParams:
                //dados informados inv�lidos
                MostrarMensagem("N�o foi poss�vel criar a sua conta!\nVerifique os dados informados!");
                break;

            case PlayFabErrorCode.UsernameNotAvailable:
                // Nome de usu�rio j� em uso
                MostrarMensagem("N�o foi poss�vel criar a sua conta!\nNome de usu�rio j� em uso!");
                break;

            case PlayFabErrorCode.EmailAddressNotAvailable:
                // Email ou senha inv�lidos
                MostrarMensagem("J� possui uma conta para o e-mail informado!");
                break;

            default:
                //Erro inesperado!
                Debug.LogError(error.ErrorMessage);
                MostrarMensagem(error.ErrorMessage);
                break;
        }
    }

    //M�todo utilizado para exibir uma mensagem ao jogador sobre a solicita��o que ele fez.
    public void MostrarMensagem(string texto)
    {
        Mensagem.text = texto;
    }

    public void PegaDadosJogador(string id)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayFabID,
            Keys = null
        }, result => {

            if (result.Data == null || !result.Data.ContainsKey(id))
            {
                Debug.Log("Conte�do vazio!");
            }

            else if (result.Data.ContainsKey(id))
            {
                PlayerPrefs.SetString(id, result.Data[id].Value);
            }

        }, (error) => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void SalvaDadosJogador(string id, string valor)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {id, valor}
            }
        },
        result => Debug.Log("Dados do jogador atualizados com sucesso!"),
        error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void PegaDisplayName(string playFabId)
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        },
        result => NickName = result.PlayerProfile.DisplayName,
        error => Debug.Log(error.ErrorMessage));
    }
    #endregion    
}
