using GAF;
using GAF.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtificialLife
{
  class ChromosomeEvolution
  {
    const int kPopulationSize = 50;
    const int kMaxEvaluationCount = 100;
    const int kChromosomeLength = 10;

    public void StartEvolution()
    {
      const double crossoverProbability = 0.65;
      const double mutationProbability = 0.08;
      const int elitismPercentage = 5;

      //create a Population of 1000 random chromosomes of length 50
      var population = new Population(kPopulationSize, kChromosomeLength, false, false);

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
      return (currentEvaluation >= kMaxEvaluationCount);
    }

    private void ga_OnGenerationComplete( object sender, GaEventArgs e )
    {
      //get the best solution 
      var chromosome = e.Population.GetTop( 1 )[0];

      //decode chromosome
      double fitness = CreateAndEvaluate( chromosome, true );

      Console.WriteLine( "Final Chromosome Fitmess: " + fitness );
      Console.WriteLine( "Evaluations: " + e.Evaluations );
    }

    /// <summary>
    /// Create the bot and evaluate its performance without any output
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double EvaluateChromosome( Chromosome chromosome )
    {
      return CreateAndEvaluate( chromosome, false );
    }


    /// <summary>
    /// function used for testing of a specific chromosome
    /// </summary>
    public void EvaluateSpecificChromosome()
    {
      string testChromosome = "0000010101";

      Chromosome chromosome = new Chromosome( testChromosome );
      CreateAndEvaluate( chromosome, true );
    }


    private int GetGeneValue(Chromosome aChromosome, int aGeneLength, int aStartBit )
    {
      int geneValue = 0;
      for (int bit = 0; bit < aGeneLength; bit++)
      {
        geneValue += Convert.ToInt32(aChromosome.ToBinaryString(aStartBit + bit, 1)) * (1 << bit);
      }

      return geneValue;
    }

    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private double CreateAndEvaluate(Chromosome aChromosome, bool aShowGrid)
    {
      // 4 bits for grid side length = value (0 to 16)  + 5
      // 3 bits for population size = 100 + (50 * value(0-7))
      // 3 bits for generations = 100 *(value(0-7) + 1)      

      int gridSideLength = GetGeneValue(aChromosome, 4, 0) + 5;
      int populationSize = (100 + (50 *  GetGeneValue(aChromosome, 3, 4)));
      int generations = (100 * (GetGeneValue(aChromosome, 3,7) + 1));

      int botType = 2;

      Evolution evolution = new Evolution( gridSideLength, populationSize, generations, botType );
      evolution.StartEvolution();


      if (aShowGrid)
      {
        Console.WriteLine("Grid Size = " + gridSideLength);
        Console.WriteLine("Population Size = " + populationSize);
        Console.WriteLine("Generations = " + generations);
        Console.WriteLine("Fitness = " + evolution.itsFitness);
      }

      //return evolution.itsFitness;


      // total number of cells processed =  ((grid size * grid size) * population size * number of generations)
      // fitness = 250000 / total number of cells processed
      // 2500000 = (5 * 5) * 100 * 100 (the smallest possible value for "total number of cells processed")

      double totalCellsProcessed = (gridSideLength * gridSideLength) * populationSize * generations;

      return (250000.0 / totalCellsProcessed);
    }
  }
}
