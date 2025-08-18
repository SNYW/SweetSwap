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

        private static bool isLoading;
        static SceneLoader()
        {
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        
        public static async Task LoadScene(int sceneIndex)
        {
            if(isLoading) Debug.LogError($"Trying to load {sceneIndex} while another scene is loading");
            isLoading = true;
            await AnimationManager.AnimateFade(GetPanel(), true);
            SceneManager.LoadScene(sceneIndex);
            isLoading = false;
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