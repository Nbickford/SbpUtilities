using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SBPSearcher
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            
            Console.WriteLine("SBPSearcher- the program to settle the question once and for all!");
            
            Globals.x = 4;
            Globals.y = 4;
            Puzzles.x = 4;
            Puzzles.y = 4;
            Puzzles.xy = Puzzles.x * Puzzles.y;

            Solver.SolveBoard(new List<byte[,]>() { new byte[,] { { 1, 0,2, 3 }, { 0, 4, 2, 3 }, { 0, 4, 4, 4 }, { 0, 0, 4, 5 } } }, solveType.Solve, 15, 1, true, new byte[]{1});
            SearchForPuzzles();
            
            Console.ReadLine();
        }
        static void SearchForPuzzles()
        {
            //This does all the work. Basically, it makes a HashSet with all the end positions, counts them, and then proceeds to find the hardest end-position
            //to start-position, that is, solving the group associated with the end position loaded.
            
            //The main algorithm in this function also needs to detect the other end positions in this group (by doing a diameter search), and remove them from the HashSet or otherwise mark them as solved.

            //Here's the (conjectured) algorithm for finding an element in a group S which is furthest away from any nodes in a set F:
            //Oh darn, the idea I had for it doesn't work!

            //New approach:
            //dl=-100;
            //dn=-1;
            //F=readpoint
            //S=null
            //S2=null
            //Repeat until dl>=dn:
            //Find_furthest (F->S); //At this step remove all Fs from hashset
            //dl=dn;
            //S2=S
            //dn=solve(S->F)
            //end repeat
            //return S2,dl

            //This works much better:
            //Do diameter search
            //Do end solve

            //int p=1;
            HashSet<byte[,]> boards;//=new HashSet<byte[,]>();
            //HashSet<byte[,]> hoards;
            String[] maxPuzzles = new String[16];
            int[] maxMoves = new int[16];
            byte[,] parsepuzzle;
            TextReader tr;
            String line;
            String puzstr;
            int pieceposition=0;
            byte piece;

            int pwidth;
            int pheight;

            byte[,] currentPuzzle;

            for(int p=1;p<16;p++){
            tryagain:
                /*Console.WriteLine("Enter location for file p={0}", p);
                try
                {
                    tr = new StreamReader(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("Try again.");
                    goto tryagain;
                }*/
                tr = new StreamReader("C:\\Users\\neil\\Documents\\Visual Studio 2010\\Projects\\SBPFinder(small)\\SBPFinder(small)\\bin\\Debug\\4x4_p=" + p + ".txt");

                //Populate the hashset.
                boards=new HashSet<byte[,]>(new ArrayComparer());
                //hoards = new HashSet<byte[,]>(new ArrayComparer());
                line=tr.ReadLine();
                while(line!=null){
                    boards.Add(str_t_p(line));
                    //hoards.Add(str_t_p(line));//hoards is entirely for the purpose of keeping the warning system down. //NOT ANY LONGER! 2011.11.2
                    line=tr.ReadLine();
                }
                
                while(boards.Count !=0){
                    currentPuzzle =boards.First();//new byte[,] { { 1, 2, 3, 4 }, { 1, 5, 6, 7 }, { 8, 5, 5, 0 }, { 9, 10, 11, 12 } };
                    
                    //parse(line);
                    //piece = FindEndingPiece();
                    piece = FindEndingPiece(currentPuzzle);
                    //Find the ending piece position.
                    for (int y = 0; y < 4; y++)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            if (currentPuzzle[y, x] == piece)
                            {
                                pieceposition = 4 * y + x;
                                goto exit; //I'm so so sorry.

                            }
                        }
                    }
                exit:
                    /*if(ArrayEquals(currentPuzzle,new byte[,]{{0,0,0,0},{0,0,0,1},{0,2,2,0},{0,2,3,4}})){
                        Console.ReadLine();
                        Solver.SolveBoard(new List<byte[,]>() { currentPuzzle }, solveType.AllNonSpecificSolutions,pieceposition, piece,true);
                        Console.ReadLine();
                    }*/
                    Solver.SolveBoard(new List<byte[,]>() { currentPuzzle }, solveType.AllNonSpecificSolutions,pieceposition, piece,false,new byte[]{piece});
                    //Parse and remove all puzzles from the file.
                foreach (byte[,] puz in Results.results)
                {
                    //puzstr = p_t_str(puz);
                    parse(p_t_str(puz));//DURR.
                    parsepuzzle = par_t_puz();
                    if(ArrayEquals(parsepuzzle,new byte[,]{{0,0,0,0},{0,0,0,1},{0,2,2,3},{0,2,0,4}})){
                        Console.WriteLine("That's 'amore");
                        byte t = FindEndingPiece(parsepuzzle);
                        Console.WriteLine(t);
                    }
                    if (boards.Contains(parsepuzzle))
                    {
                        boards.Remove(parsepuzzle);
                    }
                    else
                    {
                        
                        //if (!hoards.Contains(parsepuzzle))
                        //{
                            byte danger = FindEndingPiece(parsepuzzle); //I meant to name it dangerwillrobinson, but that's a bit long.

                            if (((CanMoveLeft(Puzzles.curparse, danger) || CanMoveUp(Puzzles.curparse, danger))))
                            {
                                Console.WriteLine("OOPS! Something went wrong! Current puzzle:");
                                Console.WriteLine(p_t_str(puz));
                                Console.ReadLine();
                            }
                           /* if (hoards.Contains(parsepuzzle))
                            {
                                Console.WriteLine("Pizza pie!");
                            }*/
                            
                        //}
                    }
                    
                        
                        
                
                }
                    // Step 2 of the algorithm. (Not so simple NOW, is it?)
                    //Calculate width & height of piece in lower-right.
                    //Could be made faster.
                pwidth = 0; 
                pheight = 0;
                for (int y = 0; y < Globals.y; y++)
                {
                    for (int x = 0; x < Globals.x; x++)
                    {
                        if (currentPuzzle[y, x] == piece)
                        {
                            if (Globals.y - y > pheight)
                            {
                                pheight = Globals.y - y;
                            }
                            if (Globals.x- x > pwidth)
                            {
                                pwidth = Globals.x - x;
                            }
                        }
                    }
                }
                Solver.SolveBoard(Results.results, solveType.AllSolutions, pieceposition % Globals.x + pwidth - Globals.x, piece, false, new byte[] { piece }); //That's nontrivial to come by randomly.
                //Solver.SolveBoard(Results.results, solveType.AllSolutions, pieceposition % Globals.x + pwidth - Globals.x, piece, false, new byte[] { piece }); //That's nontrivial to come by randomly.
                    //Take the last board. That's the result.
                Console.WriteLine("Group done! Best puzzle:");
                
                if (Results.results.Count != 0)
                {
                    if (Results.resultnum == 41)
                    {
                        Console.WriteLine("WHOOMP! (There it is)");
                    }
                    Console.WriteLine(p_t_str(Results.results.Last()));
                    Console.WriteLine("With {0} moves", Results.resultnum);
                    if (Results.resultnum > maxMoves[p])
                    {
                        maxMoves[p] = Results.resultnum;
                        maxPuzzles[p] = p_t_str(Results.results.Last());
                    }
                }
                else
                {
                    Console.WriteLine("Nothing!");
                }
                
                Console.WriteLine("Best score so far: {0} with score of {1}", maxPuzzles[p], maxMoves[p]);
                Console.WriteLine("{0} more to go in file", boards.Count);
                }

                Console.Beep();
                }
            
        }
        static byte[,] str_t_p(string puz)
        {
            //String to puzzle.
            byte[,] result = new byte[Globals.y, Globals.x];
            for(int y=0;y<4;y++){
                for (int x = 0; x < 4; x++)
                {
                    result[y, x] = CharToByte(puz[2 * (4 * y + x)]) ;
                }
            }
            return result;
        }
        static string p_t_str(byte[,] puz)
        {
            StringBuilder sb=new StringBuilder();
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    sb.Append(puz[y, x]);
                    sb.Append(" ");
                }
            }
            return sb.ToString();

        }
        static byte[,] par_t_puz()
        {
            //Turns the parsed puzzle into a proper 2D puzzle.
            byte[,] result = new byte[Globals.y, Globals.x];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    result[y, x] = Puzzles.curparse[4 * y + x];
                }
            }
            return result;
        }
        static byte[] puz_t_par(byte[,] puz)
        {
            //Turns a 2D puzzle into a "parsed" puzzle (not really)
            byte[] result = new byte[Puzzles.xy];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    result[4 * y + x]=puz[y, x];
                }
            }
            return result;
        }
        static void parse(string puz)
        {
            //Parser version 3. No bugs here!
            //Given a DE-ENCODED string, this routine parses it.
            /*
             * i.e 1 0 2 0 3 3 0 9 7->
             * 102
             * 033
             * 097
             * ->
             * 102
             * 033
             * 045
             * */

            Puzzles.curhigh = 1;
            Puzzles.curparse = new byte[Puzzles.xy]; //Defaulted to all 0s.
            List<Int32> nodes = new List<Int32>(); //number->position. Simple BFS. 
            Int32[] arpuz = new Int32[Puzzles.xy];
            Int32 a = 0;
            string[] nums=puz.Split(' ');
            for (int i = 0; i < Puzzles.xy; i++) 
            {
                //Create the table of cells.
                arpuz[a] = Byte.Parse(nums[i]);
                a++;
            }
            for (Int32 i = 0; i < (Puzzles.xy); i = i + 1)
            {

                a = arpuz[i]; //The value we want.
                if ((arpuz[i] != 0) && (Puzzles.curparse[i] == 0)) //If it isn't zero & we haven't met it before
                {

                    nodes.Add((Byte)i);
                    Int32 count = 1;
                    Int32 position;
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
                                nodes.Add(position - 1);
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
                                nodes.Add(position + Puzzles.x);
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
        static byte FindEndingPiece(byte[,] tab)
        {
            //Finds and returns the goal piece, if such a piece exists.

            /*List<byte> a = new List<byte>(Puzzles.x);
            List<byte> b = new List<byte>(Puzzles.y);
            for (int i = Globals.x-1; i >= 0; i--)
            {
                a.Add(tab[Globals.y-1,i]);
            }
            for (int i = Globals.y-1; i >= 0; i --)
            {
                b.Add(tab[i,Globals.x-1]);
            }
            return a.Intersect(b).Max();*/
            return tab[Globals.y - 1, Globals.x - 1];

        }
        //The next 2 algorithms are borrowed from SBPFinder. They're to check whether the "OOPS"es are any problem.
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
                    if (i % Puzzles.x == 0)
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

        static byte CharToByte(char val)
        {
            byte ret = (byte)val;
            if (48 <= ret && ret <= 57) 
            {
                return (byte)(ret - 48); // 0 to 9
            }
            if (97 <= ret && ret <= 122) 
            {
                return (byte)(ret - 87);//10 to 35
            }
            if (65 <= ret && ret <= 90)
            {
                return (byte)(ret - 29); //36 to 61
            }
            return 0;
        }
        static bool ArrayEquals(byte[,] x, byte[,] y)
        {
            //Tests whether two arrays are equivalent.
            for (int i = 0; i < Globals.x; i++)
            {
                for (int j = 0; j < Globals.y; j++)
                {
                    if (x[j, i] != y[j, i])
                    {
                        return false;
                    }
                }
            }

            return true;


        }

    }
    public class Puzzles //Just a global class, to appease the parser.
    {
        public static byte curhigh;
        public static byte[] curparse;
        public static int xy;
        public static int x;
        public static int y;
    }
    public class Results
    {
        public static List<byte[,]> results;
        public static int resultnum;
    }
    public class ArrayComparer : IEqualityComparer<byte[,]>
    {
        public bool Equals(byte[,] x, byte[,] y)
        {
            //Tests whether two arrays are equivalent.
            for (int i = 0; i < Globals.x; i++)
            {
                for (int j = 0; j < Globals.y; j++)
                {
                    if (x[j, i] != y[j, i])
                    {
                        return false;
                    }
                }
            }

            return true;


        }
        public int GetHashCode(byte[,] obj)
        {
            //Quick 'n simple array hash code generator.
            int ret = 0;
            //int mret;
            //int numMax;

            for (int y = 0; y < Globals.y; y++)
            {
                for (int x = 0; x < Globals.x; x++)
                {
                    ret = (ret * 17 + obj[y,x] + 1); //Primes should probably be larger.
                }
            }
            
            return ret;
        }
    }
}
