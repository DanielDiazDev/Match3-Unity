using Match3.Scripts.Persistence;
using Match3.Scripts.Score;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using Resolution = Reflex.Enums.Resolution;

namespace Match3.Scripts.Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterValue("Hello");

        }
    }
}