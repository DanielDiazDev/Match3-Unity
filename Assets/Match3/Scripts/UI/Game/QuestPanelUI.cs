using System;
using System.Collections.Generic;
using Match3.Scripts.Data.Quests;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using UnityEngine;

namespace Match3.Scripts.UI.Game
{
    public class QuestPanelUI : MonoBehaviour, IEventObserver
    {
        [SerializeField] private QuestItemUI questItemPrefab;
        [SerializeField] private Transform container;

        private void Start()
        {
            EventQueue.Instance.Subscribe(EventIds.QuestsInitialized, this);
        }

        private void OnDestroy()
        {
            EventQueue.Instance.Unsubscribe(EventIds.QuestsInitialized, this);
        }

        private void Initialize(List<QuestProgress> quests)
        {
            foreach (var quest in quests)
            {
                var pieceQuest = quest as PieceDestructionProgress;
                if (pieceQuest == null) continue;

                var item = Instantiate(questItemPrefab, container);
                item.Initialize(pieceQuest);
            }
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId != EventIds.QuestsInitialized) return;
            var questsInitializedEventData = (QuestsInitializedEventData)eventData;
            Initialize(questsInitializedEventData.questProgress);
        }
    }
}