using Match3.Scripts.Data;
using Match3.Scripts.Data.Item;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LevelDataSO))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        private const int CellSize = 40;
        private TileSO selectedTile;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelDataSO levelData = (LevelDataSO)target;

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
                EditorUtility.SetDirty(levelData);
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
                    var tile = levelData.initialPieces[pos];

                    Texture texture = tile && tile.sprite
                        ? tile.sprite.texture
                        : Texture2D.grayTexture;

                    if (GUILayout.Button(texture,
                            GUILayout.Width(CellSize),
                            GUILayout.Height(CellSize)))
                    {
                        levelData.initialPieces[pos] = selectedTile;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}