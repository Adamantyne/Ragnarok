using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Coletaveis : MonoBehaviourPun
{
    public GameObject itemPrefab;
    public string PlayfabItemID;

    //Lembre-se o objeto deverá estar com seu Collider marcado como Trigger 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.gameObject.GetPhotonView().IsMine)
            return;

        if(Inventario.instance.QtdItens < Inventario.instance.QtdMaximaItens && col.CompareTag("Player"))
        {
            GetComponent<CircleCollider2D>().enabled = false;
            Inventario.instance.QtdItens++;
            Inventario.instance.ItemColetado(PlayfabItemID, 0);

            PhotonNetwork.Destroy(gameObject);
        }
    }
}