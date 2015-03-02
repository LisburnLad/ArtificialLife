using GAF;
using GAF.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  class Grid
  {
    const int MaxEvaluationCount = 500;


    //const int numberOfBands = 3;
    int itsSideLength =5;
    int itsChromosomeLength = 0;
    int itsGeneLength = 4;


    public Grid()
    {
      //itsSideLength = (numberOfBands * 2) - 1;
      itsChromosomeLength = ((itsSideLength * itsSideLength) * itsGeneLength);
    }

    public void Generate()
    {
      Console.WriteLine("Generating Grid");
      //Console.WriteLine("Number of Bands: " + numberOfBands);
      Console.WriteLine("Chromosome Length: " + itsChromosomeLength);



      string testChromosome = "1110000101111101010111010001011101011100110100101001010111001010001100110001100101010010000010000110";

      //string testChromosome = "0001001000111110111110110011011111000011100101101110001111101010111011000111010100000010100011010100";


      //string testChromosome = "1101001111111011011010111000001101000000001100001101010010011010011100101110101110101100110100001010";


      //string testChromosome = "0100010010011101011101100110000001100111011101011011110111000101010001100001110000111011010110001010";


      //string testChromosome = "1110010011010000110001000100111111011001111111010110011000101011001000000100001101011010011011000100";

      //string testChromosome = "0011111011000110010000110101101101000110010001010001011110101001110101101010110100011010101000101001";
      Chromosome chromosome = new Chromosome(testChromosome);
      double fitness = CreateGridFromChromosome(chromosome,true);

      //for (int test = 0; test < 100; test++)
      //{
      //  Chromosome chromosome = new Chromosome(itsChromosomeLength);
      //  double fitness = CreateGridFromChromosome(chromosome);

      //  Console.WriteLine("Chromosome Fitmess: " + fitness);
      //}
    }


    private void ga_OnGenerationComplete(object sender, GaEventArgs e)
    {
      //get the best solution 
      var chromosome = e.Population.GetTop(1)[0];

      //decode chromosome
      double fitness = CreateGridFromChromosome(chromosome,true);

      Console.WriteLine("Final Chromosome Fitmess: " + fitness);

      Console.WriteLine("Evaluations: " + e.Evaluations);

      ////get x and y from the solution 
      //var x1 = Convert.ToInt32(chromosome.ToBinaryString(0, chromosome.Count / 2), 2);
      //var y1 = Convert.ToInt32(chromosome.ToBinaryString(chromosome.Count / 2, chromosome.Count / 2), 2);

      ////Adjust range to -100 to +100 
      //var rangeConst = 200 / (System.Math.Pow(2, chromosome.Count / 2) - 1);
      //var x = (x1 * rangeConst) - 100;
      //var y = (y1 * rangeConst) - 100;

      ////display the X, Y and fitness of the best chromosome in this generation 
      //Console.WriteLine("x:{0} y:{1} Fitness{2}", x, y, e.Population.MaximumFitness);
    }


    public void StartEvolution()
    {
      const double crossoverProbability = 0.65;
      const double mutationProbability = 0.08;
      const int elitismPercentage = 5;

      //create a Population of 1000 random chromosomes of length 50
      var population = new Population(5000, itsChromosomeLength, false, false);

      //create the genetic operators 
      var elite = new Elite(elitismPercentage);

      var crossover = new Crossover(crossoverProbability, true)
      {
          CrossoverType = CrossoverType.SinglePoint
      };

      var mutation = new BinaryMutate(mutationProbability, true);

      //create the GA itself 
      var ga = new GeneticAlgorithm(population, EvaluateChromosome);

      //subscribe to the GAs Generation Complete event 
      ga.OnGenerationComplete += ga_OnGenerationComplete;

      //add the operators to the ga process pipeline 
      ga.Operators.Add(elite);
      ga.Operators.Add(crossover);
      ga.Operators.Add(mutation);

      //run the GA 
      ga.Run(TerminateFunction);
    }


    private bool TerminateFunction(Population population, int currentGeneration, long currentEvaluation) 
    { 
      //example termination criterion 
      return (currentEvaluation >= MaxEvaluationCount);
    }


    /// <summary>
    /// Create the grid and evaluate its performance without any output
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double EvaluateChromosome(Chromosome chromosome)
    {
      return CreateGridFromChromosome( chromosome,false);
    }

    /// <summary>
    /// generate and evaluate a grid using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double CreateGridFromChromosome(Chromosome chromosome,bool aShowGrid)
    {
      int[,] grid = CreateGrid(chromosome, aShowGrid);
      char[,] output = InitializeOutput(grid);


      return EvaluateForMoveInLShape(aShowGrid, grid, output, aShowGrid);
      //return EvaluateForMoveFastAndStop(aShowGrid, grid, output, aShowGrid);
      //return EvaluateForMoveAndStop(aShowGrid, grid, output, aShowGrid);
      //return EvaluateForDiagonalMove(aShowGrid, grid, output, aShowGrid);
      //return EvaluateForStraightLineMove(aShowGrid, grid, output, aShowGrid);
      //return EvaluateForChange(aShowGrid, grid, output);
    }

    private double EvaluateForMoveInLShape(bool aShowGrid, int[,] grid, char[,] output, bool aShowOutput)
    {
      const int kMazeSideLength = 21;
      char[,] mazePosition = new char[21, 21];

      char[,] lastOutput = new char[itsSideLength, itsSideLength];

      int gridMiddle = itsSideLength / 2;

      int x = 0;
      int y = 5;

      int maxX = x;
      int maxY = y;
      int xAtTurn = 0;

      int score = 0;
      int maximumPasses = 20;
      int pass = 0;
      for (; pass < maximumPasses; pass++)
      {
        EvaluateGrid(pass, grid, output, aShowGrid);

        // check that this pass resulted in a move
        // - don't reward for sitting still
        int currentY = y;
        int currentX = x;

        if( x > maxX)
        {
          maxX = x;
        }

        if (y > maxY)
        {
          maxY = y;
        }

        if (output[0, gridMiddle] == '1' && y > 0)
        {
          y--;
        }

        if (output[itsSideLength - 1, gridMiddle] == '1' && y < (kMazeSideLength - 1))
        {
          y++;
        }

        if (output[gridMiddle, 0] == '1' && x > 0)
        {
          x--;
        }

        if (output[gridMiddle, itsSideLength - 1] == '1' && x < (kMazeSideLength - 1))
        {
          x++;
        }


        if (x < 3 && y == currentY && x > maxX)
        {
          score += 10;
        }
        else if (x >= 3 && x == xAtTurn && y > currentY)
        {
          xAtTurn = x;
          score += y;
        }

        if (score < 0)
        {
          score = 0;
        }

        if (score > 100)
        {
          score = 100;
        }

        mazePosition[y, x] = 'x';


        // removed the check for changes, since this allows bots to move to the square and stop
        if (CompareWithLastOutput(output, lastOutput) == true)
        {
          // the new output is identical to the last output
          break;
        }

        CopyOutput(output, lastOutput);
      }

      if (aShowOutput)
      {
        ShowMaze(mazePosition, kMazeSideLength);

        Console.WriteLine("Score: " + score);
        Console.WriteLine("Passes: " + pass);
      }

      return ((double)score / (double)100);
    }


    private double EvaluateForMoveFastAndStop(bool aShowGrid, int[,] grid, char[,] output, bool aShowOutput)
    {
      const int kMazeSideLength = 21;
      char[,] mazePosition = new char[21, 21];

      char[,] lastOutput = new char[itsSideLength, itsSideLength];

      int gridMiddle = itsSideLength / 2;

      int x = 0;
      int y = 10;

      int score = 0;
      int maximumPasses = 20;
      int pass = 0;
      for (;pass < maximumPasses; pass++)
      {
        EvaluateGrid(pass, grid, output, aShowGrid);

        // check that this pass resulted in a move
        // - don't reward for sitting still
        int currentX = x;

        if (output[0, gridMiddle] == '1' && y > 0)
        {
          y--;
        }

        if (output[itsSideLength - 1, gridMiddle] == '1' && y < (kMazeSideLength - 1))
        {
          y++;
        }

        if (output[gridMiddle, 0] == '1' && x > 0)
        {
          x--;
        }

        if (output[gridMiddle, itsSideLength - 1] == '1' && x < (kMazeSideLength - 1))
        {
          x++;
        }


        // check that the horizontal position has changed or it has reached the middle
        // - also make sure it stays on the middle line
        if (y == 10 && (x != currentX || x == 5))
        {
          // increase the score while the bot is moving towards the middle of the maze
          if (x < 5)
          {
            score += x;
          }
          else if (x == 5)
          {
            score += 20;
          }
          else
          {
            // start decreasing the score when the bot has moved past the centre of the maze            
            score -= x;
          }

          if(score <0)
          {
            score = 0;
          }

          if( score > 100)
          {
            score = 100;
          }
        }

        mazePosition[y, x] = 'x';


        // removed the check for changes, since this allows bots to move to the square and stop
        if (CompareWithLastOutput(output, lastOutput) == true)
        {
          // the new output is identical to the last output
          break;
        }

        CopyOutput(output, lastOutput);
      }

      if (aShowOutput)
      {
        ShowMaze(mazePosition, kMazeSideLength);

        Console.WriteLine("Score: " + score);
        Console.WriteLine("Passes: " + pass);
      }

      return ((double)score / (double)100);
    }


    /// <summary>
    /// look for a bot that moves and then stops (or one that pauses before starting)
    /// </summary>
    /// <param name="aShowGrid"></param>
    /// <param name="grid"></param>
    /// <param name="output"></param>
    /// <param name="aShowOutput"></param>
    /// <returns></returns>
    private double EvaluateForMoveAndStop(bool aShowGrid, int[,] grid, char[,] output, bool aShowOutput)
    {
      const int kMazeSideLength = 21;
      char[,] mazePosition = new char[21, 21];

      char[,] lastOutput = new char[itsSideLength, itsSideLength];

      int gridMiddle = itsSideLength / 2;

      int x = 0;
      int y = 0;

      int score = 0;
      int maximumPasses = 20;
      for (int pass = 0; pass < maximumPasses; pass++)
      {
        EvaluateGrid(pass, grid, output, aShowGrid);

        // check that this pass resulted in a move
        // - don't reward for sitting still
        int currentX = x;

        if (output[0, gridMiddle] == '1' && y > 0)
        {
          y--;
        }

        if (output[itsSideLength - 1, gridMiddle] == '1' && y < (kMazeSideLength - 1))
        {
          y++;
        }

        if (output[gridMiddle, 0] == '1' && x > 0)
        {
          x--;
        }

        if (output[gridMiddle, itsSideLength - 1] == '1' && x < (kMazeSideLength - 1))
        {
          x++;
        }

        // check that the horizontal position has changed
        if (x != currentX)
        {
          // increase the score while the bot is moving towards the middle of the maze
          if (x <= 10)
          {
            if (score < 55)
            {
              score += x; // max score = 55
            }
          }
          else
          {
            // start decreasing the score when the bot has moved past the centre of the maze
            score -= (x - 10);
          }
        }

        mazePosition[y, x] = 'x';

        if (CompareWithLastOutput(output, lastOutput) == true)
        {
          // the new output is identical to the last output
          break;
        }

        CopyOutput(output, lastOutput);
      }

      if (aShowOutput)
      {
        ShowMaze(mazePosition, kMazeSideLength);
      }

      return ((double)score / (double)55);
    }


    /// <summary>
    /// reward bots that move diagonally across the maze
    /// </summary>
    /// <param name="aShowGrid"></param>
    /// <param name="grid"></param>
    /// <param name="output"></param>
    /// <param name="aShowOutput"></param>
    /// <returns></returns>
    private double EvaluateForDiagonalMove(bool aShowGrid, int[,] grid, char[,] output, bool aShowOutput)
    {
      const int kMazeSideLength = 21;
      char[,] mazePosition = new char[21, 21];

      char[,] lastOutput = new char[itsSideLength, itsSideLength];

      int gridMiddle = itsSideLength / 2;

      int x = 0;
      int y = 0;

      int maximumPasses = 20;
      for (int pass = 0; pass < maximumPasses; pass++)
      {
        EvaluateGrid(pass, grid, output, aShowGrid);

        if (output[0, gridMiddle] == '1' && y > 0)
        {
          y--;
        }

        if (output[itsSideLength - 1, gridMiddle] == '1' && y < (kMazeSideLength - 1))
        {
          y++;
        }

        if (output[gridMiddle, 0] == '1' && x > 0)
        {
          x--;
        }

        if (output[gridMiddle, itsSideLength - 1] == '1' && x < (kMazeSideLength - 1))
        {
          x++;
        }

        mazePosition[y, x] = 'x';

        if (CompareWithLastOutput(output, lastOutput) == true)
        {
          // the new output is identical to the last output
          break;
        }

        CopyOutput(output, lastOutput);
      }

      if (aShowOutput)
      {
        ShowMaze(mazePosition, kMazeSideLength);
      }

      return ((double)(x+y) / (double)(maximumPasses*2));
    }


    /// <summary>
    /// reward bots that move in a horizontal straight line
    /// - did it in population of 10 and 10 generations
    /// </summary>
    /// <param name="aShowGrid"></param>
    /// <param name="grid"></param>
    /// <param name="output"></param>
    /// <param name="aShowOutput"></param>
    /// <returns></returns>
    private double EvaluateForStraightLineMove(bool aShowGrid, int[,] grid, char[,] output, bool aShowOutput)
    {
      const int kMazeSideLength = 21;
      char[,] mazePosition = new char[21, 21];

      char[,] lastOutput = new char[itsSideLength, itsSideLength];

      int gridMiddle = itsSideLength / 2;

      int x = 0;
      int y = 0;

      int maximumPasses = 20;
      for (int pass = 0; pass < maximumPasses; pass++)
      {
        EvaluateGrid(pass, grid, output, aShowGrid);

        if (output[0, gridMiddle] == '1' && y > 0)
        {
          y--;
        }

        if (output[itsSideLength - 1, gridMiddle] == '1' && y < (kMazeSideLength - 1))
        {
          y++;
        }

        if (output[gridMiddle, 0] == '1' && x > 0)
        {
          x--;
        }

        if (output[gridMiddle, itsSideLength - 1] == '1' && x < (kMazeSideLength - 1))
        {
          x++;
        }

        mazePosition[y,x] = 'x';

        if (CompareWithLastOutput(output, lastOutput) == true)
        {
          // the new output is identical to the last output
          break;
        }

        CopyOutput(output, lastOutput);
      }

      if (aShowOutput)
      {
        ShowMaze(mazePosition, kMazeSideLength);
      }

      return ((double)x / (double)maximumPasses);

    }

    private void ShowMaze(char[,] mazePosition, int kMazeSideLength)
    {
      for (int row = 0; row < kMazeSideLength; row++)
      {
        Console.Write("   ");

        for (int col = 0; col < kMazeSideLength; col++)
        {
          Console.Write(mazePosition[row, col] + " ");
        }
        Console.WriteLine();
      }

      Console.WriteLine("__________________________________");     
    }


    /// <summary>
    /// Test if the grid is changing over a certain number of iterations
    /// </summary>
    /// <param name="aShowGrid"></param>
    /// <param name="grid"></param>
    /// <param name="lastOutput"></param>
    /// <param name="output"></param>
    /// <returns></returns>
    private double EvaluateForChange(bool aShowGrid, int[,] grid, char[,] output)
    {
      char[,] lastOutput = new char[itsSideLength, itsSideLength];

      int pass = 0;
      int maximumPasses = 20;
      for (; pass < maximumPasses; pass++)
      {
        EvaluateGrid(pass, grid, output, aShowGrid);

        if (CompareWithLastOutput(output, lastOutput) == true)
        {
          // the new output is identical to the last output
          break;
        }

        CopyOutput(output, lastOutput);
      }

      return ((double)pass / (double)maximumPasses);
    }

    private int[,] CreateGrid(Chromosome chromosome, bool aShowGrid)
    {
      if (aShowGrid)
      {
        Console.WriteLine("Chromosome: " + chromosome.ToBinaryString());
      }

      int[,] grid = new int[itsSideLength, itsSideLength];

      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          // each gene is made up from 4-bits
          int geneValue = 0;
          for( int bit = 0; bit < itsGeneLength; bit++)
          {
            geneValue += Convert.ToInt32(chromosome.ToBinaryString((row * itsSideLength) + col + bit, 1)) * (1 << bit);
          }
          
          grid[row, col] = geneValue;
        }
      }
      
      // remove cells with no connections
      PruneGrid(grid);

      if (aShowGrid)
      {
        ShowGrid(grid);
      }

      return grid;
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
    private void PruneGrid(int[,] grid)
    {
      int[,] lastGrid = new int[itsSideLength, itsSideLength];

      do
      {
        //ShowGrid(grid);
        CopyGrid(grid, lastGrid);

        RemoveNodesWithNoOutput(grid);
        RemoveNodesWithNoInput(grid);
      }
      while(CompareWithLastGrid(grid, lastGrid) == false);
    }

    private void RemoveNodesWithNoInput(int[,] grid)
    {
      // remove nodes with no input
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          // test for input from above    
          bool above = (row > 0
           && (grid[row - 1, col] == 1
           || grid[row - 1, col] == 4
           || grid[row - 1, col] == 5
           || grid[row - 1, col] == 6
           || grid[row - 1, col] == 8
           || grid[row - 1, col] == 9
           || grid[row - 1, col] == 11
           || grid[row - 1, col] == 14));

          // test for input from below
          bool below = (row < (itsSideLength - 1)
           && (grid[row + 1, col] == 1
           || grid[row + 1, col] == 3
           || grid[row + 1, col] == 5
           || grid[row + 1, col] == 6
           || grid[row + 1, col] == 7
           || grid[row + 1, col] == 10
           || grid[row + 1, col] == 11
           || grid[row + 1, col] == 14));

          // test for input from left
          bool left = (col > 0
           && (grid[row, col - 1] == 2
           || grid[row, col - 1] == 3
           || grid[row, col - 1] == 4
           || grid[row, col - 1] == 5
           || grid[row, col - 1] == 7
           || grid[row, col - 1] == 8
           || grid[row, col - 1] == 11
           || grid[row, col - 1] == 13));

          // test for input from right
          bool right = (col < (itsSideLength - 1)
           && (grid[row, col + 1] == 2
           || grid[row, col + 1] == 3
           || grid[row, col + 1] == 4
           || grid[row, col + 1] == 6
           || grid[row, col + 1] == 9
           || grid[row, col + 1] == 10
           || grid[row, col + 1] == 11
           || grid[row, col + 1] == 15));

          if (!above && !below && !left && !right)
          {
            grid[row, col] = 0;
          }

          // vertical '|'
          if (grid[row, col] == 1 && !above && !below)
          {
            grid[row, col] = 0;
          }

          // horizontal '-'
          if (grid[row, col] == 2 && !left && !right)
          {
            grid[row, col] = 0;
          }


          ///   case 3:  Console.Write("┴");                            
          if (grid[row, col] == 3 && !left && !above && !right)
          {
            grid[row, col] = 0;
          }

          ///   case 4:  Console.Write("┬");
          if (grid[row, col] == 4 && !left && !below && !right)
          {
            grid[row, col] = 0;
          }

          ///   case 5:  Console.Write("├");
          if (grid[row, col] == 5 && !above && !right && !below)
          {
            grid[row, col] = 0;
          }

          ///   case 6:  Console.Write("┤");
          if (grid[row, col] == 6 && !below && !left && !above)
          {
            grid[row, col] = 0;
          }


          ///   case 7:  Console.Write("└");                              
          if (grid[row, col] == 7 && !above && !right)
          {
            grid[row, col] = 0;
          }

          ///   case 8:  Console.Write("┌");
          if (grid[row, col] == 8 && !below && !right)
          {
            grid[row, col] = 0;
          }

          ///   case 9:  Console.Write("┐");
          if (grid[row, col] == 9 && !left && !below)
          {
            grid[row, col] = 0;
          }

          ///   case 10: Console.Write("┘");
          if (grid[row, col] == 10 && !left && !above)
          {
            grid[row, col] = 0;
          }
        }
      }
    }

    private void RemoveNodesWithNoOutput(int[,] grid)
    {
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          // remove any nodes that point into an empty cell
          if (grid[row, col] == 0)
          {
            // examine cell above
            if (row > 0 
            &&(grid[row - 1, col] == 1
            || grid[row - 1, col] == 8
            || grid[row - 1, col] == 9
            || grid[row - 1, col] == 14 ))
            {
              grid[row - 1, col] = 0;
            }

            // examine cell below
            if (row < (itsSideLength - 1) 
            && (grid[row + 1, col] == 1
              ||grid[row + 1, col] == 7
              ||grid[row + 1, col] == 10
              ||grid[row + 1, col] == 12))
            {
              grid[row + 1, col] = 0;
            }

            // examine cell to left
            if (col > 0 
            && (grid[row, col - 1] == 2
            || grid[row, col - 1] == 7
            || grid[row, col - 1] == 8
            || grid[row, col - 1] == 13))
            {
              grid[row, col - 1] = 0;
            }

            // examine cell to right
            if (col < (itsSideLength - 1) 
            && (grid[row, col + 1] == 2
            || grid[row, col + 1] == 9
            || grid[row, col + 1] == 10
            || grid[row, col + 1] == 15))
            {
              grid[row, col + 1] = 0;
            }
          }


          //
          // remove nodes with outputs into each other

          //12: Console.Write("↑");
          //13: Console.Write("→");
          //14: Console.Write("↓");
          //15: Console.Write("←");

          // vertical pair
          if( grid[row,col] == 12 && (row > 0 && grid[row-1,col] == 14))
          {
            grid[row,col] = 0;
          }

          // horizontal pair
          if (grid[row, col] == 15 && (col > 0 && grid[row, col-1] == 13))
          {
            grid[row, col] = 0;
          }
        }
      }
    }



    /// <summary>
    /// copy the output to the last output
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    private void CopyOutput(char[,] output, char[,] lastOutput)
    {
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          lastOutput[row, col] = output[row, col];
        }
      }
    }

    /// <summary>
    /// test if the two supplied outputs are the same
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    /// <returns>true if the grids are identical</returns>
    private bool CompareWithLastOutput(char[,] output, char[,] lastOutput)
    {
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          if( output[row,col] != lastOutput[row,col])
          {
            return false;
          }
        }
      }

      return true;
    }


    /// <summary>
    /// copy the grid to the last grid
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    private void CopyGrid(int[,] grid, int[,] lastGrid)
    {
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          lastGrid[row, col] = grid[row, col];
        }
      }
    }


    /// <summary>
    /// test if the two supplied grids are the same
    /// </summary>
    /// <param name="output"></param>
    /// <param name="lastOutput"></param>
    /// <returns>true if the grids are identical</returns>
    private bool CompareWithLastGrid(int[,] grid, int[,] lastGrid)
    {
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          if (grid[row, col] != lastGrid[row, col])
          {
            return false;
          }
        }
      }

      return true;
    }


    private char[,] InitializeOutput(int[,] grid)
    {
      char[,] output = new char[itsSideLength, itsSideLength];

      // initially set all connections to zero and all other cells blank
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          if (grid[row, col] > 0)
          {
            output[row, col] = '0';
          }
          else
          {
            output[row, col] = '-';
          }
        }
      }
      return output;
    }


    /// <summary>
    ///  NAND gate output = 1, unless all 3 inputs are 1
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="output"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private bool CellValue(int[,] grid, char[,] output, int row, int col)
    {
      bool cellValue = true;

      switch( grid[row,col] )
      {
        case 12:  // "↑"
          cellValue &= ValueOfLeftCell (grid, output, row, col);
          cellValue &= ValueOfRightCell(grid, output, row, col);
          cellValue &= ValueOfCellBelow(grid, output, row, col);          
          break;     

        case 13:  // "→"    
          cellValue &= ValueOfCellAbove( grid, output, row, col);
          cellValue &=  ValueOfLeftCell( grid, output, row, col);
          cellValue &= ValueOfCellBelow( grid, output, row, col); 
          break;     

        case 14:  // "↓"
          cellValue &= ValueOfLeftCell ( grid, output, row, col);
          cellValue &= ValueOfRightCell( grid, output, row, col);
          cellValue &= ValueOfCellAbove( grid, output, row, col); 
          break;

        case 15:  // "←"
          cellValue &= ValueOfCellAbove( grid, output, row, col);
          cellValue &= ValueOfRightCell( grid, output, row, col);
          cellValue &= ValueOfCellBelow( grid, output, row, col); 
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

    private bool ValueOfCellAbove(int[,] grid, char[,] output, int aRow, int aCol)
    {
      // value of cell above
      if (aRow > 0 )
      {
        int row = aRow - 1;
        int col = aCol;

        if (grid[row, col] == 1
        || grid[row, col] == 4
        || grid[row, col] == 5
        || grid[row, col] == 6
        || grid[row, col] == 8
        || grid[row, col] == 9
        || grid[row, col] == 11
        || grid[row, col] == 14)
        {
          if (output[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }

    private bool ValueOfCellBelow(int[,] grid, char[,] output, int aRow, int aCol)
    {
      // value of cell below
      if (aRow < (itsSideLength - 1))
      {
        int row = aRow + 1;
        int col = aCol;

        if (grid[row, col] == 1
        || grid[row, col] == 3
        || grid[row, col] == 5
        || grid[row, col] == 6
        || grid[row, col] == 7
        || grid[row, col] == 10
        || grid[row, col] == 11
        || grid[row, col] == 12)
        {
          if (output[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }


    private bool ValueOfRightCell(int[,] grid, char[,] output, int aRow, int aCol)
    {
      // value of right cell
      if (aCol < (itsSideLength - 1))
      {
        int row = aRow;
        int col = aCol + 1;

        if (grid[row, col] == 2
        || grid[row, col] == 3
        || grid[row, col] == 4
        || grid[row, col] == 6
        || grid[row, col] == 9
        || grid[row, col] == 10
        || grid[row, col] == 11
        || grid[row, col] == 15)
        {
          if (output[row, col] == '0')
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
    private bool ValueOfLeftCell(int[,] grid, char[,] output, int aRow, int aCol)
    {
      // value of left cell
      if (aCol > 0  )
      {
        int row = aRow;
        int col = aCol - 1;

        if(grid[row, col] == 2
        || grid[row, col] == 3
        || grid[row, col] == 4
        || grid[row, col] == 5
        || grid[row, col] == 7
        || grid[row, col] == 8
        || grid[row, col] == 11
        || grid[row, col] == 13)
        {
          if( output[row, col] == '0')
          {
            return false;
          }
        }
      }
      return true;
    }


    /// <summary>
    /// Do a single pass of the grid to evaluate the value of each cell
    /// </summary>
    /// <param name="grid"></param>
    private void EvaluateGrid(int aPassNumber, int[,] grid, char[,] output,bool aShowOutput)
    {      
      // create a new grid for output and reset all its cells to '-'
      char[,] newOutput = new char[itsSideLength, itsSideLength];
      for (int row = 0; row < itsSideLength; row++)
      {        
        for (int col = 0; col < itsSideLength; col++)
        {
          newOutput[row,col] = '-';
        }
      }
      
      // set the gate outputs according to the current outputs
      //
      // case 12: Console.Write("↑"); break;
      // case 13: Console.Write("→"); break;
      // case 14: Console.Write("↓"); break;
      // case 15: Console.Write("←"); break;     
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          if (grid[row, col] >= 12 && grid[row, col] <= 15)
          {
            newOutput[row, col] = (CellValue(grid, output, row, col) ? '1' : '0');

            switch (grid[row, col])
            {
              // case 12: Console.Write("↑"); break;
              case 12:
                SetCellAbove(grid, newOutput, row, col, newOutput[row, col], direction.below);
                break;

              // case 13: Console.Write("→"); break;
              case 13:
                SetCellRight(grid, newOutput, row, col, newOutput[row, col], direction.left);
                break;

              // case 14: Console.Write("↓"); break;
              case 14:
                SetCellBelow(grid, newOutput, row, col, newOutput[row, col], direction.above);
                break;

              // case 15: Console.Write("←"); break;
              case 15:
                SetCellLeft(grid, newOutput, row, col, newOutput[row, col], direction.right);
                break;
            }
          }
        }
      }

      // maintain the state of any cells that haven't changed
      for (int row = 0; row < itsSideLength; row++)
      {
        for (int col = 0; col < itsSideLength; col++)
        {
          if( newOutput[row, col] != '-' )
          {
            output[row, col] = newOutput[row, col];
          }
        }
      }

      if (aShowOutput)
      {
        ShowOutput(aPassNumber, output);
      }
    }

    enum direction { above, left, right, below };

    private void SetPath(int[,] grid, char[,] newOutput, int row, int col, char aValue, direction aDirection)
    {
      // if the cell has already been set to '1' don't change its value
      if (grid[row, col] > 0 && grid[row, col] < 12 && newOutput[row, col] != '1' && !(newOutput[row, col] == '0' && aValue == '0'))
      {
        // set the cell value
        newOutput[row, col] = aValue;

        switch (grid[row, col])
        {
          ///   // straight connections
          ///   case 1:  Console.Write("|");
          case 1:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 2:  Console.Write("─");
          case 2:
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   // T connections
          ///   case 3:  Console.Write("┴");
          case 3:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 4:  Console.Write("┬");
          case 4:
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 5:  Console.Write("├");
          case 5:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 6:  Console.Write("┤");
          case 6:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   // L connections 
          ///   case 7:  Console.Write("└");
          case 7:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 8:  Console.Write("┌");
          case 8:
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 9:  Console.Write("┐");
          case 9:
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   case 10: Console.Write("┘");
          case 10:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            break;

          ///   // full connection
          ///   case 11: Console.Write("┼");
          case 11:
            SetCellAbove(grid, newOutput, row, col, aValue, aDirection);
            SetCellBelow(grid, newOutput, row, col, aValue, aDirection);
            SetCellLeft(grid, newOutput, row, col, aValue, aDirection);
            SetCellRight(grid, newOutput, row, col, aValue, aDirection);
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


    private void SetCellRight(int[,] grid, char[,] newOutput, int row, int col, char aValue, direction aDirection)
    {
      if (aDirection != direction.right && col < (itsSideLength - 1)
       && (grid[row, col+1] == 2
        || grid[row, col+1] == 3
        || grid[row, col+1] == 4
        || grid[row, col+1] == 6
        || grid[row, col+1] == 9
        || grid[row, col+1] == 10
        || grid[row, col+1] == 11))
      {
        SetPath(grid, newOutput, row, col + 1, aValue, direction.left);
      }
    }

    private void SetCellLeft(int[,] grid, char[,] newOutput, int row, int col, char aValue, direction aDirection)
    {
      if (aDirection != direction.left && col > 0
       && (grid[row, col-1] == 2
        || grid[row, col-1] == 3
        || grid[row, col-1] == 4
        || grid[row, col-1] == 5
        || grid[row, col-1] == 7
        || grid[row, col-1] == 8
        || grid[row, col-1] == 11))
      {
        SetPath(grid, newOutput, row, col - 1, aValue, direction.right);
      }
    }

    private void SetCellBelow(int[,] grid, char[,] newOutput, int row, int col, char aValue, direction aDirection)
    {
      if (aDirection != direction.below && row < (itsSideLength - 1)
       && (grid[row+1, col] == 1
        || grid[row+1, col] == 3
        || grid[row+1, col] == 5
        || grid[row+1, col] == 6
        || grid[row+1, col] == 7
        || grid[row+1, col] == 10
        || grid[row+1, col] == 11))
      {
        SetPath(grid, newOutput, row + 1, col, aValue, direction.above);
      }
    }

    private void SetCellAbove(int[,] grid, char[,] newOutput, int row, int col, char aValue, direction aDirection)
    {
      if (aDirection != direction.above && row > 0 
       && (grid[row-1,col] == 1
        || grid[row-1,col] == 4
        || grid[row-1,col] == 5
        || grid[row-1,col] == 6
        || grid[row-1,col] == 8
        || grid[row-1,col] == 9
        || grid[row-1,col] == 11))
      {
        SetPath(grid, newOutput, row - 1, col, aValue, direction.below);
      }
    }

    private void ShowOutput(int aPassNumber, char[,] output)
    {
      Console.WriteLine(aPassNumber + ".");

      for (int row = 0; row < itsSideLength; row++)
      {
        Console.Write("   ");

        for (int col = 0; col < itsSideLength; col++)
        {
          Console.Write(output[row, col] + " ");
        }
        Console.WriteLine();
      }

      Console.WriteLine("____________");
    }

    private void ShowGrid(int[,] grid)
    {
      Console.WriteLine();

      for (int row = 0; row < itsSideLength; row++)
      {
        Console.Write("   ");

        for (int col = 0; col < itsSideLength; col++)
        {          
          switch( grid[row,col])
          {
            // empty cell
            case 0:  Console.Write(" "); break;
            
            // straight connections
            case 1:  Console.Write("|"); break;
            case 2:  Console.Write("─"); break; 

            // T connections
            case 3:  Console.Write("┴"); break; 
            case 4:  Console.Write("┬"); break; 
            case 5:  Console.Write("├"); break; 
            case 6:  Console.Write("┤"); break;

            // L connections 
            case 7:  Console.Write("└"); break;
            case 8:  Console.Write("┌"); break;
            case 9:  Console.Write("┐"); break;
            case 10: Console.Write("┘"); break; 
            
            // full connection
            case 11: Console.Write("┼"); break;

            // nodes
            case 12: Console.Write("↑"); break;
            case 13: Console.Write("→"); break;
            case 14: Console.Write("↓"); break;
            case 15: Console.Write("←"); break;
          }

          Console.Write(" ");
        }
        Console.WriteLine();
      }
      Console.WriteLine("____________");
    }


  }
}
