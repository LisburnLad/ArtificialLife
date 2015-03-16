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

namespace WindowsFormsApplication1
{
  public partial class Maze : Form
  {
    Bitmap Backbuffer;

    Bitmap Topbuffer;

    const int BallAxisSpeed = 2;

    Point BallPos = new Point(30, 30);
    Point BallSpeed = new Point(BallAxisSpeed, BallAxisSpeed);
    const int BallSize = 50;

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

      int width = ClientSize.Width/ 20;
      int height = ClientSize.Height/ 20;

      Backbuffer = new Bitmap( (width * 20)+2, (height *20)-1);

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

        int width = (Backbuffer.Width - 2) / 20;
        int height = (Backbuffer.Height - 2) / 20;

        Pen dashPen = new Pen( Color.DarkGray, 1 );
        dashPen.DashStyle = DashStyle.Dash;

        // draw grid lines
        for(int row = 0; row <= 21; row++)
        {
          //if(row < (20 - 1))
          {
            graphics.DrawLine( dashPen, 0, (row * height), Backbuffer.Width, (row * height) );
          }

          for(int col = 0; col <= 21; col++)
          {
            int x = (col * width);
            int y = (row * height);

            if(row == 0)
            {
              graphics.DrawLine( dashPen, x, 0, x, Backbuffer.Height );
            }
          }
        }
      }
    }

    void Draw( int aRow, int aCol )
    {
      if(Topbuffer != null)
      {
        int width = (Topbuffer.Width - 2) / 20;
        int height = (Topbuffer.Height - 2) / 20;

        using(var g = Graphics.FromImage( Topbuffer ))
        {
          g.Clear( Color.White );

          using(Pen pen = new Pen( Color.Black, 2 ))
          {
            Brush brush = new SolidBrush( Color.Red );

            g.FillRectangle( brush, (aCol * width), (aRow * height), width, height );  // redraws background
            g.DrawRectangle( pen, (aCol*width), (aRow*height), width, height );            

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
      int sideLength3 = 11;
      string testChromosome4 = "111101001001000101110000011011011110111";

      Chromosome chromosome = new Chromosome( testChromosome4 );
      CreateAndEvaluateBot( chromosome, sideLength3, 2, true, (TestBotHandler)botEventHandler );
    }

    /// <summary>
    /// generate and evaluate a bot using the supplied chromosome
    /// </summary>
    /// <param name="chromosome"></param>
    /// <returns></returns>
    private static void CreateAndEvaluateBot( Chromosome chromosome, int aSideLength, int aBotType, bool aShowGrid, TestBotHandler botEventHandler )
    {
      // create the test that the bot is to perform
      //Test currentTest = new TestStraightLineMove();
      //Test currentTest = new TestDiagonalMove();
      //Test currentTest = new TestForMoveAndStop();
      //Test currentTest = new TestForFastMoveAndStop();
      Test currentTest = new TestForMoveThereAndBack();
      
      // create the bot
      Bot bot = new Bot( chromosome, currentTest, aSideLength, aBotType, aShowGrid );
      bot.BotEvent += botEventHandler;
      
      // see how this bot gets on with the supplied test
      bot.InitializeOutput();

      for(int pass = 0; pass < 20; pass++)
      {
        bot.TestBotForOnePass( aShowGrid, pass );

        Thread.Sleep( 1000 );
      }
    }



    delegate void OnBotEventDelegate( object sender, BotEvent aEvent );
    public void OnBotEvent( object sender, BotEvent aEvent)
    {
      // make sure we're on the UI thread
      if(this.InvokeRequired == false)
      {
        Draw( aEvent.Row, aEvent.Col );
      }
      else
      {
        OnBotEventDelegate botEventDelegate = new OnBotEventDelegate( OnBotEvent );
        this.BeginInvoke( botEventDelegate, new object[] { sender, aEvent } );
      }
    }
  }
}
