using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckGround : MonoBehaviour
{
    public bool isGround = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.layer == LayerData.GroundLayerIndex)
        {
            isGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerData.GroundLayerIndex)
        {
            isGround = false;
        }
    }
}
