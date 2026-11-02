using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Grid;
using UnityEngine;

namespace Match3.Scripts.Core.States
{
    /// <summary>
    /// Genera las piezas iniciales del tablero al comenzar el nivel.
    /// </summary>
    public class GeneratePiecesState : IMatchState
    {
        private readonly MatchStateContext _ctx;
        private readonly GameObject _backgroundTilePrefab;

        public GeneratePiecesState(MatchStateContext ctx, GameObject backgroundTilePrefab)
        {
            _ctx = ctx;
            _backgroundTilePrefab = backgroundTilePrefab;
        }

        public void Enter()
        {
            _ctx.SetBusy(true);
        }

        public async UniTask<MatchState> Execute()
        {
            var width = _ctx.LevelDataSO.width;
            var height = _ctx.LevelDataSO.height;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var position = new Vector2Int(x, y);

                    if (!_ctx.LevelDataSO.initialGridData.TryGetValue(position, out var cellData))
                    {
                        var gridObject = new GridObject(_ctx.GridSystem, x, y);
                        gridObject.SetPlayable(false);
                        _ctx.GridSystem.SetValue(x, y, gridObject);
                        continue;
                    }

                    SpawnBackground(position);

                    if (cellData.block != null)
                    {
                        _ctx.PieceSpawner.SpawnBlock(cellData.block.id, position, _ctx.Transform);
                    }
                    else if (cellData.tile != null)
                    {
                        var piece = _ctx.PieceSpawner.Spawn(cellData.tile.id, position, _ctx.Transform);
                        // piece.PlaySpawnAnimation();
                    }
                    else
                    {
                        var gridObject = new GridObject(_ctx.GridSystem, x, y);
                        gridObject.SetPlayable(true);
                        _ctx.GridSystem.SetValue(x, y, gridObject);
                    }

                    if (cellData.layer != null)
                    {
                        _ctx.PieceSpawner.SpawnLayer(cellData.layer.id, position, _ctx.Transform);
                    }

                    // Pequeño yield para no bloquear el hilo en tableros grandes
                    if ((x * height + y) % 10 == 0)
                        await UniTask.Yield();
                }
            }

            _ctx.NotifyMovesChanged(_ctx.LevelDataSO.maxMoves);
            _ctx.RemainingMoves = _ctx.LevelDataSO.maxMoves;

            return MatchState.Idle;
        }

        public void Exit()
        {
            _ctx.SetBusy(false);
        }

        private void SpawnBackground(Vector2Int position)
        {
            if (_backgroundTilePrefab == null) return;
            UnityEngine.Object.Instantiate(
                _backgroundTilePrefab,
                _ctx.GridSystem.GetWorldPositionCenter(position.x, position.y),
                Quaternion.identity,
                _ctx.Transform);
        }
    }
}
