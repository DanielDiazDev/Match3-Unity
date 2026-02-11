using Cysharp.Threading.Tasks;

namespace Match3.Scripts.General
{
    public interface ICommand
    {
        UniTask Execute();
    }
}