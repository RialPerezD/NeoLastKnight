using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public List<string> file_names;

    int worldWidth = 13;
    int worldHeight = 7;

    Dictionary<int, Dictionary<int, int[,]>> world_fragments;
    List<int[,]> level_layout_positions;
    public int[,] level;

    public List<Tile> block_tile_list;                  // El tile para el bloque (el valor del mapa minimo es 1)
    public List<GameObject> spawneable_items_list;      // El objeto a spawnear (el valor del mapa minimo es 50)
    public List<GameObject> spawneable_enemy_list;      // El enemigo a spawnear (el valor del mapa minimo es 75)
    public GameObject playerGO;

    public List<GameObject> objectsInScene;
    public GameObject player;

    // Para que quede centrada la generacion del mundo
    int world_x_offset = 6;
    int world_y_offset = 9;

    public static int playerNumber = 100;
    public static int objectsStart = 50;
    public static int enemysStart = 75;


    void Awake()
    {
        world_fragments = FileLoader.LoadWorldFromFiles(file_names);
        //Tests();
    }


    void Tests()
    {
        GenerateWorldLayout(2, true);
        PrintMatrixWithBlocks(level);
    }


    public void GenerateObjects(Tilemap tilemap)
    {
        objectsInScene = new List<GameObject>();
        world_y_offset += worldHeight * (level_layout_positions.Count - 2);

        for (int y = 0; y < level.GetLength(0); y++)
        {
            for (int x = 0; x < level.GetLength(1); x++)
            {
                int cellValue = level[y, x];
                if (cellValue == 0) continue;

                Vector3Int tilePos = new Vector3Int(x - world_x_offset, -y + world_y_offset, 0);
                Vector2Int logicPos = new Vector2Int(x, level.GetLength(0) - y);
                float yValue = (-y + world_y_offset + 1) * GameManager.grid_y_scale;
                Vector3 objectPos = new Vector3((x - world_x_offset) * GameManager.grid_x_scale, yValue, 0);

                if (cellValue < objectsStart)
                {
                    tilemap.SetTile(tilePos, block_tile_list[cellValue - 1]);
                }
                else if (cellValue == playerNumber)
                {
                    GameObject go = Instantiate(playerGO, objectPos, Quaternion.identity, tilemap.transform);
                    go.GetComponent<PlayerMovement>().posicion = logicPos;
                    player = go;
                }
                else if (cellValue >= objectsStart && cellValue < enemysStart)
                {
                    GameObject go = Instantiate(spawneable_items_list[cellValue - objectsStart], objectPos, Quaternion.identity, tilemap.transform);
                    go.GetComponent<Item>().posicion = logicPos;
                    objectsInScene.Add(go);
                }
                else
                {
                    float yCorrection = 0f;
                    if (cellValue == enemysStart) yCorrection = GameManager.grid_y_scale * 0.3f;
                    else if (cellValue == enemysStart + 2)
                    {
                        for (int ny = y - 1; ny <= y; ny++)
                        {
                            for (int nx = x - 1; nx <= x + 1; nx++)
                            {
                                level[ny, nx] = enemysStart + 2;
                            }
                        }

                        x++;
                        yCorrection = 0.9f;
                    }

                    objectPos.y += yCorrection;

                    GameObject go = Instantiate(spawneable_enemy_list[cellValue - enemysStart], objectPos, Quaternion.identity, tilemap.transform);
                    go.GetComponent<Enemy>().posicion = logicPos;
                    go.GetComponent<Enemy>().type = cellValue - enemysStart; 
                    objectsInScene.Add(go);
                }
            }
        }
    }



    public void CombineMatricesVertically()
    {
        if (level_layout_positions == null || level_layout_positions.Count == 0)
        {
            level = new int[0, 0];
            return;
        }

        int rowsPerMatrix = level_layout_positions[0].GetLength(0);
        int cols = level_layout_positions[0].GetLength(1);
        int totalRows = rowsPerMatrix * level_layout_positions.Count;

        int[,] combined = new int[totalRows, cols];

        for (int i = 0; i < level_layout_positions.Count; i++)
        {
            int[,] matrix = level_layout_positions[i];

            // Validación de tamaño coherente
            if (matrix.GetLength(0) != rowsPerMatrix || matrix.GetLength(1) != cols)
                throw new ArgumentException("Todas las matrices deben tener el mismo tamaño.");

            for (int row = 0; row < rowsPerMatrix; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    combined[i * rowsPerMatrix + row, col] = matrix[row, col];
                }
            }
        }

        level = combined;
    }


    public void PrintMatrixWithBlocks(int[,] matrix)
    {
        int totalRows = matrix.GetLength(0);
        int totalCols = matrix.GetLength(1);

        int numBlockRows = totalRows / worldHeight;
        int numBlockCols = totalCols / worldWidth;

        string line = "";
        for (int blockRow = 0; blockRow < numBlockRows; blockRow++)
        {
            for (int innerRow = 0; innerRow < worldHeight; innerRow++)
            {

                for (int blockCol = 0; blockCol < numBlockCols; blockCol++)
                {
                    for (int innerCol = 0; innerCol < worldWidth; innerCol++)
                    {
                        int globalRow = blockRow * worldHeight + innerRow;
                        int globalCol = blockCol * worldWidth + innerCol;

                        line += matrix[globalRow, globalCol].ToString().PadLeft(2) + " ";
                    }

                    line += "   ";
                }

                line += "\n";
            }
            line += "--------------------------------------\n";
        }
        print(line);
    }


    public void GenerateWorldLayout(int worldID, bool randomize)
    {
        level_layout_positions = new List<int[,]>();
        List<int[,]> layout = new List<int[,]>();

        if (!world_fragments.ContainsKey(worldID))
        {
            Debug.LogWarning($"World ID {worldID} no encontrado.");
            level_layout_positions = layout;
            return;
        }

        var roomDict = world_fragments[worldID];
        List<int> keys = new List<int>(roomDict.Keys);
        keys.Sort(); // Ordenamos por clave para que la 0 sea primera y la más alta última

        if (keys.Count == 0)
        {
            level_layout_positions = layout;
            return;
        }

        int firstKey = keys[0];
        int lastKey = keys[keys.Count - 1];

        layout.Add(roomDict[firstKey]); // Primera sala

        List<int> middleKeys = keys.GetRange(1, keys.Count - 2); // Salas intermedias

        if (randomize)
        {
            ShuffleList(middleKeys);
        }

        foreach (int key in middleKeys)
        {
            layout.Add(roomDict[key]);
        }

        if (keys.Count > 1)
            layout.Add(roomDict[lastKey]); // Ultima sala

        level_layout_positions = layout;

        CombineMatricesVertically();
    }


    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
