using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;

public class Inventario : MonoBehaviourPun
{
    public List<GameObject> Itens = new List<GameObject>(); // Itens do jogador
    public GameObject InventarioUI; 
    public int QtdMaximaItens = 5; //Controla a quantidade máxima de itens no inventário
    public int QtdItens; //Controla a quantidade de itens atuais no inventário
    bool adicionando = false; //Garante que apenas um item será adicionado por vez
    public List<GameObject> ItensInventario = new List<GameObject>(); //Prefabs

    public static Inventario instance;

    void Start()
    {
        instance = PlayerController.meuHeroi.InventarioJogador;
        PegaItens();
    }

    public void AdicionaItem(GameObject item, string ItemInstanceId)
    {
        GameObject i = Instantiate(item, InventarioUI.transform);
        i.GetComponent<ItemInventario>().ItemInstanceId = ItemInstanceId;
        Itens.Add(i);
    }

    //pega os itens que o jogador possui
    public void PegaItens() 
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            foreach (var Item in result.Inventory)
            {
                Debug.Log("Item: " + Item.DisplayName);
                foreach (var itemInv in ItensInventario)
                {
                    if(itemInv.name == Item.DisplayName)
                    {
                        AdicionaItem(itemInv, Item.ItemInstanceId);
                    }
                }
            }
            QtdItens = Itens.Count;
        },
        erro =>
        {
            Debug.Log("Erro: " + erro.ErrorMessage);
        });        
	}

    public void ItemColetado(string item, int preco)
    {
        Debug.Log("ItemColetado");
        StartCoroutine(Aguardar());
        IEnumerator Aguardar()
        {
            yield return new WaitUntil( () => adicionando == false );
            adicionando = true;

            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
            {
                ItemId = item,
                VirtualCurrency = "VL",
                Price = preco
            },
            result =>
            {
                AtualizaItens();
            },
            error =>
            {
                Debug.Log("Erro: " + error.ErrorMessage);
            });
        }
    }

    public void AtualizaItens()
    {
        Debug.Log("AtualizandoItens");
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            if(Itens.Count > 0)
            {
                foreach (var Item in result.Inventory)
                {
                    foreach (var itemInv in Itens)
                    {
                        if(itemInv.GetComponent<ItemInventario>().ItemInstanceId != Item.ItemInstanceId)
                        {
                            foreach (var itemNovo in ItensInventario)
                            {
                                if(itemNovo.name == Item.DisplayName)
                                {
                                    AdicionaItem(itemNovo, Item.ItemInstanceId);
                                    adicionando = false;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var Item in result.Inventory)
                {
                    foreach (var itemInv in ItensInventario)
                    {
                        if(itemInv.name == Item.DisplayName)
                        {
                            AdicionaItem(itemInv, Item.ItemInstanceId);
                            adicionando = false;
                        }
                    }
                }
                
            }
        },
        erro =>
        {
            Debug.Log("Erro: " + erro.ErrorMessage);
        });
    }
}
