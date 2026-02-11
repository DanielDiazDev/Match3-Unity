using System;
using Assets.Match3.Scripts.Grid;
using Match3.Scripts.Grid;
using Match3.Scripts.Pieces;
using UnityEngine;

namespace Match3.Scripts.Match.Systems.Spawner
{
    public class PieceSpawner
{
    private readonly PieceFactory _piecesFactory;
    private readonly PiecesConfigurationSO _piecesConfigurationSO;
    private readonly GridSystem<GridObject> _gridSystem;

    public PieceSpawner(PiecesConfigurationSO piecesConfigurationSo, GridSystem<GridObject> gridSystem)
    {
        _piecesConfigurationSO = piecesConfigurationSo;
        _gridSystem = gridSystem;
        _piecesFactory = new PieceFactory(_piecesConfigurationSO); // TODO: Inyección
    }

    public Piece Spawn(string id, Vector2Int position, Transform parent)
    {
        return SpawnInternal(
            id,
            position,
            parent,
            (worldPos, t) => _piecesFactory.Create(id, worldPos, t),
            (gridObject, element) => gridObject.SetPiece(element)
        );
    }

    public Block SpawnBlock(string id, Vector2Int position, Transform parent)
    {
        return SpawnInternal(
            id,
            position,
            parent,
            (worldPos, t) => _piecesFactory.CreateBlock(id, worldPos, t),
            (gridObject, element) => gridObject.SetBlock(element)
        );
    }

    public Layer SpawnLayer(string id, Vector2Int position, Transform parent)
    {
        return SpawnInternal(
            id,
            position,
            parent,
            (worldPos, t) => _piecesFactory.CreateLayer(id, worldPos, t),
            (gridObject, element) => gridObject.SetLayer(element)
        );
    }

    private T SpawnInternal<T>(
        string id,
        Vector2Int position,
        Transform parent,
        Func<Vector3, Transform, T> factoryMethod,
        Action<GridObject, T> assignMethod)
    {
        var worldPosition = _gridSystem.GetWorldPositionCenter(position.x, position.y);

        var element = factoryMethod(worldPosition, parent);

        var gridObject = _gridSystem.GetValue(position.x, position.y);
        if (gridObject == null)
        {
            gridObject = new GridObject(_gridSystem, position.x, position.y);
            _gridSystem.SetValue(position.x, position.y, gridObject);
        }

        assignMethod(gridObject, element);

        return element;
    }

    }
}