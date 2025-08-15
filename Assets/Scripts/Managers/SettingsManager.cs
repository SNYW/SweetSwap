using Settings;
using UnityEngine;

namespace Managers
{
    public class SettingsManager : IManager
    {
        public GameSettings ActiveSettings;
        public void Init()
        {
            ActiveSettings = Resources.Load<GameSettings>("Game Data/Game Settings");
            Debug.Log("Loaded Settings");
        }

        public void Dispose()
        {
            ActiveSettings = null;
        }
    }
}