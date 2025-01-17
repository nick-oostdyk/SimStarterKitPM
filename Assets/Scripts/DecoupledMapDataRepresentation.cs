using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

// public static class MapDataInstance
// {


// }

public class MapData
{
	static public MapData instance;

	public int[,] mapTiles;
	public LinkedList<MapSpriteDataRepresentation> mapSprites;

	public int numTilesX = 20;
	public int numTilesY = 15;

	int selectedEditorButtonTextureID;
	int selectedEditorButtonMapObjectType;

	public void Init()
	{
		CreateMapTiles();
		mapSprites = new LinkedList<MapSpriteDataRepresentation>();
		//mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.Fighter1, 10, 2));
		//mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.BlackMage4, 3, 2));
		//mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.BlackMage1, 2, 3));
	}
	private void CreateMapTiles()
	{
		mapTiles = new int[numTilesX, numTilesY];
		for (int i = 0; i < numTilesX; i++)
			for (int j = 0; j < numTilesY; j++)
				mapTiles[i, j] = TextureSpriteID.Grass;
	}
	public void ProcessEditorButtonPressed(int mapObjectType, int textureID)
	{
		selectedEditorButtonMapObjectType = mapObjectType;
		selectedEditorButtonTextureID = textureID;
	}
	public void ProcessMapTilePressed(int x, int y)
	{
		if (selectedEditorButtonMapObjectType == MapObjectTypeID.Tile)
			mapTiles[x, y] = selectedEditorButtonTextureID;
		else if (selectedEditorButtonMapObjectType == MapObjectTypeID.Sprite)
		{
			if (selectedEditorButtonTextureID != TextureSpriteID.SpriteEraser)
			{
				MapSpriteDataRepresentation removeMe = null;
				foreach (MapSpriteDataRepresentation s in mapSprites)
				{
					if (s.x == x && s.y == y)
					{
						removeMe = s;
						break;
					}
				}
				if (removeMe != null)
					mapSprites.Remove(removeMe);

				mapSprites.AddLast(new MapSpriteDataRepresentation(selectedEditorButtonTextureID, x, y));

			}
			else if (selectedEditorButtonTextureID == TextureSpriteID.SpriteEraser)
			{
				MapSpriteDataRepresentation removeMe = null;
				foreach (MapSpriteDataRepresentation s in mapSprites)
				{
					if (s.x == x && s.y == y)
					{
						removeMe = s;
						break;
					}
				}
				if (removeMe != null)
					mapSprites.Remove(removeMe);
			}

		}
	}
	public void ProcessResize(int x, int y)
	{
		Debug.Log("Processing Resize: " + x + "," + y);

		numTilesX = x;
		numTilesY = y;


		LinkedList<MapSpriteDataRepresentation> removeMes = new LinkedList<MapSpriteDataRepresentation>();

		foreach (MapSpriteDataRepresentation s in mapSprites)
		{
			if (s.x >= numTilesX || s.y >= numTilesY)
				removeMes.AddLast(s);
		}

		foreach (MapSpriteDataRepresentation s in removeMes)
			mapSprites.Remove(s);


	}
	public void ProcessSaveMap(string name)
	{
		Debug.Log("Process SaveMap: " + name);
	}
	public void ProcessLoadMap(string name)
	{
		Debug.Log("Process LoadMap: " + name);
	}

	public MapData MakeDeepCopyOfModelData()
	{
		MapData c = new MapData();

		c.numTilesX = numTilesX;
		c.numTilesY = numTilesY;

		c.mapTiles = new int[numTilesX, numTilesY];

		for (int i = 0; i < numTilesX; i++)
		{
			for (int j = 0; j < numTilesY; j++)
			{
				c.mapTiles[i, j] = mapTiles[i, j];
			}
		}

		c.mapSprites = new LinkedList<MapSpriteDataRepresentation>();

		foreach (MapSpriteDataRepresentation s in mapSprites)
			c.mapSprites.AddLast(s.DeepCopy());

		return c;

	}

	public void DoTheConwayThing()
	{
		int[,] nextGenMapTiles = new int[numTilesX, numTilesY];

		for (int i = 0; i < numTilesX; i++)
			for (int j = 0; j < numTilesY; j++)
				_conway_CheckCell(i, j);
		mapTiles = nextGenMapTiles;

		#region old
		//if (mapTiles[i, j] == TextureSpriteID.Grass)
		//{
		//	int count = GetNeighbourCount(i, j);

		//	if (count == 2 || count == 3)
		//		nextGenMapTiles[i, j] = TextureSpriteID.Grass;
		//	else
		//		nextGenMapTiles[i, j] = TextureSpriteID.Water;
		//}
		//else
		//{
		//	int count = GetNeighbourCount(i, j);

		//	if (count == 3)
		//		nextGenMapTiles[i, j] = TextureSpriteID.Grass;
		//	else
		//		nextGenMapTiles[i, j] = TextureSpriteID.Water;
		//}
		#endregion
	}

	private int _conway_CheckCell(int x, int y)
	{
		int current = mapTiles[x, y];
		int count = GetNeighbourCount(x, y);

		if (current == TextureSpriteID.Water)
			if (count == 3) return TextureSpriteID.Grass;

		if (current == TextureSpriteID.Grass)
			if (count == 2 || count == 3) return TextureSpriteID.Grass;

		return TextureSpriteID.Water;
	}

	public int GetNeighbourCount(int x, int y)
	{
		int neighbourCount = 0;

		for (int i = x - 1; i <= x + 1; ++i)
			for (int j = y - 1; j <= y + 1; ++j)
			{
				if (i == x && j == y) continue;
				if (i < 0 || i >= numTilesX - 1) continue;
				if (j < 0 || j >= numTilesY - 1) continue;

				if (mapTiles[i, j] == TextureSpriteID.Grass)
					++neighbourCount;
			}

		#region old code
		/*
		if (y < numTilesY - 1)
		{
			if (mapTiles[x, y + 1] == TextureSpriteID.Grass)
				neighbourCount++;
		}

		if (y < numTilesY - 1 && x < numTilesX - 1)
		{
			if (mapTiles[x + 1, y + 1] == TextureSpriteID.Grass)
				neighbourCount++;
		}

		if (x < numTilesX - 1)
		{
			if (mapTiles[x + 1, y] == TextureSpriteID.Grass)
				neighbourCount++;
		}

		if (x < numTilesX - 1 && y > 0)
		{
			if (mapTiles[x + 1, y - 1] == TextureSpriteID.Grass)
				neighbourCount++;
		}

		if (y > 0)
		{
			if (mapTiles[x, y - 1] == TextureSpriteID.Grass)
				neighbourCount++;
		}

		if (x > 0 && y > 0)
		{
			if (mapTiles[x - 1, y - 1] == TextureSpriteID.Grass)
				neighbourCount++;
		}
		if (x > 0)
		{
			if (mapTiles[x - 1, y] == TextureSpriteID.Grass)
				neighbourCount++;
		}
		if (x > 0 && y < numTilesY - 1)
		{
			if (mapTiles[x - 1, y + 1] == TextureSpriteID.Grass)
				neighbourCount++;
		}
		*/
		#endregion
		return neighbourCount;
	}


	public async Task<bool> DoesPathExist(TileLocation start, TileLocation end)
	{
		#region psuedocode
		//GetTraversableNeighbours()

		//mark neighbours as checked
		//get neighbours' neighbours

		//looping through GetTraversableNeighbours()
		//check if one of the neighbors == end

		//
		#endregion

		// old manual check, we don't need this anymore! see below!
		//if (start.x == end.x && start.y == end.y)
		//	return true;

		// this is to show the search visually
		var editorLogic = TileEditorLogic.tileEditorLogic.GetComponent<TileEditorLogic>();
		LinkedList<TileLocation> checkedTileLocations = new LinkedList<TileLocation>();
		LinkedList<TileLocation> toBeCheckedTileLocations = new LinkedList<TileLocation>();

		#region first manual attempt of search
		// foreach (TileLocation tl in GetTraversableNeighbours(start.x, start.y))
		// {
		//     if (tl.x == end.x && tl.y == end.y)
		//         return true;

		//     toBeCheckedTileLocations.AddLast(tl);
		// }
		#endregion

		// these commented lines and the above check can be eliminated 
		// if we only seed the `tobeChecked` list with the first square to start.
		//checkedTileLocations.AddLast(start);
		//toBeCheckedTileLocations = GetTraversableNeighbours(start);
		toBeCheckedTileLocations.AddLast(start);

		Debug.Log("********");
		Debug.Log("Initial Neighbours:");
		foreach (TileLocation tl in toBeCheckedTileLocations)
			Debug.Log(tl.x + "," + tl.y);

		// foreach (TileLocation tl in toBeCheckedTileLocations)
		while (toBeCheckedTileLocations.Count > 0)
		{
			TileLocation tileToCheck = toBeCheckedTileLocations.First.Value;
			toBeCheckedTileLocations.RemoveFirst();

			//if (tileToCheck.x == end.x && tileToCheck.y == end.y)
			if (tileToCheck.Equals(end))
			{
				mapSprites.AddLast(new MapSpriteDataRepresentation(TextureSpriteID.BlackMage1, tileToCheck.x, tileToCheck.y));
				editorLogic.DestroyMapVisuals();
				editorLogic.CreateMapVisuals();
				return true;
			}

			mapTiles[tileToCheck.x, tileToCheck.y] = TextureSpriteID.Tent;
			checkedTileLocations.AddLast(tileToCheck);
			editorLogic.DestroyMapVisuals();
			editorLogic.CreateMapVisuals();

			foreach (TileLocation tl in GetTraversableNeighbours(tileToCheck.x, tileToCheck.y))
			{
				if (DoesListContainTileLocation(tl, checkedTileLocations))
					continue;
				if (DoesListContainTileLocation(tl, toBeCheckedTileLocations))
					continue;

				toBeCheckedTileLocations.AddLast(tl);

				Debug.Log("Adding: " + tl.x + "," + tl.y);
			}
			await Task.Delay(20);
		}

		return false;
	}


	private bool DoesListContainTileLocation(TileLocation tl, LinkedList<TileLocation> list)
	{
		foreach (TileLocation t in list)
			if (tl.Equals(t)) return true;

		return false;
	}


	public LinkedList<TileLocation> GetTraversableNeighbours(int x, int y)
	{
		LinkedList<TileLocation> neighbours = new LinkedList<TileLocation>();

		for (int i = x - 1; i <= x + 1; ++i)
			for (int j = y - 1; j <= y + 1; ++j)
			{
				if (i == x && j == y) continue;
				if (i < 0 || i >= numTilesX - 1) continue;
				if (j < 0 || j >= numTilesY - 1) continue;

				if (mapTiles[i, j] == TextureSpriteID.Grass)
					neighbours.AddLast(new TileLocation(i, j));
			}

		#region old code
		/*
		if (y < numTilesY - 1)
		{
			if (mapTiles[x, y + 1] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x, y + 1));
		}

		if (y < numTilesY - 1 && x < numTilesX - 1)
		{
			if (mapTiles[x + 1, y + 1] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x + 1, y + 1));
		}

		if (x < numTilesX - 1)
		{
			if (mapTiles[x + 1, y] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x + 1, y));
		}

		if (x < numTilesX - 1 && y > 0)
		{
			if (mapTiles[x + 1, y - 1] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x + 1, y - 1));
		}

		if (y > 0)
		{
			if (mapTiles[x, y - 1] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x, y - 1));
		}

		if (x > 0 && y > 0)
		{
			if (mapTiles[x - 1, y - 1] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x - 1, y - 1));
		}
		if (x > 0)
		{
			if (mapTiles[x - 1, y] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x - 1, y));
		}
		if (x > 0 && y < numTilesY - 1)
		{
			if (mapTiles[x - 1, y + 1] == TextureSpriteID.Grass)
				neighbours.AddLast(new TileLocation(x - 1, y + 1));
		}
		*/
		#endregion

		return neighbours;
	}

	public void SerializeMapData()
	{
		string jsonData = JsonConvert.SerializeObject(this);

		Debug.Log(jsonData);

		string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "saveThing.txt";

		StreamWriter sw = new StreamWriter(filePath);
		sw.WriteLine(jsonData);
		sw.Close();
	}

	public void DeserializeAndLoadMapData()
	{
		string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "saveThing.txt";
		StreamReader sr = new StreamReader(filePath);
		string jsonReadData = sr.ReadLine();

		MapData md = JsonConvert.DeserializeObject<MapData>(jsonReadData);

		MapData.instance = md;
	}

}

public class MapSpriteDataRepresentation
{
	public int id;
	public int x, y;

	public MapSpriteDataRepresentation(int ID, int X, int Y)
	{
		id = ID;
		x = X;
		y = Y;
	}

	public MapSpriteDataRepresentation DeepCopy()
	{
		return new MapSpriteDataRepresentation(id, x, y);
	}
}


public static class MapObjectTypeID
{
	public const int Tile = 1;
	public const int Sprite = 2;
}

public class TileLocation
{
	public int x, y;

	public TileLocation(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public override bool Equals(object obj)
	{
		TileLocation tl = obj as TileLocation;
		Debug.Assert(tl != null);
		return x == tl.x && y == tl.y;
	}

	public override int GetHashCode() => (x, y).GetHashCode();
}


// public class MapDataDeepCopy
// {
//     public int[,] mapTiles;
//     public LinkedList<MapSpriteDataRepresentation> mapSprites;

//     public int numTilesX;
//     public int numTilesY;

// }






// resize
// Serialization and Deserialization
// Unit test Input
// Deep copy model data
// Conway’s game of life
