using System;
using System.Threading.Tasks;
using Settings;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
            try
            {
                await AnimationManager.AnimateFade(GetPanel(), true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SceneLoader Fade Out Exception: {ex}");
            }

            try
            {
                SceneManager.LoadScene(sceneIndex);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SceneLoader Scene Load Exception: {ex}");
            }
        }

        private static void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            _ = AnimateFadeSafe();
        }

        private static async Task AnimateFadeSafe()
        {
            try
            {
                await AnimationManager.AnimateFade(GetPanel(), false);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SceneLoader Fade In Exception: {ex}");
            }
        }

        private static FadePanel GetPanel()
        {
            var existingPanel = Object.FindObjectOfType<FadePanel>();
            if (existingPanel != null) return existingPanel;

            try
            {
                return Object.Instantiate(Settings.fadePanelPrefab);
            }
            catch (Exception ex)
            {
                Debug.LogError($"SceneLoader Panel Instantiate Exception: {ex}");
                return null;
            }
        }
    }
}
