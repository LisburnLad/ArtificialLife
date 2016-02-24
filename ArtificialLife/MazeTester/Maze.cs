using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArtificialLife;
using GAF;
using System.IO;

namespace WindowsFormsApplication1
{
  public partial class Maze : Form
  {
    /// <summary>
    /// Delay in hundredths of a second between each bot move in the grid
    /// </summary>
    static int kBotDelay = 200; 


    Bitmap Backbuffer;

    Bitmap Topbuffer;

    const int BallAxisSpeed = 2;

    Point BallPos = new Point(30, 30);
    Point BallSpeed = new Point(BallAxisSpeed, BallAxisSpeed);
    const int BallSize = 50;

    /// <summary>
    /// 21 cells in each direction (0 - 20)
    /// </summary>
    const int kGridSize = 21;

    public Maze()
    {
      InitializeComponent();

      this.SetStyle(
      ControlStyles.UserPaint |
      ControlStyles.AllPaintingInWmPaint |
      ControlStyles.DoubleBuffer, true);

      this.ResizeEnd += new EventHandler(Maze_CreateBackBuffer);
      this.Load += new EventHandler(Maze_CreateBackBuffer);
      this.Paint += new PaintEventHandler(Maze_Paint);

      //this.KeyDown += new KeyEventHandler(Maze_KeyDown);


      TestBotHandler botEventHandler = new TestBotHandler( OnBotEvent );

      Thread getIPCapabilitiesThread = new Thread( EvaluateSpecificChromosome );
      //getIPCapabilitiesThread.Name = aThreadIndex.ToString();
      getIPCapabilitiesThread.Start( botEventHandler );

      //EvaluateSpecificChromosome();
    }

    //void Maze_KeyDown(object sender, KeyEventArgs e)
    //{
    //    if (e.KeyCode == Keys.Left)
    //        BallSpeed.X = -BallAxisSpeed;
    //    else if (e.KeyCode == Keys.Right)
    //        BallSpeed.X = BallAxisSpeed;
    //    else if (e.KeyCode == Keys.Up)
    //        BallSpeed.Y = -BallAxisSpeed; // Y axis is downwards so -ve is up.
    //    else if (e.KeyCode == Keys.Down)
    //        BallSpeed.Y = BallAxisSpeed;
    //}

    public Bitmap Superimpose( Bitmap largeBmp, Bitmap smallBmp )
    {
      Graphics g = Graphics.FromImage( largeBmp );
      g.CompositingMode = CompositingMode.SourceOver;
      smallBmp.MakeTransparent();
      int margin = 5;
      int x = largeBmp.Width - smallBmp.Width - margin;
      int y = largeBmp.Height - smallBmp.Height - margin;
      g.DrawImage( smallBmp, new Point( 0, 0 ) );
      return largeBmp;
    }

    void Maze_Paint(object sender, PaintEventArgs e)
    {
      CreateBitmapGrid();
      Bitmap bitmap = Superimpose( Backbuffer, Topbuffer );

      e.Graphics.DrawImageUnscaled( bitmap, (ClientSize.Width - Backbuffer.Width)/2, (ClientSize.Height - Backbuffer.Height)/2 );

      //if(Backbuffer != null)
      //{
      //  e.Graphics.DrawImageUnscaled( Backbuffer, Point.Empty );
      //}

      //if(Topbuffer != null)
      //{
      //  e.Graphics.CompositingMode = CompositingMode.SourceOver;
      //  e.Graphics.DrawImageUnscaled( Topbuffer, Point.Empty );
      //}
    }

    void Maze_CreateBackBuffer(object sender, EventArgs e)
    {
      if(Backbuffer != null)
      {
        Backbuffer.Dispose();
      }

      int width = ClientSize.Width / (kGridSize - 1);
      int height = ClientSize.Height / (kGridSize - 1);

      Backbuffer = new Bitmap((width * (kGridSize - 1)) + 2, (height * (kGridSize - 1)) - 1);

      if(Topbuffer != null)
      {
        Topbuffer.Dispose();
      }

      Topbuffer = new Bitmap( Backbuffer.Width, Backbuffer.Height );
      Topbuffer.MakeTransparent( Color.White );

      CreateBitmapGrid();
    }

    private void CreateBitmapGrid()
    {
      using(var graphics = Graphics.FromImage( Backbuffer ))
      {
        graphics.Clear( Color.White );

        int width = (Backbuffer.Width - (kGridSize-1)) / kGridSize;
        int height = (Backbuffer.Height - (kGridSize - 1)) / kGridSize;

        Pen dashPen = new Pen( Color.DarkGray, 1 );
        dashPen.DashStyle = DashStyle.Dash;

        // draw grid lines
        for(int row = 0; row <= (kGridSize+1); row++)
        {
          if( row > 0 )
          {
            // add the horizontal grid line
            graphics.DrawLine(dashPen, width, (row * height), (Backbuffer.Width-14), (row * height));
          }

          for (int col = 0; col <= (kGridSize + 1); col++)
          {
            int x = (col * width);
            int y = (row * height);

            if (row == 0 && col > 0 && col < (kGridSize + 1))
            {
              Rectangle rect = new Rectangle(x, y, width, height);
              graphics.SmoothingMode = SmoothingMode.AntiAlias;
              graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
              graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
              
              StringFormat drawFormat = new StringFormat();
              drawFormat.Alignment = StringAlignment.Center;
              drawFormat.LineAlignment = StringAlignment.Far;
              graphics.DrawString((col - 1).ToString(), new Font("Courier New", 7), Brushes.Black, rect, drawFormat);

              graphics.Flush();
            }
            else
            {
              if (col == 0 && row > 0 && row < (kGridSize + 1))
              {
                Rectangle rect = new Rectangle(x, y, width, height);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Far;
                drawFormat.LineAlignment = StringAlignment.Center;
                graphics.DrawString((row - 1).ToString(), new Font("Courier New", 7), Brushes.Black, rect, drawFormat);

                graphics.Flush();
              }
              else
              {
                if (row == 1)
                {
                  // add the vertical grid lines
                  graphics.DrawLine(dashPen, x, y, x, Backbuffer.Height - y);
                }
              }
            }
          }
        }
      }
    }

    void Draw( int aRow, int aCol )
    {
      if(Topbuffer != null)
      {
        int width = (Topbuffer.Width - (kGridSize - 1)) / kGridSize;
        int height = (Topbuffer.Height - (kGridSize - 1)) / kGridSize;

        using(var g = Graphics.FromImage( Topbuffer ))
        {
          g.Clear( Color.White );

          using(Pen pen = new Pen( Color.Black, 2 ))
          {
            Brush brush = new SolidBrush( Color.Red );

            int x = ((aCol+1) * width);
            int y = ((aRow+1) * height);

            g.FillRectangle( brush, x, y, width, height );  // redraws background
            g.DrawRectangle( pen, x + 1, y + 1, width - 2, height - 2 );            

            brush.Dispose();
          }
        }

        Invalidate();
      }
    }


    /// <summary>
    /// function used for testing of a specific chromosome
    /// </summary>
    public static void EvaluateSpecificChromosome( object botEventHandler )
    {
      // chromosome to solve "TestForMoveAndStop"
      //string specificChromosome = "110000101101100101010111010111010011100001010101011111110010001100011101100000011000011101101000010000010101011001111011011100101001100011101001001000111" +
      //                            "1111110111001010001101101010101001001110110110101110001110111000010101111001100100010101";

      // TestForFastMoveAndStop
      //string specificChromosome = "010101010101000111000100111101011100001101011000011001011010101001100101110111001011010110111111010000000111110001101110001100101111100101100101001110000" +
      //                            "1110000000001111001010111010101101110011001101011110001001100110111001000100000101101001";

      // TestForMoveThereAndBack
      //string specificChromosome = "100011100101110111010000111110001110110110010000001101001001110000100011110101000111011001010011001100000001011010100011011011000100010011100011100011000" +
      //                            "1010100100101101001000000111100001001110001110100000111001010101111011011000100101001001";

      // TestRunningInCircles
      //string specificChromosome = "101110101110101111100111100001011101111110001010110010110010111010000100001011111101001110011011100100111111110000100011011101010110000001100111010011101" +
      //                            "1011100110001000010000011110101101111101110110010001010100101111000010001110011001001000";

      // TestEatCheese
      string specificChromosome1 = "001010001100001101101011001111011101101000011011111111101010111010110011010001000001101100111000110000101110100101111000110000010110111111100111101011101" +
                                  "1100011100100011101001100001000010110100000100110011100110100010111101001100101000010101";

      string specificChromosome2 = "011111111000100000100011110011000100100001001110101110110110011100010010001110011001010100110110110010000111101110100010001110001010011110000101001100110" +
                                  "1100110101011110010100101100000110001001110100100001111000110101011001110000000110010111";

      string specificChromosome3 = "110001011111010010111001101011100100001011110100101101100001110010110010011010100000000011101001101110111001101000001100000100101010011011110010000111010" +
                                   "0100000000111110011110111100001101101010101001010011011111010101011110100100101100111000";

      string specificChromosome4 = "011110001000010110000110011000001110001001100011000101010011101010001101010000100011001000100111101000100010100011111001110100111100111100111011100000011" +
                                   "00001001111001000010011010110001001011101000100000000101110001001001110011011010111101011010101001000100010111100011101";


      string rules = "0001" + // north input  - A
                     "0000" + // east input   - 
                     "0001" + // south input  - A
                     "0101" + // west input   - H
                     "0000" + "0000" + "0010" + // A - 0001 - X-X-B
                     "0000" + "0011" + "0000" + // B - 0010 - X-C-X
                     "1110" + "0111" + "0000" + // C - 0011 - N-G-X
                     "0000" + "1010" + "0000" + // D - 0100 - X-J-X 
                     "0000" + "1000" + "0000" + // E - 0101 - X-H-X 
                     "0000" + "1111" + "0000" + // F - 0110 - X-O-X
                     "0100" + "1001" + "0000" + // G - 0111 - D-I-X
                     "0000" + "0101" + "0000" + // H - 1000 - X-E-X
                     "1101" + "0000" + "0000" + // I - 1001 - M-X-X
                     "0000" + "1111" + "0000" + // J - 1010 - X-O-X
                     "0010" + "0000" + "0000" + // K - 1011 - B-X-X
                     "0000" + "0000" + "0000" + // L - 1100 

                     "0110" + "11" + "10" + "01" + "00" + // M - 1101 - [F,dist,area,dir,padding]

                     "0000" + "0110" + "0000";  // N - 1110 - X-F-X 

      string target = "0010"; // O - 1111 - targeted connection - target = B = local connection (= Output Cell for targeted connection)
      string distance = "01";  // medium-short
      string area = "01";      // medium-short
      string direction = "01"; // left
      string padding = "00";

      string nodeTypes = "000" + // a 
                         "001" + // b - local connection 
                         "001" + // c - local connection
                         "001" + // d - local connection
                         "001" + // e - local connection  
                         "110" + // f - trigger  
                         "000" + // g - 
                         "101" + // h - 
                         "000" + // i - 
                         "001" + // j - local connection  
                         "100" + // k - delay
                         "000" + // l - 
                         "010" + // m - targeted connection  
                         "001" + // n - targeted connection  
                         "010";  // o - targeted connection  

      string specificChromosome = rules + target + distance + area + direction + padding + nodeTypes;
      //string specificChromosome = rules + nodeTypes;

      Chromosome chromosome = new Chromosome(specificChromosome4);
      double score = CreateAndEvaluateBot(chromosome, true, (TestBotHandler)botEventHandler);
    }


    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private static double CreateAndEvaluateBot(Chromosome chromosome, bool aShowGrid, TestBotHandler botEventHandler)
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
      bot.BotEvent += botEventHandler;
      
      // see how this bot gets on with the supplied test
      return bot.Evaluate(aShowGrid, @"C:\Temp\ArtificialLife\tests\Evolution", kBotDelay);
    }

    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    //private static void CreateAndEvaluateBot( Chromosome chromosome, int aSideLength, int aBotType, bool aShowGrid, TestBotHandler botEventHandler )
    //{
    //  // create the test that the bot is to perform
    //  //Test currentTest = new TestStraightLineMove();
    //  //Test currentTest = new TestDiagonalMove();
    //  //Test currentTest = new TestForMoveAndStop();
    //  //Test currentTest = new TestForFastMoveAndStop();
    //  //Test currentTest = new TestForMoveThereAndBack();
    //  Test currentTest = new TestRunningInCircles();
      
    //  // create the bot
    //  Bot bot = new Bot( chromosome, currentTest, aSideLength, aBotType, aShowGrid );
    //  bot.BotEvent += botEventHandler;
      
    //  // see how this bot gets on with the supplied test
    //  bot.InitializeOutput();

    //  for(int pass = 0; pass < 40; pass++)
    //  {
    //    bot.TestBotForOnePass( aShowGrid, pass );

    //    Thread.Sleep( 500 );
    //  }
    //}



    delegate void OnBotEventDelegate( object sender, BotEvent aEvent );
    public void OnBotEvent( object sender, BotEvent aEvent)
    {      
      // make sure we're on the UI thread
      if(this.InvokeRequired == false)
      {
        this.Text = "Pass: " + aEvent.Step + " x: " + aEvent.Col + " y: " + aEvent.Row;

        Draw( aEvent.Row, aEvent.Col );
        
        string filename = @"C:\Temp\ArtificialLife\tests\Evolution" + "\\Positions.txt";
        using (TextWriter writer = new StreamWriter(filename, (aEvent.Step > 0)))
        {
          writer.WriteLine("{0} : {1},{2}", aEvent.Step, aEvent.Row, aEvent.Col);
        }
      }
      else
      {
        OnBotEventDelegate botEventDelegate = new OnBotEventDelegate( OnBotEvent );
        this.BeginInvoke( botEventDelegate, new object[] { sender, aEvent } );
      }
    }
  }
}
