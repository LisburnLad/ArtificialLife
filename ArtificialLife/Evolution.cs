using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;
using GAF.Operators;

namespace ArtificialLife
{
  class Evolution
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

      //create a Population of 1000 random chromosomes of length 50
      var population = new Population(itsPopulationSize, chromosomeLength, false, false);

      //create the genetic operators 
      var elite = new Elite( elitismPercentage );

      var crossover = new Crossover( crossoverProbability, true )
      {
        CrossoverType = CrossoverType.DoublePoint
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


    /// <summary>
    /// function used for testing of a specific chromosome
    /// </summary>
    public void EvaluateSpecificChromosome()
    {
      // 1 delay gate
      //string testChromosome = @"00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" + // end of line 0
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" + // end of line 1  
      //                         "00000" +
      //                         "01000" +
      //                         "10001" +
      //                         "01001" +
      //                         "00000" + // end of line 2  
      //                         "00000" +
      //                         "00111" +
      //                         "01111" +
      //                         "01010" +
      //                         "00000" +  // end of line 3 
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000"; // end of line 4

      //// 2 delay gates
      //string testChromosome = @"00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" + // end of line 0
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" + // end of line 1  
      //                         "01000" +
      //                         "10001" +
      //                         "10001" +
      //                         "01001" +
      //                         "00000" + // end of line 2  
      //                         "00111" +
      //                         "00010" +
      //                         "01111" +
      //                         "01010" +
      //                         "00000" +  // end of line 3 
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000" +
      //                         "00000"; // end of line 4


      // 3 delay gates
      string testChromosome =  "00000" +
                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" + // end of line 3 
               
                               "00000" +
                               "01000" +
                               "10001" +
                               "10001" +
                               "10001" +
                               "10010" +  // end of line 1

                               "00000" +
                               "10000" +
                               "00000" +                               
                               "00000" +
                               "00000" +
                               "01110" + // end of line 0

                               "00000" +
                               "10000" +
                               "10011" +
                               "10011" +
                               "10011" +
                               "01010" + // end of line 2    

                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" + // end of line 3 

                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" +
                               "00000" +
                               "00000"; // end of line 4

      int sideLength = Bot.GetSideLengthFromChromosome(testChromosome);

      int sideLength2 = 6;
      string testChromosome2 = "001" +      // starting cell - North, Delay (B)
                               "011110" +   // rule A - LA, FC, RB
                               "000011" +   // rule B - L-, F-, RC
                               "100000";    // rule C - LB, F-, R-

      string testChromosome3 = "110" +      // starting cell - West, Nand (A)
                               "010010" +   // rule A - LA, F-, RB
                               "110011" +   // rule B - LC, F-, RC
                               "100000";    // rule C - LB, F-, R-

      int sideLength3 = 11;
      string testChromosome4 = "111101001001000101110000011011011110111";

      Chromosome chromosome = new Chromosome( testChromosome4 );
      CreateAndEvaluateBot(  chromosome, sideLength3, 2, true );
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
      Test currentTest = new TestForMoveThereAndBack();

      // create the bot
      Bot bot = new Bot( chromosome, currentTest, aSideLength, aBotType, aShowGrid );

      // see how this bot gets on with the supplied test
      return bot.Evaluate( aShowGrid );
    }
  }
}
