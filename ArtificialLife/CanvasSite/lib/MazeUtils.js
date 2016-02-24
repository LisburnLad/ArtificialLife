// namespace
var ArtificialLife = ArtificialLife || {};

// check that Object.create exists
// - if it doesn't then form our own version of it
if( typeof Object.create !== "function" )
{
  Object.create = function ( o )
  {
    function F() { }
    F.prototype = o;
    return new F();
  };
}


ArtificialLife.MazeUtils =
{
  // extension function to allow inheritance
  extend: function ( props )
  {
    var prop, obj;
    obj = Object.create( this );
    for( prop in props )
    {
      if( props.hasOwnProperty( prop ) )
      {
        obj[prop] = props[prop];
      }
    }
    return obj;
  },

  // the number of cells in the maze
  mazeWidthCells: 21,
  mazeHeightCells: 21,

  // the pixel of each cell
  cellWidth: 20,
  cellHeight: 20,

  // when set true grid numbers will be applied
  addCellNumbers: true,

  // the width of the margin at the top and left of the grid when grid numbering is on
  marginWidth: 30,

  // canvas draw parameters
  borderWidth: 2,
  mazeBodyColor: 'SteelBlue',
  mazeBorderColor: 'black',
  mazeTextColor: 'black',

  // bot parameters
  updateSpeed: 5,  // number of cells per second that the bot moves

  botBodyColor: 'Yellow',
  botBorderColor: 'black'
}