using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeEventArgs : EventArgs
  {
    public int itsRow { get; set; }
    public int itsCol { get; set; }
    public DirectionFlag itsDirection { get; set; }
    public bool itsValue { get; set; }
    public bool itsTargetNode { get; set; }
    public int itsLevel { get; set; }


    /// <summary>
    /// to specify the output state of a node has changed
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <param name="aDirection"></param>
    /// <param name="aValue"></param>
    /// <param name="aTargetNode"></param>
    public NodeEventArgs(int aRow, int aCol, DirectionFlag aDirection, bool aValue, bool aTargetNode, int aPropagationLevel)
    {
      itsRow = aRow;
      itsCol = aCol;
      itsDirection = aDirection;
      itsValue = aValue;
      itsTargetNode = aTargetNode;
      itsLevel = aPropagationLevel;
    }

    /// <summary>
    /// to specify that the internal state of a node has changed
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    public NodeEventArgs(int aRow, int aCol, int aPropagationLevel)
    {
      itsRow = aRow;
      itsCol = aCol;
      itsLevel = aPropagationLevel;
    }
  }
}
