using System;
using System.Collections.Generic;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Score;

namespace Match3.Scripts.UI.Score
{
    public class ScoreController : IDisposable, IEventObserver
    {
       // private readonly ScoreSystem _scoreSystem;
        private readonly ScoreViewModel _viewModel;
        private readonly List<IDisposable> _disposables = new();
        
        
        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.ScoreChanged:
                {
                    var scoreChangedEventData = (ScoreChangedEventData)eventData;
                    OnScoreChanged(scoreChangedEventData.Score);
                    break;
                }
                case EventIds.MultiplierChanged:
                {
                    var multiplierScoreChangedEventData = (MultiplierScoreChangedEventData)eventData;
                    OnMultiplierChanged(multiplierScoreChangedEventData.Score);
                    break;
                }
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ScoreController(ScoreViewModel viewModel)
        {
            _viewModel = viewModel;
            EventQueue.Instance.Subscribe(EventIds.ScoreChanged, this);
            EventQueue.Instance.Subscribe(EventIds.MultiplierChanged, this);
            _viewModel.Score.Value = 0;
            _viewModel.Multiplier.Value = 0;
        }

        private void OnScoreChanged(int score)
        {
            _viewModel.Score.Value = score;
        }

        private void OnMultiplierChanged(float multiplier)
        {
            _viewModel.Multiplier.Value = multiplier;
        }

        public void Dispose()
        {
           EventQueue.Instance.Unsubscribe(EventIds.ScoreChanged, this);
           EventQueue.Instance.Unsubscribe(EventIds.MultiplierChanged, this);
        }

        
    }
}