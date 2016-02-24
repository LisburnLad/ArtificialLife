using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArtificialLife;
using GAF;
using System.IO;
using System.Text;
using NUnit.VisualStudio.TestAdapter;

namespace ArtificialLifeUnitTests
{

  /// <summary>
  /// test that the operation of one bot doesn't interfere with another
  /// </summary>
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestBasic()
    {
      Console.WriteLine("Starting");
    }

    /// <summary>
    /// Test a bot whose chromosome defines possibly unique values for each input cell.
    /// Plus the chromosome has been extended to use 5-bits to reference each connection
    /// Chromosome length now =  bits
    /// </summary>
    [TestMethod]
    public void TestBotWithUniqueInputs()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00000" + // east input   - X
                     "00000" + // south input  - X
                     "00010" + // west input   - B

                     "00000" + "00010" + "00000" + "00001" + // A - 00001 - x-B-x - local connection
                     "00000" + "00011" + "00000" + "00001" + // B - 00010 - x-C-x - local connection                               
                     "00000" + "00100" + "00101" + "00011" + // C - 00011 - x-D-E - Nand    
                     "00000" + "00100" + "00000" + "00000" + // D - 00100 - x-D-x - local connection   
                     "00000" + "00000" + "00101" + "00000" + // E - 00101 - x-x-E - local connection   
                     "00000" + "00000" + "00000" + "00000" + // F - 00110 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000" + // N - 01110 
                     "00000" + "00000" + "00000" + "00000" + // O - 01111 

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
                     "00000" + "00000" + "00000" + "00000" + //30 - 11110
                     "00000" + "00000" + "00000" + "00000";  //31 - 11111
                         

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestBotWithUniqueInputs");

      string chromosomeString = rules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      Test test1 = new TestStraightLineMove();
      
      BotBase bot1 = new BotBase(chromosome, test1, gridSideLength, false);
      bot1.SaveGrid(testDirectory + "\\nand_gate1.bmp");
      bot1.CreateProcessor(testDirectory);

      double result = bot1.TestBot(true, testDirectory, 10, 0);
      test1.WriteTestOutput(testDirectory, "bot1.txt");
    }


    /// <summary>
    /// Test different types of targeted connection
    /// </summary>
    [TestMethod]
    public void TestBadChromosome()
    {
      int gridSideLength = 15;


string rules2 = "010001010101100001000111111010000010111001001101101010011111001000000001100010010100100110000001001001110011010111000000011100010000011110011111001111111" +
"101001011001111011101101101111101111101001101011100110011111000111000000000111001010110001111101111001110111001111011101011000101100001011011100110011011111001101111" +
"110100110000000100001010010011001000011000110011011000010100100110100111100001000010100110000000001011111100110011110011110101001000111110011010001101100011110011011" +
"1000000100101100111000100011001111011101000011100111010111000011000010101001000011101100000100011011101100001010011110011110011100011110011000101101100010010";

string rules = "011010101111111111010100000011111100101000001110100100100000111001110101111010001101101100101111011111001010011011110110011010100111011110000011010111110" +
"110011110101110000011010010111100111101000100011111101101100111100010111010000000000011000110110110111101101010111101110011110010111000011101011000010111111010101001" +
"100111101111111010101110000111111110111111010001011110000100000111000101001101000110000000111111001011010101001001010111111101000111001001011110100001011001001110001" +
"1000000111011010101111101100101010110101110111101010111011000001110001011100100101110111110010111010011001010101111010010111110001000001100101011111100001111";

      //string rules = "00010" + // north input  - 2
      //               "00001" + // east input   - 1
      //               "00100" + // south input  - 4
      //               "00101" + // west input   - 5

      string rules3 = "00000" + // north input  - 2
                     "00000" + // east input   - 1
                     "00000" + // south input  - 4
                     "00101" + // west input   - 5

                     "01101" + "10010" + "01110" + "00001" + // 1  - 00001
                     "11001" + "10001" + "01011" + "00010" + // 2  - 00010
                     "00100" + "01010" + "10111" + "11001" + // 3  - 00011
                     "10110" + "01111" + "10011" + "01001" + // 4  - 00100 - 22-15-19 - empty-TC-nand

                     "00000" + "00001" + "10100" + "00101" + // 5  - 00101 - 0-1-20 - empty-local-empty
                     "11011" + "01100" + "10011" + "11110" + // 6  - 00110
                     "00110" + "01000" + "00001" + "00011" + // 7  - 00111
                     "01011" + "10001" + "00111" + "00010" + // 8  - 01000

                     "11010" + "10110" + "10001" + "11010" + // 9  - 01001
                     "00110" + "10010" + "01101" + "00100" + // 10 - 01010
                     "00000" + "00110" + "11000" + "01010" + // 11 - 01011
                     "10001" + "11100" + "00110" + "10111" + // 12 - 01100

                     "10001" + "11001" + "11010" + "00100" + // 13 - 01101
                     "10110" + "01000" + "00111" + "11000" + // 14 - 01110
                     "00111" + "00010" + "10000" + "11101" + // 15 - 01111
                     "10001" + "01000" + "00100" + "00011" + // 16 - 10000

                     "01011" + "10110" + "00110" + "10111" + // 17 - 10001
                     "01000" + "11110" + "00000" + "01011" + // 18 - 10010
                     "10010" + "10110" + "11001" + "00011" + // 19 - 10011 - 18-22-25 - empty-empty-empty
                     "11110" + "10100" + "10101" + "00000" + // 20 - 10100

                     "01100" + "00100" + "11011" + "00111" + // 21 - 10101
                     "11001" + "01001" + "10010" + "01101" + // 22 - 10110
                     "10001" + "00000" + "11001" + "10010" + // 23 - 10111
                     "01110" + "01001" + "11101" + "00010" + // 24 - 11000

                     "11001" + "01101" + "10100" + "11110" + // 25 - 11001
                     "10100" + "00010" + "10100" + "11110" + // 26 - 11010
                     "11110" + "11011" + "11101" + "11111" + // 27 - 11011
                     "01101" + "00011" + "00101" + "10011" + // 28 - 11100

                     "01010" + "11001" + "01110" + "10110" + // 29 - 11101
                     "01101" + "11010" + "11011" + "00010" + // 30 - 11110
                     "10101" + "10001" + "10010" + "01001";  // 31 - 11111


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestBadChromosome");

      string chromosomeString = rules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      Test test1 = new TestEatCheese();
      
      BotBase bot1 = new BotBase(chromosome, test1, gridSideLength, true);
      bot1.SaveGrid(testDirectory + "\\nand_gate1.bmp");

      double result = bot1.TestBot(true, testDirectory, 60, 0);
      test1.WriteTestOutput(testDirectory, "bot1.txt");
    }


    /// <summary>
    /// Test different types of targeted connection
    /// </summary>
    [TestMethod]
    public void TestTargetedConnectionTypes()
    {
      int gridSideLength = 15;

      string rules = "00000" + // north input  -                      
                     "00000" + // east input   - 
                     "00001" + // south input  - 
                     "00000" + // west input   -                      
                                                                  
                     "00000" + "00000" + "00000" + "10000" + //1  - 00001
                     "00000" + "00000" + "00000" + "00000" + //2  - 00010
                     "00000" + "00000" + "00000" + "00000" + //3  - 00011
                     "00000" + "00000" + "00000" + "00000" + //4  - 00100
                                                                  
                     "00000" + "00000" + "00000" + "00000" + //5  - 00101
                     "00000" + "00000" + "00000" + "00000" + //6  - 00110
                     "00000" + "00000" + "00000" + "00000" + //7  - 00111
                     "00000" + "00000" + "00000" + "00000" + //8  - 01000
                                                                  
                     "00000" + "00000" + "00000" + "00000" + //9  - 01001
                     "00000" + "00000" + "00000" + "00000" + //10 - 01010
                     "00000" + "00000" + "00000" + "00000" + //11 - 01011
                     "00000" + "00000" + "00000" + "00000" + //12 - 01100
                                                                  
                     "00000" + "00000" + "00000" + "00000" + //13 - 01101
                     "00000" + "00000" + "00000" + "00000" + //14 - 01110
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
                     "00000" + "00000" + "00000" + "00000" + //30 - 11110
                     "00000" + "00000" + "00000" + "00000";  //31 - 11111
                     

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestTargetedConnectionTypes");
      
      string chromosomeString = rules;      
      Chromosome chromosome = new Chromosome(chromosomeString);

      Test test1 = new TestEatCheese();
      Test test2 = new TestEatCheese();

      BotBase bot1 = new BotBase(chromosome, test1, gridSideLength, true);
      bot1.SaveGrid(testDirectory + "\\nand_gate1.bmp");
    }





    [TestMethod]
    public void TestEatCheeseBot()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A                     
                     "00000" + // east input   - 
                     "00001" + // south input  - A
                     "00101" + // west input   - H                     
                     "00000" + "00000" + "00010" + "00000" + //1 A - 0001 - X-X-B
                     "00000" + "00011" + "00000" + "00001" + //2 B - 0010 - X-C-X   - local connection 
                     "01110" + "00111" + "00000" + "00001" + //3 C - 0011 - N-G-X   - local connection 
                     "00000" + "01010" + "00000" + "01000" + //4 D - 0100 - X-J-X   - pulse
                     "00000" + "01000" + "00000" + "00001" + //5 E - 0101 - X-H-X   - local connection                    
                     "00000" + "01111" + "00000" + "00110" + //6 F - 0110 - X-O-X   - trigger  
                     "00100" + "01001" + "00000" + "00000" + //7 G - 0111 - D-I-X   -                   
                     "00000" + "00101" + "00000" + "00101" + //8 H - 1000 - X-E-X   - xor                   
                     "01101" + "00000" + "00000" + "00000" + //9 I - 1001 - M-X-X   - local connection 
                     "00000" + "01111" + "00000" + "00001" + //10 J - 1010 - X-O-X  - local connection                   
                     "00010" + "00000" + "00000" + "00111" + //11 K - 1011 - B-X-X  - or                                 
                     "00000" + "00000" + "00000" + "00000" + //12 L - 1100 - X-X-X
                     "00110" + "11" + "10" + "01" + "0000" + "10000" + //13 M - 1101 - [F, long dist, medium long area, left, padding] - targets trigger node
                     "00000" + "00110" + "00000" + "00111" + //14 N - 1110 - X-F-X   - or
                     "00010" + "01" + "01" + "01" + "0000" + "10000" + //15 O - 1111 - [B, medium short dist, medium short area, left, padding] - targets output node

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
                     "00000" + "00000" + "00000" + "00000" + //30 - 11110
                     "00000" + "00000" + "00000" + "00000";  //31 - 11111


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestEatCheeseBot");

      //string chromosomeString = rules + nodeTypes;
      string chromosomeString = rules;
      //string chromosomeString = rules + target + "10" + "01" + "01" + padding + nodeTypes;
      Chromosome chromosome = new Chromosome(chromosomeString);

      Test test1 = new TestEatCheese();
      Test test2 = new TestEatCheese();

      BotBase bot1 = new BotBase(chromosome, test1, gridSideLength, true);
      bot1.SaveGrid(testDirectory + "\\nand_gate1.bmp");
      //bot1.CreateProcessor(testDirectory);

      //BotBase bot2 = new BotBase(chromosome, test2, gridSideLength, false);
      //bot2.SaveGrid(testDirectory + "\\nand_gate2.bmp");      
      //bot2.CreateProcessor(testDirectory);



      //bool northInput = true;
      //bool eastInput = true;
      //bool southInput = true;
      //bool westInput = false;
      //bot1.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput1.txt");

      //northInput = true;
      //eastInput = true;
      //southInput = false;
      //westInput = true;
      //bot1.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput1.txt");

      //test1.WriteTestOutput(testDirectory,"bot1.txt");
      //test2.WriteTestOutput(testDirectory, "bot2.txt");

      //double result = bot1.Evaluate(true, testDirectory);

      double result = bot1.TestBot(true, testDirectory, 60, 0);
      test1.WriteTestOutput(testDirectory, "bot1.txt");
    }



    #region Node Type Tests

    /// <summary>
    /// Test that connections are used when the cell output isn't a node
    /// </summary>
    [TestMethod]
    public void TestCellConnections()
    {
      int gridSideLength = 15;

      string rules = "00000" + // north input  - A
                     "00000" + // east input   - A
                     "00001" + // south input  - A
                     "00000" + // west input   - A
                     "00000" + "00000" + "00010" + "00001" + // A - 00001 - X-X-B
                     "00000" + "00011" + "00000" + "00001" + // B - 00010 - X-C-X
                     "00100" + "00111" + "00000" + "00001" + // C - 00011 - D-G-X
                     "00000" + "00110" + "00000" + "00001" + // D - 00100 - X-E-X    
                     "00000" + "00110" + "00000" + "00001" + // E - 00101 - X-F-X     
                     "00000" + "01111" + "00000" + "00110" + // F - 00110 - X-O-X
                     "01000" + "01010" + "00000" + "00000" + // G - 00111 - H-J-X 
                     "00000" + "01001" + "00000" + "00001" + // H - 01000 - X-I-X 
                     "00000" + "01111" + "00000" + "00110" + // I - 01001 - X-O-X
                     "00100" + "00000" + "00000" + "00001" + // J - 01010 - D-X-X
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000";  // N - 01110 
                                                  
      string rules1 = "00000" + // north input  - A
                      "00000" + // east input   - A
                      "00001" + // south input  - A
                      "00000" + // west input   - A
                      "00000" + "00000" + "00010" + "00000" + // A - 00001 - X-X-B
                      "00000" + "00011" + "00000" + "00001" + // B - 00010 - X-C-X
                      "01110" + "00111" + "00000" + "00001" + // C - 00011 - N-G-X
                      "00000" + "01010" + "00000" + "00111" + // D - 00100 - X-J-X 
                      "00000" + "00110" + "00000" + "00001" + // E - 00101 - X-F-X
                      "00000" + "01111" + "00000" + "00110" + // F - 00110 - X-O-X
                      "00100" + "01000" + "00000" + "00001" + // G - 00111 - D-H-X                     
                      "00000" + "00000" + "00000" + "00001" + // H - 01000 
                      "00000" + "00000" + "00000" + "00001" + // I - 01001 
                      "00000" + "01111" + "00000" + "00101" + // J - 01010 - X-O-X                     
                      "00000" + "00000" + "00000" + "00000" + // K - 01011 
                      "00000" + "00000" + "00000" + "00000" + // L - 01100 
                      "00000" + "00000" + "00000" + "00010" + // M - 01101 
                      "00000" + "00101" + "00000" + "00001";  // N - 01110 - X-E-X 

       string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                         "00000" + "00000" + "00000" + "00000" + //30 - 11110
                         "00000" + "00000" + "00000" + "00000";  //31 - 11111

      //"00010" + "01" + "01" + "01" + "0000" + "10000" + //15 O - 1111 - [B, medium short dist, medium short area, left, padding] - targets output node

      string target = "00001"; // O - 1111 - targeted connection - target = A
      string distance = "01";  // medium-short
      string area = "01";      // medium-short
      string direction = "01"; // left
      string padding = "0000";
      string type = "10000"; // o - targeted connection  

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestCellConnections");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");
    }



    [TestMethod]
    public void TestTriggerNode()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00011" + "01111" + "00000" + // A - 00001 - X-C-O
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                            
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - x-D-x  
                     "00000" + "00101" + "00000" + "00110" + // D - 00100 - x-E-x    
                     "00000" + "00101" + "00000" + "00000" + // E - 00101 - x-E-x     
                     "00000" + "00000" + "00000" + "00000" + // F - 00110
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000";  // N - 01110 

      string target = "00100"; // O - 1111 - targeted connection - target = D
      string distance = "11";
      string area = "01";
      string direction = "01";
      string padding = "0000";
      string type = "10000"; // o - targeted connection

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestTriggerNode");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = false;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // only north input is true
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(true, aNorthOutput);  
      Assert.AreEqual(false, aEastOutput);  
      Assert.AreEqual(false, aSouthOutput); 
      Assert.AreEqual(false, aWestOutput);   

      northInput = false;
      eastInput = false;
      southInput = false;
      westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(true, aNorthOutput);
      Assert.AreEqual(false, aEastOutput);
      Assert.AreEqual(false, aSouthOutput);
      Assert.AreEqual(false, aWestOutput); 

      // put the east input back to false - the east output should remain set
      northInput = false;
      eastInput = true;
      southInput = false;
      westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "ThirdOutput.txt");
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(false, aNorthOutput);
      Assert.AreEqual(true, aEastOutput);
      Assert.AreEqual(false, aSouthOutput);
      Assert.AreEqual(false, aWestOutput); 

      // put the east input back to true - the east output should now turn off
      northInput = true;
      eastInput = true;
      southInput = false;
      westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FourthOutput.txt");
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(true, aNorthOutput);
      Assert.AreEqual(true, aEastOutput);
      Assert.AreEqual(false, aSouthOutput);
      Assert.AreEqual(false, aWestOutput); 
    }


    [TestMethod]
    public void TestChainedOrNode()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00011" + "01111" + "00000" +  // A - 00001 - X-C-O
                     "00000" + "00000" + "00000" + "00000" +  // B - 00010                            
                     "00000" + "00100" + "00000" + "00000" +  // C - 00011 - x-D-x  
                     "00000" + "00101" + "00000" + "00111" +  // D - 00100 - x-E-x    
                     "00000" + "00100" + "00000" + "00000" +  // E - 00101 - x-D-x     
                     "00000" + "00000" + "00000" + "00000" +  // F - 00110
                     "00000" + "00000" + "00000" + "00000" +  // G - 00111 
                     "00000" + "00000" + "00000" + "00000" +  // H - 01000 
                     "00000" + "00000" + "00000" + "00000" +  // I - 01001 
                     "00000" + "00000" + "00000" + "00000" +  // J - 01010 
                     "00000" + "00000" + "00000" + "00000" +  // K - 01011 
                     "00000" + "00000" + "00000" + "00000" +  // L - 01100 
                     "00000" + "00000" + "00000" + "00000" +  // M - 01101 
                     "00000" + "00000" + "00000" + "00000";   // N - 01110 

      string target = "00100"; // O - 1111 - targeted connection - target = D
      string distance = "11";
      string area = "01";
      string direction = "01";
      string padding = "0000";
      string type = "10000"; // - targeted connection  

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestChainedOrNode");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = false;
      bool eastInput = false;
      bool southInput = false;
      bool westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt",false);

      // only north input is true
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(false, aNorthOutput);
      Assert.AreEqual(false, aEastOutput);
      Assert.AreEqual(true, aSouthOutput);
      Assert.AreEqual(true, aWestOutput); 

      northInput = false;
      eastInput = false;
      southInput = true;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt",false);
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(false, aNorthOutput);
      Assert.AreEqual(true, aEastOutput);
      Assert.AreEqual(true, aSouthOutput);
      Assert.AreEqual(true, aWestOutput);   
    }



    [TestMethod]
    public void TestChainedDelayNode()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00011" + "01111" + "00000" + // A - 00001 - X-C-O
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                            
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - x-D-x  
                     "00000" + "00101" + "00000" + "00100" + // D - 00100 - x-E-x    
                     "00000" + "00100" + "00000" + "00000" + // E - 00101 - x-D-x     
                     "00000" + "00000" + "00000" + "00000" + // F - 00110
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000";  // N - 01110 


      string target = "00100"; // O - 01111 - targeted connection - target = D
      string distance = "11";
      string area = "01";
      string direction = "01";
      string padding = "0000";
      string type = "10000";

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestChainedDelayNode");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = false;
      bool eastInput = false;
      bool southInput = false;
      bool westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt", false);

      // only west input is true
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(false, aNorthOutput); 
      Assert.AreEqual(false, aEastOutput);  
      Assert.AreEqual(true, aSouthOutput);  
      Assert.AreEqual(true, aWestOutput);   

      northInput = false;
      eastInput = false;
      southInput = true;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt", false);
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(false, aNorthOutput);
      Assert.AreEqual(true, aEastOutput);
      Assert.AreEqual(true, aSouthOutput);
      Assert.AreEqual(true, aWestOutput);   
    }

    [TestMethod]
    public void TestDelayNode()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00011" + "01111" + "00000" + // A - 00001 - X-C-O
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                            
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - x-D-x  
                     "00000" + "00101" + "00000" + "00100" + // D - 00100 - x-E-x - delay node   
                     "00000" + "00101" + "00000" + "00000" + // E - 00101 - x-E-x     
                     "00000" + "00000" + "00000" + "00000" + // F - 00110
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000";  // N - 01110 

      string target = "00100"; // O - 01111 - targeted connection - target = D
      string distance = "11";
      string area = "01";
      string direction = "01";
      string padding = "0000";
      string type = "10000"; // o - targeted connection 

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestDelayNode");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = false;
      bool westInput = false;
      bot.ShowAllPropagationSteps = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt",true);

      // only north input is true
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual( true, aNorthOutput );  // 1 + 0 = true
      Assert.AreEqual( false, aEastOutput );  // 0 + 0 = false
      Assert.AreEqual( false, aSouthOutput ); // 0 + 0 = false
      Assert.AreEqual( true, aWestOutput );   // 0 + 1 = true

      northInput = true;
      eastInput = true;
      southInput = false;
      westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt",true);
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual( true, aNorthOutput );  // 1 + 1 = true
      Assert.AreEqual( true, aEastOutput );  // 1 + 0 = true
      Assert.AreEqual( false, aSouthOutput ); // 0 + 0 = false
      Assert.AreEqual( true, aWestOutput );   // 0 + 1 = true
    }

    [TestMethod]
    public void TestXOrGate()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00011" + "01111" + "00000" + // A - 00001 - X-C-O
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                            
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - x-D-x  
                     "00000" + "00101" + "00000" + "00101" + // D - 00100 - x-E-x    
                     "00000" + "00101" + "00000" + "00000" + // E - 00101 - x-E-x     
                     "00000" + "00000" + "00000" + "00000" + // F - 00110
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000";  // N - 01110 
                                                  
      string target = "00100"; // O - 1111 - targeted connection - target = D
      string distance = "11";  
      string area = "01";      
      string direction = "01"; 
      string padding = "0000";
      string type = "10010";

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111

      //string nodeTypes = "00000" + // a
      //                   "00000" + // b 
      //                   "00001" + // c - local connection
      //                   "00101" + // d - XOr
      //                   "00001" + // e - local connection
      //                   "00000" + // f
      //                   "00000" + // g 
      //                   "00000" + // h 
      //                   "00000" + // i 
      //                   "00000" + // j 
      //                   "00000" + // k 
      //                   "00000" + // l 
      //                   "00000" + // m 
      //                   "00000" + // n
      //                   "00010";  // o - targeted connection  


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestXOrGate");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");
            
      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = false;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // only north input is true
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(true, aNorthOutput);  // 1 + 0 = true
      Assert.AreEqual(false, aEastOutput);  // 0 + 0 = false
      Assert.AreEqual(false, aSouthOutput); // 0 + 0 = false
      Assert.AreEqual(true, aWestOutput);   // 0 + 1 = true

      northInput = true;
      eastInput = true;
      southInput = false;
      westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");            
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(false, aNorthOutput);  // 1 + 1 = false
      Assert.AreEqual(true, aEastOutput);  // 1 + 0 = true
      Assert.AreEqual(false, aSouthOutput); // 0 + 0 = false
      Assert.AreEqual(true, aWestOutput);   // 0 + 1 = true
    }

    [TestMethod]
    public void TestOrGate()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00010" + "00000" + "00000" +           // A - 00001 - X-B-X
                     "00000" + "00011" + "00000" + "00000" +           // B - 00010 - x-C-x                                 
                     "00000" + "00100" + "00000" + "00000" +           // C - 00011 - x-D-x  
                     "00000" + "00101" + "00000" + "00111" +           // D - 00100 - x-E-x    
                     "00000" + "00101" + "00000" + "00000" +           // E - 00101 - x-E-x     
                     "00000" + "00000" + "00000" + "00000" +           // F - 00110
                     "00000" + "00000" + "00000" + "00000" +           // G - 00111 
                     "00000" + "00000" + "00000" + "00000" +           // H - 01000 
                     "00000" + "00000" + "00000" + "00000" +           // I - 01001 
                     "00000" + "00000" + "00000" + "00000" +           // J - 01010 
                     "00000" + "00000" + "00000" + "00000" +           // K - 01011 
                     "00000" + "00000" + "00000" + "00000" +           // L - 01100 
                     "00000" + "00000" + "00000" + "00000" +           // M - 01101 
                     "00000" + "00000" + "00000" + "00000" +           // N - 01110 
                     "00000" + "00000" + "00000" + "00000";            // O - 01111 

      //string nodeTypes = "00000" + // a
      //                   "00001" + // b - local connection
      //                   "00001" + // c - local connection
      //                   "00111" + // d - Or
      //                   "00001" + // e - local connection
      //                   "00000" + // f
      //                   "00000" + // g 
      //                   "00000" + // h 
      //                   "00000" + // i 
      //                   "00000" + // j 
      //                   "00000" + // k 
      //                   "00000" + // l 
      //                   "00000" + // m 
      //                   "00000" + // n
      //                   "00000";  // o   

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111



      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestOrGate");

      string chromosomeString = rules + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);
      
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\bot.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = true;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // the outputs should be the same as the inputs
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(northInput, aNorthOutput);
      Assert.AreEqual(eastInput, aEastOutput);
      Assert.AreEqual(southInput, aSouthOutput);
      Assert.AreEqual(westInput, aWestOutput);

      northInput = false;
      eastInput = true;
      southInput = false;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");

      // the outputs should be the same as the inputs
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(northInput, aNorthOutput);
      Assert.AreEqual(eastInput, aEastOutput);
      Assert.AreEqual(southInput, aSouthOutput);
      Assert.AreEqual(westInput, aWestOutput);
    }



    /// <summary>
    /// Test that the value of a connection propagates to all joined connections and that the Nand gate works ok
    /// </summary>
    [TestMethod]
    public void TestConnectionsWithNand()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00000" + "00011" + "00000" +   // A - 00001 - X-X-C
                     "00000" + "01111" + "00000" + "00000" +   // B - 00010 - x-O-x - forward targeted connection   
                     "00000" + "00100" + "00000" + "00000" +   // C - 00011 - x-D-x - local forward connection 
                     "00000" + "00101" + "00000" + "00000" +   // D - 00100 - x-E-x - local forward connection   
                     "00110" + "00000" + "00000" + "00001" +   // E - 00101 - F-x-x - left Nand   
                     "00000" + "00010" + "00000" + "00011" +   // F - 00110 - x-B-x - local forward connection
                     "00000" + "00000" + "00000" + "00000" +   // G - 00111 
                     "00000" + "00000" + "00000" + "00000" +   // H - 01000 
                     "00000" + "00000" + "00000" + "00000" +   // I - 01001 
                     "00000" + "00000" + "00000" + "00000" +   // J - 01010 
                     "00000" + "00000" + "00000" + "00000" +   // K - 01011 
                     "00000" + "00000" + "00000" + "00000" +   // L - 01100 
                     "00000" + "00000" + "00000" + "00000" +   // M - 01101 
                     "00000" + "00000" + "00000" + "00001";    // N - 01110 

      string target = "01110"; // O - 1111 - targeted connection - target = N
      string distance = "01";  // medium-short
      string area = "01";      // medium-short
      string direction = "01"; // left
      string padding = "0000";
      string type = "10000";

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111

      //string nodeTypes = "00000" + // a 
      //                   "00001" + // b - local connection 
      //                   "00001" + // c - local connection
      //                   "00001" + // d - local connection
      //                   "00001" + // e - local connection
      //                   "00011" + // f - nand  
      //                   "00000" + // g 
      //                   "00000" + // h 
      //                   "00000" + // i 
      //                   "00000" + // j 
      //                   "00000" + // k 
      //                   "00000" + // l 
      //                   "00000" + // m 
      //                   "00001" + // n - NorthSouth (= Output Cell for targeted connection)
      //                   "00010";  // o - targeted connection    

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestConnectionsWithNand");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\TestConnectionsWithNand.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = true;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // the outputs should be the inverse of the inputs
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(!northInput, aNorthOutput);
      Assert.AreEqual(!eastInput, aEastOutput);
      Assert.AreEqual(!southInput, aSouthOutput);
      Assert.AreEqual(!westInput, aWestOutput);

      northInput = false;
      eastInput = true;
      southInput = false;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");

      // the outputs should be the inverse of the inputs
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(!northInput, aNorthOutput);
      Assert.AreEqual(!eastInput, aEastOutput);
      Assert.AreEqual(!southInput, aSouthOutput);
      Assert.AreEqual(!westInput, aWestOutput);
    }


    /// <summary>
    /// Test that the value of a connection propagates to all joined connections
    /// </summary>
    [TestMethod]
    public void TestConnections()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00000" + "00011" + "00000" + // A - 00001 - X-X-C
                     "00000" + "00000" + "00000" + "00000" + // B - 00010 
                     "00000" + "00100" + "00000" + "00001" + // C - 00011 - x-D-x - local forward connection 
                     "00000" + "00101" + "00000" + "00001" + // D - 00100 - x-E-x - local forward connection   
                     "01111" + "00000" + "00000" + "00001" + // E - 00101 - O-x-x - left targeted connection    
                     "00000" + "00000" + "00000" + "00000" + // F - 00110
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00001";  // N - 01110 

      string target = "01110"; // O - 1111 - targeted connection - target = N
      string distance = "01";  // medium-short
      string area = "01";      // medium-short
      string direction = "01"; // left
      string padding = "0000";
      string type = "10000"; // o - targeted connection  

      //string nodeTypes = "00000" + // a 
      //                   "00000" + // b 
      //                   "00001" + // c - local connection
      //                   "00001" + // d - local connection
      //                   "00001" + // e - local connection
      //                   "00000" + // f
      //                   "00000" + // g 
      //                   "00000" + // h 
      //                   "00000" + // i 
      //                   "00000" + // j 
      //                   "00000" + // k 
      //                   "00000" + // l 
      //                   "00000" + // m 
      //                   "00001" + // n - NorthSouth (= Output Cell for targeted connection)
      //                   "00010";  // o - targeted connection    

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111
        

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestConnections");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\test_connections.bmp");
      
      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = true;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // the outputs should be the same as the inputs
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(northInput, aNorthOutput);
      Assert.AreEqual(eastInput, aEastOutput);
      Assert.AreEqual(southInput, aSouthOutput);
      Assert.AreEqual(westInput, aWestOutput);

      northInput = false;
      eastInput = true;
      southInput = false;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");

      // the outputs should be the same as the inputs
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(northInput, aNorthOutput);
      Assert.AreEqual(eastInput, aEastOutput);
      Assert.AreEqual(southInput, aSouthOutput);
      Assert.AreEqual(westInput, aWestOutput);
    }


    [TestMethod]
    public void TestNandGate()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00010" + "00000" + "00000" +  // A - 00001 - X-B-X
                     "00000" + "01111" + "00000" + "00011" +  // B - 00010 - X-O-X
                     "00000" + "00000" + "00000" + "00000" +  // C - 00011   
                     "00000" + "00000" + "00000" + "00000" +  // D - 00100     
                     "00000" + "00000" + "00000" + "00000" +  // E - 00101      
                     "00000" + "00000" + "00000" + "00000" +  // F - 00110
                     "00000" + "00000" + "00000" + "00000" +  // G - 00111 
                     "00000" + "00000" + "00000" + "00000" +  // H - 01000 
                     "00000" + "00000" + "00000" + "00000" +  // I - 01001 
                     "00000" + "00000" + "00000" + "00000" +  // J - 01010 
                     "00000" + "00000" + "00000" + "00000" +  // K - 01011 
                     "00000" + "00000" + "00000" + "00000" +  // L - 01100 
                     "00000" + "00000" + "00000" + "00000" +  // M - 01101 
                     "00000" + "00000" + "00000" + "00000";   // N - 01110 

      string target = "01110"; // O - 1111 - targeted connection - target = N
      string distance = "01";
      string area = "11";
      string direction = "10"; // forward       
      string padding = "0000";
      string type = "10000";

      //string nodeTypes = "00000" + // a
      //                   "00011" + // b - Nand
      //                   "00000" + // c 
      //                   "00000" + // d 
      //                   "00000" + // e 
      //                   "00000" + // f
      //                   "00000" + // g 
      //                   "00000" + // h 
      //                   "00000" + // i 
      //                   "00000" + // j 
      //                   "00000" + // k 
      //                   "00000" + // l 
      //                   "00000" + // m 
      //                   "00001" + // n - NorthSouth (= Output Cell for targeted connection)
      //                   "00010";  // o - targeted connection    

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111



      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestNandGate");

      string chromosomeString = rules + target + distance + area + direction + padding + type + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);      
      bot.SaveGrid(testDirectory + "\\nand_gate.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput  = false;
      bool southInput = true;
      bool westInput  = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // the outputs should be the inverse of the inputs
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(!northInput, aNorthOutput);
      Assert.AreEqual(!eastInput, aEastOutput);
      Assert.AreEqual(!southInput, aSouthOutput);
      Assert.AreEqual(!westInput, aWestOutput);

      northInput = false;
      eastInput  = true;
      southInput = false;
      westInput  = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");

      // the outputs should be the inverse of the inputs
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(!northInput, aNorthOutput);
      Assert.AreEqual(!eastInput, aEastOutput);
      Assert.AreEqual(!southInput, aSouthOutput);
      Assert.AreEqual(!westInput, aWestOutput);
    }

    [TestMethod]
    public void TestDoubleNandGate()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00010" + "00000" + "00000" +        // A - 00001 - X-B-X
                     "00000" + "00011" + "00000" + "00011" +        // B - 00010 - x-C-x    - Nand                              
                     "00000" + "00100" + "00000" + "00000" +        // C - 00011 - x-D-x    - local connection
                     "00000" + "00101" + "00110" + "00011" +        // D - 00100 - x-E-F    - Nand  
                     "00000" + "00101" + "00000" + "00000" +        // E - 00101 - x-E-x    - local connection  
                     "00000" + "00000" + "00000" + "00000" +        // F - 00110
                     "00000" + "00000" + "00000" + "00000" +        // G - 00111 
                     "00000" + "00000" + "00000" + "00000" +        // H - 01000 
                     "00000" + "00000" + "00000" + "00000" +        // I - 01001 
                     "00000" + "00000" + "00000" + "00000" +        // J - 01010 
                     "00000" + "00000" + "00000" + "00000" +        // K - 01011 
                     "00000" + "00000" + "00000" + "00000" +        // L - 01100 
                     "00000" + "00000" + "00000" + "00000" +        // M - 01101 
                     "00000" + "00000" + "00000" + "00000" +        // N - 01110 
                     "00000" + "00000" + "00000" + "00010";         // O - 01111 

      //string nodeTypes = "00000" + // a
      //                   "00011" + // b - Nand
      //                   "00001" + // c - local connection
      //                   "00011" + // d - Nand 
      //                   "00001" + // e - local connection
      //                   "00000" + // f
      //                   "00000" + // g 
      //                   "00000" + // h 
      //                   "00000" + // i 
      //                   "00000" + // j 
      //                   "00000" + // k 
      //                   "00000" + // l 
      //                   "00000" + // m 
      //                   "00000" + // n
      //                   "00010";  // o - targeted connection    

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestDoubleNandGate");

      string chromosomeString = rules + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\nand_gate.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = true;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // the outputs should be the same as the inputs
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(northInput, aNorthOutput);
      Assert.AreEqual(eastInput, aEastOutput);
      Assert.AreEqual(southInput, aSouthOutput);
      Assert.AreEqual(westInput, aWestOutput);

      northInput = false;
      eastInput = true;
      southInput = false;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");

      // the outputs should be the same as the inputs
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      Assert.AreEqual(northInput, aNorthOutput);
      Assert.AreEqual(eastInput, aEastOutput);
      Assert.AreEqual(southInput, aSouthOutput);
      Assert.AreEqual(westInput, aWestOutput);
    }


    [TestMethod]
    public void TestNandWithFeedback()
    {
      int gridSideLength = 15;

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "00010" + "00000" + "00000" + // A - 00001 - X-B-X
                     "00000" + "00011" + "00000" + "00000" + // B - 00010 - x-C-x  - local connection                                 
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - x-D-x  - local connection 
                     "00000" + "00101" + "00110" + "00011" + // D - 00100 - x-E-F  - Nand    
                     "00000" + "00101" + "00000" + "00000" + // E - 00101 - x-E-x  - local connection    
                     "00000" + "00000" + "00110" + "00000" + // F - 00110 - x-x-F 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000" + // N - 01110 
                     "00000" + "00000" + "00000" + "00010";  // O - 01111  

      string endRules = "00000" + "00000" + "00000" + "00000" + //16 - 10000
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
                        "00000" + "00000" + "00000" + "00000" + //30 - 11110
                        "00000" + "00000" + "00000" + "00000";  //31 - 11111



      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TestNandWithFeedback");

      string chromosomeString = rules + endRules;
      Chromosome chromosome = new Chromosome(chromosomeString);

      // register this node with the system event broker to allow event publication and subscription
      ArtificialLifeProperties.SystemEventBroker.Register(this);

      //Chromosome chromosome = new Chromosome(botbaseChromosome);
      BotBase bot = new BotBase(chromosome, null, gridSideLength, false);
      bot.SaveGrid(testDirectory + "\\nand_gate.bmp");

      bot.CreateProcessor(testDirectory);

      bool northInput = true;
      bool eastInput = false;
      bool southInput = true;
      bool westInput = false;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "FirstOutput.txt");

      // the outputs should be the same as the inputs
      bool aNorthOutput;
      bool aEastOutput;
      bool aSouthOutput;
      bool aWestOutput;
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      //Assert.AreEqual(northInput, aNorthOutput);
      //Assert.AreEqual(eastInput, aEastOutput);
      //Assert.AreEqual(southInput, aSouthOutput);
      //Assert.AreEqual(westInput, aWestOutput);

      northInput = false;
      eastInput = true;
      southInput = false;
      westInput = true;
      bot.Evaluate(northInput, eastInput, southInput, westInput, testDirectory, "SecondOutput.txt");

      // the outputs should be the same as the inputs
      bot.GetOutputs(out aNorthOutput, out aEastOutput, out aSouthOutput, out aWestOutput);
      //Assert.AreEqual(northInput, aNorthOutput);
      //Assert.AreEqual(eastInput, aEastOutput);
      //Assert.AreEqual(southInput, aSouthOutput);
      //Assert.AreEqual(westInput, aWestOutput);
    }



    #endregion Node Type Tests

    #region Directed Connection Tests

    static string[] twoBits = new string[4] { "00", "01", "10", "11" };

    //[TestMethod]
    //public void TestMethod1()
    //{
    //  string botbaseChromosome = "000001001" + // input   - x-G-B
    //                             "000000000" + // A - 000
    //                             "110011111" + // B - 001 - targeted connection - target = G
    //                             "000010000" + // C - 010 - x-C-x
    //                             "010000000" + // D - 011
    //                             "000000000" + // E - 100
    //                             "000000000" + // F - 101
    //                             "011010101" + // G - 110 - targeted connection - target = D
    //                             "000000000" + // H - 111
    //                             "000" + // a 
    //                             "010" + // b - targeted connection
    //                             "001" + // c - local connection
    //                             "100" + // d - delay
    //                             "000" + // e
    //                             "000" + // f
    //                             "010" + // g - targeted connection
    //                             "000";  // h                                

    //  int gridSideLength = 15;
    //  Chromosome chromosome = new Chromosome(botbaseChromosome);
    //  BotBase bot = new BotBase(chromosome, null, gridSideLength, true);
    //}


    /// <summary>
    /// create targeted connections straight from the input cells
    /// - any connections that would be placed in the central output area will be shortened until they are placed
    /// in an empty cell
    /// </summary>
    [TestMethod]
    public void TargetedConnectionLengthsMiddleOfGrid()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A
                     "00000" + "11111" + "00000" + "00000" + //1  - 00001 - X-O-X
                     "00000" + "00000" + "00000" + "00000" + //2  - 00010                                
                     "00000" + "00000" + "00000" + "00000" + //3  - 00011 
                     "00000" + "00000" + "00000" + "00000" + //4  - 00100  

                     "00000" + "00000" + "00000" + "00000" + //5  - 00101    
                     "00000" + "00000" + "00000" + "00000" + //6  - 00110 
                     "00000" + "00000" + "00000" + "00000" + //7  - 00111 
                     "00000" + "00000" + "00000" + "00000" + //8  - 01000 

                     "00000" + "00000" + "00000" + "00000" + //9  - 01001 
                     "00000" + "00000" + "00000" + "00000" + //10 - 01010 
                     "00000" + "00000" + "00000" + "00000" + //11 - 01011 
                     "00000" + "00000" + "00000" + "00000" + //12 - 01100 

                     "00000" + "00000" + "00000" + "00000" + //13 - 01101 
                     "00000" + "00000" + "00000" + "00000" + //14 - 01110 
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
                     

      string target = "11110"; //31 - 11111 - target = 30 (11110)
      string[] distances = new string[4] { "00", "01", "10", "11" };
      string areaAndDirection = "1110";
      string padding = "0000";
      string nodetype = "10000"; // - targeted connection

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("MiddleOfGrid");

      int[] expectedOutputPositions = new int[4] { 3, 5, 5, 11 };

      // test the 4 distances
      for (int index = 0; index < 4; index++)
      {
        string chromosomeString = rules + target + distances[index] + areaAndDirection + padding + nodetype;

        Chromosome chromosome = new Chromosome(chromosomeString);
        BotBase bot = new BotBase(chromosome, null, gridSideLength, false, Pruning.None);
        bot.SaveGrid(testDirectory + "\\grid_" + distances[index] + ".bmp");
        bot.SaveGridToTextFile(testDirectory + "\\grid_" + distances[index] + ".txt");

        CellType[,] grid = bot.GetGrid();
        Assert.AreEqual(CellType.ConnectionEnd, grid[expectedOutputPositions[index], 7]);
      }
    }

    /// <summary>
    /// create targeted connections straight from the input cells but with left and right directions
    /// </summary>
    [TestMethod]
    public void TargetedConnectionMiddleOfGridWithDirection()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A

                     "00000" + "11111" + "00000" + "00000" + // A - 00001 - X-O-X
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                                
                     "00000" + "00000" + "00000" + "00000" + // C - 00011 
                     "00000" + "00000" + "00000" + "00000" + // D - 00100   
                     "00000" + "00000" + "00000" + "00000" + // E - 00101    
                     "00000" + "00000" + "00000" + "00000" + // F - 00110 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000" + // N - 01110 

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

                                               
      string target = "01110"; // - 11111 - targeted connection - target = N
      string padding = "0000";
      string type = "10010";  // - targeted connection
                         

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("MiddleOfGridWithDirection");

      int[] expectedOutputPositions = new int[4] { 3, 5, 5, 11 };

      int[] expectedOutputPart1 = new int[4] { 3, 6, 9, 12 };
      int[] expectedOutputPart2 = new int[4] { 1, 2, 4, 4 };

      // test the 4 different connection distances
      for (int distance = 0; distance < 4; distance++)
      {
        // test for the 4 different target areas
        for (int area = 0; area < 4; area++)
        {
          // test for the 4 different connection directions
          foreach (RelativeDirection direction in Enum.GetValues(typeof(RelativeDirection)))
          {
            string chromosomeString = rules + target + twoBits[distance] + twoBits[area] + twoBits[(int)direction] + padding + type;
            Chromosome chromosome = new Chromosome(chromosomeString);
            
            BotBase bot = new BotBase(chromosome, null, gridSideLength, false, Pruning.None );


            StringBuilder sb = new StringBuilder("Processing: ");
            sb.AppendFormat("distance = {0}, area = {1} direction = {2}", distance, area, direction);
            bot.SaveGrid(testDirectory + "\\grid_" + twoBits[distance] + "_" + twoBits[area] + "_" + twoBits[(int)direction] + ".bmp");

            CellType[,] grid = bot.GetGrid();
            int row = 0;
            int col = 0;
            if( direction == RelativeDirection.Back || direction == RelativeDirection.Forward)
            {
              row = expectedOutputPositions[distance];
              col = 7;              
            }
            else if( direction == RelativeDirection.Left )
            {
              row = expectedOutputPart1[distance];
              col = 7 + expectedOutputPart2[area];              
            }
            else if( direction == RelativeDirection.Right )
            {            
              row = expectedOutputPart1[distance];
              col = 7 - expectedOutputPart2[area];              
            }

            Assert.AreEqual(CellType.ConnectionEnd, grid[row, col], sb.ToString());
          }
        }
      }
    }



    /// <summary>
    /// Move to the right of the input cell, to avoid the central area, and then send targeted connections
    /// across the grid. These will be evenly spaced, apart from the last connection which will be pulled back
    /// by one square to allow its output to fit.    
    /// </summary>
    [TestMethod]
    public void TargetedConnectionLengthsFullGrid()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A

                     "00000" + "00000" + "00011" + "00000" + // A - 00001 - X-X-C - right targeted connection
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                                
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
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00000" + // N - 01110 

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

      string target = "01110"; // - 11111 - targeted connection - target = N
      string[] distances = new string[4] { "00", "01", "10", "11" };
      string areaAndDirection = "1110";
      string padding = "0000";
      string type = "10010";  // - targeted connection

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("FullGrid");

      // the expected outputs are at equal positions across the grid apart from the last which is 
      // pulled back by one cell.
      int[] expectedOutputPositions = new int[4] { 3, 6, 9, 11 };

      for (int index = 0; index < 4; index++)
      {
        //string chromosomeString = rules + target + distances[index] + areaAndDirection + nodeTypes;
        string chromosomeString = rules + target + distances[index] + areaAndDirection + padding + type;

        Chromosome chromosome = new Chromosome(chromosomeString);
        BotBase bot = new BotBase( chromosome, null, gridSideLength, false, Pruning.None );
        bot.SaveGrid(testDirectory + "\\grid_" + distances[index] + ".bmp");

        CellType[,] grid = bot.GetGrid();
        Assert.AreEqual(CellType.ConnectionEnd, grid[expectedOutputPositions[index], 5]);
      }
    }

    /// <summary>
    /// Add target nodes to the grid and test that the targeted connections find them
    /// - empty cells are used (nodes A & B) - these have output connections, but don't produce a connection themselves
    /// - if they were instead set to be local connections (type = 00001) they would move one cell before branching
    /// </summary>
    [TestMethod]
    public void TargetedConnectionWithTargets()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection of node type C (which grows into a targeted connection)
      // local connection C -> local connection D -> left targeted connection H


      string rules = "00001" + // north input  - A
                     "00001" + // east input   - A
                     "00001" + // south input  - A
                     "00001" + // west input   - A

                     "00011" + "00000" + "00010" + "00000" + // A - 00001 - C-X-B - empty cell
                     "00000" + "00101" + "00000" + "00000" + // B - 00010 - x-E-x - empty cell
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - x-D-x - local forward connection    
                     "00000" + "00000" + "11111" + "00001" + // D - 00100 - x-x-O - right targeted connection    
                     "00000" + "00110" + "00000" + "00001" + // E - 00101 - x-F-x - local forward connection        
                     "00000" + "00000" + "00000" + "00011" + // F - 00110 - node (nand) 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00011" + // N - 01110 - target node (nand) 

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

                     

      string target = "01110"; // 11111 - targeted connection - target = N
      string[] distances = new string[4] { "00", "01", "10", "11" };
      string area      = "11";
      string direction = "10";
      string padding = "0000";
      string type = "10010";  // o - targeted connection    
                       

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("ConnectionWithTargets");

      // the expected outputs are at equal positions across the grid 
      // for index == 3 (longest connections) - these should find the Nand gates that already exist in the grid
      // for the shorter connections no Nand gates exist in the target areas so they should create their own
      int[] expectedOutputPositions = new int[4] { 3, 6, 9, 12 };

      // test each of the possible 4 distances that a targeted connection can have
      for (int index = 0; index < 4; index++)
      {
        string chromosomeString = rules + target + distances[index] + area + direction + padding + type;

        Chromosome chromosome = new Chromosome(chromosomeString);
        BotBase bot = new BotBase(chromosome, null, gridSideLength, false, Pruning.None);
        bot.SaveGrid(testDirectory + "\\testgrid_" + distances[index] + ".bmp");

        CellType[,] grid = bot.GetGrid();
        Assert.AreEqual(CellType.ConnectionEnd, grid[expectedOutputPositions[index], 9]);
      }
    }

    /// <summary>
    /// Test that targeted connections connect to the output nodes when these are set as the targets
    /// </summary>
    [TestMethod]
    public void TargetedOutputConnections()
    {
      int gridSideLength = 15;

      string rules = "11111" + // north input  - 
                     "11111" + // east input   - 
                     "11111" + // south input  - 
                     "11111" + // west input   - 

                     "00000" + "00000" + "00000" + "00000" + // A - 00001
                     "00000" + "00000" + "00000" + "00000" + // B - 00010                                
                     "00000" + "00000" + "00000" + "00000" + // C - 00011    
                     "00000" + "00000" + "00000" + "00000" + // D - 00100    
                     "00000" + "00000" + "00000" + "00000" + // E - 00101    
                     "00000" + "00000" + "00000" + "00000" + // F - 00110 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
                     "00000" + "00000" + "00000" + "00001" + // N - 01110 

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
                                                  


      string target = "01110"; // 11111 - targeted connection - target = N
      string[] distances = new string[4] { "00", "01", "10", "11" };
      string area = "11";
      string direction = "10"; // forward
      string padding = "0000";
      //string type = "00010"; 
      string type = "10000"; // this node is a targeted connection


                              
      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("TargetedOutputConnections");

      // the expected outputs are at equal positions across the grid 
      // for index == 3 (longest connections) - these should find the Nand gates that already exist in the grid
      // for the shorter connections no Nand gates exist in the target areas so they should create their own
      int[] expectedOutputPositions = new int[4] { 3, 6, 9, 12 };

      // test each of the possible 4 distances that a targeted connection can have
      for (int index = 0; index < 4; index++)
      //int index = 3;
      {
        string chromosomeString = rules + target + distances[index] + area + direction + padding + type;

        Chromosome chromosome = new Chromosome(chromosomeString);
        BotBase bot = new BotBase(chromosome, null, gridSideLength, false, Pruning.None);
        bot.SaveGrid(testDirectory + "\\grid_" + distances[index] + ".bmp");

        CellType[,] grid = bot.GetGrid();
        //Assert.AreEqual(CellType.ConnectionEnd, grid[expectedOutputPositions[index], 9]);
      }
    }


    /// <summary>
    /// Explicitly add target nodes to the grid (rather than growing them) and test that the targeted connections find them
    /// - tests straight ahead connections
    /// </summary>
    [TestMethod]
    public void TargetedConnectionWithPlacedTargets()
    {
      int gridSideLength = 15;

      // Chromosome length = 241 = (4x4) input cells + (15x3x4) rules + (15x3) cell types
      // - a rule of 0000 is now considered to be empty, no rule
      // - a input cell of 0000 is now no input
      // - rule definitions therefore begin at 0001 = A up to 1111 = O

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00010" + // north input  - B
                     "00010" + // east input   - B
                     "00010" + // south input  - B
                     "00010" + // west input   - B

                     "00000" + "00000" + "00000" + "00000" + // A - 00001
                     "00000" + "00000" + "00011" + "00000" + // B - 00010 - X-X-C                               
                     "00000" + "00100" + "00000" + "00000" + // C - 00011 - X-D-X - local forward connection
                     "11111" + "00000" + "00000" + "00000" + // D - 00100 - O-X-X - left targeted connection  
                     "00000" + "00000" + "00000" + "00000" + // E - 00101    
                     "00000" + "00000" + "00000" + "00000" + // F - 00110 
                     "00000" + "00000" + "00000" + "00000" + // G - 00111 
                     "00000" + "00000" + "00000" + "00000" + // H - 01000 
                     "00000" + "00000" + "00000" + "00000" + // I - 01001 
                     "00000" + "00000" + "00000" + "00000" + // J - 01010 
                     "00000" + "00000" + "00000" + "00000" + // K - 01011 
                     "00000" + "00000" + "00000" + "00000" + // L - 01100 
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
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
                        

      string target = "01110"; // 11111 - targeted connection - target = N
      string[] distances = new string[4] { "00", "01", "10", "11" };
      string areaAndDirection = "1110";
      string padding = "0000";
      string type = "10010";  // - targeted connection
                        

      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("PlacedTargets");

      // test the 4 different connection distances
      for (int index = 0; index < 4; index++)
      {
        string chromosomeString = rules + target + distances[index] + areaAndDirection + padding + type;
        Chromosome chromosome = new Chromosome(chromosomeString);

        for (int placedRow = 3; placedRow < 14; placedRow++)
        {
          // move the target cell to the new row
          CellDefinition[] placedTargets = new CellDefinition[]
          {
            new CellDefinition(placedRow,5,CellType.EastDelay),  // connection from top
            new CellDefinition(9,placedRow,CellType.NorthDelay), // connection from left
            new CellDefinition((14-placedRow),9,CellType.WestDelay),  // connection from bottom
            new CellDefinition(5,(14-placedRow),CellType.SouthDelay), // connection from right
          };

          BotBase bot = new BotBase(chromosome, null, gridSideLength, false, Pruning.None, placedTargets);
          bot.SaveGrid(testDirectory + "\\grid_" + distances[index] + "_row" + placedRow + ".bmp");

          CellType[,] grid = bot.GetGrid();
          switch (index)
          {
            case 0: // short connections

              if (placedRow < 5)
              {
                CheckConnectionEnds(placedRow, grid);
              }
              break;
            case 1: // medium-short connections
              if (placedRow >= 5 && placedRow < 8)
              {
                CheckConnectionEnds(placedRow, grid);
              }
              break;
            case 2: // medium-long connections
              if (placedRow >= 8 && placedRow < 11)
              {
                CheckConnectionEnds(placedRow, grid);
              }
              break;
            case 3: // long connections
              if (placedRow >= 11)
              {
                CheckConnectionEnds(placedRow, grid);
              }
              break;
          }
        }
      }
    }

    private static void CheckConnectionEnds(int placedRow, CellType[,] grid)
    {
      // the connection end just join with the placed node in the first rows of the grid
      Assert.AreEqual(CellType.ConnectionEnd, grid[placedRow - 1, 5]);
      Assert.AreEqual(CellType.ConnectionEnd, grid[9, placedRow - 1]);
      Assert.AreEqual(CellType.ConnectionEnd, grid[5, 14 - (placedRow - 1)]);
      Assert.AreEqual(CellType.ConnectionEnd, grid[14 - (placedRow - 1), 9]);
    }


    /// <summary>
    /// Test one chromosome that produces targeted connections
    /// - used for checking the output grid drawing
    /// </summary>
    [TestMethod]
    public void TargetedSingleTargetedConnection()
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
                     "00000" + "00000" + "00000" + "00000" + // M - 01101 
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

      string target  = "01110"; // 11111 - targeted connection - target = N
      string padding = "0000";
      string type    = "10010"; // - targeted connection 


      // create a directory for the output of this test
      string testDirectory = CreateTestDirectory("SingleTargetedConnection");
      int distance = 1;
      int area = 2;
      int direction = 1;
      int placedRow = 6;
      int placedCol = 4;

      string chromosomeString = rules + target + twoBits[distance] + twoBits[area] + twoBits[direction] + padding + type;
      Chromosome chromosome = new Chromosome(chromosomeString);
      PlaceAndTest(gridSideLength, testDirectory, distance, area, direction, chromosome, placedCol, placedRow);
    }


    /// <summary>
    /// Explicitly add target nodes to the grid (rather than growing them) and test that the targeted connections find them
    /// - tests connections that are to the right or left of the connection direction
    /// </summary>
    [TestMethod]
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
              }
            }
            else
            if (direction == 3)
            {
              // place cells to the left when testing left connection direction
              for (int placedCol = 0; placedCol < 4; placedCol++)
              {
                PlaceCellsAndTest(gridSideLength, testDirectory, distance, area, direction, chromosome, placedCol);
              }
            }
          }
        }
      }
    }


    /// <summary>
    /// Explicitly target the output nodes of the grid and test that the targeted connections find them
    /// - tests connections that are to the right or left of the connection direction
    /// </summary>
    [TestMethod]
    public void TargetedConnectionWithRightOrLeftOutputTargets()
    {
      int gridSideLength = 15;

      // move, using local connections, to the right, to avoid the central section, then send out targeted connections
      // input has RH targeted connection, of node type C

      string rules = "00010" + // north input  - B
                     "00010" + // east input   - B
                     "00010" + // south input  - B
                     "00010" + // west input   - B

                     "00000" + "00000" + "00000" + "00000" +                                       // A - 0001
                     "00000" + "00000" + "00011" + "00000" +                                       // B - 0010 - X-X-C                               
                     "00000" + "00100" + "00000" + "00001" +                                       // C - 0011 - X-D-X - local forward connection
                     "11111" + "00000" + "00000" + "00001" +                                       // D - 0100 - 31-X-X - left targeted connection  

                     "00000" + "00000" + "00000" + "00000" +                                       // E - 0101    
                     "00000" + "00000" + "00000" + "00000" +                                       // F - 0110 
                     "00000" + "00000" + "00000" + "00000" +                                       // G - 0111 
                     "00000" + "00000" + "00000" + "00000" +                                       // H - 1000 

                     "00000" + "00000" + "00000" + "00000" +                                       // I - 1001 
                     "00000" + "00000" + "00000" + "00000" +                                       // J - 1010 
                     "00000" + "00000" + "00000" + "00000" +                                       // K - 1011 
                     "00000" + "00000" + "00000" + "00000" +                                       // L - 1100 

                     "00000" + "00000" + "00000" + "00000" +                                       // M - 1101 - NorthSouth (output connection)
                     "00000" + "00000" + "00000" + "00001" +                                       // N - 1110 - targeted connection   
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
                                                
      string target = "01110"; // 31 - 11111 - targeted connection - target = N
      string padding = "0000";
      string type = "10010";

      // create a directory for the output of this test
      string testName = "RightOrLeftOutputTargets";
      string testDirectory = CreateTestDirectory(testName);
      string expectedDirectory = GetExpectedDirectory(testName);

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

            BotBase bot = new BotBase(chromosome, null, gridSideLength, true, (Pruning.All & ~Pruning.UnjoinedConnections));

            string botname = twoBits[distance] + "_" + twoBits[area] + "_" + twoBits[direction];
            bot.SaveGrid(testDirectory + "\\grid_" + botname + ".bmp");

            // test if the new grid matches with the expected
            string expectedStructure = expectedDirectory + "\\" + botname + ".txt";
            BotBase expectedBot = new BotBase(expectedStructure);
            if (CompareGrids(expectedBot.GetGrid(), bot.GetGrid(), gridSideLength) == false)
            {
              // the grids didn't match

              // save the text representation of the grid
              string testStructure = testDirectory + "\\" + botname + ".txt";
              bot.SaveGridToTextFile(testStructure);

              Assert.Fail("The generated grid did not match the expected: " + botname);
            }
          }
        }
      }
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
              bot.SaveGrid(testDirectory + "\\grid_" + twoBits[distance] + "_" + twoBits[area] + "_" + twoBits[direction] + "_row" + placedRow + "_col" + placedCol + ".bmp");
              CheckConnectionEndsRight(placedCol, placedRow, grid, sb);
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
                bot.SaveGrid(testDirectory + "\\grid_" + twoBits[distance] + "_" + twoBits[area] + "_" + twoBits[direction] + "_row" + placedRow + "_col" + placedCol + ".bmp");
                CheckConnectionEndsLeft(placedRow, placedCol, grid, sb);
              }
            }
          }
        }
    }

    private static void CheckConnectionEndsLeft(int placedRow, int placedCol, CellType[,] grid, StringBuilder sb)
    {
      // the connection end just join with the placed node in the first rows of the grid
      Assert.AreEqual(CellType.ConnectionEnd, grid[placedRow, 5 - placedCol], sb.ToString());
      Assert.AreEqual(CellType.ConnectionEnd, grid[9 + placedCol, placedRow], sb.ToString());
      Assert.AreEqual(CellType.ConnectionEnd, grid[5 - placedCol, 14 - placedRow], sb.ToString());
      Assert.AreEqual(CellType.ConnectionEnd, grid[14 - placedRow, 9 + placedCol], sb.ToString());
    }

    private static void CheckConnectionEndsRight(int placedCol, int placedRow, CellType[,] grid, StringBuilder sb)
    {
      Assert.AreEqual(CellType.ConnectionEnd, grid[placedRow, 5 + placedCol], sb.ToString());
      Assert.AreEqual(CellType.ConnectionEnd, grid[9 - placedCol, placedRow], sb.ToString());
      Assert.AreEqual(CellType.ConnectionEnd, grid[5 + placedCol, 14 - placedRow], sb.ToString());
      Assert.AreEqual(CellType.ConnectionEnd, grid[14 - placedRow, 9 - placedCol], sb.ToString());
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
    /// Return the name of the directory containing the expected results
    /// </summary>
    /// <param name="aTestName"></param>
    /// <returns></returns>
    private static string GetExpectedDirectory(string aTestName)
    {
      string currentDirectory = Directory.GetCurrentDirectory();
      int lastIndex = currentDirectory.LastIndexOf('\\');
      lastIndex = currentDirectory.LastIndexOf('\\', lastIndex - 1);

      string rootDirectory = currentDirectory.Substring(0, lastIndex);
      string testDirectory = rootDirectory + "\\" + "expected\\" + aTestName;
      if (Directory.Exists(testDirectory) == false)
      {
        Directory.CreateDirectory(testDirectory);
      }

      return testDirectory;
    }


    private static bool CompareGrids(CellType[,] aExpectedGrid, CellType[,] aGrid, int aGridSideLength)
    {
      if (aExpectedGrid != null && aGrid != null)
      {
        for (int row = 0; row < aGridSideLength; row++)
        {
          for (int col = 0; col < aGridSideLength; col++)
          {
            if (aGrid[row, col] != aExpectedGrid[row, col])
            {
              return false;
            }
          }
        }
        return true;
      }

      return false;
    }

    #endregion Directed Connection Tests

  }  
}
