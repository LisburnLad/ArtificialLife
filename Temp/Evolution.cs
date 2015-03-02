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

    public bool itsShowOnTerminate { get; set; }

    public Evolution(int aGridSideLength, int aPopulationSize, int aGenerations)
    {
      itsSideLength = aGridSideLength;
      itsPopulationSize = aPopulationSize;
      itsMaxEvaluationCount = aGenerations;
    }
    
    public void StartEvolution()
    {      
      const double crossoverProbability = 0.65;
      const double mutationProbability = 0.08;
      const int elitismPercentage = 5;

      int chromosomeLength = Bot.GetChromosomeLength(itsSideLength);

      //create a Population of 1000 random chromosomes of length 50
      var population = new Population(itsPopulationSize, chromosomeLength, false, false);

      //create the genetic operators 
      var elite = new Elite( elitismPercentage );

      var crossover = new Crossover( crossoverProbability, true )
      {
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

      if (itsShowOnTerminate)
      {
        // decode chromosome
        double fitness = CreateAndEvaluateBot( chromosome, itsSideLength, true );

        Console.WriteLine( "Final Chromosome Fitmess: " + fitness );
        Console.WriteLine( "Evaluations: " + e.Evaluations );      
      }

      itsFitness = chromosome.Fitness;

      Console.WriteLine("Final Chromosome Fitmess: " + itsFitness);
    }

    /// <summary>
    /// Create the bot and evaluate its performance without any output
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double EvaluateChromosome(Chromosome chromosome)
    {
      double score = CreateAndEvaluateBot(chromosome, itsSideLength, false);
      return score;
    }


    /// <summary>
    /// function used for testing of a specific chromosome
    /// </summary>
    public void EvaluateSpecificChromosome()
    {      
      string testChromosome = @"1110111010110001100100010010110000110010101100000100001111001100101000000000000000000\
                                000000000000000000001100000001110001010010011100011011101001001111011010101";

      int sideLength = Bot.GetSideLengthFromChromosome(testChromosome);

      Chromosome chromosome = new Chromosome( testChromosome );
      CreateAndEvaluateBot(  chromosome, sideLength, true );
    }

    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double CreateAndEvaluateBot( Chromosome chromosome, int aSideLength, bool aShowGrid )
    {
      // create the test that the bot is to perform
      //Test currentTest = new TestStraightLineMove();
      //Test currentTest = new TestDiagonalMove();
      //Test currentTest = new TestForMoveAndStop();
      Test currentTest = new TestForFastMoveAndStop();

      // create the bot
      Bot bot = new Bot( chromosome, currentTest, aSideLength, aShowGrid );

      // see how this bot gets on with the supplied test
      return bot.Evaluate( aShowGrid );
    }
  }
}
