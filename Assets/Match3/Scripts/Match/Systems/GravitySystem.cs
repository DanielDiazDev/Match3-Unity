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

                    // If we encounter a block, we can't fall *through* it.
                    // The write head must be reset to the position above the block.
                    if (readCell.HasBlock())
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
                        readCell.Clear(); // Only clears the piece, not block/layer

                        var targetPos = _grid.GetWorldPositionCenter(x, writeY);
                        tweens.Add(Tween.Position(piece.transform, targetPos, 0.3f, Ease.OutBounce));

                        movedAny = true;
                    }

                    writeY++;
                }
            }



            // 2. Diagonal Pass (Fill empty spaces from neighbors)
            // Iterate all cells from Bottom to Top (to handle cascades correctly in multi-pass, or just standard scan)
            for (int y = 0; y < _height - 1; y++) // y goes up to height-2, looking at y+1
            {
                for (int x = 0; x < _width; x++)
                {
                    var targetCell = _grid.GetValue(x, y);
                    if (!targetCell.IsEmpty()) continue;

                    // If we have a block here, we can't fill it. 
                    // (Note: IsEmpty() usually checks piece==null. If Block is present, IsEmpty should be false? 
                    // Let's verify GridObject.IsEmpty logic. 
                    // Previous Step: IsEmpty() => _piece == null && _block == null;
                    // So if Block is there, IsEmpty is false. Safe.)

                    // RESTRICTION: Only pull diagonally if the vertical path is blocked (i.e. cell above has a Block)
                    // If cell above is empty or has a piece, normal gravity should handle it (or it's waiting for refill)
                    if (_grid.IsValid(x, y + 1))
                    {
                        var cellAbove = _grid.GetValue(x, y + 1);
                        if (!cellAbove.HasBlock()) continue; 
                    }

                    // Try Pull from Top-Left (x-1, y+1)
                    if (TryMoveDiagonal(x - 1, y + 1, x, y, tweens))
                    {
                        movedAny = true;
                        continue;
                    }
                    
                    // Try Pull from Top-Right (x+1, y+1)
                    if (TryMoveDiagonal(x + 1, y + 1, x, y, tweens))
                    {
                        movedAny = true;
                        continue;
                    }
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
            // Note: This method seems less used than the main ApplyGravity, but we should update it for consistency or remove it if unused.
            // Assuming it's a helper for specific column updates.
            // To properly support blocks, we need a similar logic to ApplyGravity (bottom-up or write-pointer approach).
            
            // Re-implementing with write-pointer logic for consistency with ApplyGravity's new block awareness
            int writeY = 0;
            for (int readY = 0; readY < _height; readY++)
            {
                 var readCell = _grid.GetValue(x, readY);
                 
                 if (readCell.HasBlock())
                 {
                     writeY = readY + 1;
                     continue;
                 }

                 if (!readCell.IsEmpty())
                 {
                     if (readY != writeY)
                     {
                         MovePiece(x, readY, x, writeY);
                         moved = true;
                     }
                     writeY++;
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

        
     
      
        private bool TryMoveDiagonal(int fromX, int fromY, int toX, int toY, List<Tween> tweens)
        {
            if (!_grid.IsValid(fromX, fromY)) return false;

            var fromCell = _grid.GetValue(fromX, fromY);
            if (fromCell.IsEmpty() || fromCell.HasBlock()) return false; 
            
            // Check if 'from' piece should fall vertically instead?
            // If the space directly below 'from' (fromX, fromY - 1) is empty, 
            // it should ideally fall there (Vertical Priority).
            // However, vertical pass ran first. So if grid[fromX, fromY-1] is EMPTY, 
            // it means it WAS empty and vertical pass didn't fill it? 
            // Or maybe vertical pass filled it but left fromCell empty?
            // We must be careful.
            // Rule: Don't move diagonal if vertical path is clear.
            // Vertical path is (fromX, fromY - 1). 
            if (_grid.IsValid(fromX, fromY - 1))
            {
                var belowSource = _grid.GetValue(fromX, fromY - 1);
                if (belowSource.IsEmpty()) return false; // Should fall down, not sideways
            }

            var piece = fromCell.GetPiece();
            if (piece == null) return false;

            // Perform Move
            var toCell = _grid.GetValue(toX, toY);
            toCell.SetPiece(piece);
            fromCell.Clear(); // Clears piece (keeps block/layer if any, but we checked HasBlock above)

            var targetPos = _grid.GetWorldPositionCenter(toX, toY);
            tweens.Add(Tween.Position(piece.transform, targetPos, 0.3f, Ease.OutBounce));

            return true;
        }

    }
}