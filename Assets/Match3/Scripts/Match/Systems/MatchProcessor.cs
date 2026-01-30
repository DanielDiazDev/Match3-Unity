using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
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

    public async UniTask ProcessMatches(List<PieceMatch> matches)
    {
        foreach (var match in matches)
        {
            if (match.positions.Count < 3)
                continue;

            ClassifiedMatch classified = _classifier.Classify(match);

            Vector2Int powerUpPosition = classified.center;
            string powerUpId = GetPowerUpForMatch(classified);

            if (!string.IsNullOrEmpty(powerUpId))
            {
                _powerUpSystem.ChangeToPowerUp(powerUpId, powerUpPosition);
            }

            DestroyMatchPieces(classified, !string.IsNullOrEmpty(powerUpId));
            await UniTask.Delay(200); // Wait for destruction animation
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

    private void DestroyMatchPieces(ClassifiedMatch match, bool spawningPowerUp)
    {
        foreach (var pos in match.positions)
        {
            if (spawningPowerUp && pos == match.center)
                continue;

            _powerUpSystem.ClearCell(pos.x, pos.y);
        }
    }
}