using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Board;
using UI;

namespace Managers
{
    public class AnimationManager : IManager
    {
        public void Init() { } 
        public void PostInit() { }

        public async Task AnimateMove(List<BoardObject> movingObjects)
        {
            await Task.WhenAll(movingObjects.Select(m => m.Move()));
        }
        
        public async Task AnimateFade(FadePanel panel, bool fadeIn)
        {
            if (fadeIn) await panel.FadeIn();
            else await panel.FadeOut();
        }

        public void Dispose()
        {
           
        }
    }
}