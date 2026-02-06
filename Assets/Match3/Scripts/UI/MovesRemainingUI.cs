using System;
using Match3.Scripts.Core;
using TMPro;
using UnityEngine;

namespace Match3.Scripts.UI
{
    public class MovesRemainingUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _movesText;
        [SerializeField] private MatchManager _matchManager;

        private void OnEnable()
        {
            _matchManager.OnMovesChanged += UpdateMoves;
        }

        private void OnDestroy()
        {
            _matchManager.OnMovesChanged -= UpdateMoves;
        }

        private void UpdateMoves(int moves)
        {
            _movesText.text = moves.ToString();
        }
    }
}
