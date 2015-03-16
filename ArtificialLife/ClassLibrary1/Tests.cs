using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  public class Test
  {
    protected int kNumberOfPasses = 20;
    public int itsScore { get; set; }

    char[,] itsLastOutput = null;

    public Test()
    {
      itsScore = 0;
    }

    public int GetNumberOfPasses()
    {
      return kNumberOfPasses;
    }

    public virtual double GetFinalScore( bool aShowGrid )
    {
      return 0.0;
    }

    public virtual bool EvaluateOutput( char[,] aOutput, int aGridSideLength, int aPass, bool aShowGrid )
    { 
      // test if the bot has moved to a postion that will change its score
      UpdateScore(aPass, aShowGrid);

      // test if the new output is identical to the last output
      bool result = CompareWithLastOutput( aOutput, aGridSideLength );
        
      // copy the current output for comparison on the next iteration
      CopyOutput( aOutput, aGridSideLength );

      return result;
    }

    protected virtual void UpdateScore( int aPass, bool aShowGrid )
    {
    }

    public virtual void ShowTestOutput()
    {
    }

    /// <summary>
    /// copy the output to the last output
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    protected void CopyOutput( char[,] aOutput, int aGridSideLength )
    {
      if(itsLastOutput == null)
      {
        itsLastOutput = new char[aGridSideLength, aGridSideLength];
      }

      for(int row = 0; row < aGridSideLength; row++)
      {
        for(int col = 0; col < aGridSideLength; col++)
        {
          itsLastOutput[row, col] = aOutput[row, col];
        }
      }
    }

    /// <summary>
    /// test if the two supplied outputs are the same
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    /// <returns>true if the grids are identical</returns>
    protected bool CompareWithLastOutput( char[,] aOutput, int aGridSideLength )
    {
      if(itsLastOutput == null)
      {
        return false;
      }

      for(int row = 0; row < aGridSideLength; row++)
      {
        for(int col = 0; col < aGridSideLength; col++)
        {
          if(itsLastOutput[row, col] != aOutput[row, col])
          {
            return false;
          }
        }
      }

      return true;
    }
  }


  /// <summary>
  /// Test bots in a simple maze, where its fitness depends on movement around this maze
  /// </summary>
  public class MazeTest : Test
  {
    public int itsCol { get; set; }
    public int itsRow { get; set; }
    public int itsMaxRow { get; set; }
    public int itsMaxCol { get; set; }

    public int itsInitialRow { get; set; }
    public int itsInitialCol { get; set; }

    public bool itsRowChanged { get; set; }
    public bool itsColChanged { get; set; }

    public int itsLastRow { get; set; }
    public int itsLastCol { get; set; }

    int itsMazeSideLength = 21;
    char[,] itsMazePosition;

    public MazeTest(int aInitialRow, int aInitialCol)
    {
      itsInitialRow = aInitialRow;
      itsInitialCol = aInitialCol;

      itsMazePosition = new char[itsMazeSideLength, itsMazeSideLength];

      itsRow = itsInitialRow;
      itsCol = itsInitialCol;

      itsLastRow = itsRow;
      itsLastCol = itsCol;

      itsRowChanged = false;
      itsColChanged = false;

      // indicate the starting position
      itsMazePosition[itsRow, itsCol] = 'x';
    }

    /// <summary>
    /// Test the performance of the bot for the given pass
    /// </summary>
    /// <param name="aOutput"></param>
    /// <param name="aGridSideLength"></param>
    /// <param name="pass"></param>
    /// <param name="aShowGrid"></param>
    /// <returns>true if the bot output is the same as the previous iteration</returns>
    public override bool EvaluateOutput( char[,] aOutput, int aGridSideLength, int pass, bool aShowGrid )
    {
      int gridMiddle = aGridSideLength / 2;

      // record the new position of the bot in the maze
      ChangePosition( aOutput, aGridSideLength, gridMiddle );

      return base.EvaluateOutput( aOutput, aGridSideLength, pass, aShowGrid );
    }

    protected override void UpdateScore( int aPass, bool aShowGrid )
    {
    }

    protected void ChangePosition( char[,] aOutput, int aGridSideLength, int aGridMiddle )
    {
      if(aOutput[0, aGridMiddle] == '1' && itsRow > 0)
      {
        itsRow--;
      }

      if(aOutput[aGridSideLength - 1, aGridMiddle] == '1' && itsRow < (itsMazeSideLength - 1))
      {
        itsRow++;
      }

      if(aOutput[aGridMiddle, 0] == '1' && itsCol > 0)
      {
        itsCol--;
      }

      if(aOutput[aGridMiddle, aGridSideLength - 1] == '1' && itsCol < (itsMazeSideLength - 1))
      {
        itsCol++;
      }

      if(itsCol > itsMaxCol)
      {
        itsMaxCol = itsCol;
      }

      if(itsRow > itsMaxRow)
      {
        itsMaxRow = itsRow;
      }

      // test if the bot has moved
      itsRowChanged = (itsRow != itsLastRow);
      itsColChanged = (itsCol != itsLastCol);

      itsLastRow = itsRow;
      itsLastCol = itsCol;

      itsMazePosition[itsRow, itsCol] = 'x';
    }

    public override void ShowTestOutput()
    {
      if(itsMazePosition != null)
      {
        Console.WriteLine( "" );
        for(int row = 0; row < itsMazeSideLength; row++)
        {
          if(row == 0)
          {
            Console.Write( "   " );

            for(int col = 0; col < itsMazeSideLength; col++)
            {
              if(col == 0)
              {
                Console.Write( "┌─" );                
              }

              Console.Write( "──" );
                
              if(col == (itsMazeSideLength - 1))
              {
                Console.Write( "┐" );
              }
            }

            Console.WriteLine();
          }
          

          Console.Write( "   " );
          for(int col = 0; col < itsMazeSideLength; col++)
          {
            if(col == 0)
            {
              Console.Write( "│ " );
            }
 
            Console.Write( itsMazePosition[row, col] + " " );

            if(col == (itsMazeSideLength - 1))
            {
              Console.Write( "│" );
            }
          }
          Console.WriteLine();
          
          if(row == (itsMazeSideLength - 1))
          {            
            Console.Write( "   " );
            for(int col = 0; col < itsMazeSideLength; col++)
            {
              if(col == 0)
              {
                Console.Write( "└─" );
              }

              Console.Write( "──" );

              if(col == (itsMazeSideLength - 1))
              {
                Console.Write( "┘" );
              }
            }
          }
        }

        Console.WriteLine();
        Console.WriteLine();
      }
    }
  }


  /// <summary>
  /// Test for bots that can move in a horizontal line across the maze
  /// The best bots are those that get the furthest distance across the maze,
  /// so a bot's score is simply the horizontal position.
  /// </summary>
  public class TestStraightLineMove : MazeTest
  {
    public TestStraightLineMove() : base(10,0)
    {
    }
    
    /// <summary>
    /// a bot's score is simply its horizontal position
    /// </summary>
    protected override void UpdateScore(int aPass, bool aShowGrid)
    {
      // only count bots that move in a straight horizontal line
      if(itsRow == itsInitialRow)
      {
        itsScore = itsCol;
      }
    }

    /// <summary>
    /// the final score is the horizontal distance moved divided by the number of passes
    /// - so for a bot that moves one square per pass, it will have a perfect score
    /// </summary>
    /// <returns></returns>
    public override double GetFinalScore( bool aShowGrid )
    {
      return ((double)itsScore/(double)kNumberOfPasses);
    }
  }


  /// <summary>
  /// Test for bots that can move both horizontally and vertically, such that they
  /// move diagonally across the maze
  /// </summary>
  class TestDiagonalMove : MazeTest
  {
    public TestDiagonalMove()
      : base( 0, 0 )
    {
    }
    
    /// <summary>
    /// the final score is the sum of the horizontal and vertical positions, divided
    /// by the number of passes.
    /// So a bot that moves diagonally across the maze, from the top left corner to 
    /// the bottom right, will have a perfect score
    /// </summary>
    /// <returns></returns>
    public override double GetFinalScore( bool aShowGrid )
    {
      return ((double)(itsRow+itsCol)/(double)(kNumberOfPasses*2));
    }
  }

  class TestForMoveAndStop : MazeTest
  {
    public TestForMoveAndStop()
      : base( 10, 0 )
    {
    }

    public override double GetFinalScore( bool aShowGrid )
    {  
      // only count bots that move in a straight horizontal line
      if(itsRow == itsInitialRow)
      {
        return (double)(10 - Math.Abs( itsCol - 10 )) / 10.0;
      }
      else return 0;
    }
  }

  /// <summary>
  /// reward the bot the longer its on the middle square
  /// </summary>
  class TestForFastMoveAndStop : MazeTest
  {
    public TestForFastMoveAndStop()
      : base( 10, 0 )
    {
    }

    protected override void UpdateScore(int aPass, bool aShowGrid)
    {
      // check that the vertical position hasn't changed
      if( itsRow == itsInitialRow )
      {
        // increase the itsScore while the bot is moving towards the middle of the maze
        if(itsCol <= 10)
        {
          if(itsScore < 155)
          {
            itsScore += itsCol; // max itsScore = 155
          }
        }
        else
        {
          // start decreasing the itsScore when the bot has moved past the centre of the maze
          itsScore -= (itsCol - 10);
        }
      }

      if(aShowGrid)
      {
        Console.WriteLine("Score: " + itsScore + " (row = " + itsRow + ", col = " + itsCol + ")");
      }
    }

    public override double GetFinalScore(bool aShowGrid)
    {
      if(aShowGrid)
      {
        Console.WriteLine( "Final Score: " + itsScore );
      }

      return (double)(itsScore) / 155.0;
    }
  }


  /// <summary>
  /// reward the bot for moving to the middle square and back again
  /// </summary>
  public class TestForMoveThereAndBack : MazeTest
  {
    private int itsPreviousCol = 0;

    public TestForMoveThereAndBack()
      : base( 10, 0 )
    {
    }

    protected override void UpdateScore( int aPass, bool aShowGrid )
    {
      //Console.WriteLine( "Col: " + itsCol );

      // check that the vertical position hasn't changed
      if(itsRow == itsInitialRow)
      {
        if(aPass <= 11)
        {
          // increase the score while the bot is moving towards the middle of the maze
          if(itsCol > itsPreviousCol)
          {
            itsScore += 5;

            itsPreviousCol = itsCol;
          }
        }
        else
        {
          if(itsCol < itsPreviousCol)
          {
            itsScore += 5;

            itsPreviousCol = itsCol;
          }
        }
      }

      if(aShowGrid)
      {
        Console.WriteLine( "Score: " + itsScore + " (row = " + itsRow + ", col = " + itsCol + ")" );
      }
    }

    public override double GetFinalScore( bool aShowGrid )
    {
      if(aShowGrid)
      {
        Console.WriteLine( "Final Score: " + itsScore );
      }

      return (double)(itsScore) / 100.0;
    }
  }

}
