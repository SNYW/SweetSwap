using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            Injection.Init();
        }

        private void OnDisable()
        {
            Injection.Dispose();
        }
    }
}