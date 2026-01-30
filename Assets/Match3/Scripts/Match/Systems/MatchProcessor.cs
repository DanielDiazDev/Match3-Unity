using System.Collections.Generic;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Match.Systems.Find;
using Match3.Scripts.Match.Systems.Shapes;
using Match3.Scripts.PowerUps;
using UnityEngine;

public class MatchProcessor
{
    private GridSystem<GridObject> _grid;
    private PowerUpSystem _powerUpSystem;
    private MatchClassifier _classifier = new MatchClassifier();

    public MatchProcessor(GridSystem<GridObject> grid, PowerUpSystem powerUpSystem)
    {
        _grid = grid;
        _powerUpSystem = powerUpSystem;
    }

    public void ProcessMatches(List<PieceMatch> matches)
    {
        foreach (var match in matches)
        {
            if (match.positions.Count < 3)
                continue;

            ClassifiedMatch classified = _classifier.Classify(match);

            // Vector2Int powerUpPosition = classified.center;
            // string powerUpId = GetPowerUpForMatch(classified);
            //
            // if (!string.IsNullOrEmpty(powerUpId))
            // {
            //     _powerUpSystem.AddPowerUpToCommand(powerUpId, powerUpPosition);
            // }

            DestroyMatchPieces(classified);
        }
    }

    private string GetPowerUpForMatch(ClassifiedMatch match)
    {
        return match.shape switch
        {
            MatchShape.Line4 => "LineClear",
            MatchShape.Line5 => "ColorBomb",
            MatchShape.TShape => "Bomb",
            MatchShape.LShape => "Bomb",
            _ => null
        };
    }

    private void DestroyMatchPieces(ClassifiedMatch match)
    {
        foreach (var pos in match.positions)
        {
            // if (pos == match.center)
            //     continue;

            var gridObject = _grid.GetValue(pos.x, pos.y);
            var piece = gridObject.GetPiece();

            piece.OnDestroyed();
            gridObject.Clear();
        }
    }
}