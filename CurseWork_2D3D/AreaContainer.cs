using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseWork_2D3D
{
    public class AreaContainer
    {
        //границы области
        public List<Versh> Borders = new List<Versh>();
        //все вершины этой же области
        public List<Versh> Area = new List<Versh>();
    }
}
