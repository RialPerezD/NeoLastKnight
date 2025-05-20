using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileLoader : MonoBehaviour
{
    public static Dictionary<int, Dictionary<int, int[,]>> LoadWorldFromFiles(List<string> file_names)
    {
        Dictionary<int, Dictionary<int, int[,]>> all_block_maps = new Dictionary<int, Dictionary<int, int[,]>>();
        int fileIndex = 0;

        foreach(string file_name in file_names)
        {
            // Quitar extensión .txt y ruta completa del archivo (debe estar en Assets/Resources/)
            string resourceName = Path.GetFileNameWithoutExtension(file_name);

            // Cargar el archivo como TextAsset desde Resources
            TextAsset textAsset = Resources.Load<TextAsset>(resourceName);

            if (textAsset == null)
            {
                Debug.LogError("No se pudo cargar el archivo desde Resources: " + resourceName);
                continue;
            }

            // Leer el contenido del archivo de texto
            string[] blocks = textAsset.text.Split('-');

            int blockIndex = 0;

            // Inicializamos un nuevo diccionario para almacenar los bloques de este archivo
            Dictionary<int, int[,]> blockMapForFile = new Dictionary<int, int[,]>();


            // Recorremos los bloques separados por '-'
            foreach (string block in blocks)
            {
                if (string.IsNullOrWhiteSpace(block))
                    continue;

                // Creamos una matriz para almacenar la maya 13x7
                int[,] blockMatrix = new int[7, 13];  // 7 filas y 13 columnas

                // Recorremos las filas dentro del bloque
                string[] lines = block.Trim().Split('\n');

                for (int y = 0; y < lines.Length; y++)
                {
                    string[] columns = lines[y].Split(',');

                    for (int x = 0; x < columns.Length; x++)
                    {
                        // Convertimos el valor a entero y lo colocamos en la matriz
                        blockMatrix[y, x] = int.Parse(columns[x]);
                    }
                }

                // Almacenamos la matriz completa en el diccionario de bloques para este archivo
                blockMapForFile[blockIndex] = blockMatrix;
                blockIndex++;
            }

            // Almacenamos el diccionario de bloques de este archivo en el diccionario general
            all_block_maps[fileIndex] = blockMapForFile;
            fileIndex++;
        }

        return all_block_maps;
    }
}
