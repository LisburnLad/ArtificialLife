using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Operators;
using System.IO;
using System.Diagnostics;

namespace ArtificialLife
{
  public class Evolution
  {
    public int itsPopulationSize { get; set; }
    public int itsMaxEvaluationCount { get; set; }
    public int itsSideLength { get; set; }
    public double itsFitness { get; set; }
    public int itsBotType { get; set; }

    public bool itsShowOnTerminate { get; set; }

    public Evolution(int aGridSideLength, int aPopulationSize, int aGenerations, int aBotType)
    {
      itsSideLength = aGridSideLength;
      itsPopulationSize = aPopulationSize;
      itsMaxEvaluationCount = aGenerations;
      itsBotType = aBotType;
    }
    
    public void StartEvolution()
    {
      const double crossoverProbability = 0.75;
      const double mutationProbability = 0.10;
      const int elitismPercentage = 5;

      int chromosomeLength = Bot.GetChromosomeLength(itsSideLength,itsBotType);

      //create a Population of "itsPopulationSize" random chromosomes each of length "chromosomeLength"
      var population = new Population(itsPopulationSize, chromosomeLength, false, false);

      //create the genetic operators 
      var elite = new Elite( elitismPercentage );

      var crossover = new Crossover( crossoverProbability, true )
      {
        //CrossoverType = CrossoverType.DoublePoint
        CrossoverType = CrossoverType.SinglePoint
      };

      var mutation = new BinaryMutate( mutationProbability, true );

      //create the GA itself 
      var ga = new GeneticAlgorithm( population, EvaluateChromosome );

      //subscribe to the GAs Generation Complete event 
      ga.OnGenerationComplete += ga_OnGenerationComplete;

      //add the operators to the ga process pipeline 
      ga.Operators.Add( elite );
      ga.Operators.Add( crossover );
      ga.Operators.Add( mutation );

      //run the GA 
      ga.Run( TerminateFunction );
    }


    private bool TerminateFunction( Population population, int currentGeneration, long currentEvaluation )
    {
      //example termination criterion 
      return (currentEvaluation >= itsMaxEvaluationCount);
    }

    private void ga_OnGenerationComplete( object sender, GaEventArgs e )
    {
      //get the best solution 
      var chromosome = e.Population.GetTop( 1 )[0];

      if(itsShowOnTerminate)
      {
        //decode chromosome
        double fitness = CreateAndEvaluateBot( chromosome, itsSideLength, itsBotType, true );

        Console.WriteLine( "Final Chromosome Fitmess: " + fitness );
        Console.WriteLine( "Evaluations: " + e.Evaluations );

        Console.WriteLine( "Chromosome: " + chromosome.ToBinaryString() );
      }

      itsFitness = chromosome.Fitness;

      Console.WriteLine( "Final Chromosome Fitmess: " + itsFitness );
    }

    /// <summary>
    /// Create the bot and evaluate its performance without any output
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double EvaluateChromosome( Chromosome chromosome )
    {
      double score = CreateAndEvaluateBot(chromosome, itsSideLength, itsBotType, false);
      return score;
    }


    #region UnitTestCopies

    static string[] twoBits = new string[4] { "00", "01", "10", "11" };

    public void TargetedConnectionWithRightOrLeftPlacedTargets()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00010" + // north input  - B
                     "00010" + // east input   - B
                     "00010" + // south input  - B
                     "00010" + // west input   - B

                     "00000" + "00000" + "00000" + "00000" + // A - 00001
                     "00000" + "00000" + "00011" + "00000" + // B - 00010 - X-X-C                               
                     "00000" + "00100" + "00000" + "00001" + // C - 00011 - X-D-X - local forward connection
                     "11111" + "00000" + "00000" + "00001" + // D - 00100 - O-X-X - left targeted connection  
                     "00000" + "00000" + "00000" + "00000" + // E - 00101    
                     "00000" + "00000" + "00000" + "00000" + // F - 00110 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 - delay
                     "00000" + "00000" + "00000" + "00100" + // N - 01110    

                     "00000" + "00000" + "00000" + "00000" + //15 - 01111
                     "00000" + "00000" + "00000" + "00000" + //16 - 10000     

                     "00000" + "00000" + "00000" + "00000" + //17 - 10001
                     "00000" + "00000" + "00000" + "00000" + //18 - 10010
                     "00000" + "00000" + "00000" + "00000" + //19 - 10011
                     "00000" + "00000" + "00000" + "00000" + //20 - 10100

                     "00000" + "00000" + "00000" + "00000" + //21 - 10101
                     "00000" + "00000" + "00000" + "00000" + //22 - 10110
                     "00000" + "00000" + "00000" + "00000" + //23 - 10111
                     "00000" + "00000" + "00000" + "00000" + //24 - 11000

                     "00000" + "00000" + "00000" + "00000" + //25 - 11001
                     "00000" + "00000" + "00000" + "00000" + //26 - 11010
                     "00000" + "00000" + "00000" + "00000" + //27 - 11011
                     "00000" + "00000" + "00000" + "00000" + //28 - 11100

                     "00000" + "00000" + "00000" + "00000" + //29 - 11101
                     "00000" + "00000" + "00000" + "00000";  //30 - 11110                                         

      string target = "01110"; // O - 1111 - targeted connection - target = N      
      string padding = "0000";
      string type = "10010";  // o - targeted connection 


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("RightOrLeftPlacedTargets");


      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();


      // test the 4 different connection distances
      for (int distance = 0; distance < 4; distance++)
      {
        // test for the 4 different target areas
        for (int area = 0; area < 4; area++)
        {
          // test for the 4 different connection directions
          for (int direction = 1; direction < 4; direction++)
          {
            string chromosomeString = rules + target + twoBits[distance] + twoBits[area] + twoBits[direction] + padding + type;
            Chromosome chromosome = new Chromosome(chromosomeString);

            string chromostring = chromosome.ToBinaryString();

            if (direction == 1)
            {
              // place cells to the right when testing right connection direction
              for (int placedCol = 0; placedCol < 8; placedCol++)
              {
                PlaceCellsAndTest(gridSideLength, testDirectory, distance, area, direction, chromosome, placedCol);


                // Format and display the TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                Console.WriteLine("RunTime " + elapsedTime);
              }
            }
            else
              if (direction == 3)
              {
                // place cells to the left when testing left connection direction
                for (int placedCol = 0; placedCol < 4; placedCol++)
                {
                  PlaceCellsAndTest(gridSideLength, testDirectory, distance, area, direction, chromosome, placedCol);

                  // Format and display the TimeSpan value.
                  TimeSpan ts = stopWatch.Elapsed;
                  string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                  Console.WriteLine("RunTime " + elapsedTime);
                }
              }
          }
        }
      }


      stopWatch.Stop();
    }


    private static void PlaceCellsAndTest(int gridSideLength, string testDirectory, int distance, int area, int direction, Chromosome chromosome, int placedCol)
    {
      // put target nodes into the cells on a line to the right of the start of the target connection
      for (int placedRow = 3; placedRow < 14; placedRow++)
      {
        PlaceAndTest(gridSideLength, testDirectory, distance, area, direction, chromosome, placedCol, placedRow);
      }
    }

    private static void PlaceAndTest(int gridSideLength, string testDirectory, int distance, int area, int direction, Chromosome chromosome, int placedCol, int placedRow)
    {
      // move the target cell to the new row
      CellDefinition[] placedTargets = null;

      if (direction == 1)
      {
        // targets for left hand connections
        placedTargets = new CellDefinition[]
          {
            new CellDefinition(placedRow,6+placedCol,CellType.NorthDelay),  // connection from top
            new CellDefinition(8-placedCol,placedRow,CellType.EastDelay), // connection from left
            new CellDefinition((14-placedRow),8-placedCol,CellType.SouthDelay),  // connection from bottom
            new CellDefinition(6+placedCol,(14-placedRow),CellType.WestDelay), // connection from right
          };
      }
      else
        if (direction == 3)
        {
          // targets for right hand connections
          placedTargets = new CellDefinition[]
          {
            new CellDefinition(placedRow,4-placedCol,CellType.NorthDelay),  // connection from top
            new CellDefinition(10+placedCol,placedRow,CellType.EastDelay), // connection from left
            new CellDefinition((14-placedRow),10+placedCol,CellType.SouthDelay),  // connection from bottom
            new CellDefinition(4-placedCol,(14-placedRow),CellType.WestDelay), // connection from right
          };
        }

      // with placed cells don't perform any pruning, since the placed cells are unconnected and would be removed
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false, Pruning.None, placedTargets);
      CellType[,] grid = bot.GetGrid();

      StringBuilder sb = new StringBuilder("Processing: ");
      sb.AppendFormat("row = {0}, col = {1}", placedRow, placedCol);

      if (direction == 1)
      {
        // exclude the positions where the target nodes would be placed on top of input or output nodes
        if (!(placedRow == 6 && placedCol >= 1 && placedCol <= 2)
          && !(placedRow == 7 && placedCol >= 0 && placedCol <= 3)
          && !(placedRow == 8 && placedCol >= 1 && placedCol <= 2)
          && !(placedRow == 13 && placedCol >= 1 && placedCol <= 4)
          && !(placedCol == 7 && placedRow >= 5 && placedRow <= 7))
        {
          if ((distance == 0 && placedRow < 5)                    // short connections
            || (distance == 1 && placedRow >= 5 && placedRow < 8)  // medium-short connections
            || (distance == 2 && placedRow >= 8 && placedRow < 11) // medium-long connections
            || (distance == 3 && placedRow >= 11))                 // long connections
          {
            if ((area == 0 && placedCol < 2)
              || (area == 1 && placedCol >= 2 && placedCol < 4)
              || (area == 2 && placedCol >= 4 && placedCol < 6)
              || (area == 3 && placedCol >= 6))
            {
              //bot.SaveGrid(testDirectory + "\\grid_" + twoBits[distance] + "_" + twoBits[area] + "_" + twoBits[direction] + "_row" + placedRow + "_col" + placedCol + ".bmp");
              //CheckConnectionEndsRight(placedCol, placedRow, grid, sb);
            }
          }
        }
      }
      else
        // right-hand connection
        if (direction == 3)
        {
          // excluded position - when target cell placed on top of input cell preventing growth)
          if (!((placedRow >= 7 && placedRow <= 9) && placedCol == 3))
          {
            if ((distance == 0 && placedRow < 5)                     // short connections
              || (distance == 1 && placedRow >= 5 && placedRow < 8)  // medium-short connections
              || (distance == 2 && placedRow >= 8 && placedRow < 11) // medium-long connections
              || (distance == 3 && placedRow >= 11))                 // long connections
            {
              if (placedCol == area)
              {
                //bot.SaveGrid(testDirectory + "\\grid_" + twoBits[distance] + "_" + twoBits[area] + "_" + twoBits[direction] + "_row" + placedRow + "_col" + placedCol + ".bmp");
                //CheckConnectionEndsLeft(placedRow, placedCol, grid, sb);
              }
            }
          }
        }
    }

    //private static void CheckConnectionEndsLeft(int placedRow, int placedCol, CellType[,] grid, StringBuilder sb)
    //{
    //  // the connection end just join with the placed node in the first rows of the grid
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[placedRow, 5 - placedCol], sb.ToString());
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[9 + placedCol, placedRow], sb.ToString());
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[5 - placedCol, 14 - placedRow], sb.ToString());
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[14 - placedRow, 9 + placedCol], sb.ToString());
    //}

    //private static void CheckConnectionEndsRight(int placedCol, int placedRow, CellType[,] grid, StringBuilder sb)
    //{
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[placedRow, 5 + placedCol], sb.ToString());
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[9 - placedCol, placedRow], sb.ToString());
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[5 + placedCol, 14 - placedRow], sb.ToString());
    //  Assert.AreEqual(CellType.ConnectionEnd, grid[14 - placedRow, 9 - placedCol], sb.ToString());
    //}

    #endregion UnitTestCopies





    /// <summary>
    /// function used for testing of a specific chromosome
    /// </summary>
    public void EvaluateSpecificChromosome()
    {
      //string rules = "0000" + // north input  - A
      //               "0000" + // east input   - A
      //               "0000" + // south input  - A
      //               "0001" + // west input   - A
      //               "0000" + "0010" + "0000" + // A - 0001 - x-B-x 
      //               "0000" + "0011" + "0000" + // B - 0010 - x-C-x                                
      //               "0000" + "0100" + "0101" + // C - 0011 - x-D-E    
      //               "0000" + "0100" + "0000" + // D - 0100 - x-D-x    
      //               "0000" + "0000" + "0101" + // E - 0101 - x-x-E   
      //               "0000" + "0000" + "0000" + // F - 0110 
      //               "0000" + "0000" + "0000" + // G - 0111 
      //               "0000" + "0000" + "0000" + // H - 1000 
      //               "0000" + "0000" + "0000" + // I - 1001 
      //               "0000" + "0000" + "0000" + // J - 1010 
      //               "0000" + "0000" + "0000" + // K - 1011 
      //               "0000" + "0000" + "0000" + // L - 1100 
      //               "0000" + "0000" + "0000" + // M - 1101 
      //               "0000" + "0000" + "0000" + // N - 1110 
      //               "0000" + "0000" + "0000";  // O - 1111 

      //string nodeTypes = "001" + // a - local connection
      //                   "001" + // b - local connection
      //                   "011" + // c - Nand 
      //                   "001" + // d - local connection
      //                   "001" + // e - local connection 
      //                   "000" + // f
      //                   "000" + // g 
      //                   "000" + // h 
      //                   "000" + // i 
      //                   "000" + // j 
      //                   "000" + // k 
      //                   "000" + // l 
      //                   "000" + // m 
      //                   "000" + // n
      //                   "000";

      //string specificChromosome = "100101111100110010110010001110011111011010100101101101101011100001101111010110001111001010001100100000111100101000010101110011100100010100100101011001000" +
      //                            "0101000101010001011010111010001111101001110000111100110011100011100011000011100010011010";

      //string specificChromosome = "11101101010010100101111001110010100000110111111000010010011010000001010100110000110010110001010100111001001111000000000100000100110100110111100010000" +
      //                            "01011101001100110100111010000000001000111000011111100110001100100001001000100011001000010001";

      // chromosome to solve "TestForMoveAndStop"
      //string specificChromosome = "110000101101100101010111010111010011100001010101011111110010001100011101100000011000011101101000010000010101011001111011011100101001100011101001001000111" +
      //                            "1111110111001010001101101010101001001110110110101110001110111000010101111001100100010101";

string specificChromosome2 = "000100000100100001010110110010011100000111001100010101100010001000101010111110011011001111100110100100000000011010000101110110110010011111100011001000000" +
                            "010001101011100010011100010110101011010001110100011010010011010010000000001101100001010100011110000110101111000111001110100010010110010000011111000001110001010000111" +
                            "011000101000001000001101011101100011010111010001111000000010111001010110110010001111110101001010100000011000010011011001111100101001100100110110001000001100110010011" +
                            "1001001111010001011001011011010011110101000001010100111101111011011111011111101101000110010110011010101100101110101100110111010110110001010101100011001001001";

string specificChromosome3 = "010001010101100001000111111010000010111001001101101010011111001000000001100010010100100110000001001001110011010111000000011100010000011110011111001111111" +
"101001011001111011101101101111101111101001101011100110011111000111000000000111001010110001111101111001110111001111011101011000101100001011011100110011011111001101111" +
"110100110000000100001010010011001000011000110011011000010100100110100111100001000010100110000000001011111100110011110011110101001000111110011010001101100011110011011" +
"1000000100101100111000100011001111011101000011100111010111000011000010101001000011101100000100011011101100001010011110011110011100011110011000101101100010010";

string specificChromosome = "00010000100001000010000000000000000000000000000000000110000000000001000000000001111110000000000000010000000000000000000000000000000000000000000000" +
                            "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" + 
                            "00000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" + 
                            "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                            "00000000000000000000000000000000000001110000001000010010";


      //Chromosome chromosome = new Chromosome(rules + nodeTypes);
      Chromosome chromosome = new Chromosome(specificChromosome);
      double score = CreateAndEvaluateBot(  chromosome, true );

      Console.WriteLine("Bot Score: " + score);
    }

    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double CreateAndEvaluateBot( Chromosome chromosome, bool aShowGrid )
    {
      // create the test that the bot is to perform
      //Test currentTest = new TestStraightLineMove();
      //Test currentTest = new TestDiagonalMove();
      //Test currentTest = new TestForMoveAndStop();
      //Test currentTest = new TestForFastMoveAndStop();
      //Test currentTest = new TestForMoveThereAndBack();
      //Test currentTest = new TestRunningInCircles();
      Test currentTest = new TestEatCheese();

      // passed chromosome
      BotBase bot = new BotBase(chromosome, currentTest, 15, aShowGrid);

      // random chromosome
      //BotBase bot = new BotBase( currentTest, aShowGrid );

      // see how this bot gets on with the supplied test
      return bot.Evaluate( aShowGrid );
    }

    /// <summary>
    /// Create a directory to write the results of the specified test
    /// </summary>
    /// <param name="aTestName"></param>
    /// <returns></returns>
    private static string CreateTestDirectory(string aTestName)
    {
      string currentDirectory = Directory.GetCurrentDirectory();
      int lastIndex = currentDirectory.LastIndexOf('\\');
      lastIndex = currentDirectory.LastIndexOf('\\', lastIndex - 1);

      string rootDirectory = currentDirectory.Substring(0, lastIndex);
      string testDirectory = rootDirectory + "\\" + "tests\\" + aTestName;
      if (Directory.Exists(testDirectory) == false)
      {
        Directory.CreateDirectory(testDirectory);
      }

      // clear the directory of any file
      DirectoryInfo directoryInfo = new DirectoryInfo(testDirectory);
      foreach (FileInfo file in directoryInfo.GetFiles())
      {
        file.Delete();
      }

      return testDirectory;
    }


    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double CreateAndEvaluateBot( Chromosome chromosome, int aSideLength, int aBotType, bool aShowGrid )
    {
      // create the test that the bot is to perform
      //Test currentTest = new TestStraightLineMove();
      //Test currentTest = new TestDiagonalMove();
      //Test currentTest = new TestForMoveAndStop();
      //Test currentTest = new TestForFastMoveAndStop();
      //Test currentTest = new TestForMoveThereAndBack();
      //Test currentTest = new TestRunningInCircles();
      Test currentTest = new TestEatCheese();

      // create the bot
      //Bot bot = new Bot( chromosome, currentTest, aSideLength, aBotType, aShowGrid );

      BotBase bot = new BotBase(chromosome, currentTest, aSideLength, aShowGrid);

      // see how this bot gets on with the supplied test


      return bot.Evaluate(aShowGrid, (aShowGrid ? CreateTestDirectory("Evolution"):string.Empty),0);
    }
  }
}
