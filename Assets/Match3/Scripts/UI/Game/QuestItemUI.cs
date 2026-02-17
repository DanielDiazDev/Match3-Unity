using Match3.Scripts.Data.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Scripts.UI.Game
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private Image pieceImage;
        [SerializeField] private TMP_Text remainingText;
        [SerializeField] private GameObject completedIcon;

        private PieceDestructionProgress _progress;


        public void Initialize(PieceDestructionProgress progress)
        {
            _progress = progress;

            pieceImage.sprite = progress.TargetPiece.sprite; 
            UpdateView();

            _progress.OnProgressChanged += UpdateView;
        }

        private void UpdateView()
        {
            remainingText.text = _progress.Remaining.ToString();
            completedIcon.SetActive(_progress.IsReached);
            if (completedIcon.activeInHierarchy)
            {
                remainingText.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_progress != null)
                _progress.OnProgressChanged -= UpdateView;
        }
    }
}