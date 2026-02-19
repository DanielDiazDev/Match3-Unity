using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;

namespace Match3.Scripts.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        // Level Data
        private int levelId = 1;
        private string levelName = "NewLevel";
        private int width = 5;
        private int height = 5;
        private int moves = 20;
        private int difficulty = 1;
        private int objectiveBonus = 500;
        private int[] starThresholds = new int[] { 1000, 2000, 3000 };
        
        // Grid Data
        private Dictionary<Vector2Int, LevelCellData> gridData = new Dictionary<Vector2Int, LevelCellData>();

        // Objectives Data
        private class TempObjectiveData
        {
            public PieceSO targetPiece;
            public int amount;
        }
        private List<TempObjectiveData> tempObjectives = new List<TempObjectiveData>();

        // Palette
        private TileSO selectedTile;
        private BlockSO selectedBlock;
        private LayerSO selectedLayer;
        
        private List<TileSO> allTiles;
        private List<BlockSO> allBlocks;
        private List<LayerSO> allLayers;

        private int selectedTab = 0;
        private Vector2 scrollPosition;
        
        // Load/Save
        private LevelDataSO levelToLoad;

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            LoadResources();
        }

        private void LoadResources()
        {
            allTiles = FindAssets<TileSO>();
            allBlocks = FindAssets<BlockSO>();
            allLayers = FindAssets<LayerSO>();
        }

        private List<T> FindAssets<T>() where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if(asset != null) assets.Add(asset);
            }
            return assets;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Match 3 Level Editor", EditorStyles.boldLabel);
            
            // Top Bar: Load / New
            EditorGUILayout.BeginHorizontal("box");
            if(GUILayout.Button("New Level", GUILayout.Width(80)))
            {
                if(EditorUtility.DisplayDialog("New Level", "Are you sure? Unsaved changes will be lost.", "Yes", "No"))
                {
                    ResetLevel();
                }
            }
            
            GUILayout.FlexibleSpace();
            
            levelToLoad = (LevelDataSO)EditorGUILayout.ObjectField("Load Level", levelToLoad, typeof(LevelDataSO), false);
            if(GUILayout.Button("Load", GUILayout.Width(60)))
            {
                LoadLevel();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Level Settings", "Grid Editor", "Objectives" });

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            switch (selectedTab)
            {
                case 0:
                    DrawSettings();
                    break;
                case 1:
                    DrawGridEditor();
                    break;
                case 2:
                    DrawObjectives();
                    break;
            }

            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save & Add to Database", GUILayout.Height(30)))
            {
                CreateLevelAsset();
            }
            GUI.backgroundColor = Color.white;
        }

        private void ResetLevel()
        {
            levelId = 1;
            levelName = "NewLevel";
            width = 5;
            height = 5;
            moves = 20;
            difficulty = 1;
            gridData.Clear();
            tempObjectives.Clear();
            levelToLoad = null;
        }

        private void LoadLevel()
        {
            if(levelToLoad == null) return;
            
            levelId = levelToLoad.id;
            levelName = levelToLoad.levelName;
            width = levelToLoad.width;
            height = levelToLoad.height;
            moves = levelToLoad.maxMoves;
            difficulty = levelToLoad.difficulty;
            objectiveBonus = levelToLoad.objectiveBonus;
            if(levelToLoad.starThresholds != null && levelToLoad.starThresholds.Length >= 3)
                starThresholds = (int[])levelToLoad.starThresholds.Clone();

            gridData.Clear();
            foreach(var kvp in levelToLoad.initialGridData)
            {
                LevelCellData newData = new LevelCellData();
                if(kvp.Value != null)
                {
                    newData.tile = kvp.Value.tile;
                    newData.block = kvp.Value.block;
                    newData.layer = kvp.Value.layer;
                }
                gridData[kvp.Key] = newData;
            }

            tempObjectives.Clear();
            foreach(var q in levelToLoad.quests)
            {
                if(q is PieceDestructionObjectiveSO pdo)
                {
                    tempObjectives.Add(new TempObjectiveData{ targetPiece = pdo.targetPiece, amount = pdo.amountRequired });
                }
            }
            
            Debug.Log($"Level {levelName} loaded!");
        }

        private void DrawSettings()
        {
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            levelId = EditorGUILayout.IntField("Level ID", levelId);
            levelName = EditorGUILayout.TextField("Level Name", levelName);
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            moves = EditorGUILayout.IntField("Max Moves", moves);
            difficulty = EditorGUILayout.IntSlider("Difficulty", difficulty, 1, 5);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scoring", EditorStyles.boldLabel);
            objectiveBonus = EditorGUILayout.IntField("Objective Bonus", objectiveBonus);
            
            EditorGUILayout.BeginHorizontal();
            starThresholds[0] = EditorGUILayout.IntField("1 Star", starThresholds[0]);
            starThresholds[1] = EditorGUILayout.IntField("2 Stars", starThresholds[1]);
            starThresholds[2] = EditorGUILayout.IntField("3 Stars", starThresholds[2]);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGridEditor()
        {
            EditorGUILayout.LabelField("Grid Editor", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset/Resize Grid"))
            {
                InitializeGrid();
            }
            if (GUILayout.Button("Randomize Tiles"))
            {
                RandomizeTiles();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            DrawPalette();
            EditorGUILayout.Space();
            DrawGridVisuals();
        }

        private void InitializeGrid()
        {
            gridData.Clear();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridData[new Vector2Int(x, y)] = new LevelCellData(); 
                }
            }
        }
        
        private void RandomizeTiles()
        {
            if (allTiles == null || allTiles.Count == 0) return;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (!gridData.ContainsKey(pos) || gridData[pos] == null)
                        gridData[pos] = new LevelCellData();
                    
                    if (gridData[pos].block == null)
                    {
                        gridData[pos].tile = allTiles[Random.Range(0, allTiles.Count)];
                    }
                }
            }
        }

        private void DrawPalette()
        {
            EditorGUILayout.LabelField("Palette", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            
            // Tiles
            DrawPaletteSection("Tiles", allTiles, ref selectedTile, (t) => t.sprite);
            // Blocks
            DrawPaletteSection("Blocks", allBlocks, ref selectedBlock, (b) => b.sprite);
            // Layers
            DrawPaletteSection("Layers", allLayers, ref selectedLayer, (l) => l.sprite);

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            string status = $"Selected: ";
            if (selectedBlock != null) status += $"Block: {selectedBlock.name} (Will replace Tile/Layer)";
            else
            {
                status += $"Tile: {(selectedTile ? selectedTile.name : "None")} ";
                status += $"| Layer: {(selectedLayer ? selectedLayer.name : "None")}";
            }
            EditorGUILayout.HelpBox(status, MessageType.Info);
        }

        private void DrawPaletteSection<T>(string title, List<T> items, ref T selected, System.Func<T, Sprite> spriteSelector) where T : Object
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(120));
            GUILayout.Label(title);
            
            if (GUILayout.Button("None")) selected = null;
            
            foreach (var item in items)
            {
                if (item == null) continue;
                
                bool isSelected = (object)selected == (object)item;
                GUI.backgroundColor = isSelected ? Color.green : Color.white;
                
                EditorGUILayout.BeginHorizontal();
                // Icon
                Sprite s = spriteSelector(item);
                if(s != null)
                {
                    Texture2D tex = AssetPreview.GetAssetPreview(s);
                    if(tex == null) tex = AssetPreview.GetAssetPreview(item); // Fallback
                    
                    // Draw Button with Texture
                    if(GUILayout.Button(new GUIContent(tex, item.name), GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        selected = item;
                        if(typeof(T) == typeof(BlockSO))
                        {
                            selectedTile = null;
                            selectedLayer = null;
                        }
                        else
                        {
                            selectedBlock = null;
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button(item.name)) 
                    {
                        selected = item;
                        if(typeof(T) == typeof(BlockSO)) 
                        {
                            selectedTile = null; 
                            selectedLayer = null; 
                        }
                        else selectedBlock = null;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }

        private void DrawGridVisuals()
        {
            if (gridData.Count == 0 && width > 0 && height > 0) InitializeGrid();

            EditorGUILayout.BeginVertical();
            for (int y = height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                for (int x = 0; x < width; x++)
                {
                    DrawCell(x, y);
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCell(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (!gridData.ContainsKey(pos)) gridData[pos] = new LevelCellData();

            LevelCellData cell = gridData[pos];
            
            float cellSize = 50f;
            Rect btnRect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

            EditorGUI.DrawRect(btnRect, new Color(0.2f, 0.2f, 0.2f));

            if (cell.block != null)
            {
                DrawTexture(btnRect, cell.block.sprite);
            }
            else
            {
                if (cell.tile != null) DrawTexture(btnRect, cell.tile.sprite);
                if (cell.layer != null) DrawTexture(btnRect, cell.layer.sprite);
            }
            
            UnityEngine.Event currentEvent = UnityEngine.Event.current;
            if ((currentEvent.type == UnityEngine.EventType.MouseDown || currentEvent.type == UnityEngine.EventType.MouseDrag) && btnRect.Contains(currentEvent.mousePosition))
            {
                ApplyBrush(cell);
                Repaint();
            }

            Handles.color = Color.black;
            Handles.DrawWireDisc(btnRect.center, Vector3.forward, cellSize/2);
        }

        private void DrawTexture(Rect rect, Sprite sprite)
        {
            if(sprite == null) return;
            Texture2D tex = AssetPreview.GetAssetPreview(sprite);
            if(tex != null) GUI.DrawTexture(rect, tex);
        }

        private void ApplyBrush(LevelCellData cell)
        {
            if (selectedBlock != null)
            {
                cell.block = selectedBlock;
                cell.tile = null;
                cell.layer = null;
            }
            else
            {
                cell.block = null;
                cell.tile = selectedTile;
                cell.layer = selectedLayer;
            }
        }

        private void DrawObjectives()
        {
            EditorGUILayout.LabelField("Objectives", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Add Piece Destruction Objective"))
            {
                tempObjectives.Add(new TempObjectiveData());
            }

            EditorGUILayout.Space();

            for (int i = 0; i < tempObjectives.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.BeginVertical();
                
                tempObjectives[i].targetPiece = (PieceSO)EditorGUILayout.ObjectField("Target Piece", tempObjectives[i].targetPiece, typeof(PieceSO), false);
                tempObjectives[i].amount = EditorGUILayout.IntField("Amount", tempObjectives[i].amount);
                
                EditorGUILayout.EndVertical();
                
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(40)))
                {
                    tempObjectives.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void CreateLevelAsset()
        {
            string path = "Assets/Match3/Data/Levels"; 
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
            
            LevelDataSO newLevel = CreateInstance<LevelDataSO>();
            newLevel.id = levelId;
            newLevel.levelName = levelName;
            newLevel.width = width;
            newLevel.height = height;
            newLevel.maxMoves = moves;
            newLevel.difficulty = difficulty;
            newLevel.objectiveBonus = objectiveBonus;
            newLevel.starThresholds = starThresholds;

            // Automatically include all available TileSO assets
            newLevel.availableTiles = new List<TileSO>(allTiles);

            foreach(var kvp in gridData)
            {
                newLevel.initialGridData.Add(kvp.Key, kvp.Value);
            }

            List<QuestObjectiveSO> newQuests = new List<QuestObjectiveSO>();
            foreach(var tempObj in tempObjectives)
            {
                if(tempObj.targetPiece != null)
                {
                    PieceDestructionObjectiveSO objSO = CreateInstance<PieceDestructionObjectiveSO>();
                    objSO.name = $"Destroy_{tempObj.targetPiece.name}";
                    objSO.targetPiece = tempObj.targetPiece;
                    objSO.amountRequired = tempObj.amount;
                    newQuests.Add(objSO);
                }
            }
            newLevel.quests = newQuests;
            
            string assetPath = $"{path}/{levelName}.asset";
            
            // Check if asset exists at path
            LevelDataSO existing = AssetDatabase.LoadAssetAtPath<LevelDataSO>(assetPath);
            bool overwrite = false;
            
            if(existing != null)
            {
                if(EditorUtility.DisplayDialog("Overwrite?", $"Level {levelName} already exists. Overwrite?", "Yes", "No"))
                {
                    overwrite = true;
                }
                else
                {
                     assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                }
            }
            
            if(overwrite && existing != null)
            {
                EditorUtility.CopySerialized(newLevel, existing);
                
                // Clear old sub assets
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach(Object o in assets)
                {
                    if(o is QuestObjectiveSO) DestroyImmediate(o, true);
                }
                
                // Add new sub assets
                foreach(var q in newLevel.quests)
                {
                    AssetDatabase.AddObjectToAsset(q, existing);
                }
                
                EditorUtility.SetDirty(existing);
                AssetDatabase.SaveAssets();
                newLevel = existing; // For DB ref
            }
            else
            {
                AssetDatabase.CreateAsset(newLevel, assetPath);
                foreach(var q in newLevel.quests)
                {
                    AssetDatabase.AddObjectToAsset(q, newLevel);
                }
                AssetDatabase.SaveAssets();
            }

            AddToDatabase(newLevel);
            
            EditorUtility.DisplayDialog("Success", $"Level saved to {assetPath}", "OK");
        }

        private void AddToDatabase(LevelDataSO newLevel)
        {
            string[] guids = AssetDatabase.FindAssets("t:LevelDatabase");
            if (guids.Length == 0)
            {
                Debug.LogWarning("No LevelDatabase found! Please create one.");
                return;
            }
            
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            LevelDatabase db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(path);
            
            List<LevelDataSO> levels = new List<LevelDataSO>(db.levels != null ? db.levels : new LevelDataSO[0]);
            if (!levels.Contains(newLevel))
            {
                levels.Add(newLevel);
                db.levels = levels.ToArray();
                EditorUtility.SetDirty(db);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
