
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
        public bool IsEmpty() => _piece == null;
        public Piece GetPiece() => _piece;
        public bool HasPowerUp() => _piece?.pieceSo is PowerUpSO;


        public void Clear()
        {
            _piece = null;
        }
    }
}