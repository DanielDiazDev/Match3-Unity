using Cysharp.Threading.Tasks;

namespace Match3.Scripts.Core
{
    public interface IMatchState
    {
        void Enter();
        UniTask<MatchState> Execute();
        void Exit();
    }
}
