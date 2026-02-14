using Match3.Scripts.Data.Score;
using Match3.Scripts.Score;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Match3.Scripts.Installers
{
    public class GameInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private ScoreConfigurationSO _scoreConfigurationSO;
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterFactory<IScoreSystem>(
                container => new ScoreSystem(
                    new Match3ScoreStrategy(),
                    _scoreConfigurationSO),
                Lifetime.Scoped,
                Resolution.Eager
            );

        }
    }
}