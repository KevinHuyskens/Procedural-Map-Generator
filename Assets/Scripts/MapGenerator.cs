using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;



    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    [Range(0, 1)] public float outlinePercent;



    public float tileSize;

    Map currentMap;

    [Space(30)] [Header("UI Elements")] [Space(10)] public Slider maxObstacleHeight;
    public Slider minObstacleHeight;
    public Slider obstaclePercent;
    public Slider outlPercent;
    public Slider tlSize;
    public Slider mapSizeX;
    public Slider mapSizeY;
    public InputField seed;


    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        currentMap.maxObstacleHeight = maxObstacleHeight.value;
        currentMap.minObstacleHeight = minObstacleHeight.value;
        currentMap.obstaclePercent = obstaclePercent.value;
        outlinePercent = outlPercent.value;
        tileSize = tlSize.value;
        currentMap.mapSize.x = (int) mapSizeX.value;
        currentMap.mapSize.y = (int) mapSizeY.value;
        if (seed.text != "")
            currentMap.seed = int.Parse(seed.text);
        else
            currentMap.seed = 1;

        GenerateMap();
    }

  



    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);

        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x,y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y;y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;

                Renderer tileRenderer = newTile.GetComponent<Renderer>();
                Material tileMaterial = new Material(tileRenderer.sharedMaterial);
                tileMaterial.color = currentMap.tileColor;
                tileRenderer.sharedMaterial = tileMaterial;
            }
        }

        bool[,] obstacleMap = new bool[currentMap.mapSize.x, currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x*currentMap.mapSize.y*currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());

				Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
				
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2f, Quaternion.identity) as Transform;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight,(1 - outlinePercent) * tileSize);
                newObstacle.parent = mapHolder;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foreGroundColor, currentMap.backGroundColor, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;
            }
            else {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        while(queue.Count>0)
        {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x==0 ^y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY<obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX,neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;
    }


    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y)*tileSize;
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }
	[System.Serializable]
	public class Map {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        [Range(0.2f, 5f)]
        public float minObstacleHeight;
        [Range(0.2f, 5f)]
        public float maxObstacleHeight;
        public Color foreGroundColor;
        public Color backGroundColor;
        public Color tileColor;

        public Coord mapCenter{
            get {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
	}
}

