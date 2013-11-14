using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DifficultyEstimator
{
    //stupidly simple
    //uses the Steps metric
    //TODO: Modify so that it uses pieces, not cells.
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Going to solve the puzzle...");
            byte[] puzzle={
                              1,2,3,4,
                              5,6,7,8,
                              9,10,11,12,
                              13,14,15,0};
            byte[] goal ={
                             0,0,0,0,
                             0,0,0,0,
                             0,0,0,0,
                             0,0,0,1};
            
            Globals.x = 4;
            Globals.y = 4;
            Globals.xy=16;
            Globals.numPieces = 15;

            int sum=0;
            int temp;
            for (int i = 0; i < 10; i++)
            {
                temp=SolvePuzzle(puzzle, goal);
                Console.WriteLine(temp);
                sum+=temp;
            }
            Console.WriteLine("Done! Average is {0}", (float)sum / 10.0f);
        }

        static int SolvePuzzle(byte[] puzzle, byte[] goal)
        {
            int moveCount = 0;
            Random rand = new Random();
            List<byte[]> movesMade = new List<byte[]>();
            byte[] currentState=new byte[Globals.xy];
            puzzle.CopyTo(currentState,0);

            List<List<byte[]>> possibleMoves=new List<List<byte[]>>(); //it's a stack of lists

            IntArrayComparer areq=new IntArrayComparer();

            while(!IsSolved(currentState,goal)){
                //
                if (possibleMoves.Count == moveCount) //if we just got here
                {
                    possibleMoves.Add(GetMoves(currentState));
                }
                //fancyprint(currentState);
                for (int i = possibleMoves[moveCount].Count-1; i >=0; i--) 
                    if (movesMade.Contains(possibleMoves[moveCount][i],areq)) 
                        possibleMoves[moveCount].RemoveAt(i);

                //If there is no move to make, go back
                if (possibleMoves[moveCount].Count == 0)
                {
                    movesMade.Last().CopyTo(currentState, 0);
                    //movesMade.RemoveAt(movesMade.Count - 1);
                    possibleMoves.RemoveAt(moveCount);
                    moveCount--;
                }
                else
                {
                    //WE NEED TO GO DEEPER
                    //randomly, that is.
                    int randVal = rand.Next(possibleMoves[moveCount].Count);
                    //randVal = 0;
                    byte[] temp = new byte[Globals.xy];
                    currentState.CopyTo(temp, 0);
                    movesMade.Add(temp);
                    possibleMoves[moveCount][randVal].CopyTo(currentState,0);
                    possibleMoves[moveCount].RemoveAt(randVal);
                    moveCount++;
                }
            }

            return moveCount;
        }

        static void fancyprint(byte[] board)
        {
            
            for (int y = 0; y < Globals.y; y++)
            {
                if (y == 0)
                {
                    Console.Write('{');
                }
                else
                {
                    Console.Write(' ');
                }
                for (int x = 0; x < Globals.x; x++)
                {
                    Console.Write(board[x+Globals.x*y]);
                    if ((x == Globals.x - 1) && (y == Globals.y - 1))
                    {
                        Console.Write('}');
                    }
                    else
                    {
                        Console.Write(',');
                    }
                }
                Console.WriteLine();
            }
        }

        static bool IsSolved(byte[] board, byte[] goal)
        {
            for (int i = 0; i < Globals.xy; i++)
            {
                if (goal[i] != 0 && board[i] != goal[i]) return false;
            }
            return true;
        }

        static List<byte[]> GetMoves(byte[] board)
        {
            List<byte[]> results = new List<byte[]>();

            for (byte p = 1; p < Globals.numPieces; p++)
            {
                if (CanMove(board, p, 0, -1)) results.Add(MovePiece(board, p, 0, -1));
                if (CanMove(board, p, 0, 1)) results.Add(MovePiece(board, p, 0, 1));
                if (CanMove(board, p, -1, 0)) results.Add(MovePiece(board, p, -1,0));
                if (CanMove(board, p, 1,0)) results.Add(MovePiece(board, p,1,0));
            }

            return results;
        }

        static byte[] MovePiece(byte[] board, byte piece, int dx, int dy)
        {
            byte[] result = new byte[Globals.xy]; //this sneakily fills up with 0s automatically.
            int offset = dx + Globals.x * dy;

            for (int i = 0; i < Globals.xy; i++)
            {
                if (board[i] == piece)
                {
                    result[i + offset] = board[i];
                }
                else if(board[i]!=0)
                {
                    result[i] = board[i];
                }
            }
            return result;
        }

        static bool CanMove(byte[] board, byte piece, int dx, int dy)
        {
            for (int i = 0; i < Globals.xy; i++)
            {
                if (board[i] == piece)
                {
                    //test edges
                    if ((i % Globals.x + dx < 0) || (i % Globals.x + dx >= Globals.x) || (i + Globals.x * dy < 0) || (i + Globals.x * dy >= Globals.xy)) return false;
                    //test empty
                    byte cell = board[i + dx + Globals.x * dy];
                    if (!((cell == 0) || (cell == piece))) return false;
                }
            }
            return true;
        }
    }

    public class Globals
    {
        public static byte x;
        public static byte y;
        public static byte xy;
        public static byte numPieces;

    }

    public class IntArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] a, byte[] b)
        {
            for(int i=0;i<Globals.xy;i++) if(a[i]!=b[i]) return false;
            return true;
        }
        public int GetHashCode(byte[] obj)
        {
            int ret = 1;
            for(int i=0;i<Globals.xy;i++){
                ret=(ret*7907+obj[i]);
            }
            return ret;
        }
    }
}
