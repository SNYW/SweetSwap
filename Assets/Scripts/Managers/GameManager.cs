using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            Injection.Init();
        }

        private void OnDrawGizmos()
        {
            if(Application.isPlaying) Injection.GetManager<GridManager>().OnDrawGizmos();
        }

        private void OnDisable()
        {
            Injection.Dispose();
        }
    }
}