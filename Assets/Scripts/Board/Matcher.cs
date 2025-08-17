using System.Collections.Generic;
using Managers;
using Unity.VisualScripting;

namespace Board
{
    public class Matcher
    {
        private readonly GridManager _gridManager = Injection.GetManager<GridManager>();
        
        public bool HasMatches(GridCell[,] context, out HashSet<GridCell> matches)
        {
            matches = new HashSet<GridCell>();
            
            for (var y = 0; y < context.GetLength(1); y++)
            {
                matches.AddRange(GetMatches(_gridManager.GetRow(y)));
            }
            
            for (var x = 0; x < context.GetLength(0); x++)
            {
                matches.AddRange(GetMatches(_gridManager.GetColumn(x)));
            }
            
            return matches.Count > 0;
        }

        public HashSet<GridCell> GetMatches(List<GridCell> cells)
        {
            var matches = new HashSet<GridCell>();
            if (cells.Count < 3) return matches;

            List<GridCell> streak = new List<GridCell> { cells[0] };

            for (var i = 1; i < cells.Count; i++)
            {
                if (cells[i].GetChildObject().definition == cells[i - 1].GetChildObject().definition)
                {
                    streak.Add(cells[i]);
                }
                else
                {
                    if (streak.Count >= 3) matches.AddRange(streak);

                    streak.Clear();
                    streak.Add(cells[i]);
                }
            }

            if (streak.Count >= 3) matches.AddRange(streak);
            return matches;
        }
    }
}