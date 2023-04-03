using UnityEngine;
using Photon.Pun;
using PlayFab.ClientModels;
using PlayFab;

public class ItemInventario : MonoBehaviourPun
{
    public int poderCura;
    public string ItemInstanceId;

    public void BtnCurar()
    {
        ConsumirItem(ItemInstanceId);
    }

     public void ConsumirItem(string item)
    {
        Inventario.instance.QtdItens--;
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest()
        {
            ConsumeCount = 1,
            ItemInstanceId = item,
        },
        result =>
        {
            Debug.Log("Curar Jogador");
            PlayerController.meuHeroi.photonView.RPC("Curar",RpcTarget.All,poderCura);
            Inventario.instance.Itens.Remove(gameObject);
            Destroy(gameObject);
        },
        error =>
        {
            Debug.Log(error.ErrorMessage);
        });
    }
}