using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.General;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Shapes;
using Match3.Scripts.PowerUps;
using Match3.Scripts.Score;
using UnityEngine;

public class MatchProcessor
{
    private GridSystem<GridObject> _grid;
    private PowerUpSystem _powerUpSystem;
    private MatchClassifier _classifier = new MatchClassifier();
    private IScoreSystem _scoreSystem;

    public MatchProcessor(GridSystem<GridObject> grid, PowerUpSystem powerUpSystem, IScoreSystem scoreSystem)
    {
        _grid = grid;
        _powerUpSystem = powerUpSystem;
        _scoreSystem = scoreSystem;
    }

    public async UniTask ProcessMatches(List<PieceMatch> matches, List<Vector2Int> centerHints = null)
    {
        var powerUpsToSpawn = new Dictionary<Vector2Int, string>();
        var positionsToClear = new HashSet<Vector2Int>();

        foreach (var match in matches)
        {
            if (match.positions.Count < 3)
                continue;

            ClassifiedMatch classified = _classifier.Classify(match);
            _scoreSystem.AddMatchScore(classified);
            Vector2Int powerUpPosition = classified.center;

            // If we have center hints (from a swap), use the hint that is part of this match
            if (centerHints != null)
            {
                foreach (var hint in centerHints)
                {
                    if (match.positions.Contains(hint))
                    {
                        powerUpPosition = hint;
                        break;
                    }
                }
            }

            string powerUpId = GetPowerUpForMatch(classified);

            if (!string.IsNullOrEmpty(powerUpId))
            {
                powerUpsToSpawn[powerUpPosition] = powerUpId;
            }

            foreach (var pos in match.positions)
            {
                positionsToClear.Add(pos);

                // Handle Layer Destruction
                var cell = _grid.GetValue(pos.x, pos.y);
                if (cell.HasLayer())
                {
                    cell.GetLayer().DestroyLayer();
                    cell.ClearLayer();
                }

                // Handle Adjacent Block Destruction
                CheckAndDamageBlock(pos.x + 1, pos.y);
                CheckAndDamageBlock(pos.x - 1, pos.y);
                CheckAndDamageBlock(pos.x, pos.y + 1);
                CheckAndDamageBlock(pos.x, pos.y - 1);
            }
        }

        foreach (var kvp in powerUpsToSpawn)
        {
            _powerUpSystem.ChangeToPowerUp(kvp.Value, kvp.Key);
            positionsToClear.Remove(kvp.Key);
        }

        foreach (var pos in positionsToClear)
        {
            _powerUpSystem.ClearCell(pos.x, pos.y);
        }

        await CommandQueue.Instance.WaitForAllCommands();
        if (positionsToClear.Count > 0 || powerUpsToSpawn.Count > 0)
        {
            await UniTask.Delay(200); // Wait for destruction animations if any
        }
    }

    private string GetPowerUpForMatch(ClassifiedMatch match)
    {
        if (match.shape == MatchShape.Line4)
        {
            // Check if it's horizontal or vertical
            bool isHorizontal = match.positions.All(p => p.y == match.positions[0].y);
            return isHorizontal ? "VerticalLine" : "HorizontalLine";
        }

        return match.shape switch
        {
            MatchShape.Line5 => "ColorBomb",
            MatchShape.TShape => "Bomb",
            MatchShape.LShape => "Bomb",
            _ => null
        };
    }

    private void CheckAndDamageBlock(int x, int y)
    {
        if (_grid.IsValid(x, y))
        {
            var cell = _grid.GetValue(x, y);
            if (cell.HasBlock())
            {
                var block = cell.GetBlock();
                block.TakeDamage();
                
                if (block.CurrentHealth <= 0)
                {
                    cell.ClearBlock();
                }
            }
        }
    }

    }
