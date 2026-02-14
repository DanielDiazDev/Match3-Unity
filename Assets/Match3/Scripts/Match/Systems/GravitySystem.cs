using System.Collections.Generic;
using System.Linq;
using Assets.Match3.Scripts.Grid;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data;
using Match3.Scripts.Grid;
using PrimeTween;

namespace Match3.Scripts.Core
{
    public class GravitySystem
    {
        private GridSystem<GridObject> _grid;
        private int _width;
        private int _height;

        public GravitySystem(GridSystem<GridObject> grid, int width, int height)
        {
            _grid = grid;
            _width = width;
            _height = height;
        }

        public async UniTask<bool> ApplyGravity()
        {
            bool movedAny = false;

            Sequence sequence = default;
            bool sequenceCreated = false;

            // 1. Vertical Pass
            for (int x = 0; x < _width; x++)
            {
                int writeY = 0;

                for (int readY = 0; readY < _height; readY++)
                {
                    var readCell = _grid.GetValue(x, readY);

                    if (readCell.HasBlock() || !readCell.IsPlayable())
                    {
                        writeY = readY + 1;
                        continue;
                    }

                    if (readCell.IsEmpty())
                        continue;

                    if (readY != writeY)
                    {
                        var writeCell = _grid.GetValue(x, writeY);
                        var piece = readCell.GetPiece();

                        writeCell.SetPiece(piece);
                        readCell.Clear();

                        var targetPos = _grid.GetWorldPositionCenter(x, writeY);
                        var tween = Tween.Position(
                            piece.transform,
                            targetPos,
                            0.3f,
                            Ease.OutBounce);

                        if (!sequenceCreated)
                        {
                            sequence = Sequence.Create();
                            sequenceCreated = true;
                        }

                        sequence.Group(tween);
                        movedAny = true;
                    }

                    writeY++;
                }
            }

            // 2. Diagonal Pass
            for (int y = 0; y < _height - 1; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    var targetCell = _grid.GetValue(x, y);
                    if (!targetCell.IsEmpty())
                        continue;

                    // Diagonal Pass logic:
                    // If targetCell is empty, it means the vertical pass didn't fill it.
                    // We try to fill it from diagonal top-left or top-right.
                    if (TryMoveDiagonal(x - 1, y + 1, x, y, ref sequence, ref sequenceCreated))
                    {
                        movedAny = true;
                        continue;
                    }

                    if (TryMoveDiagonal(x + 1, y + 1, x, y, ref sequence, ref sequenceCreated))
                    {
                        movedAny = true;
                        continue;
                    }
                }
            }

            if (sequenceCreated)
            {
                await sequence;
            }

            return movedAny;
        }


        private bool TryMoveDiagonal(
            int fromX,
            int fromY,
            int toX,
            int toY,
            ref Sequence sequence,
            ref bool sequenceCreated)
        {
            if (!_grid.IsValid(fromX, fromY))
                return false;

            var fromCell = _grid.GetValue(fromX, fromY);

            if (fromCell.IsEmpty() || fromCell.HasBlock())
                return false;

            if (_grid.IsValid(fromX, fromY - 1))
            {
                var belowSource = _grid.GetValue(fromX, fromY - 1);
                if (belowSource.IsPlayable() && belowSource.IsEmpty())
                    return false;
            }

            var piece = fromCell.GetPiece();
            if (piece == null)
                return false;

            var toCell = _grid.GetValue(toX, toY);

            if (!toCell.IsPlayable())
                return false;

            toCell.SetPiece(piece);
            fromCell.Clear();

            var targetPos = _grid.GetWorldPositionCenter(toX, toY);
            var tween = Tween.Position(
                piece.transform,
                targetPos,
                0.3f,
                Ease.OutBounce);

            if (!sequenceCreated)
            {
                sequence = Sequence.Create();
                sequenceCreated = true;
            }

            sequence.Group(tween);

            return true;
        }
    }
}