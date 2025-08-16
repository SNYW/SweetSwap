using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Board;

namespace Managers
{
    public class AnimationManager : IManager
    {
        public void Init() { }

        public async Task AnimateMove(List<BoardObject> movingObjects)
        {
            await Task.WhenAll(movingObjects.Select(m => m.Move()));
        }

        public void Dispose()
        {
           
        }
    }
}