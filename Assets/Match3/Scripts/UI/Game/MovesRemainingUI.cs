using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using TMPro;
using UnityEngine;

namespace Match3.Scripts.UI.Game
{
    public class MovesRemainingUI : MonoBehaviour, IEventObserver
    {
        [SerializeField] private TextMeshProUGUI _movesText;

        private void Start()
        {
            EventQueue.Instance.Subscribe(EventIds.MovesChanged, this);
        }

        private void OnDestroy()
        {
            EventQueue.Instance.Unsubscribe(EventIds.MovesChanged, this);
        }
        public void Process(EventData eventData)
        {
            if(eventData.EventId != EventIds.MovesChanged) return;
            var movesChangedEventData = (MovesChangedEventData)eventData;
            UpdateMoves(movesChangedEventData.Moves);
        }
        private void UpdateMoves(int moves)
        {
            _movesText.text = moves.ToString();
        }

       
    }
}
