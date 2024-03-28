using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    internal class Square
    {
        public int size = 30;
        public int x, y;

        public Square(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public void MovePiece(string direction, int pieceNum, List<Square> pieceList)
        {
            string piece = "";
            if (direction == "left")
            {
                x -= 30;
            }
            else if (direction == "right")
            {
                x += 30;
            }
        }
    }
}