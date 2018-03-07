using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerEntity : MonoBehaviour
{
    GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Actor")
        {
            player = collision.gameObject;

            player.transform.SetParent(transform, true);
            Debug.Log(player.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(null, true);
            player = null;
        }
    }
}
