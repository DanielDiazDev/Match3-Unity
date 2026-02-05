using Cysharp.Threading.Tasks;
using Match3.Scripts.Data.Level;

namespace Match3.Scripts.Level
{
    public interface ILevelUseCase
    {
        LevelModel BuildLevel(int id);
        void SelectLevel(LevelDataSO levelData);
    }
}