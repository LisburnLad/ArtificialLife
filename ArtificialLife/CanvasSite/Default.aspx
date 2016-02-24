<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE HTML>
<html>
  <head>
    <title>Canvas Test</title>

    <script type="text/javascript" src="lib/MazeUtils.js"></script>
    <script type="text/javascript" src="lib/CanvasCreate.js"></script>
    <script type="text/javascript" src="lib/MazeDraw.js"></script>
    <script type="text/javascript" src="lib/BotDraw.js"></script>    
    <script type="text/javascript" src="lib/MazeRunner.js"></script>
    
    

    <style>
      body 
      {
        margin: 0px;
        padding: 0px;
      }

			canvas 
      {
				position: absolute;
				top: 0px;
				left: 0px;
				background: transparent;
			}

			.maze
      {
				z-index: -1;
			}
			.bot 
      {
				z-index: 0;
			}

		</style>


  </head> 
  <body>

    <div id="CanvasContainer"></div>

    <script>
      var mazeRunner = ArtificialLife.MazeRunner.extend();
      mazeRunner.init( "CanvasContainer" );
    </script>

  </body>
</html>      
