using System.Collections.Generic;
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
        private ScriptableObject selectedItem;
        private PainterMode painterMode = PainterMode.Tile;
        private Dictionary<Vector2Int, LevelCellData> gridBuffer;
        private bool isInitialized;

        private enum PainterMode
        {
            Tile,
            Block,
            Layer
        }

        public override void OnInspectorGUI()
        {
            LevelDataSO levelData = (LevelDataSO)target;

            if (!isInitialized || gridBuffer == null)
            {
                gridBuffer = new Dictionary<Vector2Int, LevelCellData>();
                foreach (var kvp in levelData.initialGridData)
                {
                    gridBuffer[kvp.Key] = new LevelCellData
                    {
                        tile = kvp.Value.tile,
                        block = kvp.Value.block,
                        layer = kvp.Value.layer
                    };
                }
                isInitialized = true;
            }

            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Apply Grid Changes"))
            {
                Undo.RecordObject(levelData, "Apply Grid Changes");
                levelData.initialGridData.Clear();
                foreach (var kvp in gridBuffer)
                {
                    if (kvp.Value.tile != null || kvp.Value.block != null || kvp.Value.layer != null)
                    {
                        levelData.initialGridData.Add(kvp.Key, kvp.Value);
                    }
                }
                EditorUtility.SetDirty(levelData);
            }

            if (levelData.width <= 0 || levelData.height <= 0)
            {
                EditorGUILayout.HelpBox("Width and Height must be greater than 0", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space(10);
            DrawPainterToolbar();
            DrawItemPalette();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Level Grid", EditorStyles.boldLabel);
            DrawGrid(levelData);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(levelData);
            }

            if (GUILayout.Button("Generate Random Grid (Tiles Only)"))
            {
                GenerateRandomGrid(levelData);
            }
        }

        private void DrawPainterToolbar()
        {
            EditorGUILayout.LabelField("Painter Mode", EditorStyles.boldLabel);
            painterMode = (PainterMode)GUILayout.Toolbar((int)painterMode, new string[] { "Tiles", "Blocks", "Layers" });
        }

        private void DrawItemPalette()
        {
            EditorGUILayout.LabelField(painterMode + " Palette", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            // Clear button
            GUI.backgroundColor = selectedItem == null ? Color.green : Color.white;
            if (GUILayout.Button("🧹", GUILayout.Width(40), GUILayout.Height(40)))
            {
                selectedItem = null;
            }
            GUI.backgroundColor = Color.white;

            string searchFilter = painterMode switch
            {
                PainterMode.Tile => "t:TileSO",
                PainterMode.Block => "t:BlockSO",
                PainterMode.Layer => "t:LayerSO",
                _ => ""
            };

            string[] guids = AssetDatabase.FindAssets(searchFilter);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject item = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                Sprite sprite = GetSpriteFromItem(item);

                if (sprite == null) continue;

                GUI.backgroundColor = selectedItem == item ? Color.green : Color.white;
                if (GUILayout.Button(new GUIContent(sprite.texture, item.name), GUILayout.Width(40), GUILayout.Height(40)))
                {
                    selectedItem = item;
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private Sprite GetSpriteFromItem(ScriptableObject item)
        {
            if (item is TileSO t) return t.sprite;
            if (item is BlockSO b) return b.sprite;
            if (item is LayerSO l) return l.sprite;
            return null;
        }

        private void DrawGrid(LevelDataSO levelData)
        {
            for (int y = levelData.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < levelData.width; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (!gridBuffer.ContainsKey(pos)) gridBuffer[pos] = new LevelCellData();

                    var cell = gridBuffer[pos];
                    Texture texture = GetCellTexture(cell);

                    if (GUILayout.Button(texture, GUILayout.Width(CellSize), GUILayout.Height(CellSize)))
                    {
                        PaintCell(pos);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private Texture GetCellTexture(LevelCellData cell)
        {
            if (cell.block != null && cell.block.sprite != null) return cell.block.sprite.texture;
            if (cell.tile != null && cell.tile.sprite != null) return cell.tile.sprite.texture;
            if (cell.layer != null && cell.layer.sprite != null) return cell.layer.sprite.texture;
            return Texture2D.grayTexture;
        }

        private void PaintCell(Vector2Int pos)
        {
            var cell = gridBuffer[pos];
            switch (painterMode)
            {
                case PainterMode.Tile:
                    cell.tile = selectedItem as TileSO;
                    if (cell.tile != null) cell.block = null; // Tile removes block
                    break;
                case PainterMode.Block:
                    cell.block = selectedItem as BlockSO;
                    if (cell.block != null)
                    {
                        cell.tile = null;
                        cell.layer = null;
                    }
                    break;
                case PainterMode.Layer:
                    cell.layer = selectedItem as LayerSO;
                    if (cell.layer != null) cell.block = null; // Layer removes block
                    break;
            }
            gridBuffer[pos] = cell;
        }

        private void GenerateRandomGrid(LevelDataSO levelData)
        {
            if (levelData.availableTiles == null || levelData.availableTiles.Count == 0) return;
            Undo.RecordObject(levelData, "Generate Random Grid");
            gridBuffer.Clear();
            for (int x = 0; x < levelData.width; x++)
            {
                for (int y = 0; y < levelData.height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    TileSO randomTile = levelData.availableTiles[Random.Range(0, levelData.availableTiles.Count)];
                    gridBuffer[pos] = new LevelCellData { tile = randomTile };
                }
            }
            EditorUtility.SetDirty(levelData);
        }
    }
}