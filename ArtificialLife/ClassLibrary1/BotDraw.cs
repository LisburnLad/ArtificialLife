using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ArtificialLife
{
  class BotDraw
  {
    BotStructure itsBotStructure;

    public int kSideLength 
    { 
      get
      {
        return itsBotStructure.kSideLength;
      }
    }

    CellType[,] itsGrid
    {
      get
      {
        return itsBotStructure.itsGrid;
      }
    }

    /// <summary>
    /// the directed connections that have been defined on the grid
    /// </summary>
    List<DirectedConnection> itsDirectedConnections
    {
      get
      {
        return itsBotStructure.itsDirectedConnections;
      }
    }

    public BotDraw( BotStructure aBotStructure )
    {
      itsBotStructure = aBotStructure;
    }

    /// <summary>
    /// write the structure of the grid to the console
    /// </summary>
    public  void ShowGrid()
    {
      Console.WriteLine();

      for (int row = 0; row < kSideLength; row++)
      {
        Console.Write("   ");

        for (int col = 0; col < kSideLength; col++)
        {
          Console.Write(Common.GetCellTypeChar(itsGrid[row, col]));

          Console.Write(" ");
        }
        Console.WriteLine();
      }
      Console.WriteLine("____________");

      // create an image of this grid
      //CreateGridImage(aImageName);
    }


    public void CreateGridImage(string aImageName)
    {
      int pixelLength = (kSideLength * 32) + 1;

      Bitmap bitmap = new Bitmap(pixelLength, pixelLength);
      Graphics graphics = Graphics.FromImage(bitmap);

      graphics.FillRectangle(Brushes.White, 0, 0, pixelLength, pixelLength);

      Pen linePen = new Pen(Color.Black, 2);
      linePen.StartCap = LineCap.Square;
      linePen.EndCap = LineCap.Square;

      Pen dashPen = new Pen(Color.DarkGray, 2);
      dashPen.DashStyle = DashStyle.Dash;

      Pen connectionPen = new Pen(Color.DarkBlue, 2);
      connectionPen.DashStyle = DashStyle.Dot;

      HatchBrush hb = new HatchBrush(HatchStyle.BackwardDiagonal, Color.White, Color.LightGray);
      HatchBrush intputBrush = new HatchBrush(HatchStyle.SmallCheckerBoard, Color.White, Color.DarkBlue);
      HatchBrush outputBrush = new HatchBrush(HatchStyle.SmallCheckerBoard, Color.White, Color.DarkGreen);

      // draw grid lines
      for (int row = 0; row < kSideLength; row++)
      {
        if (row == (kSideLength / 2))
        {
          // set input cells
          graphics.FillRectangle(outputBrush, (pixelLength / 2) - 48, (pixelLength - 32) / 2, 32, 32);
          graphics.FillRectangle(outputBrush, (pixelLength / 2) + 16, (pixelLength - 32) / 2, 32, 32);
        }

        if (row < (kSideLength - 1))
        {
          // draw horizontal grid lines
          graphics.DrawLine(dashPen, 0, (row * 32) + 32, pixelLength, (row * 32) + 32);
        }

        for (int col = 0; col < kSideLength; col++)
        {
          int x = (col * 32);
          int y = (row * 32);

          if (col == (kSideLength / 2))
          {
            // set input cells
            graphics.FillRectangle(outputBrush, (pixelLength - 32) / 2, (pixelLength / 2) - 48, 32, 32);
            graphics.FillRectangle(outputBrush, (pixelLength - 32) / 2, (pixelLength / 2) + 16, 32, 32);
          }

          if (row == 0)
          {
            // draw vertical grid lines
            graphics.DrawLine(dashPen, x, 0, x, pixelLength);
          }

          // add shading to outer cols
          if (col == 0 || col == (kSideLength - 1))
          {
            graphics.FillRectangle(hb, x, 0, 32, pixelLength);

            // set input cells
            graphics.FillRectangle(intputBrush, x, (pixelLength - 32) / 2, 32, 32);
          }
        }

        // add shading to outer rows 
        if (row == 0 || row == (kSideLength - 1))
        {
          graphics.FillRectangle(hb, 0, (row * 32), pixelLength, 32);

          // set input cells
          graphics.FillRectangle(intputBrush, (pixelLength - 32) / 2, (row * 32), 32, 32);
        }
      }

      foreach(var connection in itsDirectedConnections)
      {
        // test if the connection is to a row below the current
        if (connection.itsEnd.X > connection.itsStart.X)
        {          
          // southerly connection
          int x1 = (connection.itsStart.Y * 32) + 16;
          int y1 = (connection.itsStart.X * 32) + 16;
          int x2 = (connection.itsStart.Y * 32) + 16;

          // test if the start and end are on the same column
          if( connection.itsEnd.Y == connection.itsStart.Y )
          {            
            int y2 = (connection.itsEnd.X * 32);
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }          
          else if( connection.itsEnd.Y < connection.itsStart.Y )
          {
            x1 = (connection.itsStart.Y * 32);
            y1 = (connection.itsStart.X * 32) + 16;
            x2 = (connection.itsEnd.Y * 32) + 16;

            int y2 = y1;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);

            x1 = x2;
            y1 = (connection.itsEnd.X * 32);
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
          else if( connection.itsEnd.Y > connection.itsStart.Y )
          {            
            int y2 = (connection.itsEnd.X * 32) + 16;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);

            x1 = (connection.itsEnd.Y * 32);
            y1 = y2;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
        }
        else if (connection.itsEnd.X < connection.itsStart.X)
        {
          // northerly connection
          int x1 = (connection.itsStart.Y * 32) + 16;
          int y1 = (connection.itsStart.X * 32);
          int x2 = (connection.itsStart.Y * 32) + 16;          

          // test if the start and end are on the same column
          if (connection.itsEnd.Y == connection.itsStart.Y)
          {
            int y2 = (connection.itsEnd.X * 32) + 16;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
          else if (connection.itsEnd.Y < connection.itsStart.Y)
          {
            int y2 = (connection.itsEnd.X * 32) + 16;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);

            x1 = (connection.itsEnd.Y * 32);
            y1 = y2;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
          else if (connection.itsEnd.Y > connection.itsStart.Y)
          {
            x1 = (connection.itsStart.Y * 32) + 32;
            y1 = (connection.itsStart.X * 32) + 16;
            x2 = (connection.itsEnd.Y * 32) + 16; 

            int y2 = y1;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);

            x1 = x2;
            y1 = (connection.itsEnd.X * 32) + 32;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
        }
        else if (connection.itsEnd.Y > connection.itsStart.Y)
        {
          // easterly connection
          int x1 = (connection.itsStart.Y * 32) + 16;
          int y1 = (connection.itsStart.X * 32) + 16;          
          int y2 = (connection.itsStart.X * 32) + 16;

          // test if the start and end are on the same column
          if (connection.itsEnd.X == connection.itsStart.X)
          {
            int x2 = (connection.itsEnd.Y * 32);
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
          //else if (connection.itsEnd.Y < connection.itsStart.Y)
          //{
          //  int x2 = (connection.itsEnd.Y * 32) +16;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);

          //  y1 = (connection.itsEnd.X * 32) + 16;
          //  x1 = x2;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          //}
          //else if (connection.itsEnd.Y > connection.itsStart.Y)
          //{
          //  int x2 = (connection.itsEnd.Y * 32) + 16;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);

          //  y1 = (connection.itsEnd.X * 32);
          //  x1 = x2;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          //}
        }
        else if (connection.itsEnd.Y < connection.itsStart.Y)
        {
          // easterly connection
          int x1 = (connection.itsStart.Y * 32);
          int y1 = (connection.itsStart.X * 32) + 16;
          int y2 = (connection.itsStart.X * 32) + 16;                   

          // test if the start and end are on the same column
          if (connection.itsEnd.X == connection.itsStart.X)
          {
            int x2 = (connection.itsEnd.Y * 32) + 16;
            graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          }
          //else if (connection.itsEnd.Y < connection.itsStart.Y)
          //{
          //  int x2 = (connection.itsEnd.Y * 32) + 16;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);

          //  y1 = (connection.itsEnd.X * 32) + 16;
          //  x1 = x2;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          //}
          //else if (connection.itsEnd.Y > connection.itsStart.Y)
          //{
          //  int x2 = (connection.itsEnd.Y * 32);
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);

          //  y1 = (connection.itsEnd.X * 32);
          //  x1 = x2;
          //  graphics.DrawLine(connectionPen, x1, y1, x2, y2);
          //}
        }
      }

      for (int row = 0; row < kSideLength; row++)
      {
        for (int col = 0; col < kSideLength; col++)
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
            case CellType.NorthNand:
              DrawNorthNand(graphics, linePen, x, y);
              break;
            case CellType.EastNand:
              DrawEastNand(graphics, linePen, x, y);
              break;
            case CellType.SouthNand:
              DrawSouthNand(graphics, linePen, x, y);
              break;
            case CellType.WestNand:
              DrawWestNand(graphics, linePen, x, y);
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

            case CellType.NorthXor:
              DrawNorthXor(graphics, linePen, x, y);
              break;
            case CellType.EastXor:
              DrawEastXor(graphics, linePen, x, y);
              break;
            case CellType.SouthXor:
              DrawSouthXor(graphics, linePen, x, y);
              break;
            case CellType.WestXor:
              DrawWestXor(graphics, linePen, x, y);
              break;

            case CellType.NorthTrigger:
              DrawNorthTrigger(graphics, linePen, x, y);
              break;
            case CellType.EastTrigger:
              DrawEastTrigger(graphics, linePen, x, y);
              break;
            case CellType.SouthTrigger:
              DrawSouthTrigger(graphics, linePen, x, y);
              break;
            case CellType.WestTrigger:
              DrawWestTrigger(graphics, linePen, x, y);
              break;

            case CellType.NorthOr:
              DrawNorthOr(graphics, linePen, x, y);
              break;
            case CellType.EastOr:
              DrawEastOr(graphics, linePen, x, y);
              break;
            case CellType.SouthOr:
              DrawSouthOr(graphics, linePen, x, y);
              break;
            case CellType.WestOr:
              DrawWestOr(graphics, linePen, x, y);
              break;

            case CellType.NorthPulse:
              DrawNorthPulse(graphics, linePen, x, y);
              break;
            case CellType.EastPulse:
              DrawEastPulse(graphics, linePen, x, y);
              break;
            case CellType.SouthPulse:
              DrawSouthPulse(graphics, linePen, x, y);
              break;
            case CellType.WestPulse:
              DrawWestPulse(graphics, linePen, x, y);
              break;

            case CellType.ConnectionStart:
              DrawConnectionStart(graphics, linePen, x,y, row,col );
              break;

            case CellType.ConnectionEnd:
              DrawConnectionEnd(graphics, linePen, x,y,row, col);
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
    private static void DrawNand(Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints)
    {
      using (SolidBrush brush = new SolidBrush(Color.FromArgb(0x7F, 0x92, 0xFF)))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddCurve(linePoints);
        path.AddLines(sidePoints);
        graphics.FillPath(brush, path);
      }

      // draw the top curve
      graphics.DrawCurve(linePen, linePoints);

      // draw the sides        
      graphics.DrawLines(linePen, sidePoints);
    }


    private void DrawConnectionStart(Graphics graphics, Pen linePen, int x, int y, int aRow, int aCol )
    {
      // search the list of targeted connections for this start point
      Point start = new Point(aRow, aCol);
      int index = 0;
      for (; index < itsBotStructure.itsDirectedConnections.Count; index++ )
      {
        if (itsBotStructure.itsDirectedConnections[index].itsStart == start)
        {
          break;
        }
      }

      DrawTargetedConnection(graphics, linePen, x, y, Color.Yellow, index);
    }

    private void DrawConnectionEnd(Graphics graphics, Pen linePen, int x, int y, int aRow, int aCol)
    {
      // search the list of targeted connections for this end point
      Point end = new Point(aRow, aCol);
      int index = 0;
      for (; index < itsBotStructure.itsDirectedConnections.Count; index++)
      {
        if (itsBotStructure.itsDirectedConnections[index].itsEnd == end)
        {
          break;
        }
      }

      DrawTargetedConnection(graphics, linePen, x, y, Color.Orange, index);
    }

    private static void DrawTargetedConnection(Graphics graphics, Pen linePen, int x, int y, Color aColor, int aIndex)
    {
      int size = 32;
     
      using (SolidBrush brush = new SolidBrush(aColor))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddEllipse(x, y, size, size);
        graphics.FillPath(brush, path);
        graphics.DrawEllipse(linePen, x, y, size, size);

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        StringFormat format = new StringFormat();
        format.LineAlignment = StringAlignment.Center;
        format.Alignment = StringAlignment.Center;

        Rectangle rect = new Rectangle(x, y, size, size);

        // add the connection index, making it 1 based rather than 0 based
        graphics.DrawString((aIndex+1).ToString(), new Font("Tahoma", 10, FontStyle.Bold), new SolidBrush(Color.Black), rect, format);
        
        graphics.Flush();
      }
    }


    /// <summary>
    /// Draw the main shape for a delay node
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="linePen"></param>
    /// <param name="linePoints"></param>
    /// <param name="sidePoints"></param>
    private static void DrawDelay(Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints)
    {
      using (SolidBrush brush = new SolidBrush(Color.FromArgb(0x00, 0x9B, 0x0E)))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddLines(linePoints);
        path.AddLines(sidePoints);
        graphics.FillPath(brush, path);

        // draw the top curve
        graphics.DrawLines(linePen, linePoints);

        // draw the sides        
        graphics.DrawLines(linePen, sidePoints);
      }
    }

    private static void DrawXOR(Graphics graphics, Pen linePen, Point[] linePoints, Point[] sidePoints, Color aColor)
    {
      using (SolidBrush brush = new SolidBrush(aColor))
      {
        // draw the shading for the top curve
        GraphicsPath path = new GraphicsPath();
        path.AddLines(linePoints);
        path.AddLines(sidePoints);
        graphics.FillPath(brush, path);

        // draw the top curve
        graphics.DrawLines(linePen, linePoints);

        // draw the sides        
        graphics.DrawLines(linePen, sidePoints);
      }
    }


    private static Point[] GetNorthSidePoints(int x, int endX, int midY, int endY)
    {
      Point[] sidePoints = { 
                               new Point(x, midY),                               
                               new Point(x, endY),                               
                               new Point(endX, endY),
                               new Point(endX, midY)                    
                             };
      return sidePoints;
    }

    private static Point[] GetEastSidePoints(int x, int y, int midX, int endY)
    {
      Point[] sidePoints = { 
                               new Point(midX, y),
                               new Point(x, y),                               
                               new Point(x, endY),
                               new Point(midX, endY)                    
                             };
      return sidePoints;
    }

    private static Point[] GetSouthSidePoints(int x, int y, int endX, int midY)
    {
      Point[] sidePoints = { 
                             new Point(x, midY),                               
                             new Point(x, y),                               
                             new Point(endX, y),
                             new Point(endX, midY)                    
                           };
      return sidePoints;
    }

    private static Point[] GetWestSidePoints(int y, int midX, int endX, int endY)
    {
      Point[] sidePoints = { 
                               new Point(midX, y),                               
                               new Point(endX, y),                               
                               new Point(endX, endY),
                               new Point(midX, endY)                    
                             };
      return sidePoints;
    }

    private static void DrawWestNand(Graphics graphics, Pen linePen, int x, int y)
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

      Point[] sidePoints = GetWestSidePoints(y, midX, endX, endY);

      DrawNand(graphics, linePen, linePoints, sidePoints);

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse(linePen, x, (midY) - radius, (radius * 2), (radius * 2));
    }

    private static void DrawEastNand(Graphics graphics, Pen linePen, int x, int y)
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

      Point[] sidePoints = GetEastSidePoints(x, y, midX, endY);

      DrawNand(graphics, linePen, linePoints, sidePoints);

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse(linePen, (endX) - (radius * 2), (midY) - radius, (radius * 2), (radius * 2));
    }

    private static void DrawNorthNand(Graphics graphics, Pen linePen, int x, int y)
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

      Point[] sidePoints = GetNorthSidePoints(x, endX, midY, endY);

      DrawNand(graphics, linePen, linePoints, sidePoints);

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse(linePen, (midX) - radius, y, (radius * 2), (radius * 2));
    }

    private static void DrawSouthNand(Graphics graphics, Pen linePen, int x, int y)
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

      Point[] sidePoints = GetSouthSidePoints(x, y, endX, midY);

      DrawNand(graphics, linePen, linePoints, sidePoints);

      // draw the top circle
      int radius = 4;
      graphics.DrawEllipse(linePen, (midX) - radius, (endY) - (radius * 2), (radius * 2), (radius * 2));
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
                              new Point(midX, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints(y, midX, endX, endY);

      DrawDelay(graphics, linePen, linePoints, sidePoints);
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
                               new Point(midX, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints(x, y, midX, endY);

      DrawDelay(graphics, linePen, linePoints, sidePoints);
    }



    private static void DrawNorthDelay(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints(x, endX, midY, endY);

      DrawDelay(graphics, linePen, linePoints, sidePoints);
    }


    private static void DrawSouthDelay(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY)                       
                             };

      Point[] sidePoints = GetSouthSidePoints(x, y, endX, midY);

      DrawDelay(graphics, linePen, linePoints, sidePoints);
    }




    ///
    /// Exclusive-OR Gates
    /// 


    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="linePen"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private static void DrawWestXor(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                              new Point(midX, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints(y, midX, endX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.FromArgb(0x9B, 0x00, 0x0E));
    }


    private static void DrawEastXor(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints(x, y, midX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.FromArgb(0x9B, 0x00, 0x0E));
    }



    private static void DrawNorthXor(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints(x, endX, midY, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.FromArgb(0x9B, 0x00, 0x0E));
    }


    private static void DrawSouthXor(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY)                       
                             };

      Point[] sidePoints = GetSouthSidePoints(x, y, endX, midY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.FromArgb(0x9B, 0x00, 0x0E));
    }


    private static void DrawWestTrigger(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                              new Point(midX, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints(y, midX, endX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkOrange);
    }


    private static void DrawEastTrigger(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints(x, y, midX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkOrange);
    }



    private static void DrawNorthTrigger(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints(x, endX, midY, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkOrange);
    }


    private static void DrawSouthTrigger(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY)                       
                             };

      Point[] sidePoints = GetSouthSidePoints(x, y, endX, midY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkOrange);
    }


    //
    // OR Nodes
    //


    private static void DrawWestOr(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                              new Point(midX, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints(y, midX, endX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkTurquoise);
    }


    private static void DrawEastOr(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints(x, y, midX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkTurquoise);
    }



    private static void DrawNorthOr(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints(x, endX, midY, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkTurquoise);
    }


    private static void DrawSouthOr(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY)                       
                             };

      Point[] sidePoints = GetSouthSidePoints(x, y, endX, midY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.DarkTurquoise);
    }





    private static void DrawWestPulse(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                              new Point(midX, y ),                               
                              new Point(x     , midY - 1),
                              new Point(x     , midY),
                              new Point(midX, endY - 1)                       
                            };

      Point[] sidePoints = GetWestSidePoints(y, midX, endX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.Silver);
    }


    private static void DrawEastPulse(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(midX, y ),
                               new Point(endX, y + mid - 1),
                               new Point(endX, y + mid),
                               new Point(midX, endY - 1)                       
                             };

      Point[] sidePoints = GetEastSidePoints(x, y, midX, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.Silver);
    }



    private static void DrawNorthPulse(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, y ),
                               new Point(midX, y ),
                               new Point(endX - 1, midY)
                             };

      Point[] sidePoints = GetNorthSidePoints(x, endX, midY, endY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.Silver);
    }


    private static void DrawSouthPulse(Graphics graphics, Pen linePen, int x, int y)
    {
      int mid = 16;
      int width = 32;
      int midX = x + mid;
      int endX = x + width;
      int midY = y + mid;
      int endY = y + width;

      // array of points for top curve
      Point[] linePoints = { 
                               new Point(x     , midY),
                               new Point(midX - 1, endY),
                               new Point(midX, endY),
                               new Point(endX - 1, midY)                       
                             };

      Point[] sidePoints = GetSouthSidePoints(x, y, endX, midY);

      DrawXOR(graphics, linePen, linePoints, sidePoints, Color.Silver);
    }  

  }
}
