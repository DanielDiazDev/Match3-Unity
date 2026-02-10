
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Pieces;

namespace Match3.Scripts.Grid
{
    public class GridObject
    {
        private GridSystem<GridObject> _gridSystem;
        private Piece _piece;
        public int X { get; private set; }

        public int Y { get; private set; }

        private Block _block;
        private Layer _layer;

        public GridObject(GridSystem<GridObject> gridSystem, int x, int y)
        {
            _gridSystem = gridSystem;
            X = x;
            Y = y;
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetPiece(Piece piece) => _piece = piece;
        public bool IsEmpty() => _piece == null && _block == null; // Block also fills the cell
        public Piece GetPiece() => _piece;
        public bool HasPowerUp() => _piece?.pieceSo is PowerUpSO;

        // Block logic
        public void SetBlock(Block block) => _block = block;
        public Block GetBlock() => _block;
        public bool HasBlock() => _block != null;
        public void ClearBlock() => _block = null;

        // Layer logic
        public void SetLayer(Layer layer) => _layer = layer;
        public Layer GetLayer() => _layer;
        public bool HasLayer() => _layer != null;
        public void ClearLayer() => _layer = null;

        public void Clear()
        {
            _piece = null;
            // Note: We don't clear Blocks or Layers here by default, as they might persist? 
            // Actually, if a piece moves, we just clear the piece. 
            // If we want to clear EVERYTHING, we'd need a separate method or explicit calls.
            // Gravity moves PIECES. Blocks stay put. 
        }
    }
}