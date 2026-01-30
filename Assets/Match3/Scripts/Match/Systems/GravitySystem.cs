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
            var tweens = new List<Tween>();

            for (int x = 0; x < _width; x++)
            {
                int writeY = 0;

                for (int readY = 0; readY < _height; readY++)
                {
                    var readCell = _grid.GetValue(x, readY);
                    if (readCell.IsEmpty())
                        continue;

                    if (readY != writeY)
                    {
                        var writeCell = _grid.GetValue(x, writeY);
                        var piece = readCell.GetPiece();

                        writeCell.SetPiece(piece);
                        readCell.Clear();

                        var targetPos = _grid.GetWorldPositionCenter(x, writeY);
                        tweens.Add(Tween.Position(piece.transform, targetPos, 0.3f, Ease.OutBounce));

                        movedAny = true;
                    }

                    writeY++;
                }
            }

            if (movedAny)
            {
                await UniTask.WhenAll(tweens.Select(t => t.ToUniTask()));
            }

            return movedAny;
        }
        private bool ApplyGravityToColumn(int x)
        {
            bool moved = false;

            for (int y = 0; y < _height; y++)
            {
                if (_grid.GetValue(x, y).IsEmpty())
                {
                    for (int above = y + 1; above < _height; above++)
                    {
                        if (!_grid.GetValue(x, above).IsEmpty())
                        {
                            MovePiece(x, above, x, y);
                            moved = true;
                            break;
                        }
                    }
                }
            }

            return moved;
        }
        private void MovePiece(int fromX, int fromY, int toX, int toY)
        {
            var fromCell = _grid.GetValue(fromX, fromY);
            var toCell = _grid.GetValue(toX, toY);

            var piece = fromCell.GetPiece();

            toCell.SetPiece(piece);
            fromCell.Clear();

            Tween.StopAll(piece.transform);
            piece.transform.position = _grid.GetWorldPositionCenter(toX, toY);
        }

        
     
      
    }
}