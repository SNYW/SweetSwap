using System.Threading.Tasks;
using Settings;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public static class SceneLoader
    {
        private static readonly GameSettings Settings = Injection.GetManager<SettingsManager>().ActiveSettings;
        private static readonly AnimationManager AnimationManager = Injection.GetManager<AnimationManager>();

        static SceneLoader()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        
        public static async Task LoadScene(int sceneIndex)
        {
            await AnimationManager.AnimateFade(GetPanel(), true);
            SceneManager.LoadScene(sceneIndex);
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            _ = AnimationManager.AnimateFade(GetPanel(), false);
        }

        private static FadePanel GetPanel()
        {
            var existingPanel = Object.FindObjectOfType<FadePanel>();
            return existingPanel != null ? existingPanel : Object.Instantiate(Settings.fadePanelPrefab);
        }
    }
}