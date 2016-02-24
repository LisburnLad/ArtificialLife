using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  /// <summary>
  /// Bot input node
  /// - the output value of this will be set directly
  /// </summary>
  public class NodeInput : NodeBase
  {
    public NodeInput(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {

    }
  }  
}
