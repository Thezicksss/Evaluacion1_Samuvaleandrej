using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileType 
{
	public string name;
	public bool isWalkable = true;
	public float costToMove = 1;
	public GameObject tileVisualPrefab;

}
