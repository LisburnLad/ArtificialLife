using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;

namespace ArtificialLife
{
  class Bot
  {
    static int itsSideLength = 5;
    static int itsGeneLength = 4;

    /// <summary>
    /// the chromosome representing this bot
    /// </summary>
    Chromosome itsChromosome;

    Test itsTest;

    /// <summary>
    /// the grid representing the structure of the bot
    /// </summary>
    int[,] itsGrid;

    /// <summary>
    /// the output of this bot at each stage of evaluation
    /// </summary>
    char[,] itsOutput;


    /// <summary>
    /// Calculate the length of chromosome required given the size of the grid and gene length
    /// </summary>
    /// <returns></returns>
    public static int GetChromosomeLength()
    {
      return ((itsSideLength * itsSideLength) * itsGeneLength);
    }


    /// <summary>
    /// create the bot's grid from the supplied chromosome
    /// </summary>
    /// <param name="aChromosome"></param>
    public Bot( Chromosome aChromosome, Test aTest, bool aShowGrid )      
    {
      // take a copy of the chromosome
      itsChromosome = aChromosome;

      // create the grid for this chromosome
      CreateGrid( aShowGrid );

      itsTest = aTest;
    }


    /// <summary>
    /// Create the grid from this bots chromosome
    /// </summary>
    /// <param name="aShowGrid"></param>
    private void CreateGrid( bool aShowGrid )
    {
      if(aShowGrid)
      {
        Console.WriteLine( "Chromosome: " + itsChromosome.ToBinaryString() );
      }

      itsGrid = new int[itsSideLength, itsSideLength];

      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          
          int geneValue = 0;
          for(int bit = 0; bit < itsGeneLength; bit++)
          {
            geneValue += Convert.ToInt32( itsChromosome.ToBinaryString( (row * itsSideLength) + col + bit, 1 ) ) * (1 << bit);
          }

          itsGrid[row, col] = geneValue;
        }
      }

      // remove cells with no connections
      PruneGrid();

      if(aShowGrid)
      {
        ShowGrid();
      }
    } 

    /// <summary>
    /// generate and evaluate a grid using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    public double Evaluate( bool aShowGrid )
    {
      // initialize the output
      InitializeOutput();

      return TestBot( aShowGrid );
    }


    #region Output Functionality


    /// <summary>
    /// allocate and initialize the output array
    /// </summary>
    private void InitializeOutput()
    {
      itsOutput = new char[itsSideLength, itsSideLength];

      // initially set all connections to zero and all other cells blank
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(itsGrid[row, col] > 0)
          {
            itsOutput[row, col] = '0';
          }
          else
          {
            itsOutput[row, col] = '-';
          }
        }
      }
    }


    private double TestBot( bool aShowGrid )
    {
      int pass = 0;
      for(; pass < itsTest.GetNumberOfPasses(); pass++)
      {
        EvaluateGrid( pass, aShowGrid );

        itsTest.EvaluateOutput( itsOutput, itsSideLength, pass, aShowGrid );
      }

      if(aShowGrid)
      {
        itsTest.ShowTestOutput();
      }

      return itsTest.GetFinalScore( aShowGrid );
    }

    /// <summary>
    /// Do a single pass of the grid to evaluate the value of each cell
    /// </summary>
    /// <param name="grid"></param>
    private void EvaluateGrid( int aPassNumber, bool aShowOutput )
    {
      // create a new itsGrid for itsOutput and reset all its cells to '-'
      char[,] newOutput = new char[itsSideLength, itsSideLength];
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          newOutput[row, col] = '-';
        }
      }

      // set the gate outputs according to the current outputs
      //
      // case 12: Console.Write("↑"); break;
      // case 13: Console.Write("→"); break;
      // case 14: Console.Write("↓"); break;
      // case 15: Console.Write("←"); break;     
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(itsGrid[row, col] >= 12 && itsGrid[row, col] <= 15)
          {
            newOutput[row, col] = (CellValue( itsGrid, itsOutput, row, col ) ? '1' : '0');

            switch(itsGrid[row, col])
            {
              // case 12: Console.Write("↑"); break;
              case 12:
                SetCellAbove( newOutput, row, col, newOutput[row, col], direction.below );
                break;

              // case 13: Console.Write("→"); break;
              case 13:
                SetCellRight( newOutput, row, col, newOutput[row, col], direction.left );
                break;

              // case 14: Console.Write("↓"); break;
              case 14:
                SetCellBelow( newOutput, row, col, newOutput[row, col], direction.above );
                break;

              // case 15: Console.Write("←"); break;
              case 15:
                SetCellLeft( newOutput, row, col, newOutput[row, col], direction.right );
                break;
            }
          }
        }
      }

      // maintain the state of any cells that haven't changed
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(newOutput[row, col] != '-')
          {
            itsOutput[row, col] = newOutput[row, col];
          }
        }
      }

      if(aShowOutput)
      {
        ShowOutput( aPassNumber );
      }
    }

    enum direction { above, left, right, below };

    private void SetPath( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // if the cell has already been set to '1' don't change its value
      if(itsGrid[row, col] > 0 && itsGrid[row, col] < 12 && newOutput[row, col] != '1' && !(newOutput[row, col] == '0' && aValue == '0'))
      {
        // set the cell value
        newOutput[row, col] = aValue;

        switch(itsGrid[row, col])
        {
          ///   // straight connections
          ///   case 1:  Console.Write("|");
          case 1:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 2:  Console.Write("─");
          case 2:
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   // T connections
          ///   case 3:  Console.Write("┴");
          case 3:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 4:  Console.Write("┬");
          case 4:
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 5:  Console.Write("├");
          case 5:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 6:  Console.Write("┤");
          case 6:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   // L connections 
          ///   case 7:  Console.Write("└");
          case 7:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 8:  Console.Write("┌");
          case 8:
            SetCellBelow(  newOutput, row, col, aValue, aDirection );
            SetCellRight(  newOutput, row, col, aValue, aDirection );
            break;

          ///   case 9:  Console.Write("┐");
          case 9:
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 10: Console.Write("┘");
          case 10:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   // full connection
          ///   case 11: Console.Write("┼");
          case 11:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;
        }
      }
    }


    ///   case 1:  Console.Write("|");
    ///   case 2:  Console.Write("─");
    ///   
    ///   // T connections
    ///   case 3:  Console.Write("┴");
    ///   case 4:  Console.Write("┬");
    ///   case 5:  Console.Write("├");
    ///   case 6:  Console.Write("┤");
    ///   
    ///   // L connections 
    ///   case 7:  Console.Write("└");
    ///   case 8:  Console.Write("┌");
    ///   case 9:  Console.Write("┐");
    ///   case 10: Console.Write("┘");
    ///   
    ///   // full connection
    ///   case 11: Console.Write("┼");


    private void SetCellRight( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      if(aDirection != direction.right && col < (itsSideLength - 1)
       && (itsGrid[row, col + 1] == 2
        || itsGrid[row, col + 1] == 3
        || itsGrid[row, col + 1] == 4
        || itsGrid[row, col + 1] == 6
        || itsGrid[row, col + 1] == 9
        || itsGrid[row, col + 1] == 10
        || itsGrid[row, col + 1] == 11))
      {
        SetPath( newOutput, row, col + 1, aValue, direction.left );
      }
    }

    private void SetCellLeft( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      if(aDirection != direction.left && col > 0
       && (itsGrid[row, col - 1] == 2
        || itsGrid[row, col - 1] == 3
        || itsGrid[row, col - 1] == 4
        || itsGrid[row, col - 1] == 5
        || itsGrid[row, col - 1] == 7
        || itsGrid[row, col - 1] == 8
        || itsGrid[row, col - 1] == 11))
      {
        SetPath( newOutput, row, col - 1, aValue, direction.right );
      }
    }

    private void SetCellBelow( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      if(aDirection != direction.below && row < (itsSideLength - 1)
       && (itsGrid[row + 1, col] == 1
        || itsGrid[row + 1, col] == 3
        || itsGrid[row + 1, col] == 5
        || itsGrid[row + 1, col] == 6
        || itsGrid[row + 1, col] == 7
        || itsGrid[row + 1, col] == 10
        || itsGrid[row + 1, col] == 11))
      {
        SetPath( newOutput, row + 1, col, aValue, direction.above );
      }
    }

    private void SetCellAbove( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      if(aDirection != direction.above && row > 0
       && (itsGrid[row - 1, col] == 1
        || itsGrid[row - 1, col] == 4
        || itsGrid[row - 1, col] == 5
        || itsGrid[row - 1, col] == 6
        || itsGrid[row - 1, col] == 8
        || itsGrid[row - 1, col] == 9
        || itsGrid[row - 1, col] == 11))
      {
        SetPath( newOutput, row - 1, col, aValue, direction.below );
      }
    }



    /// <summary>
    ///  NAND gate output = 1, unless all 3 inputs are 1
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="output"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private bool CellValue( int[,] grid, char[,] output, int row, int col )
    {
      bool cellValue = true;

      switch(grid[row, col])
      {
        case 12:  // "↑"
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case 13:  // "→"    
          cellValue &= ValueOfCellAbove( row, col );
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case 14:  // "↓"
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellAbove( row, col );
          break;

        case 15:  // "←"
          cellValue &= ValueOfCellAbove( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;
      }

      return !cellValue;
    }

    ///   // straight connections
    ///   case 1:  Console.Write("|");
    ///   case 2:  Console.Write("─");
    ///   
    ///   // T connections
    ///   case 3:  Console.Write("┴");
    ///   case 4:  Console.Write("┬");
    ///   case 5:  Console.Write("├");
    ///   case 6:  Console.Write("┤");
    ///   
    ///   // L connections 
    ///   case 7:  Console.Write("└");
    ///   case 8:  Console.Write("┌");
    ///   case 9:  Console.Write("┐");
    ///   case 10: Console.Write("┘");
    ///   
    ///   // full connection
    ///   case 11: Console.Write("┼");
    ///   
    ///   // nodes
    ///   case 12: Console.Write("↑");
    ///   case 13: Console.Write("→");
    ///   case 14: Console.Write("↓");
    ///   case 15: Console.Write("←");

    private bool ValueOfCellAbove( int aRow, int aCol )
    {
      // value of cell above
      if(aRow > 0)
      {
        int row = aRow - 1;
        int col = aCol;

        if(itsGrid[row, col] == 1
        || itsGrid[row, col] == 4
        || itsGrid[row, col] == 5
        || itsGrid[row, col] == 6
        || itsGrid[row, col] == 8
        || itsGrid[row, col] == 9
        || itsGrid[row, col] == 11
        || itsGrid[row, col] == 14)
        {
          if(itsOutput[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }

    private bool ValueOfCellBelow( int aRow, int aCol )
    {
      // value of cell below
      if(aRow < (itsSideLength - 1))
      {
        int row = aRow + 1;
        int col = aCol;

        if(itsGrid[row, col] == 1
        || itsGrid[row, col] == 3
        || itsGrid[row, col] == 5
        || itsGrid[row, col] == 6
        || itsGrid[row, col] == 7
        || itsGrid[row, col] == 10
        || itsGrid[row, col] == 11
        || itsGrid[row, col] == 12)
        {
          if(itsOutput[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }


    private bool ValueOfRightCell( int aRow, int aCol )
    {
      // value of right cell
      if(aCol < (itsSideLength - 1))
      {
        int row = aRow;
        int col = aCol + 1;

        if(itsGrid[row, col] == 2
        || itsGrid[row, col] == 3
        || itsGrid[row, col] == 4
        || itsGrid[row, col] == 6
        || itsGrid[row, col] == 9
        || itsGrid[row, col] == 10
        || itsGrid[row, col] == 11
        || itsGrid[row, col] == 15)
        {
          if(itsOutput[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }


    /// <summary>
    /// get the value of the left hand cell, if it contains a connection into the current cell
    /// </summary>
    /// <param name="output"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>
    /// the value of the left hand cell, if it contains a connection into the current cell, 
    /// true otherwise
    /// </returns>
    private bool ValueOfLeftCell( int aRow, int aCol )
    {
      // value of left cell
      if(aCol > 0)
      {
        int row = aRow;
        int col = aCol - 1;

        if(itsGrid[row, col] == 2
        || itsGrid[row, col] == 3
        || itsGrid[row, col] == 4
        || itsGrid[row, col] == 5
        || itsGrid[row, col] == 7
        || itsGrid[row, col] == 8
        || itsGrid[row, col] == 11
        || itsGrid[row, col] == 13)
        {
          if(itsOutput[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }


    /// <summary>
    /// write the current state of the grid output to the console
    /// </summary>
    /// <param name="aPassNumber"></param>
    private void ShowOutput( int aPassNumber )
    {
      Console.WriteLine( aPassNumber + "." );

      for(int row = 0; row < itsSideLength; row++)
      {
        Console.Write( "   " );

        for(int col = 0; col < itsSideLength; col++)
        {
          Console.Write( itsOutput[row, col] + " " );
        }
        Console.WriteLine();
      }

      Console.WriteLine( "____________" );
    }

    #endregion

    /// <summary>
    /// write the structure of the grid to the console
    /// </summary>
    private void ShowGrid()
    {
      Console.WriteLine();

      for(int row = 0; row < itsSideLength; row++)
      {
        Console.Write( "   " );

        for(int col = 0; col < itsSideLength; col++)
        {
          switch(itsGrid[row, col])
          {
            // empty cell
            case 0: Console.Write( " " ); break;

            // straight connections
            case 1: Console.Write( "|" ); break;
            case 2: Console.Write( "─" ); break;

            // T connections
            case 3: Console.Write( "┴" ); break;
            case 4: Console.Write( "┬" ); break;
            case 5: Console.Write( "├" ); break;
            case 6: Console.Write( "┤" ); break;

            // L connections 
            case 7: Console.Write( "└" ); break;
            case 8: Console.Write( "┌" ); break;
            case 9: Console.Write( "┐" ); break;
            case 10: Console.Write( "┘" ); break;

            // full connection
            case 11: Console.Write( "┼" ); break;

            // nodes
            case 12: Console.Write( "↑" ); break;
            case 13: Console.Write( "→" ); break;
            case 14: Console.Write( "↓" ); break;
            case 15: Console.Write( "←" ); break;
          }

          Console.Write( " " );
        }
        Console.WriteLine();
      }
      Console.WriteLine( "____________" );
    }



    /// <summary>
    /// remove unused nodes or connections
    /// 
    ///   // empty cell
    ///   case 0:  Console.Write(" ");
    ///   
    ///   // straight connections
    ///   case 1:  Console.Write("|");
    ///   case 2:  Console.Write("─");
    ///   
    ///   // T connections
    ///   case 3:  Console.Write("┴");
    ///   case 4:  Console.Write("┬");
    ///   case 5:  Console.Write("├");
    ///   case 6:  Console.Write("┤");
    ///   
    ///   // L connections 
    ///   case 7:  Console.Write("└");
    ///   case 8:  Console.Write("┌");
    ///   case 9:  Console.Write("┐");
    ///   case 10: Console.Write("┘");
    ///   
    ///   // full connection
    ///   case 11: Console.Write("┼");
    ///   
    ///   // nodes
    ///   case 12: Console.Write("↑");
    ///   case 13: Console.Write("→");
    ///   case 14: Console.Write("↓");
    ///   case 15: Console.Write("←");
    /// 
    /// </summary>
    /// <param name="grid"></param>
    private void PruneGrid()
    {
      int[,] lastGrid = new int[itsSideLength, itsSideLength];

      do
      {
        // copy the current bot grid to the 'lastGrid' 
        CopyGrid( lastGrid );

        RemoveNodesWithNoOutput();
        RemoveNodesWithNoInput();
      }
      while(CompareWithLastGrid( lastGrid ) == false);
    }


    /// <summary>
    /// copy the grid to the last grid
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    private void CopyGrid( int[,] lastGrid )
    {
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          lastGrid[row, col] = itsGrid[row, col];
        }
      }
    }


    /// <summary>
    /// test if the two supplied grids are the same
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    /// <returns>true if the grids are identical</returns>
    private bool CompareWithLastGrid( int[,] lastGrid )
    {
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          if(itsGrid[row, col] != lastGrid[row, col])
          {
            return false;
          }
        }
      }

      return true;
    }


    private void RemoveNodesWithNoInput()
    {
      // remove nodes with no input
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          // test for input from above    
          bool above = (row > 0
           && (itsGrid[row - 1, col] == 1
           || itsGrid[row - 1, col] == 4
           || itsGrid[row - 1, col] == 5
           || itsGrid[row - 1, col] == 6
           || itsGrid[row - 1, col] == 8
           || itsGrid[row - 1, col] == 9
           || itsGrid[row - 1, col] == 11
           || itsGrid[row - 1, col] == 14));

          // test for input from below
          bool below = (row < (itsSideLength - 1)
           && (itsGrid[row + 1, col] == 1
           || itsGrid[row + 1, col] == 3
           || itsGrid[row + 1, col] == 5
           || itsGrid[row + 1, col] == 6
           || itsGrid[row + 1, col] == 7
           || itsGrid[row + 1, col] == 10
           || itsGrid[row + 1, col] == 11
           || itsGrid[row + 1, col] == 14));

          // test for input from left
          bool left = (col > 0
           && (itsGrid[row, col - 1] == 2
           || itsGrid[row, col - 1] == 3
           || itsGrid[row, col - 1] == 4
           || itsGrid[row, col - 1] == 5
           || itsGrid[row, col - 1] == 7
           || itsGrid[row, col - 1] == 8
           || itsGrid[row, col - 1] == 11
           || itsGrid[row, col - 1] == 13));

          // test for input from right
          bool right = (col < (itsSideLength - 1)
           && (itsGrid[row, col + 1] == 2
           || itsGrid[row, col + 1] == 3
           || itsGrid[row, col + 1] == 4
           || itsGrid[row, col + 1] == 6
           || itsGrid[row, col + 1] == 9
           || itsGrid[row, col + 1] == 10
           || itsGrid[row, col + 1] == 11
           || itsGrid[row, col + 1] == 15));

          if(!above && !below && !left && !right)
          {
            itsGrid[row, col] = 0;
          }

          // vertical '|'
          if(itsGrid[row, col] == 1 && !above && !below)
          {
            itsGrid[row, col] = 0;
          }

          // horizontal '-'
          if(itsGrid[row, col] == 2 && !left && !right)
          {
            itsGrid[row, col] = 0;
          }


          ///   case 3:  Console.Write("┴");                            
          if(itsGrid[row, col] == 3 && !left && !above && !right)
          {
            itsGrid[row, col] = 0;
          }

          ///   case 4:  Console.Write("┬");
          if(itsGrid[row, col] == 4 && !left && !below && !right)
          {
            itsGrid[row, col] = 0;
          }

          ///   case 5:  Console.Write("├");
          if(itsGrid[row, col] == 5 && !above && !right && !below)
          {
            itsGrid[row, col] = 0;
          }

          ///   case 6:  Console.Write("┤");
          if(itsGrid[row, col] == 6 && !below && !left && !above)
          {
            itsGrid[row, col] = 0;
          }


          ///   case 7:  Console.Write("└");                              
          if(itsGrid[row, col] == 7 && !above && !right)
          {
            itsGrid[row, col] = 0;
          }

          ///   case 8:  Console.Write("┌");
          if(itsGrid[row, col] == 8 && !below && !right)
          {
            itsGrid[row, col] = 0;
          }

          ///   case 9:  Console.Write("┐");
          if(itsGrid[row, col] == 9 && !left && !below)
          {
            itsGrid[row, col] = 0;
          }

          ///   case 10: Console.Write("┘");
          if(itsGrid[row, col] == 10 && !left && !above)
          {
            itsGrid[row, col] = 0;
          }
        }
      }
    }

    private void RemoveNodesWithNoOutput()
    {
      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          // remove any nodes that point into an empty cell
          if(itsGrid[row, col] == 0)
          {
            // examine cell above
            if(row > 0
            && (itsGrid[row - 1, col] == 1
            || itsGrid[row - 1, col] == 8
            || itsGrid[row - 1, col] == 9
            || itsGrid[row - 1, col] == 14))
            {
              itsGrid[row - 1, col] = 0;
            }

            // examine cell below
            if(row < (itsSideLength - 1)
            && (itsGrid[row + 1, col] == 1
              || itsGrid[row + 1, col] == 7
              || itsGrid[row + 1, col] == 10
              || itsGrid[row + 1, col] == 12))
            {
              itsGrid[row + 1, col] = 0;
            }

            // examine cell to left
            if(col > 0
            && (itsGrid[row, col - 1] == 2
            || itsGrid[row, col - 1] == 7
            || itsGrid[row, col - 1] == 8
            || itsGrid[row, col - 1] == 13))
            {
              itsGrid[row, col - 1] = 0;
            }

            // examine cell to right
            if(col < (itsSideLength - 1)
            && (itsGrid[row, col + 1] == 2
            || itsGrid[row, col + 1] == 9
            || itsGrid[row, col + 1] == 10
            || itsGrid[row, col + 1] == 15))
            {
              itsGrid[row, col + 1] = 0;
            }
          }


          //
          // remove nodes with outputs into each other

          //12: Console.Write("↑");
          //13: Console.Write("→");
          //14: Console.Write("↓");
          //15: Console.Write("←");

          // vertical pair
          if(itsGrid[row, col] == 12 && (row > 0 && itsGrid[row - 1, col] == 14))
          {
            itsGrid[row, col] = 0;
          }

          // horizontal pair
          if(itsGrid[row, col] == 15 && (col > 0 && itsGrid[row, col - 1] == 13))
          {
            itsGrid[row, col] = 0;
          }
        }
      }
    }
  }
}



