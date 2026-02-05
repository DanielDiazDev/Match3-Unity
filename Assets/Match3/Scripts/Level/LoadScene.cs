using UnityEngine.SceneManagement;

namespace Match3.Scripts.Level
{
    public class LoadScene : ILoadScene
    {
        public void Load()
        {
            SceneManager.LoadScene("Game");
        }
    }
}