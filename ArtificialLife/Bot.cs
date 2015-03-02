﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ArtificialLife
{
  class Bot
  {
    static int itsGeneLength = 5;

    /// <summary>
    /// the length of the side of the square grid
    /// </summary>
    public int itsSideLength { get; set; }

    /// <summary>
    /// the chromosome representing this bot
    /// </summary>
    Chromosome itsChromosome;

    Test itsTest;

    /// <summary>
    /// the output of this bot at each stage of evaluation
    /// </summary>
    char[,] itsOutput;


    enum CellType
    {
      EmptyCell,          // " " - 0

      // Straight Connections
      NorthSouth,         // "|" - 1
      WestEast,           // "─" - 2

      // T Connections
      WestNorthEast,      // "┴" - 3
      EastSouthWest,      // "┬" - 4
      NorthEastSouth,     // "├" - 5
      SouthWestNorth,     // "┤" - 6

      // L Connections
      NorthEast,          // "└" - 7
      EastSouth,          // "┌" - 8
      SouthWest,          // "┐" - 9
      WestNorth,          // "┘" - 10

      // Full Connection
      NorthEastSouthWest, // "┼" - 11

      // Nodes
      NorthNode,          // "↑" - 12
      EastNode,           // "→" - 13
      SouthNode,          // "↓" - 14
      WestNode,           // "←" - 15

      // Delay Nodes
      NorthDelay,         // "▲" - 16
      EastDelay,          // "►" - 17 - 10001
      SouthDelay,         // "▼" - 18
      WestDelay           // "◄" - 19

    };

    /// <summary>
    /// the grid representing the structure of the bot
    /// </summary>
    CellType[,] itsGrid;

    /// <summary>
    /// return the length of chromosome required for the supplied side length (for a square grid)
    /// </summary>
    /// <param name="aSideLength"></param>
    /// <returns></returns>
    public static int GetChromosomeLength(int aSideLength)
    {
      return ((aSideLength * aSideLength) * itsGeneLength);
    }

    /// <summary>
    /// given a chromosome, calculate the length of each grid side (assuming a square grid)
    /// </summary>
    /// <param name="aChromosome"></param>
    /// <returns></returns>
    public static int GetSideLengthFromChromosome( string aChromosome )
    {
      int chromosomeLength = aChromosome.Length;

      int numberOfGenes = chromosomeLength / itsGeneLength;

      return Convert.ToInt32( System.Math.Sqrt(numberOfGenes));
    }


    /// <summary>
    /// create the bot's grid from the supplied chromosome
    /// </summary>
    /// <param name="aChromosome"></param>
    public Bot( Chromosome aChromosome, Test aTest, int aSideLength, bool aShowGrid )      
    {
      itsSideLength = aSideLength;

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

      try
      {
      itsGrid = new CellType[itsSideLength, itsSideLength];

      for(int row = 0; row < itsSideLength; row++)
      {
        for(int col = 0; col < itsSideLength; col++)
        {
          CellType geneValue = CellType.EmptyCell;
          for(int bit = 0; bit < itsGeneLength; bit++)
          {            
            int position = (row * itsGeneLength * itsSideLength) + (col * itsGeneLength) + bit;
            geneValue += Convert.ToInt32( itsChromosome.ToBinaryString( position, 1 ) ) * (1 << (itsGeneLength - (bit + 1)));
          }

          itsGrid[row, col] = geneValue;
        }
      }

      // remove cells with no connections
      PruneGrid(aShowGrid);

      if(aShowGrid)
      {
        ShowGrid("c:\\grid.bmp");
      }
    } 

      catch
      {
        Console.WriteLine("An error occurred during grid creation: ");
        Console.WriteLine("Chromosome: " + itsChromosome.ToBinaryString());
        Console.WriteLine("Side Length: " + itsSideLength);
        Console.WriteLine("Chromosome Length: " + GetChromosomeLength(itsSideLength));
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
          // retain delay node output
          if(itsOutput[row, col] == '>' || itsOutput[row, col] == '>')
          {
            newOutput[row, col] = itsOutput[row, col];
          }
          else
          {
            newOutput[row, col] = '-';
          }
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
          if(itsGrid[row, col] >= CellType.NorthNode && itsGrid[row, col] <= CellType.WestNode)
          {
            newOutput[row, col] = (CellValue( itsGrid, itsOutput, row, col ) ? '1' : '0');

            switch(itsGrid[row, col])
            {
              // case 12: Console.Write("↑"); break;
              case CellType.NorthNode:
                SetCellAbove( newOutput, row, col, newOutput[row, col], direction.below );
                break;

              // case 13: Console.Write("→"); break;
              case CellType.EastNode:
                SetCellRight( newOutput, row, col, newOutput[row, col], direction.left );
                break;

              // case 14: Console.Write("↓"); break;
              case CellType.SouthNode:
                SetCellBelow( newOutput, row, col, newOutput[row, col], direction.above );
                break;

              // case 15: Console.Write("←"); break;
              case CellType.WestNode:
                SetCellLeft( newOutput, row, col, newOutput[row, col], direction.right );
                break;
            }
          }

          if(itsGrid[row, col] >= CellType.NorthDelay && itsGrid[row, col] <= CellType.WestDelay)
          {
            bool cellValue = CellValue( itsGrid, itsOutput, row, col );
            
            char output = '0';

            // if the current node value is '>' then output will be 1
            if(newOutput[row, col] == '>')
            {
              // test if the output should be switched next time
              if(cellValue == false)
              {
                newOutput[row, col] = '<';
              }

              output = '1';
            }
            else
            {
              // test if the output should be switched next time
              if(cellValue == true)
              {
                newOutput[row, col] = '>';
              }

              // if the current node value is '<' then output will be 0
              output = '0';
            }

            switch(itsGrid[row, col])
            {
              
              case CellType.NorthDelay:
                SetCellAbove( newOutput, row, col, output, direction.below );
                break;

              
              case CellType.EastDelay:
                SetCellRight( newOutput, row, col, output, direction.left );
                break;

              
              case CellType.SouthDelay:
                SetCellBelow( newOutput, row, col, output, direction.above );
                break;

              
              case CellType.WestDelay:
                SetCellLeft( newOutput, row, col, output, direction.right );
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
      if(itsGrid[row, col] > 0 && itsGrid[row, col] < CellType.NorthNode && newOutput[row, col] != '1' && !(newOutput[row, col] == '0' && aValue == '0'))
      {
        // set the cell value
        newOutput[row, col] = aValue;

        switch(itsGrid[row, col])
        {
          ///   // straight connections
          ///   case 1:  Console.Write("|");
          case CellType.NorthSouth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 2:  Console.Write("─");
          case CellType.WestEast:
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   // T connections
          ///   case 3:  Console.Write("┴");
          case CellType.WestNorthEast:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 4:  Console.Write("┬");
          case CellType.EastSouthWest:
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 5:  Console.Write("├");
          case CellType.NorthEastSouth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 6:  Console.Write("┤");
          case CellType.SouthWestNorth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   // L connections 
          ///   case 7:  Console.Write("└");
          case CellType.NorthEast:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 8:  Console.Write("┌");
          case CellType.EastSouth:
            SetCellBelow(  newOutput, row, col, aValue, aDirection );
            SetCellRight(  newOutput, row, col, aValue, aDirection );
            break;

          ///   case 9:  Console.Write("┐");
          case CellType.SouthWest:
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   case 10: Console.Write("┘");
          case CellType.WestNorth:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            break;

          ///   // full connection
          ///   case 11: Console.Write("┼");
          case CellType.NorthEastSouthWest:
            SetCellAbove( newOutput, row, col, aValue, aDirection );
            SetCellBelow( newOutput, row, col, aValue, aDirection );
            SetCellLeft( newOutput, row, col, aValue, aDirection );
            SetCellRight( newOutput, row, col, aValue, aDirection );
            break;
        }
      }
    }


    private void SetCellRight( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell to the right has a West connection
      if(aDirection != direction.right && col < (itsSideLength - 1)
       && (itsGrid[row, col + 1] == CellType.WestEast
        || itsGrid[row, col + 1] == CellType.WestNorthEast
        || itsGrid[row, col + 1] == CellType.EastSouthWest
        || itsGrid[row, col + 1] == CellType.SouthWestNorth
        || itsGrid[row, col + 1] == CellType.SouthWest
        || itsGrid[row, col + 1] == CellType.WestNorth
        || itsGrid[row, col + 1] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row, col + 1, aValue, direction.left );
      }
    }

    private void SetCellLeft( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell to the left has an East connection
      if(aDirection != direction.left && col > 0
       && (itsGrid[row, col - 1] == CellType.WestEast
        || itsGrid[row, col - 1] == CellType.WestNorthEast
        || itsGrid[row, col - 1] == CellType.EastSouthWest
        || itsGrid[row, col - 1] == CellType.NorthEastSouth
        || itsGrid[row, col - 1] == CellType.NorthEast
        || itsGrid[row, col - 1] == CellType.EastSouth
        || itsGrid[row, col - 1] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row, col - 1, aValue, direction.right );
      }
    }

    private void SetCellBelow( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell below has a North connection
      if(aDirection != direction.below && row < (itsSideLength - 1)
       && (itsGrid[row + 1, col] == CellType.NorthSouth
        || itsGrid[row + 1, col] == CellType.WestNorthEast
        || itsGrid[row + 1, col] == CellType.NorthEastSouth
        || itsGrid[row + 1, col] == CellType.SouthWestNorth
        || itsGrid[row + 1, col] == CellType.NorthEast
        || itsGrid[row + 1, col] == CellType.WestNorth
        || itsGrid[row + 1, col] == CellType.NorthEastSouthWest))
      {
        SetPath( newOutput, row + 1, col, aValue, direction.above );
      }
    }

    private void SetCellAbove( char[,] newOutput, int row, int col, char aValue, direction aDirection )
    {
      // test if the cell above has a North connection
      if(aDirection != direction.above && row > 0
       && (itsGrid[row - 1, col] == CellType.NorthSouth
        || itsGrid[row - 1, col] == CellType.EastSouthWest
        || itsGrid[row - 1, col] == CellType.NorthEastSouth
        || itsGrid[row - 1, col] == CellType.SouthWestNorth
        || itsGrid[row - 1, col] == CellType.EastSouth
        || itsGrid[row - 1, col] == CellType.SouthWest
        || itsGrid[row - 1, col] == CellType.NorthEastSouthWest))
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
    private bool CellValue( CellType[,] grid, char[,] output, int row, int col )
    {
      bool cellValue = true;

      // test if this is a delay node
      // - delay nodes will do an OR operation on the inputs
      bool delayNode = false;
      if(grid[row, col] >= CellType.NorthDelay && grid[row, col] <= CellType.WestDelay)
      {
        delayNode = true;
        cellValue = false;
      }

      switch(grid[row, col])
      {
        case CellType.NorthNode:  // "↑"
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case CellType.EastNode:  // "→"    
          cellValue &= ValueOfCellAbove( row, col );
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case CellType.SouthNode:  // "↓"
          cellValue &= ValueOfLeftCell( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellAbove( row, col );
          break;

        case CellType.WestNode:  // "←"
          cellValue &= ValueOfCellAbove( row, col );
          cellValue &= ValueOfRightCell( row, col );
          cellValue &= ValueOfCellBelow( row, col );
          break;

        case CellType.NorthDelay:  // "▲"
          cellValue |= ValueOfLeftCell( row, col );         
          cellValue |= ValueOfRightCell( row, col );        
          cellValue |= ValueOfCellBelow( row, col );        
          break;

        case CellType.EastDelay:  // "►"    
          cellValue |= ValueOfCellAbove( row, col );
          cellValue |= ValueOfLeftCell( row, col );
          cellValue |= ValueOfCellBelow( row, col );
          break;

        case CellType.SouthDelay:  // "▼"
          cellValue |= ValueOfLeftCell( row, col );
          cellValue |= ValueOfRightCell( row, col );
          cellValue |= ValueOfCellAbove( row, col );
          break;

        case CellType.WestDelay:  // "◄"
          cellValue |= ValueOfCellAbove( row, col );
          cellValue |= ValueOfRightCell( row, col );
          cellValue |= ValueOfCellBelow( row, col );
          break;
      }

      if(delayNode)
      {
        return cellValue;
      }

      return !cellValue;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'South' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForSouthConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.NorthSouth
          || itsGrid[row, col] == CellType.EastSouthWest
          || itsGrid[row, col] == CellType.NorthEastSouth
          || itsGrid[row, col] == CellType.SouthWestNorth
          || itsGrid[row, col] == CellType.EastSouth
          || itsGrid[row, col] == CellType.SouthWest
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.SouthNode
          || itsGrid[row, col] == CellType.SouthDelay;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'North' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForNorthConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.NorthSouth
          || itsGrid[row, col] == CellType.WestNorthEast
          || itsGrid[row, col] == CellType.NorthEastSouth
          || itsGrid[row, col] == CellType.SouthWestNorth
          || itsGrid[row, col] == CellType.NorthEast
          || itsGrid[row, col] == CellType.WestNorth
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.NorthNode
          || itsGrid[row, col] == CellType.NorthDelay;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'West' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForWestConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.WestEast
          || itsGrid[row, col] == CellType.WestNorthEast
          || itsGrid[row, col] == CellType.EastSouthWest
          || itsGrid[row, col] == CellType.SouthWestNorth
          || itsGrid[row, col] == CellType.SouthWest
          || itsGrid[row, col] == CellType.WestNorth
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.WestNode
          || itsGrid[row, col] == CellType.WestDelay;
    }


    /// <summary>
    /// Test if the cell at the specified location has a 'East' output
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool TestForEastConnection( int row, int col )
    {
      return itsGrid[row, col] == CellType.WestEast
          || itsGrid[row, col] == CellType.WestNorthEast
          || itsGrid[row, col] == CellType.EastSouthWest
          || itsGrid[row, col] == CellType.NorthEastSouth
          || itsGrid[row, col] == CellType.NorthEast
          || itsGrid[row, col] == CellType.EastSouth
          || itsGrid[row, col] == CellType.NorthEastSouthWest
          || itsGrid[row, col] == CellType.EastNode
          || itsGrid[row, col] == CellType.EastDelay;
    }


    /// <summary>
    /// if the cell above has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfCellAbove( int aRow, int aCol )
    {
      // value of cell above - cells with South connection
      if(aRow > 0)
      {
        int row = aRow - 1;
        int col = aCol;

        if(TestForSouthConnection( row, col ))
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
    /// if the cell below has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfCellBelow( int aRow, int aCol )
    {
      // value of cell below
      if(aRow < (itsSideLength - 1))
      {
        int row = aRow + 1;
        int col = aCol;

        // connections with a North output
        if(TestForNorthConnection( row, col ))
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
    /// if the cell to the right has an output of zero and has a connection into this cell then return false 
    /// </summary>
    /// <param name="aRow"></param>
    /// <param name="aCol"></param>
    /// <returns></returns>
    private bool ValueOfRightCell( int aRow, int aCol )
    {
      // value of right cell
      if(aCol < (itsSideLength - 1))
      {
        int row = aRow;
        int col = aCol + 1;

        // connections with a West output
        if(TestForWestConnection( row, col ))
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

        // connnections with an East output
        if(TestForEastConnection( row, col ))
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


    #region Node Drawing

    /// <summary>
    /// write the structure of the grid to the console
    /// </summary>
    private void ShowGrid( string aImageName )
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
            case CellType.EmptyCell: Console.Write( " " ); break;

            // straight connections
            case CellType.NorthSouth: Console.Write("│"); break;
            case CellType.WestEast: Console.Write( "─" ); break;

            // T connections
            case CellType.WestNorthEast: Console.Write( "┴" ); break;
            case CellType.EastSouthWest: Console.Write("┬"); break;
            case CellType.NorthEastSouth: Console.Write( "├" ); break;
            case CellType.SouthWestNorth: Console.Write("┤"); break;

            // L connections 
            case CellType.NorthEast: Console.Write("└"); break;
            case CellType.EastSouth: Console.Write("┌"); break;
            case CellType.SouthWest: Console.Write("┐"); break;
            case CellType.WestNorth: Console.Write("┘"); break;

            // full connection
            case CellType.NorthEastSouthWest: Console.Write("┼"); break;

            // nodes
            case CellType.NorthNode: Console.Write("↑"); break;
            case CellType.EastNode: Console.Write("→"); break;
            case CellType.SouthNode: Console.Write("↓"); break;
            case CellType.WestNode: Console.Write("←"); break;

            case CellType.NorthDelay: Console.Write( "▲"); break;
            case CellType.EastDelay:  Console.Write( "►"); break;
            case CellType.SouthDelay: Console.Write( "▼"); break;
            case CellType.WestDelay:  Console.Write( "◄"); break;

            default: Console.Write(" "); break;
          }

          Console.Write( " " );
        }
        Console.WriteLine();
      }
      Console.WriteLine( "____________" );

      // create an image of this grid
      CreateGridImage(aImageName);
    }


    private void CreateGridImage(string aImageName)
    {
      int pixelLength = (itsSideLength * 32) + 1;

      Bitmap bitmap = new Bitmap(pixelLength, pixelLength);
      Graphics graphics = Graphics.FromImage(bitmap);

      graphics.FillRectangle(Brushes.White, 0, 0, pixelLength, pixelLength);

      Pen linePen = new Pen(Color.Black,2);
      linePen.StartCap = LineCap.Square;
      linePen.EndCap = LineCap.Square;     


      Pen dashPen = new Pen(Color.DarkGray, 2);
      dashPen.DashStyle = DashStyle.Dash;

      // draw grid lines
      for(int row = 0; row < itsSideLength; row++)
      {
        if(row < (itsSideLength - 1))
        {
          graphics.DrawLine( dashPen, 0, (row * 32) + 32, pixelLength, (row * 32) + 32 );
        }

        for(int col = 0; col < itsSideLength; col++)
        {
          int x = (col * 32);
          int y = (row * 32);

          if(row == 0)
          {
            graphics.DrawLine( dashPen, x, 0, x, pixelLength );
          }
        }
      }
      
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          int x = (col * 32);
          int y = (row * 32);

          switch (itsGrid[row, col])
          {
            // empty cell
            case CellType.EmptyCell: break;

            // straight connections
            case CellType.NorthSouth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              break;
            case CellType.WestEast:
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;

            // L connections 
            case CellType.NorthEast:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 16);
              graphics.DrawLine(linePen, x + 16, y + 16, x + 32, y + 16);
              break;
            case CellType.EastSouth:
              graphics.DrawLine(linePen, x + 16, y + 16, x + 16, y + 32);
              graphics.DrawLine(linePen, x + 16, y + 16, x + 32, y + 16);
              break;
            case CellType.SouthWest:
              graphics.DrawLine(linePen, x + 16, y + 16, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 16, y + 16);
              break;
            case CellType.WestNorth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 16);
              graphics.DrawLine(linePen, x, y + 16, x + 16, y + 16);
              break;

            // T connections
            case CellType.WestNorthEast:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 16);
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;
            case CellType.EastSouthWest:
              graphics.DrawLine(linePen, x + 16, y + 16, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;
            case CellType.NorthEastSouth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              graphics.DrawLine(linePen, x + 16, y + 16, x + 32, y + 16);
              break;
            case CellType.SouthWestNorth:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 16, y + 16);
              break;

            // full connection
            case CellType.NorthEastSouthWest:
              graphics.DrawLine(linePen, x + 16, y, x + 16, y + 32);
              graphics.DrawLine(linePen, x, y + 16, x + 32, y + 16);
              break;

            // nodes
            case CellType.NorthNode:
              DrawNorthNode(graphics, linePen, x, y);
              break;
            case CellType.EastNode:
              DrawEastNode(graphics, linePen, x, y);
              break;
            case CellType.SouthNode:
              DrawSouthNode(graphics, linePen, x, y);
              break;
            case CellType.WestNode:
              DrawWestNode(graphics, linePen, x, y);
              break;

            // delays
            case CellType.NorthDelay:
              DrawNorthDelay(graphics, linePen, x, y);
              break;
            case CellType.EastDelay:
              DrawEastDelay(graphics, linePen, x, y);
              break;
            case CellType.SouthDelay:
              DrawSouthDelay(graphics, linePen, x, y);
              break;
            case CellType.WestDelay:
              DrawWestDelay(graphics, linePen, x, y);
              break;

            default: break;
          }
        }
      }

      //graphics.DrawRectangle(linePen, 0, 0, pixelLength, pixelLength);

      linePen.Dispose();
      dashPen.Dispose();

      bitmap.Save(aImageName);
    }


    /// <summary>
    /// Draw the main shape of a node
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="linePen"></param>
    /// <param name="linePoints"></param>
    /// <param name="sidePoints"></param>
    private static void DrawNode( Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints )
    {
      using(SolidBrush brush = new SolidBrush( Color.FromArgb( 0x7F, 0x92, 0xFF ) ))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddCurve( linePoints );
        path.AddLines( sidePoints );
        graphics.FillPath( brush, path );
      }

      // draw the top curve
      graphics.DrawCurve( linePen, linePoints );

      // draw the sides        
      graphics.DrawLines( linePen, sidePoints );
    }


    /// <summary>
    /// Draw the main shape for a delay node
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="linePen"></param>
    /// <param name="linePoints"></param>
    /// <param name="sidePoints"></param>
    private static void DrawDelay( Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints )
    {
      using(SolidBrush brush = new SolidBrush( Color.FromArgb( 0x00, 0x9B, 0x0E ) ))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddLines( linePoints );
        path.AddLines( sidePoints );
        graphics.FillPath( brush, path );

        // draw the top curve
        graphics.DrawLines( linePen, linePoints );

        // draw the sides        
        graphics.DrawLines( linePen, sidePoints );
      }
    }


    private static Point[] GetNorthSidePoints( int x, int endX, int midY, int endY )
    {
      Point[] sidePoints = { 
                               new Point(x, midY),                               
                               new Point(x, endY),                               
                               new Point(endX, endY),
                               new Point(endX, midY)                    
                             };
      return sidePoints;
    }

    private static Point[] GetEastSidePoints( int x, int y, int midX, int endY )
    {
      Point[] sidePoints = { 
                               new Point(midX, y),
                               new Point(x, y),                               
                               new Point(x, endY),
                               new Point(midX, endY)                    
                             };
      return sidePoints;
    }

    private static Point[] GetSouthSidePoints( int x, int y, int endX, int midY )
    {
      Point[] sidePoints = { 
                             new Point(x, midY),                               
                             new Point(x, y),                               
                             new Point(endX, y),
                             new Point(endX, midY)                    
                           };
      return sidePoints;
    }

    private static Point[] GetWestSidePoints( int y, int midX, int endX, int endY )
    {
      Point[] sidePoints = { 
                               new Point(midX, y),                               
                               new Point(endX, y),                               
                               new Point(endX, endY),
                               new Point(midX, endY)                    
                             };
      return sidePoints;
    }

    private static void DrawWestNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX,   y),
                               new Point(x + 12, y+ 4 ),
                               new Point(x + 8,  y+ mid),
                               new Point(x + 12, y+ 28),
                               new Point(midX,   endY - 1)                       
                             };

      Point[] sidePoints = GetWestSidePoints( y, midX, endX, endY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, x, (midY) - radius, (radius * 2), (radius * 2) );
    }

    private static void DrawEastNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX, y),
                               new Point(x + 20, y+ 4 ),
                               new Point(x + 24, midY),
                               new Point(x + 20, y+ 28),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints( x, y, midX, endY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, (endX) - (radius * 2), (midY) - radius, (radius * 2), (radius * 2) );
    }

    private static void DrawNorthNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x, midY),
                               new Point(x + 4, y + 12),
                               new Point(midX, y + 8),
                               new Point(x + 28, y + 12),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints( x, endX, midY, endY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, (midX) - radius, y, (radius * 2), (radius * 2) );
    }
    
    private static void DrawSouthNode( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                             new Point(x, midY),
                             new Point(x + 4, y + 20),
                             new Point(midX, y + 24),
                             new Point(x + 28, y + 20),
                             new Point(endX - 1, midY)                       
                           };

      Point[] sidePoints = GetSouthSidePoints( x, y, endX, midY );

      DrawNode( graphics, linePen, linePoints, sidePoints );

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse( linePen, (midX) - radius, (endY) - (radius * 2), (radius * 2), (radius * 2) );
    }


    //
    // Draw Delay Nodes
    //


    private static void DrawWestDelay(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                              new Point(midX - 1, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX - 1, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints( y, midX, endX, endY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }
    
    private static void DrawEastDelay(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX + 1, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX + 1, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints( x, y, midX, endY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }



    private static void DrawNorthDelay( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY - 1),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY - 1)
                             };

      Point[] sidePoints = GetNorthSidePoints( x, endX, midY, endY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }


    private static void DrawSouthDelay( Graphics graphics, Pen linePen, int x, int y )
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY + 1),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY + 1)                       
                             };

      Point[] sidePoints = GetSouthSidePoints( x, y, endX, midY );

      DrawDelay( graphics, linePen, linePoints, sidePoints );
    }


    #endregion Node Drawing
    

    /// <summary>
    /// remove unused nodes or connections
    /// </summary>
    /// <param name="grid"></param>
    private void PruneGrid(bool aShowGrid)
    {
      CellType[,] lastGrid = new CellType[itsSideLength, itsSideLength];

      int pass = 0;
      do
      {
        if (aShowGrid)
        {
          ShowGrid("c:\\prune_" + pass + ".bmp");
        }

        // copy the current bot grid to the 'lastGrid' 
        CopyGrid( lastGrid );

        RemoveNodesWithNoOutput();
        RemoveNodesWithNoInput();

        pass++;
      }
      while(CompareWithLastGrid( lastGrid ) == false);
    }


    /// <summary>
    /// copy the grid to the last grid
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    private void CopyGrid(CellType[,] lastGrid)
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
    private bool CompareWithLastGrid(CellType[,] lastGrid)
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
          // test for input from above - connections that point south
          bool above = (row > 0 && TestForSouthConnection((row-1),col));

          // test for input from below - connections that point north
          bool below = (row < (itsSideLength - 1) && TestForNorthConnection((row+1),col));

          // test for input from left - connections that point east
          bool left = (col > 0 && TestForEastConnection(row,(col-1)));

          // test for input from right - connections that point west
          bool right = (col < (itsSideLength - 1) && TestForWestConnection(row,(col+1)));

          if(!above && !below && !left && !right)
          {
            itsGrid[row, col] = 0;
          }

          // vertical '|'
          if(itsGrid[row, col] == CellType.NorthSouth && !above && !below)
          {
            itsGrid[row, col] = 0;
          }

          // horizontal '-'
          if(itsGrid[row, col] == CellType.WestEast && !left && !right)
          {
            itsGrid[row, col] = 0;
          }
          
          /// "┴"                           
          if(itsGrid[row, col] == CellType.WestNorthEast && !left && !above && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "┬"
          if(itsGrid[row, col] == CellType.EastSouthWest && !left && !below && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "├"
          if(itsGrid[row, col] == CellType.NorthEastSouth && !above && !right && !below)
          {
            itsGrid[row, col] = 0;
          }

          /// "┤"
          if(itsGrid[row, col] == CellType.SouthWestNorth && !below && !left && !above)
          {
            itsGrid[row, col] = 0;
          }


          /// "└"                            
          if(itsGrid[row, col] == CellType.NorthEast && !above && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "┌"
          if(itsGrid[row, col] == CellType.EastSouth && !below && !right)
          {
            itsGrid[row, col] = 0;
          }

          /// "┐"
          if(itsGrid[row, col] == CellType.SouthWest && !left && !below)
          {
            itsGrid[row, col] = 0;
          }

          /// "┘"
          if(itsGrid[row, col] == CellType.WestNorth && !left && !above)
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
          // remove any nodes that point only into an empty cell
          if(itsGrid[row, col] == 0)
          {
            // examine cell above
            if(row > 0
            && (itsGrid[row - 1, col] == CellType.NorthSouth
            || itsGrid[row - 1, col] == CellType.EastSouth
            || itsGrid[row - 1, col] == CellType.SouthWest
            || itsGrid[row - 1, col] == CellType.SouthNode))
            {
              itsGrid[row - 1, col] = 0;
            }

            // examine cell below
            if(row < (itsSideLength - 1)
            && (itsGrid[row + 1, col] == CellType.NorthSouth
              || itsGrid[row + 1, col] == CellType.NorthEast
              || itsGrid[row + 1, col] == CellType.WestNorth
              || itsGrid[row + 1, col] == CellType.NorthNode))
            {
              itsGrid[row + 1, col] = 0;
            }

            // examine cell to left
            if(col > 0
            && (itsGrid[row, col - 1] == CellType.WestEast
            || itsGrid[row, col - 1] == CellType.NorthEast
            || itsGrid[row, col - 1] == CellType.EastSouth
            || itsGrid[row, col - 1] == CellType.EastNode))
            {
              itsGrid[row, col - 1] = 0;
            }

            // examine cell to right
            if(col < (itsSideLength - 1)
            && (itsGrid[row, col + 1] == CellType.WestEast
            || itsGrid[row, col + 1] == CellType.SouthWest
            || itsGrid[row, col + 1] == CellType.WestNorth
            || itsGrid[row, col + 1] == CellType.WestNode))
            {
              itsGrid[row, col + 1] = 0;
            }
          }


          //
          // remove nodes with outputs into each other

          // vertical pair
          if(itsGrid[row, col] == CellType.NorthNode && (row > 0 && itsGrid[row - 1, col] == CellType.SouthNode))
          {
            itsGrid[row, col] = 0;
          }

          // horizontal pair
          if(itsGrid[row, col] == CellType.WestNode && (col > 0 && itsGrid[row, col - 1] == CellType.EastNode))
          {
            itsGrid[row, col] = 0;
          }

          // vertical pair
          if(itsGrid[row, col] == CellType.NorthDelay && (row > 0 && itsGrid[row - 1, col] == CellType.SouthDelay))
          {
            itsGrid[row, col] = 0;
          }

          // horizontal pair
          if(itsGrid[row, col] == CellType.WestDelay && (col > 0 && itsGrid[row, col - 1] == CellType.EastDelay))
          {
            itsGrid[row, col] = 0;
          }
        }
      }
    }
  }
}



