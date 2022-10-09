using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileMap : MonoBehaviour 
{
	public GameObject selectedUnit;
	public TileType[] tileTypes;

	int mapSizeX = 20;
	int mapSizeY = 10;
	int[,] tiles;
	Node[,] graph;

	void Start() 
	{
		selectedUnit.GetComponent<Unit>().tileX = (int)selectedUnit.transform.position.x;
		selectedUnit.GetComponent<Unit>().tileY = (int)selectedUnit.transform.position.y;
		selectedUnit.GetComponent<Unit>().map = this;

		TileMapData();
		PathfindingGraph();
		TileMapGenerate();
	}

	void TileMapData() 
	{
		tiles = new int[mapSizeX,mapSizeY];
		
		int x,y;
		
		for(x=0; x < mapSizeX; x++) 
		{
			for(y=0; y < mapSizeY; y++) 
			{
				tiles[x,y] = 0;
			}
		}
		//Agua//
		for(x=3; x <= 5; x++) 
		{
			for(y=3; y < 5; y++) 
			{
				tiles[x,y] = 2;
			}
		}
		for (x = 1; x <= 2; x++)
		{
			for (y = 3; y < 4; y++)
			{
				tiles[x, y] = 2;
			}
		}
		for (x = 9; x <= 11; x++)
		{
			for (y = 5; y < 6; y++)
			{
				tiles[x, y] = 2;
			}
		}
		for (x = 10; x <= 12; x++)
		{
			for (y = 6; y < 7; y++)
			{
				tiles[x, y] = 2;
			}
		}
		//Montaña//
		for (x = 15; x <= 18; x++)
		{
			for (y = 4; y < 6; y++)
			{
				tiles[x, y] = 1;
			}
		}
		for (x = 11; x <= 15; x++)
		{
			for (y = 1; y < 2; y++)
			{
				tiles[x, y] = 1;
			}
		}
		for (x = 1; x <= 5; x++)
		{
			for (y = 8; y < 9; y++)
			{
				tiles[x, y] = 1;
			}
		}
		for (x = 2; x <= 5; x++)
		{
			for (y = 0; y < 1; y++)
			{
				tiles[x, y] = 1;
			}
		}
		for (x = 14; x <= 19; x++)
		{
			for (y = 9; y < 10; y++)
			{
				tiles[x, y] = 1;
			}
		}

	}

	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY) 
	{

		TileType tt = tileTypes[ tiles[targetX,targetY] ];

		if(UnitCanEnterTile(targetX, targetY) == false)
			return Mathf.Infinity;

		float cost = tt.costToMove;

		if( sourceX!=targetX && sourceY!=targetY) {
			cost += 0.001f;
		}

		return cost;

	}

	void PathfindingGraph() 
	{
		graph = new Node[mapSizeX,mapSizeY];

		
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {
				graph[x,y] = new Node();
				graph[x,y].x = x;
				graph[x,y].y = y;
			}
		}

		
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {

			
				if(x > 0)
					graph[x,y].neighbours.Add( graph[x-1, y] );
				if(x < mapSizeX-1)
					graph[x,y].neighbours.Add( graph[x+1, y] );
				if(y > 0)
					graph[x,y].neighbours.Add( graph[x, y-1] );
				if(y < mapSizeY-1)
					graph[x,y].neighbours.Add( graph[x, y+1] );


			}
		}
	}

	void TileMapGenerate() {
		for(int x=0; x < mapSizeX; x++) {
			for(int y=0; y < mapSizeY; y++) {
				TileType tt = tileTypes[ tiles[x,y] ];
				GameObject go = (GameObject)Instantiate( tt.tileVisualPrefab, new Vector3(x, y, 0), Quaternion.identity );

				ClickableTile ct = go.GetComponent<ClickableTile>();
				ct.tileX = x;
				ct.tileY = y;
				ct.map = this;
			}
		}
	}

	public Vector3 TileCoordToWorldCoord(int x, int y) {
		return new Vector3(x, y, 0);
	}

	public bool UnitCanEnterTile(int x, int y) 
	{
		return tileTypes[ tiles[x,y] ].isWalkable;
	}

	public void GeneratePathTo(int x, int y) {
		selectedUnit.GetComponent<Unit>().currentPath = null;

		if( UnitCanEnterTile(x,y) == false ) 
		{

			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		List<Node> unvisited = new List<Node>();
		
		Node source = graph[
		                    selectedUnit.GetComponent<Unit>().tileX, 
		                    selectedUnit.GetComponent<Unit>().tileY
		                    ];
		
		Node target = graph[
		                    x, 
		                    y
		                    ];
		
		dist[source] = 0;
		prev[source] = null;

		foreach(Node v in graph) 
		{
			if(v != source) {
				dist[v] = Mathf.Infinity;
				prev[v] = null;
			}

			unvisited.Add(v);
		}

		while(unvisited.Count > 0) 
		{
			Node u = null;

			foreach(Node possibleU in unvisited) 
			{
				if(u == null || dist[possibleU] < dist[u]) 
				{
					u = possibleU;
				}
			}

			if(u == target)
			{
				break;	
			}

			unvisited.Remove(u);

			foreach(Node v in u.neighbours) 
			{
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if( alt < dist[v] ) 
				{
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		if(prev[target] == null) 
		{
			return;
		}

		List<Node> currentPath = new List<Node>();

		Node curr = target;

		while(curr != null) 
		{
			currentPath.Add(curr);
			curr = prev[curr];
		}

		currentPath.Reverse();

		selectedUnit.GetComponent<Unit>().currentPath = currentPath;
	}

}
