using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

/*NOTE:
*Clearing lines doesnt work consitently sometimes some of the squares arent deleted
*Rotating doesnt work yet
*Peices can be clipped into other peices if you move them into the side of them
*/
namespace Tetris
{
    public partial class GameScreen : UserControl
    {
        List<Rectangle> grid = new List<Rectangle>();
        static List<Square> movingPieces = new List<Square>();
        List<Square> landedPieces = new List<Square>();

        //the first grid square is at the starting values plus the square size
        int startX = 70;
        int startY = 40;
        int squareSize = 30;
        int multiplier = 1;
        int score = 0;
        Random random = new Random();

        bool up, down, left, right;
        bool canMoveRight, canMoveLeft;
        //bool leave = false;

        Pen whitePen = new Pen(Color.White);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush greenBrush = new SolidBrush(Color.Green);

        SoundPlayer blockDrop = new SoundPlayer(Properties.Resources.BlockDrop);
        SoundPlayer lineClear = new SoundPlayer(Properties.Resources.LineClear);

        public GameScreen()
        {
            InitializeComponent();
            GameInitialize();
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    up = true;
                    break;
                case Keys.Down:
                    down = true;
                    break;
                case Keys.Left:
                    left = true;
                    break;
                case Keys.Right:
                    right = true;
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    up = false;
                    break;
                case Keys.Down:
                    down = false;
                    break;
                case Keys.Left:
                    left = false;
                    break;
                case Keys.Right:
                    right = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            #region moving pieces
            List<Rectangle> tempMovingList = new List<Rectangle>();

            //adding the controlled peice to a temporary list
            foreach (Square s in movingPieces)
            {
                Rectangle tempRec = new Rectangle(s.x, s.y, 30, 30);
                tempMovingList.Add(tempRec);
            }

            if (left)
            {
                //moving the temporary rectangles in the desired direction
                bool tempLeft = false;
                for (int i = 0; ++i < tempMovingList.Count;)
                {
                    int tempX = tempMovingList[i].X - 30;
                    tempMovingList[i] = new Rectangle(tempX, tempMovingList[i].Y, 30, 30);
                }

                //checking if the temporary rectangles collided with anything
                foreach (Rectangle r in tempMovingList)
                {
                    foreach (Square s in landedPieces)
                    {
                        //if not allow movement otherwise no
                        Rectangle landedRec = new Rectangle(s.x, s.y, 30, 30);
                        if (r.IntersectsWith(landedRec) && tempLeft)
                        {
                            tempLeft = false;
                            break;
                        }
                        else
                        {
                            tempLeft = true;
                        }
                    }
                }

                if (tempLeft)
                {
                    canMoveLeft = true;
                }
            }

            else if (right)
            {
                bool tempRight = false;
                for (int i = 0; ++i < tempMovingList.Count;)
                {
                    int tempX = tempMovingList[i].X + 30;
                    tempMovingList[i] = new Rectangle(tempX, tempMovingList[i].Y, 30, 30);
                }

                for (int i = 0; ++i < tempMovingList.Count;)
                {
                    for (int j = 0; j < landedPieces.Count; j++)
                    {
                        Rectangle landedRec = new Rectangle(landedPieces[j].x, landedPieces[j].y, 30, 30);
                        if (tempMovingList[i].IntersectsWith(landedRec) && tempRight)
                        {
                            tempRight = false;
                            break;
                        }
                        else
                        {
                            tempRight = true;
                        }
                    }
                }

                if (tempRight)
                {
                    canMoveRight = true;
                }
            }


            foreach (Square s in movingPieces)
            {
                Rectangle movingRecLeft = new Rectangle(s.x - 5, s.y, 5, 30);
                Rectangle movingRecRight = new Rectangle(s.x + 30, s.y, 5, 30);
                if (movingRecLeft.X <= 100)
                {
                    canMoveLeft = false;
                    break;
                }
                else
                {
                    canMoveLeft = true;
                }

                if (movingRecRight.X >= 400)
                {
                    canMoveRight = false;
                    break;
                }
                else
                {
                    canMoveRight = true;
                }
            }

            //might work if you can figure out whats wrong
            /*
            foreach (Square s in movingPieces)
            {
                movingRecLeft = new Rectangle(s.x - 5, s.y, 5, 30);
                movingRecRight = new Rectangle(s.x + 30, s.y, 5, 30);
                for (int j = 0; j < landedPieces.Count; j++)
                {
                    //checking if the piece can move left
                    if (leave == false)
                    {
                        Rectangle landedRec = new Rectangle(landedPieces[j].x, landedPieces[j].y, 30, 30);
                        if (movingRecLeft.IntersectsWith(landedRec))
                        {
                            canMoveLeft = false;
                            leave = true;
                            break;
                        }
                        else
                        {
                            canMoveLeft = true;
                        }
                    }
                }

                for (int j = 0; j < landedPieces.Count; j++)
                {
                    //checking if the piece can move right
                    if (leave == false)
                    {
                        Rectangle landedRec = new Rectangle(landedPieces[j].x, landedPieces[j].y, 30, 30);
                        if (movingRecRight.IntersectsWith(landedRec))
                        {
                            canMoveRight = false;
                            leave = true;
                            break;
                        }
                        else
                        {
                            canMoveRight = true;
                        }
                    }
                }

                if (movingRecLeft.X <= 100)
                {
                    canMoveLeft = false;
                    leave = true;
                    break;
                }
                else
                {
                    canMoveLeft = true;
                    leave = false;
                }

                if (movingRecRight.X >= 400)
                {
                    canMoveRight = false;
                    leave = true;
                    break;
                }
                else
                {
                    canMoveRight = true;
                    leave = false;
                }
                Refresh();
            }
            */


            foreach (Square s in movingPieces)
            {
                if (left && canMoveLeft)
                {
                    s.MovePiece("left", 1, movingPieces);
                }
                else if (right && canMoveRight)
                {
                    s.MovePiece("right", 1, movingPieces);
                }

                if (s.y < 640)
                {
                    s.y += 30;
                }

                if (up)
                {
                    s.MovePiece("up", 1, movingPieces);
                }
            }
            //leave = false;
            #endregion

            #region floor and piece collisons
            for (int i = 0; i < movingPieces.Count; i++)
            {
                Rectangle movingRec = new Rectangle(movingPieces[i].x, movingPieces[i].y + 30, 30, 30);
                for (int j = 0; j < landedPieces.Count; j++)
                {
                    //checking for intersection between every square in the controlled piece and all landed squares
                    Rectangle landedRec = new Rectangle(landedPieces[j].x, landedPieces[j].y, 30, 30);
                    if (landedRec.IntersectsWith(movingRec))
                    {
                        blockDrop.Play();
                        CreatePiece();
                        if (landedPieces[j].y <= 100)
                        {
                            GameOver();
                        }
                        break;
                    }
                }

                if (movingPieces[i].y == 640)
                {
                    blockDrop.Play();
                    CreatePiece();
                    break;
                }
            }
            #endregion

            #region line clear
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    int counter = 0;
                    foreach (Square s in landedPieces)
                    {
                        //checking how many peices are on each line
                        if (s.y == startY)
                        {
                            counter++;
                        }

                        //clearing the line if its full
                        if (counter >= 10)
                        {
                            for (int j = 0; j < landedPieces.Count; j++)
                            {
                                if (landedPieces[j].y == startY)
                                {
                                    landedPieces.RemoveAt(j);
                                }
                            }

                            //moving all pieces above the cleared line down 1 line
                            foreach (Square l in landedPieces)
                            {
                                if (l.y < startY)
                                {
                                    l.y += 30;
                                }
                            }
                            score++;
                            lineClear.Play();
                        }
                    }
                }
                catch
                {
                    break;
                }
                startY += 30;
            }

            startY = 70;
            #endregion

            Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.DrawRectangle(whitePen, movingRecLeft);
            //e.Graphics.DrawRectangle(whitePen, movingRecRight);

            foreach (Rectangle r in grid)
            {
                e.Graphics.DrawRectangle(whitePen, r.X, r.Y, r.Width, r.Height);
            }

            foreach (Square s in movingPieces)
            {
                e.Graphics.FillRectangle(redBrush, s.x, s.y, 30, 30);
                e.Graphics.DrawRectangle(whitePen, s.x, s.y, 30, 30);
            }

            foreach (Square l in landedPieces)
            {
                e.Graphics.FillRectangle(greenBrush, l.x, l.y, 30, 30);
            }

            if (up)
            {
                e.Graphics.RotateTransform(90);
            }

            ScoreLabel.Text = score + "";
        }

        void GameInitialize()
        {
            //each grid square is 30 pixels X 30 pixels
            for (int i = 0; i < 200; i++)
            {
                //creating each grid square and adding it to a list
                if (grid.Count % 10 == 0)
                {
                    startY += squareSize;
                    multiplier = 1;
                    Rectangle square = new Rectangle(startX + squareSize * multiplier, startY, squareSize, squareSize);
                    grid.Add(square);
                }
                else
                {
                    Rectangle square = new Rectangle(startX + squareSize * multiplier, startY, squareSize, squareSize);
                    grid.Add(square);
                }
                multiplier++;
            }

            CreatePiece();

            startY = 70;
        }

        void CreatePiece()
        {
            for (int k = 0; k < movingPieces.Count; k++)
            {
                landedPieces.Add(movingPieces[k]);
            }

            movingPieces.Clear();

            int piece = random.Next(1, 8);

            if (piece == 1) //T-Block
            {
                Square newSquare1 = new Square(250, 70);
                Square newSquare2 = new Square(220, 100);
                Square newSquare3 = new Square(250, 100);
                Square newSquare4 = new Square(280, 100);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
            else if (piece == 2) //L-block
            {
                Square newSquare1 = new Square(220, 70);
                Square newSquare2 = new Square(220, 100);
                Square newSquare3 = new Square(220, 130);
                Square newSquare4 = new Square(250, 130);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
            else if (piece == 3) //J-Block
            {
                Square newSquare1 = new Square(250, 70);
                Square newSquare2 = new Square(250, 100);
                Square newSquare3 = new Square(250, 130);
                Square newSquare4 = new Square(220, 130);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
            else if (piece == 4) //O-Block
            {
                Square newSquare1 = new Square(220, 70);
                Square newSquare2 = new Square(220, 100);
                Square newSquare3 = new Square(250, 70);
                Square newSquare4 = new Square(250, 100);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
            else if (piece == 5) //S-Block
            {
                Square newSquare1 = new Square(250, 70);
                Square newSquare2 = new Square(220, 70);
                Square newSquare3 = new Square(220, 100);
                Square newSquare4 = new Square(190, 100);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
            else if (piece == 6) //Z-Block
            {
                Square newSquare1 = new Square(220, 70);
                Square newSquare2 = new Square(250, 70);
                Square newSquare3 = new Square(250, 100);
                Square newSquare4 = new Square(280, 100);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
            else if (piece == 7) //I-Block
            {
                Square newSquare1 = new Square(190, 70);
                Square newSquare2 = new Square(220, 70);
                Square newSquare3 = new Square(250, 70);
                Square newSquare4 = new Square(280, 70);
                movingPieces.Add(newSquare1);
                movingPieces.Add(newSquare2);
                movingPieces.Add(newSquare3);
                movingPieces.Add(newSquare4);
            }
        }

        void GameOver()
        {
            Application.Exit();
        }
    }
}