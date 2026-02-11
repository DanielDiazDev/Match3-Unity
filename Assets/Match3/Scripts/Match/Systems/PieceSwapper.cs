using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Grid;
using PrimeTween;
using UnityEngine;

namespace Match3.Scripts.Match.Systems
{
    public class PieceSwapper
    {
        private GridSystem<GridObject> _gridSystem;

        public PieceSwapper(GridSystem<GridObject> gridSystem)
        {
            _gridSystem = gridSystem;
        }

        public async UniTask SwapTile(Vector2Int a, Vector2Int b)
        {
            var objA = _gridSystem.GetValue(a.x, a.y);
            var objB = _gridSystem.GetValue(b.x, b.y);

            if (objA?.GetPiece() == null || objB?.GetPiece() == null)
                return;

            var posA = _gridSystem.GetWorldPositionCenter(a.x, a.y);
            var posB = _gridSystem.GetWorldPositionCenter(b.x, b.y);

            var sequence = Sequence.Create()
                .Group(Tween.Position(objA.GetPiece().transform, posB, 0.3f, Ease.InQuad))
                .Group(Tween.Position(objB.GetPiece().transform, posA, 0.3f, Ease.InQuad));

            // Update logical grid immediately
            _gridSystem.SetValue(a.x, a.y, objB);
            _gridSystem.SetValue(b.x, b.y, objA);

            objA.SetPosition(b.x, b.y);
            objB.SetPosition(a.x, a.y);

            await sequence;
        }

    }
}