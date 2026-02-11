using System.Collections.Generic;
using Match3.Scripts.Pieces;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Match3.Scripts.Match.Systems.Spawner
{
   public class PieceFactory
{
    private readonly PiecesConfigurationSO _config;

    private readonly Dictionary<string, IObjectPool<Piece>> _piecePools = new();
    private readonly Dictionary<string, IObjectPool<Block>> _blockPools = new();
    private readonly Dictionary<string, IObjectPool<Layer>> _layerPools = new();

    public PieceFactory(PiecesConfigurationSO config)
    {
        _config = config;
    }

    #region PIECES

    public Piece Create(string id, Vector2 position, Transform parent)
    {
        if (!_piecePools.ContainsKey(id))
        {
            var pieceSo = _config.GetPieceById(id);

            _piecePools[id] = new ObjectPool<Piece>(
                () =>
                {
                    var obj = Object.Instantiate(pieceSo.prefab).GetComponent<Piece>();
                    obj.SetPool(_piecePools[id]);
                    return obj;
                },
                piece => piece.OnSpawned(),
                piece => piece.OnDespawned(),
                piece => Object.Destroy(piece.gameObject),
                false,
                10,
                100
            );
        }

        var pool = _piecePools[id];
        var pieceInstance = pool.Get();

        pieceInstance.transform.SetParent(parent);
        pieceInstance.transform.position = position;
        pieceInstance.Init(_config.GetPieceById(id));

        return pieceInstance;
    }

    #endregion

    #region BLOCKS

    public Block CreateBlock(string id, Vector2 position, Transform parent)
    {
        if (!_blockPools.ContainsKey(id))
        {
            var blockSo = _config.GetBlockById(id);

            _blockPools[id] = new ObjectPool<Block>(
                () =>
                {
                    var obj = Object.Instantiate(blockSo.prefab).GetComponent<Block>();
                    obj.SetPool(_blockPools[id]);
                    return obj;
                },
                block => block.OnSpawned(),
                block => block.OnDespawned(),
                block => Object.Destroy(block.gameObject)
            );
        }

        var pool = _blockPools[id];
        var blockInstance = pool.Get();

        blockInstance.transform.SetParent(parent);
        blockInstance.transform.position = position;
        blockInstance.Init(_config.GetBlockById(id));

        return blockInstance;
    }

    #endregion

    #region LAYERS

    public Layer CreateLayer(string id, Vector2 position, Transform parent)
    {
        if (!_layerPools.ContainsKey(id))
        {
            var layerSo = _config.GetLayerById(id);

            _layerPools[id] = new ObjectPool<Layer>(
                () =>
                {
                    var obj = Object.Instantiate(layerSo.prefab).GetComponent<Layer>();
                    obj.SetPool(_layerPools[id]);
                    return obj;
                },
                layer => layer.OnSpawned(),
                layer => layer.OnDespawned(),
                layer => Object.Destroy(layer.gameObject)
            );
        }

        var pool = _layerPools[id];
        var layerInstance = pool.Get();

        layerInstance.transform.SetParent(parent);
        layerInstance.transform.position = position;
        layerInstance.Init(_config.GetLayerById(id));

        return layerInstance;
    }

    #endregion
}
}