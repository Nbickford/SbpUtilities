using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SBP
{
    enum OutputMode { Silent, Verbose };
    class Puzzles
    {
        public static Int32[] curparse = new Int32[20];
        public static Int32 curhigh = 1;
        public static Int32 BestBoardNum = 0;
        public static string[] boards;
        public static string BestResultString = "";
        public static string ResultString = "";
        public static Int32 x;
        public static Int32 y;
        public static Int32 xy;
        public static Int32[] bestAllboard = new Int32[20];
        public static Int32 numboards;
        public static Int32 denomboards;
        public static TextWriter tw = new StreamWriter("problems.txt");
        public static OutputMode type;
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("SBP-Sliding Block Puzzle Solver and Creator");
            Console.WriteLine("Enter your choice-");
            Console.WriteLine("1-Parse Puzzle");
            Console.WriteLine("2-Solve Puzzle");
            Console.WriteLine("3-Evolve Puzzles");
            Console.WriteLine(LetterToNumber('0'));
            Puzzles.type = OutputMode.Verbose;
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Enter in puzzle (4x5,one line)");
                    string puz = Console.ReadLine();
                    Puzzles.x = 4;
                    Puzzles.y = 5;
                    parse(puz);
                    WritePuzArray(Puzzles.curparse);
                    break;
                case "2":
                    Console.WriteLine("Enter in puzzle (4x5,one line)");
                    Puzzles.x = 4;
                    Puzzles.y = 4;
                    Puzzles.xy = Puzzles.x * Puzzles.y;
                    string puz2 = Console.ReadLine();
                    Solve(puz2,false,true,1,new TimeSpan(0,10,0),false);

                    break;
                case "3":
                    
                    Evolver("pi");
                    break;
                case "42":
                    Puzzles.type = OutputMode.Silent;
                    Console.WriteLine("Special feature! Going through all puzzles in files...");
                    Puzzles.x = 4;
                    Puzzles.y = 4;
                    Puzzles.xy = Puzzles.x * Puzzles.y;
                    Int32[] maxes = new Int32[16];
                    String[] maxPuzzles = new String[16];
                    TextReader tr;
                    TextWriter tw;
                    
                    String line;
                    Int32 moves;
                    Int32 numPuzzle;
                    Int32 length;
                    Dictionary<String, Boolean> puzzles;
                    StringReader sr;
                    String line2;
                    Char temp;
                    StringBuilder puzzletoadd;
                    TimeSpan waitTime = new TimeSpan(0, 0, 10);
                    for (Int32 i = 10; i < 16; i++)
                    {
                        tr=new StreamReader("C:\\Users\\neil\\Documents\\Visual Studio 2010\\Projects\\SBPFinder\\SBPFinder\\bin\\x64\\Debug\\4x4_p="+i+".txt");
                        //Find length of file
                        length = 0;
                        line = tr.ReadLine();
                        while (line != null)
                        {
                            length++;
                            line = tr.ReadLine();
                        }
                        tr.Close();
                        //Populate Dictionary
                        puzzles = new Dictionary<String, Boolean>(length);
                        tr = new StreamReader("C:\\Users\\neil\\Documents\\Visual Studio 2010\\Projects\\SBPFinder\\SBPFinder\\bin\\x64\\Debug\\4x4_p=" + i + ".txt");
                        line = tr.ReadLine();
                        while (line != null)
                        {
                            puzzles.Add(line, false);
                            line = tr.ReadLine();
                        }
                        tr.Close();
                        //Actually solve them
                        tr = new StreamReader("C:\\Users\\neil\\Documents\\Visual Studio 2010\\Projects\\SBPFinder\\SBPFinder\\bin\\x64\\Debug\\4x4_p=" + i + ".txt");
                        tw = new StreamWriter("4x4_p=" + i + "-solved.txt",false,Encoding.ASCII);
                        line = tr.ReadLine();
                        numPuzzle=0;
                        while (line != null)
                        {
                            
                            if (!(puzzles[line]))
                            {
                                numPuzzle++;
                                Puzzles.curparse = LineToArray(line);
                                moves = Solve("hi", false, false,1,waitTime,false);
                                sr = new StringReader(Puzzles.ResultString);
                                line2 = sr.ReadLine(); puzzletoadd = new StringBuilder();
                                while (line2 != null)
                                {
                                    if (line2.Length > 0)
                                    {
                                        if (line2[0] == '#')
                                        {
                                            puzzletoadd = new StringBuilder();
                                            for (Int32 k = 0; k < Puzzles.y; k++)
                                            {
                                                line2 = sr.ReadLine().ToLower();
                                                for (Int32 l = 2; l <= 2 * (Puzzles.x + 1)-2;l=l+2 )
                                                {
                                                    temp = line2[l];
                                                    if (temp == ' ')
                                                    {
                                                        puzzletoadd.Append('0');
                                                    }
                                                    else
                                                    {
                                                        puzzletoadd.Append((LetterToNumber(temp)+1).ToString());
                                                    }
                                                    puzzletoadd.Append(" ");
                                                }
                                            }
                                            sr.ReadLine();
                                            for (Int32 k = 0; k < 2 * Puzzles.x; k = k + 2)
                                            {
                                                if (Byte.Parse(puzzletoadd[k].ToString()) > 0)
                                                {
                                                    if (Byte.Parse(puzzletoadd[k].ToString()) == 1)
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        //Skip it.
                                                        goto exit;
                                                    }
                                                }
                                            }
                                            puzzles[puzzletoadd.ToString()] = true; //we've visited it.
                                        exit: ;
                                        }
                                    }
                                    line2 = sr.ReadLine();
                                }
                                
                                    
                                    
                                    Console.WriteLine("Finished puzzle " + numPuzzle + " of " + length + " in file " + i);
                                    Console.WriteLine("Best puzzle so far: {0} with {1} moves", maxPuzzles[i], maxes[i]);
                                if (moves > maxes[i]) { maxes[i] = moves; maxPuzzles[i] = line; }
                                tw.WriteLine(line);
                                tw.WriteLine(moves);
                            }
                            line = tr.ReadLine();
                        }
                        tr.Close();
                        tw.Close();
                    }
                    for (Int32 i = 10; i < 16; i++)
                    {
                        
                        Console.WriteLine(maxes[i]+" moves:");
                        Console.WriteLine(maxPuzzles[i]);
                        Console.WriteLine();
                    }
                    break;
            }
            Puzzles.tw.Close();
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
        static void Evolver(string nein)
        {
            TimeSpan waitTime = new TimeSpan(1, 0, 0);
            Console.WriteLine("Input starting board");
            string startboard = Console.ReadLine();
            Console.WriteLine("Input width");
            Puzzles.x = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Input height");
            Puzzles.y = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Input 1/probability of change");
            Int32 prob = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Input number of generations maximum");
            Int32 MaxGen = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Input number of boards to generate at each stage");
            Int32 NumBoards = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Input maximum number of pieces");
            Int32 MaxPieces = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Be Thorough? (y/n)");
            Puzzles.xy = Puzzles.x * Puzzles.y;
            bool all=true;
            if (Console.ReadLine() == "n")
            {
                all = false;
            }
            
            string board1 = startboard;
            string board2 = startboard;
            Int32 num1 = 0;
            Int32 num2 = 0;
            Puzzles.boards = new string[NumBoards];
            Puzzles.curparse = new Int32[Puzzles.x * Puzzles.y];
            Puzzles.bestAllboard = new Int32[Puzzles.x * Puzzles.y];
            for (Int32 i = 1; i <= MaxGen; i = i + 1)
            {
                num1 = 0;
                num2 = 0;
                EvolveBoards(board1, board2, prob, NumBoards,MaxPieces);
                for (Int32 j = 0; j < NumBoards; j = j + 1)
                {
                    Int32 level = Solve(Puzzles.boards[j],all,true,10,waitTime,true);
                    
                    if (level > num2)
                    {
                        if (level > num1)
                        {
                            num1 = level;
                            board1 = Puzzles.boards[j];
                        }
                        else
                        {
                            num2 = level;
                            board2 = Puzzles.boards[j];
                        }
                    }
                    if (level > Puzzles.BestBoardNum)
                    {
                        //I need to update this.
                        parse(Puzzles.boards[j]);
                        



                    }
                    Puzzles.numboards = i * NumBoards + j;
                    Puzzles.denomboards = MaxGen * NumBoards;
                    Console.WriteLine("On board " + (i * NumBoards + j) + " of " + (MaxGen * NumBoards));
                }
            }


        }
        static Int32 Solve(string puz,bool all,bool parseQ,Int32 msWait,TimeSpan totalWait,bool WaitForExit)
        {
            //Tried to write my own solver, but decided to use JimSlide instead.
            if (parseQ)
            {
                parse(puz);
            }
            Int32 bestNum = 0;
             
            WritePuzArray(Puzzles.curparse);
            if (Puzzles.curparse.Max() == 0)
            {
                return 0;
            }
            Int32 UpMost = 2000;
            Int32 LeftMost = 2000;
            Int32 RightMost = 0;
            Int32 DownMost = 0;
            Int32 numLines = 0;
            Int32 steps = 0;
            if (!all)
            {
                foreach (Int32 i in Position(Puzzles.curparse, 1))
                {
                    if (i % Puzzles.x < LeftMost)
                    {
                        LeftMost = i % Puzzles.x;
                    }
                    if (i / Puzzles.x < UpMost)
                    {
                        UpMost = i / Puzzles.x;
                    }
                    if (i % Puzzles.x > RightMost)
                    {
                        RightMost = i % Puzzles.x;
                    }
                    if (i / Puzzles.x > DownMost)
                    {
                        DownMost = i / Puzzles.x;
                    }
                }
                Int32 UpLeft = UpMost * Puzzles.y + LeftMost;
                Int32 DownRight = DownMost * Puzzles.y + RightMost;
                Int32 Width = RightMost - LeftMost + 1;
                Int32 Height = DownMost - UpMost + 1;
                numLines = SolveIndividualPuzzle(1, Puzzles.x-Width,Puzzles.y-Height,msWait,totalWait,WaitForExit);
                steps = steps = Puzzles.ResultString.Length - Puzzles.ResultString.Replace("Move", "").Length;
                if (steps > Puzzles.BestBoardNum)
                {
                    bestNum = steps;
                    Puzzles.bestAllboard = Puzzles.curparse;
                    Puzzles.BestResultString = Puzzles.ResultString;
                }
                if (!File.Exists("level" + steps + ".txt"))
                {
                    WritePuzzleToFile("level" + steps + ".txt", Puzzles.curparse);
                    TextWriter sw = new StreamWriter("level" + steps + "-solution.txt");
                    sw.WriteLine(Puzzles.ResultString);
                    sw.Close();
                }
            }
            else
            {
                
                
                //for each piece
                for (Int32 p = 1; p <= Puzzles.curparse.Max(); p = p + 1)
                {
                    UpMost = 2000;
                    LeftMost = 2000;
                    RightMost = 0;
                    DownMost = 0;
                    foreach (Int32 i in Position(Puzzles.curparse, p))
                    {
                        if ((i % Puzzles.x) < LeftMost)
                        {
                            LeftMost = i % Puzzles.x;
                        }
                        if (i / Puzzles.x < UpMost)
                        {
                            UpMost = i / Puzzles.x;
                        }
                        if ((i % Puzzles.x) > RightMost)
                        {
                            RightMost = i % Puzzles.x;
                        }
                        if (i / Puzzles.x > DownMost)
                        {
                            DownMost = i / Puzzles.x;
                        }
                    }
                    Int32 UpLeft = UpMost * Puzzles.y + LeftMost;
                    Int32 DownRight = DownMost * Puzzles.y + RightMost;
                    Int32 Width = RightMost - LeftMost +1;
                    Int32 Height = DownMost - UpMost + 1;
                    //This may seem dumb, but remember that JimSlide indexes by 0, not 1.
                    for (Int32 x = 0; x <= (Puzzles.x - Width); x = x + 1)
                    {
                        for (Int32 y = 0; y <= (Puzzles.y - Height); y = y + 1)
                        {
                            Console.WriteLine("On board " + Puzzles.numboards + " of " + Puzzles.denomboards);
                            numLines = SolveIndividualPuzzle(p, x,y,msWait,totalWait,WaitForExit);
                            steps = (Puzzles.ResultString.Length - Puzzles.ResultString.Replace("move", "").Length)/4;
                            if (steps > bestNum)
                            {
                                bestNum = steps;
                                Puzzles.bestAllboard = Puzzles.curparse;
                                Puzzles.BestResultString = Puzzles.ResultString;
                            }
                            if (!File.Exists("level" + steps + ".txt"))
                            {
                                WritePuzzleToFile("level" + steps + ".txt", Puzzles.curparse);
                                TextWriter sw = new StreamWriter("level" + steps + "-solution.txt");
                                sw.WriteLine(Puzzles.ResultString);
                                sw.Close();
                            }
                           
                           
                            
                        }
                    }

                }
                
                steps = bestNum;
            }
           
            Console.WriteLine(steps);
            //Puzzles.ResultString = output;
            if (steps < 1)
            {
                steps = 0;
            }
            //Lastly, delete the temp files that JimSlide forgets.
            //TODO
            
            return steps;



        }
        static Int32 SolveIndividualPuzzle(Int32 goalPiece, Int32 goalX,Int32 goalY,Int32 msWait, TimeSpan totalWait,Boolean waitForExit)
        {
            //Now that we have that info, start writing the puzzle to puz.txt!
            TextWriter tw = new StreamWriter("puz.txt",false,Encoding.ASCII);
            
            tw.WriteLine("xsize: " + Puzzles.x);
            tw.WriteLine("ysize: " + Puzzles.y);
            for (Int32 piece = 1; piece <= Puzzles.curparse.Max(); piece = piece + 1)
            {
                tw.Write("piece: " + piece);
                //First set to zero
                //TODO: Eliminate negative values by finding true zero value, thus reducing number of diameter searches.
                Int32 xtemp = Position(Puzzles.curparse, piece)[0];
                Int32 ytemp = xtemp / Puzzles.x;
                xtemp = xtemp % Puzzles.x;
                foreach (Int32 i in Position(Puzzles.curparse, piece))
                {
                    tw.Write(" (" + ((i % Puzzles.x) - xtemp) + "," + ((i / Puzzles.x) - ytemp) + ")");
                }
                tw.WriteLine();
            }
            //Now duplicate checking
            List<string> pieces = new List<string>();
           
            for (Int32 piece = 1; piece <= Puzzles.curparse.Max(); piece = piece + 1)
            {
                string toadd = "";
                Int32 sub = Position(Puzzles.curparse, piece)[0];
                foreach (Int32 i in Position(Puzzles.curparse, piece))
                {
                    toadd += (i - sub).ToString() + " ";
                }
                pieces.Add(toadd);
            }
            //That made the strings for the pieces, but not the duplicate check.
            List<List<Int32>> dup = new List<List<Int32>>();
            for (Int32 piece = 2 ; piece <= Puzzles.curparse.Max(); piece = piece + 1)
            {
                if (piece!=goalPiece)
                {
                    string PieceString = pieces[piece - 1];
                    bool FoundMatch = false;
                    Int32 NumMatch = 0;
                    for (Int32 i = 0; i < dup.Count(); i = i + 1)
                    {
                        if (pieces[dup[i][0] - 1] == PieceString)
                        {
                            FoundMatch = true;
                            NumMatch = i;
                            break;
                        }
                    }
                    if (FoundMatch)
                    {
                        dup[NumMatch].Add(piece);
                    }
                    else
                    {
                        List<Int32> ubertemp = new List<Int32>();
                        ubertemp.Add(piece);
                        dup.Add(ubertemp);
                    }
                }
                

            }
            //Now the put lines- same, but not zero-based
            for (Int32 piece = 1; piece <= Puzzles.curparse.Max(); piece = piece + 1)
            {
                if ((piece == goalPiece)||(piece==1))
                {
                    tw.Write("put: " + piece);
                }
                else
                {
                    if ((dup[OverlySpecificEncapsulatedListFinder(dup, piece)][0] == piece))
                    {
                        tw.Write("put: " + piece);
                    }
                    else
                    {
                        tw.Write("put: " + dup[OverlySpecificEncapsulatedListFinder(dup, piece)][0]);
                    }
                }

                foreach (Int32 i in Position(Puzzles.curparse, piece))
                {
                    tw.Write(" (" + (i % Puzzles.x) + "," + (i / Puzzles.x) + ")");
                    break;
                }
                tw.WriteLine();
            }
            //Now the winning condition...
            
                tw.WriteLine("win: " + goalPiece + " (" + (goalX) + "," + (goalY) + ")");
            
                
            
            //Almost done! now, the memory...
            tw.WriteLine("bigmem: 512000000");
            tw.WriteLine("smallmem: 128000000");
            tw.Close();

            //Run JimSlide
            Process proc = new Process();

            //proc.StartInfo.WorkingDirectory = @"C:\\Users\\neil\\Desktop\\Eccentricity 2\\jimslide\\";
            proc.StartInfo.FileName = "JIMSLIDE.EXE";
            //proc.StartInfo.Arguments = "puz.txt > board.sol";

            proc.StartInfo.UseShellExecute = false;
            //proc.StartInfo.RedirectStandardInput=true;
            proc.StartInfo.RedirectStandardOutput = true;
            //proc.StartInfo.RedirectStandardError = true;
            
            DateTime now = DateTime.Now;
            proc.Start();
            
            string output;



            if (!waitForExit)
            {
                while ((!proc.HasExited))
                {

                    if (DateTime.Now.Subtract(now) >= totalWait)
                    {
                        proc.Kill();
                        
                        output = proc.StandardOutput.ReadLine();
                        if (output == null)
                        {
                            Puzzles.tw.WriteLine(ToPuzArray(Puzzles.curparse));
                            Puzzles.tw.WriteLine("-inf.loop");
                            output = "";
                        }
                        else
                        {
                            if (msWait > 1000)
                            {
                                Puzzles.tw.WriteLine(ToPuzArray(Puzzles.curparse));
                                Puzzles.tw.WriteLine("close error - must evaluate manually");
                                output = "";
                            }
                            else
                            {
                                output = proc.StandardOutput.ReadToEnd();
                                Console.WriteLine(output.Length);
                                return SolveIndividualPuzzle(goalPiece, goalX, goalY, msWait * 4, totalWait,true);
                            }

                        }
                        Puzzles.tw.WriteLine();
                        Puzzles.tw.Flush();
                        goto exit;
                    }
                    System.Threading.Thread.Sleep(msWait);


                }
                output = proc.StandardOutput.ReadToEnd();
            }
            else
            {
                //proc.WaitForExit();
                output = proc.StandardOutput.ReadToEnd();
            }
            exit:

            if (Puzzles.type == OutputMode.Verbose)
            {
                Console.WriteLine(output);
            }

            Int32 numLines = CountLines(output);
            proc.Close();//Just making sure it closes.
            
            Puzzles.ResultString = output;
            
            return numLines;
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
            Puzzles.curparse = new Int32[Puzzles.xy]; //Defaulted to all 0s.
            List<Int32> nodes = new List<Int32>(); //number->position. Simple BFS. 
            Int32[] arpuz = new Int32[Puzzles.xy];
            Int32 a = 0;
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


        static void EvolveBoards(string a, string b, Int32 prob, Int32 numboards,Int32 MaxPieces)
        {
            //Interleave:
            string ab = "";
            for (Int32 i = 0; i < (Puzzles.x * Puzzles.y); i = i + 1)
            {
                if (i % 2 == 0)
                {
                    ab = ab + a.Substring(i, 1);
                }
                else
                {
                    ab = ab + b.Substring(i, 1);
                }
            }
            //Mutate
            Random rand = new Random();
            for (Int32 i = 0; i < numboards; i = i + 1)
            {
                string toadd = "";
                for (Int32 j = 0; j < (Puzzles.x * Puzzles.y); j = j + 1)
                {
                    if (rand.Next(prob) != 0)
                    {
                        toadd += ab[j];
                    }
                    else
                    {
                        toadd += ((Math.Abs(Int32.Parse(ab.Substring(j, 1)) + rand.Next(2) * 2 - 1)) % Math.Min(5,MaxPieces+1)).ToString();//Add or subtract 1
                    }
                }
                parse(toadd);

                if (Puzzles.curparse.Max() > MaxPieces)
                {
                    i = i - 1;
                }
                else
                {
                    Puzzles.boards[i] = toadd;
                }
            }

        }
        static void WritePuzArray(Int32[] array)
        {
            Int32 pos = 1;
            foreach (Int32 i in array)
            {
                Console.Write(i);
                if (pos % Puzzles.x == 0)
                {
                    Console.WriteLine(",");
                }
                else
                {
                    Console.Write(",");
                }
                pos++;
            }
        }
        static String ToPuzArray(Int32[] array)
        {
            String result="";
            Int32 pos = 1;
            foreach (Int32 i in array)
            {
               result+=i;
                if (pos % Puzzles.x == 0)
                {
                    result+=(",\n");
                }
                else
                {
                    result+=(",");
                }
                pos++;
            }
            return result;
        }
        static void WriteList(List<Int32> input)
        {
            Int32 pos = 1;
            foreach (Int32 i in input)
            {
                Console.Write(i);
                if (pos % 4 == 0)
                {
                    Console.WriteLine(",");
                }
                else
                {
                    Console.Write(",");
                }
                pos++;
            }
        }
        static List<Int32> Position(Int32[] input, Int32 tobefound)
        {
            List<Int32> output = new List<Int32>();
            Int32 ndex = 0;
            foreach (Int32 i in input)
            {
                if (i == tobefound)
                {
                    output.Add(ndex);
                }
                ndex++;
            }
            return output;
        }
        static Int32[] ListToArray(List<Int32> input)
        {
            Int32[] output = new Int32[input.Count()];
            Int32 index = 0;
            foreach (Int32 i in input)
            {
                output[index] = i;
                index++;
            }
            return output;
        }

        static Int32[] LineToArray(String line)
        {
            Int32[] result = new Int32[Puzzles.xy];
            for (Int32 k = 0; k < line.Length; k = k + 2)
            {
                
                result[(k / 2)] = LetterToNumber(line[k]);
            }
            return result;
        }
        static int LetterToNumber(Char c)
        {
            if (c >= '0' && c <= '9')
            { // If c is in the range of '0' to '9'
                return (int)c - (int)'0';
                // return the value 0 to 9
            }
            else if (c >= 'A' && c <= 'Z')
            { // If c is in the range of 'A' to 'Z'
                return (int)c - (int)'A' + 10;
                // Subtract 'A' to find out how far in the alphabet it is
                // (A = 0, B = 1...)
                // Add ten to get range of 10-25
            }
            else if (c >= 'a' && c <= 'z')
            {
                return (int)c - (int)'a' + 10;
                // Same as above, except lowercase
            }
            else
                return 0;
        }
        static Int32 CountLines(string a)
        {
            Int32 result = 0;
            for (Int32 i = 0; i < a.Length - 1; i = i + 1)
            {
                if (a.Substring(i, 1) == "\n")
                {
                    result++;
                }
            }
            return result;
        }
        static void WritePuzzleToFile(string filename, Int32[] parsedpuzzle)
        {
            TextWriter tw = new StreamWriter(filename);
            foreach (Int32 i in parsedpuzzle)
            {
                tw.WriteLine(i);
            }
            tw.Close();
        }
        static Int32 OverlySpecificEncapsulatedListFinder(List<List<Int32>> data, Int32 piece)
        {


            for (Int32 i = 0; i < data.Count(); i = i + 1)
            {
                for (Int32 j = 0; j < data[i].Count(); j = j + 1)
                {
                    if (data[i][j] == piece)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

    }
}
