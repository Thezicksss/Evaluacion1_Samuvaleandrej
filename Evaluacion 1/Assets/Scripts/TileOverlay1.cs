using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileOverlay1 : MonoBehaviour
{
   
    void Update()
    {
        
    }
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }
}
