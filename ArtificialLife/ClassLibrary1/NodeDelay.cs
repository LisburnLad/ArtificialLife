using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class NodeDelay : NodeBase
  {
    public NodeDelay(CellType aCellType, int aRow, int aCol, DirectionFlag aNodeInputs)
      : base(aCellType, aRow, aCol, aNodeInputs)
    {
      itsStates = new Dictionary<int, bool>();
      itsStates[0] = false;
    }

    /// <summary>
    /// the state of the last input sum
    /// </summary>
    //private bool itsLastInputSum = false;

    ///// <summary>
    ///// OR gate output = 1, if any of the inputs are 1
    ///// - don't include sides with no input in the calculation    
    ///// </summary>
    //protected override void CalculateOutput()
    //{
    //  // if the node has no inputs at all then its output is false
    //  if (itsInputConnections == DirectionFlag.None)
    //  {
    //    Output = 0;
    //  }
    //  else
    //  {
    //    bool inputSum = CalculateInputSum();

    //    // set the flag to show it the internally stored output state has changed
    //    //InternalStateChanged = (itsLastInputSum != inputSum);

    //    InternalStateChanged = (LastStepInputSum != PreviousStepInputSum);

    //    // set the output of the gate        
    //    //Output = (itsLastInputSum ? 1 : 0);

    //    Output = (PreviousStepInputSum ? 1 : 0);
                
    //    // store the new value
    //    //itsLastInputSum = inputSum;
    //  }
    //}

    //public int PreviousStep = -1;
    //public bool PreviousStepInputSum { get; set; }

    //public int LastStep = 0;
    //public bool LastStepInputSum { get; set; }


    Dictionary<int, bool> itsStates = null;

    /// <summary>
    /// take a snapshot of the input sum at the current propagation step
    /// - this then acts as the output of the node at a later propagation step
    /// </summary>
    /// <param name="aPropagationStep"></param>
    public override void TakeInputSnapshot(int aPropagationStep)
    {
      // make sure the node holds up to 2 states
      if( itsStates.ContainsKey(aPropagationStep-1) == false )
      {
        // if 2 states back doesn't exist it indicates that the node has had a stable output for a number of steps
        // so set the previous state to this
        itsStates[aPropagationStep - 1] = (Output>0);
      }

      itsStates[aPropagationStep] = CalculateInputSum();
  
      // get rid of the old state that is no longer needed
      if( itsStates.ContainsKey(aPropagationStep-3) )
      {
        itsStates.Remove(aPropagationStep - 3);
      }
    }


    public override bool PropagateOutput(int aPropagationPassLevel)
    {
      bool outputState = (Output > 0);

      // the index of the propagation step whose value should be used for the output
      int keystep = aPropagationPassLevel - 2;

      if (itsStates.ContainsKey(keystep))
      { 
        ArtificialLifeProperties.Log("Propagate Node: row = " + itsRow + " col = " + itsCol + " dir = " + itsInputDirection + " state = " + (itsStates[keystep] != outputState));

        // reset the nodes input flags and output changed fired
        itsInputDirection = DirectionFlag.None;
        
        // test if the internal state changed when the sum of the inputs was last calculated
        InternalStateChanged = (itsStates[aPropagationPassLevel - 1] != itsStates[keystep]);

        // set the output to the sum of the inputs from 2 steps ago
        Output = (itsStates[keystep] ? 1 : 0);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Calculate the current sum of the nodes input
    /// </summary>
    /// <returns></returns>
    private bool CalculateInputSum()
    {
      bool inputSum = false;

      if (itsInputConnections.HasFlag(DirectionFlag.North))
      {
        inputSum |= itsNorthInput;
      }

      if (itsInputConnections.HasFlag(DirectionFlag.East))
      {
        inputSum |= itsEastInput;
      }

      if (itsInputConnections.HasFlag(DirectionFlag.South))
      {
        inputSum |= itsSouthInput;
      }

      if (itsInputConnections.HasFlag(DirectionFlag.West))
      {
        inputSum |= itsWestInput;
      }

      return inputSum;
    }
  }
}
