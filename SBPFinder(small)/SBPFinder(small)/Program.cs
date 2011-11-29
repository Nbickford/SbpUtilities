using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SBPFinder_small_
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SBPFINDER - A program for finding all the valid SBPs of a {0}x{1} grid.", Puzzles.x, Puzzles.y);
            Console.WriteLine("Press enter to begin...");
            Console.ReadLine();


            Puzzles.curparse = new Byte[Puzzles.x * Puzzles.y];
            Puzzles.E = new Byte[Puzzles.x, Puzzles.y];
            Puzzles.xy = (Byte)(Puzzles.x * Puzzles.y);
            Puzzles.tws = new TextWriter[Puzzles.xy];
            for (Int32 i = 0; i < Puzzles.xy; i++)
            {
                Puzzles.tws[i] = new StreamWriter(Puzzles.x + "x" + Puzzles.y + "_p=" + i + ".txt");
            }

            Puzzles.estTotal = 2 * Math.Pow(3, Puzzles.x + Puzzles.y - 2) * Math.Pow(4, (Puzzles.x - 1) * (Puzzles.y - 1));
            Console.WriteLine("Estimated number of puzzles to check through: " + Puzzles.estTotal);
            Console.WriteLine("Time: {0}", DateTime.Now);

            //Make the encoding and decoding mechanisms
            for (Int32 i = 0; i < Puzzles.x; i++)
            {
                for (Int32 j = 0; j < Puzzles.y; j++)
                {
                    if (i + j < Puzzles.x)
                    {
                        Puzzles.E[i, j] = (Byte)(TriangleNumber(i + j + 1) - i - 1);
                    }
                    else
                    {
                        Puzzles.E[i, j] = (Byte)(Puzzles.xy - TriangleNumber(Puzzles.x + Puzzles.y - i - j - 1) + Puzzles.x - i - 1);
                    }
                }
            }
            Puzzles.D = new Byte[Puzzles.xy, 2];
            for (Int32 i = 0; i < Puzzles.x; i++)
            {
                for (Int32 j = 0; j < Puzzles.y; j++)
                {
                    Puzzles.D[Puzzles.E[i, j], 0] = (Byte)i;
                    Puzzles.D[Puzzles.E[i, j], 1] = (Byte)j;
                }
            }
            nextBoard(0, new Byte[Puzzles.x * Puzzles.y]);
            for (Int32 i = 0; i < Puzzles.xy; i++)
            {
                Puzzles.tws[i].Close();
            }
            Console.WriteLine("Done! Went through {0} puzzles and recieved {1} different, with {2} collisions.", Puzzles.triedPuzzles, Puzzles.numPuzzles, Puzzles.collidePuzzles);
            Console.WriteLine("Write the sorted file? (y/n) This will take up memory and hard disk space");
            trier:
            try
            {
                if (Console.ReadLine().ToLower()[0] == 'y')
                {
                    Console.WriteLine("Sorting the array...");
                    Puzzles.tw = new StreamWriter(Puzzles.x + "x" + Puzzles.y + "-sorted.txt");
                    List<byte[]> temp = new List<byte[]>(Puzzles.boards);
                    temp.Sort(GreaterThan);
                    Console.WriteLine("Writing data...");
                    foreach (byte[] lis in temp)
                    {

                        Puzzles.tw.WriteLine(niceformat(qBS(lis)));
                    }
                    Puzzles.tw.Close();
                    Console.WriteLine("Done!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Try again.");
                goto trier;
            }
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        static void nextBoard(Byte position, Byte[] cells)
        {
            if (position == Puzzles.xy)//If we've gone off the board
            {


                Puzzles.triedPuzzles++;

                String parsePuzzle;
                StringBuilder puz = new StringBuilder();

                for (Byte i = 0; i < Puzzles.x; i++)
                {
                    for (Byte j = 0; j < Puzzles.y; j++)
                    {


                        puz.Append(cells[Puzzles.E[i,j]].ToString()); //Decode the array into a string
                    }

                }
                parsePuzzle = puz.ToString();

                if (parsePuzzle.Contains("0"))
                {
                    parse(parsePuzzle); //Parse the DECODED array
                    //Say,chap, doesn't this seem awfully strange? We're making a string out of a puzzle, then converting it to an array!
                    //String b = qBS(Puzzles.curparse);

                    if (Puzzles.boards.Contains(Puzzles.curparse))
                    {
                        Puzzles.collidePuzzles++;
                    }
                    else
                    {
                        byte maxpiece = FindEndingPiece();
                        if (maxpiece != 0) //If there even is an ending piece.
                        {
                            //Each of the first 2 algorithms I reference here are actually imported from the project "SSBP-Solver". (that is, the step version).
                            //The whole purpose of them is to find positions which could be end positions for 
                            //I've modified them a bit to work with 1-dimensional boards. Mostly by simulating an x and y.
                            //Turns out, doing things like this is perfectly valid for searches if you work from diameter searches
                            //Idea from http://arxiv.org/PS_cache/cs/pdf/0502/0502068v1.pdf. Authors: http://homepages.cwi.nl/~tromp/ and http://cilibrar.com/ (I think)

                            if ((CanMoveLeft(Puzzles.curparse, maxpiece) || CanMoveUp(Puzzles.curparse, maxpiece)) && (IsEndingPosition(Puzzles.curparse, maxpiece)))
                            {

                                Puzzles.numPuzzles++;

                                Puzzles.boards.Add(Puzzles.curparse); //Next time, give each one a specific number.
                                Puzzles.tws[Puzzles.curparse.Max()].WriteLine(BitsToString(Puzzles.curparse));

                                //Puzzles.tw.WriteLine(BitsToString(Puzzles.curparse));
                                if (Puzzles.numPuzzles % 100000 == 0)
                                {
                                    Console.WriteLine("Went through {0} puzzles and recieved {1} different, with {2} collisions. ({3} of those had no holes) About " + (((Double)Puzzles.triedPuzzles) / (Puzzles.estTotal / 100.0)).ToString() + "% done.", Puzzles.triedPuzzles, Puzzles.numPuzzles, Puzzles.collidePuzzles, Puzzles.noHolePuzzles, Puzzles.collidePuzzles);
                                    Console.WriteLine("Time: {0}", DateTime.Now);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Puzzles.noHolePuzzles++;
                }
                //else abort

            }
            else
            {

                List<Byte> nums = new List<Byte>();
                Byte[] copy = new Byte[Puzzles.xy];
                for (Int32 i = 0; i < Puzzles.xy; i++)
                {
                    copy[i] = cells[i];
                }
                nums.Add(0); //It can be a hole
                Byte x = Puzzles.D[position, 0];
                Byte y = Puzzles.D[position, 1];



                if ((x != 0) || (y != 0))
                {
                    if (y == 0)
                    {
                        Byte ppos = (Byte)(position - 1);

                        while (Puzzles.D[ppos, 1] != y)
                        {
                            nums.Add(cells[ppos]);
                            ppos--;
                        }
                        nums.Add(cells[ppos]);
                    }
                    else
                    {
                        Byte ppos = (Byte)(position - 1);

                        while (Puzzles.D[ppos, 0] != x)
                        {
                            nums.Add(cells[ppos]);
                            ppos--;
                        }
                        nums.Add(cells[ppos]);
                    }
                }

                for (Byte i = 1; i < 5; i++)  //i<5. It must be exactly <5, or there will be problems. Big problems.
                {
                    if (!(nums.Contains(i)))
                    {
                        nums.Add(i); //It can be one belonging to neither.
                        break;
                    }
                }

                //Recurse
                foreach (Byte b in (nums.Union(nums)))
                {
                    copy[position] = b;
                    nextBoard((Byte)(position + 1), copy);
                }
            }

        }

        static String BitsToString(Byte[] bits)
        {
            //This is the most useless subroutine in the program. It wastes memory, converts an array that has been converted to a string then converted to an array back into a string.
            //SERIOUSLY.
            //If we can prove no-parse needed for these types, it will be great!
            StringBuilder giveback = new StringBuilder(Puzzles.xy * 2);
            foreach (Byte num in bits)
            {
                giveback.Append(Puzzles.asciiCharacters[num] + " ");
            }
            return giveback.ToString();
        }
        static String qBS(Byte[] bits)
        {
            StringBuilder giveback = new StringBuilder(Puzzles.xy);
            foreach (Byte num in bits)
            {
                giveback.Append(Puzzles.asciiCharacters[num]);
            }
            return giveback.ToString();
        }


        static void parse(string puz)
        {
            //Given a DE-ENCODED string, this routine parses it.
            /*
             * i.e 102033097->
             * 102
             * 033
             * 097
             * ->
             * 102
             * 033
             * 045
             * */

            Puzzles.curhigh = 1;
            Puzzles.curparse = new Byte[Puzzles.xy]; //Defaulted to all 0s.
            List<Byte> nodes = new List<Byte>(); //number->position. Simple BFS. 
            Byte[] arpuz = new Byte[Puzzles.xy];
            Byte a = 0;
            foreach (Char b in puz)
            {
                //Create the table of cells.
                arpuz[a] = Byte.Parse(b.ToString()); //FAIL.
                a++;
            }
            for (Int32 i = 0; i < (Puzzles.xy); i = i + 1)
            {

                a = arpuz[i]; //The value we want.
                if ((arpuz[i] != 0) && (Puzzles.curparse[i] == 0)) //If it isn't zero & we haven't met it before
                {

                    nodes.Add((Byte)i);
                    Byte count = 1;
                    Byte position;
                    Puzzles.curparse[i] = Puzzles.curhigh;
                    while (count != 0) //While there are still nodes to visit
                    {
                        position = nodes[0];
                        if (position % Puzzles.x != 0) //If it's not on the left side
                        {
                            if ((arpuz[position - 1] == a) && (Puzzles.curparse[position - 1] == 0)) //If it's the same and we haven't seen it before (to prevent an inf. loop)
                            {
                                //Set the array position
                                Puzzles.curparse[position - 1] = Puzzles.curhigh;
                                //Add the node
                                nodes.Add((Byte)(position - 1));
                                count++;
                            }

                        }
                        if (position % Puzzles.x != Puzzles.x - 1) //If it's not on the right side
                        {
                            if ((arpuz[position + 1] == a) && (Puzzles.curparse[position + 1] == 0))
                            {
                                Puzzles.curparse[position + 1] = Puzzles.curhigh;
                                nodes.Add((Byte)(position + 1));
                                count++;
                            }
                        }
                        if (position >= Puzzles.x) //If it's not on the bottom
                        {
                            if ((arpuz[position - Puzzles.x] == a) && (Puzzles.curparse[position - Puzzles.x] == 0))
                            {
                                Puzzles.curparse[position - Puzzles.x] = Puzzles.curhigh;
                                nodes.Add((Byte)(position - Puzzles.x));
                                count++;
                            }
                        }
                        if (position < Puzzles.xy - Puzzles.x) //If it's not on the top
                        {
                            if ((arpuz[position + Puzzles.x] == a) && (Puzzles.curparse[position + Puzzles.x] == 0))
                            {
                                Puzzles.curparse[position + Puzzles.x] = Puzzles.curhigh;
                                nodes.Add((Byte)(position + Puzzles.x));
                                count++;
                            }
                        }
                        nodes.RemoveAt(0); //Delete the node; we're done with it.
                        count--;
                    }
                    Puzzles.curhigh++;
                }
                //All of the positions for a should now be placed in Puzzles.curparse.



            }
        }


        static Int32 max(Int32 a, Int32 b)
        {
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
        static Byte max(Byte a, Byte b)
        {
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
        static Byte min(Byte a, Byte b)
        {
            if (b > a)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
        static Int32 TriangleNumber(Int32 a)
        {
            return ((a) * (a + 1)) / 2;
        }
        static String niceformat(String line)
        {
            StringBuilder result = new StringBuilder();
            foreach (Char c in line)
            {
                result.Append(c);
                result.Append(" ");
            }
            return result.ToString();
        }


        public static int GreaterThan(byte[] a, byte[] b)
        {
            for (int i = 0; i < Puzzles.xy; i++)
            {
                if (a[i] > b[i])
                {
                    return 1;
                }
                if (a[i] < b[i])
                {
                    return -1;
                }
            }
            return 0;
        }
        //Added on 6/28/2011

        //*************************************************
        //* Imported and modified from SSBPSolver.
        //* Sees whether a piece can move in a direction.
        //*************************************************
        static Boolean CanMoveUp(byte[] board, byte piece)
        {
            //Note: there are faster ways to do this involving arrays for each piece and other things like that.
            //These algorithms are contained within the latest version of SSBPSolver; for now, this is good enough.

            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == piece)
                {
                    if (i < Puzzles.x)
                    {
                        return false;
                    }
                    else
                    {
                        if (!((board[i - Puzzles.x] == 0) || (board[i - Puzzles.x] == piece))) //This could be 1 or 2 instructions faster.
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        static Boolean CanMoveLeft(byte[] board, byte piece)
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == piece)
                {
                    if (i%Puzzles.x==0)
                    {
                        return false;
                    }
                    else
                    {
                        if (!((board[i - 1] == 0) || (board[i - 1] == piece))) //This could be 1 or 2 instructions faster.
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        static Boolean IsEndingPosition(byte[] board, byte piece)
        {
            //HAIKU DOCUMENTATION TIME!

            //is in right column?
            //and is it in bottom row?
            //return true; else false.

            //Go through the right column
            Boolean valid=false;
            for (int i = Puzzles.x - 1; i < board.Length; i += Puzzles.x)
            {
                if (board[i] == piece)
                {
                    valid = true;
                    break;
                }
            }
            if(valid==false){
                return false;
            }else{
                valid=false;
            }
        
            //Go through the bottom row
            for (int i = Puzzles.xy - Puzzles.x; i < Puzzles.xy; i++)
            {
                if (board[i] == piece)
                {
                    valid = true;
                    break;
                }
            }
            return valid;
        }
        static byte FindEndingPiece()
        {
            //Finds and returns the goal piece, if such a piece exists.
            //We only need to check one row. If no piece is found, this returns 0 and the function that called this doesn't add the board.
            //8-18-2011: OOPS! That doesn't work! Try
            //000
            //111
            //120
            //This algorithm would return 2, which is WRONG
            /*for (int i = Puzzles.xy - 1; i >= Puzzles.xy - Puzzles.x; i--)
            {
                if (Puzzles.curparse[i] != 0)
                {
                    return Puzzles.curparse[i];
                }
            }
            return 0;*/
            //8-20-2011: OOPS! That makes things ambiguous! Much simpler now.
            /*List<byte> a = new List<byte>(Puzzles.x);
            List<byte> b = new List<byte>(Puzzles.y);
            for (int i = Puzzles.xy - 1; i >= Puzzles.xy - Puzzles.x; i--)
            {
                a.Add(Puzzles.curparse[i]);
            }
            for (int i = Puzzles.xy - 1; i >= 0; i -= Puzzles.x)
            {
                b.Add(Puzzles.curparse[i]);
            }
            return a.Intersect(b).Max();*/
            return Puzzles.curparse[Puzzles.xy - 1];

            
        }


    }
    public class IntArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;


        }
        public int GetHashCode(byte[] obj)
        {
            //Gets the hash code, irrespective of permutation.
            int ret = 0;
            //int mret;
            //int numMax;
            for (int i = 0; i < Puzzles.xy; i++)
            {
                
                ret = (69069*ret+obj[i]); //Primes should probably be smaller.
            }
            return ret;
        }
    }



    public class Puzzles
    {
        public static Byte[] curparse; //Memory is good, you know.
        public static Byte x = 4;
        public static Byte y = 5;
        public static Byte xy;
        public static Byte[,] E; //The encoding matrix
        public static Byte[,] D;//The decoding matrix
        public static HashSet<byte[]> boards = new HashSet<byte[]>(new IntArrayComparer()); //Used to store the found boards.
        public static Byte curhigh = 1;
        public static TextWriter tw;
        //public static List<Byte> U=new List<Byte>(){0,1,2,3};//The universal set.
        public static Int64 triedPuzzles = 0;
        public static Int64 numPuzzles = 0;
        public static Int64 collidePuzzles = 0;
        public static Int64 noHolePuzzles = 0;
        public static Double estTotal;
        //public static Byte[] n = new Byte[0];
        public static Char[] asciiCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public static TextWriter[] tws;
    }
}
