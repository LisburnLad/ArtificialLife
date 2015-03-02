using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GAF;

namespace ArtificialLife
{
  class Program
  {
    static void Main( string[] args )
    {
      Console.WriteLine( "Starting Program" );

      //ChromosomeEvolution chromosomeEvolution = new ChromosomeEvolution();
      //chromosomeEvolution.StartEvolution();
      //chromosomeEvolution.EvaluateSpecificChromosome();


      int gridSideLength = 5;
      int populationSize = 100;
      int generations = 100;
      Evolution evolution = new Evolution(gridSideLength, populationSize, generations);
      evolution.itsShowOnTerminate = true;
      //evolution.StartEvolution();
      evolution.EvaluateSpecificChromosome();
      
      Console.ReadLine();
    }
  }
}
