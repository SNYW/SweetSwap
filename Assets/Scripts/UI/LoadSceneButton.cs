using Managers;
using UnityEngine;

namespace UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        public void LoadScene(int sceneIndex)
        {
            _ = SceneLoader.LoadScene(sceneIndex);
        }
    }
}