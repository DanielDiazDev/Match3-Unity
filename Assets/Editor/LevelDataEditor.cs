using System.Collections.Generic;
using Match3.Scripts.Data;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Level;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelDataSO))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        private const int CellSize = 40;
        private TileSO selectedTile;
        private Dictionary<Vector2Int, TileSO> gridBuffer;
        private bool isInitialized;
        public override void OnInspectorGUI()
        {
            LevelDataSO levelData = (LevelDataSO)target;

            if (!isInitialized)
            {
                gridBuffer = new Dictionary<Vector2Int, TileSO>(levelData.initialPieces);
                isInitialized = true;
            }


            DrawDefaultInspector();

            EditorGUILayout.Space(10);

           
            if (GUILayout.Button("Apply Grid Changes"))
            {
                Undo.RecordObject(levelData, "Apply Grid Changes");

                levelData.initialPieces.Clear();

                foreach (var kvp in gridBuffer)
                    levelData.initialPieces.Add(kvp.Key, kvp.Value);

                EditorUtility.SetDirty(levelData);
            }

            if (levelData.width <= 0 || levelData.height <= 0)
            {
                EditorGUILayout.HelpBox("Width and Height must be greater than 0", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);
            DrawTilePalette();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Level Grid", EditorStyles.boldLabel);
            DrawGrid(levelData);

            if (GUI.changed)
            {
                Undo.RecordObject(levelData, "Grid Change");
                EditorUtility.SetDirty(levelData);
            }
        }


        private void DrawTilePalette()
        {
            EditorGUILayout.LabelField("Tile Palette", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            // Botón limpiar
            GUI.backgroundColor = selectedTile == null ? Color.green : Color.white;
            if (GUILayout.Button("🧹", GUILayout.Width(40), GUILayout.Height(40)))
            {
                selectedTile = null;
            }

            GUI.backgroundColor = Color.white;

            // Buscar todos los TileSO del proyecto
            string[] guids = AssetDatabase.FindAssets("t:TileSO");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TileSO tile = AssetDatabase.LoadAssetAtPath<TileSO>(path);

                if (tile == null || tile.sprite == null)
                    continue;

                GUI.backgroundColor = selectedTile == tile ? Color.green : Color.white;

                if (GUILayout.Button(
                        new GUIContent(tile.sprite.texture, tile.name),
                        GUILayout.Width(40),
                        GUILayout.Height(40)))
                {
                    selectedTile = tile;
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid(LevelDataSO levelData)
        {
            for (int y = levelData.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < levelData.width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);

                    if (!gridBuffer.ContainsKey(pos))
                        gridBuffer[pos] = null;

                    var tile = gridBuffer[pos];

                    Texture texture = tile && tile.sprite
                        ? tile.sprite.texture
                        : Texture2D.grayTexture;

                    if (GUILayout.Button(texture,
                            GUILayout.Width(CellSize),
                            GUILayout.Height(CellSize)))
                    {
                        gridBuffer[pos] = selectedTile; 
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

    }
}