using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SBPSearcher
{
    //This is a modification of Nbickford's SSBPSolver, made to be called as a function and modified to give back results.
    //The original documentation is reprinted below.

    //**************************************************
    //SSBPSolver-A program designed to be a replacement for Jimslide, at least for the kind of analysis SBP is doing.
    //It uses BFT search to find the hardest positions to get to from an intitial position, using the step metric.
    //Also, it conserves memory by "packing" puzzles into a much smaller format describing where each instance of each piece is placed.
    //It also implements duplicate checking, and a (hopefully fast enough) hash function.
    //Supports grids of any size. However, the latest release only supports up to 63 pieces. By changing a lot of bytes to Int16s, you can
    //It also takes advantage of the fact that you can change the positions of the pieces in the packed board without expanding and repacking.

    //Structure of the program:
    //The main function calls GenerateNextBoards() repeatedly, until no more boards have been found. It then prints out the moves needed to get to the farthest away position.
    //GenerateNextBoards steps the 3 arrays (clearing Globals.newboards), checks for each piece if it can move in any direction, and then checks to see if that position has already been reached.
    public enum solveType { Diameter = 0, Solve = 1, AllSolutions = 2, FullList = 3 , AllNonSpecificSolutions=4};
    class Solver
    {
        /// <summary>
        /// Solve a sliding-block puzzle given the puzzle, solve type, goal position, goal piece, and different pieces (oh and also if it should spew things to the screen)
        /// </summary>
        /// <param name="startboards">The initial state(s) of the puzzle to be solved.</param>
        /// <param name="type">The type of solution to give.</param>
        /// <param name="goalPosition">Where the goal piece should be moved.</param>
        /// <param name="goalPiece">The goal piece.</param>
        /// <param name="verbose">Should I tell you everything about the number 42 and this puzzle?</param>
        /// <param name="differentPieces">The pieces which are painted differently from the others.</param>
        public static void SolveBoard(List<byte[,]> startboards, solveType type,int goalPosition,int goalPiece,bool verbose,byte[] differentPieces)
        {
            //Solve type
            //solveType type = solveType.Solve;
            //int goalPosition = 2;
            //int goalPiece = 22; //Only needed if solveType=1 or 2.
            int NumSolutions = 0; //Only for solveType.AllSolutions
            Boolean WriteToFile = false; //TODO: Actually use this
            //Boolean verbose = true;

            Results.results = new List<byte[,]>();
            Results.resultnum = 0;

            DateTime startTime = DateTime.Now;
            DateTime moveTime;

            Globals.boards = new Dictionary<byte[], int[]>(new IntArrayComparer());  //At some point I should change the second type to a smaller type.
            Globals.lastboards = new Dictionary<byte[], int[]>(new IntArrayComparer()); //Maybe tomorrow.
            Globals.newboards = new Dictionary<byte[], int[]>(new IntArrayComparer());
            Globals.PuzzlesConsidered = 0;
            Globals.TotalPuzzles = 0;
            //System.IO.TextWriter tw = new System.IO.StreamWriter("output.log"); //Start writing the log file.
            
            //testboard Must be the same size as Globals.y*Globals.x
            //By the way, if you're wondering why all the coordinates in the program are like (y,x), it's to make the above look nice.


            //Globals.x = 7;
            //Globals.y = 10;
            Globals.numPieces = Max(startboards[0]); //The maximum number of testboard

            MakePieceArray(startboards[0],differentPieces); //Initialize all the arrays.

            byte[] packtest2;
            for(int i=0;i<startboards.Count;i++)
            {
                packtest2 = Pack(startboards[i]); //The only use of the pack function.
                if (verbose)
                {
                    foreach (byte k in Unpack(packtest2))
                    {
                        Console.Write(k + " ");
                    }
                }
                Globals.newboards.Add(packtest2, new int[0]);//Initalize the puzzle (s).
            }
            int step = 0;//This is the read generation
            moveTime = DateTime.Now;
            while (Globals.newboards.Count != 0)
            {
                

                //check for solutions
                if ((type != solveType.Diameter) && (type != solveType.FullList))
                {
                    if (verbose)
                    {
                        Console.WriteLine("Checking for solution...");
                    }
                    foreach (KeyValuePair<byte[], int[]> pair in Globals.newboards)
                    {
                        if (IsAtGoal(pair.Key,goalPiece,type,goalPosition))
                        {
                            if (type == solveType.Solve)
                            {
                                if (verbose)
                                {
                                    Console.WriteLine("FOUND SOLUTION AT {0} MOVES!", step);
                                    WriteBoard(pair);
                                }
                                    Results.results.Add(Unpack(pair.Key));
                                
                                goto donehere; //I am ashamed.
                            }
                            else
                            {
                                if (verbose)
                                {
                                    WriteBoard(pair);
                                }
                                    Results.results.Add(Unpack(pair.Key));
                                    Results.resultnum = step;
                                NumSolutions++;
                            }
                        }
                    }

                }


                Globals.TotalPuzzles += Globals.newboards.Count;
                //tw.WriteLine(step + " steps: " + Globals.newboards.Count + " boards, " + DateTime.Now+"({0} total puzzles, {1} total puzzles considered)",Globals.TotalPuzzles,Globals.PuzzlesConsidered);
                //tw.Flush();
                if (verbose)
                {
                    Console.WriteLine(step + " steps: " + Globals.newboards.Count + " boards, " + DateTime.Now + "({0} total puzzles, {1} total puzzles considered, {2} boards per second)", Globals.TotalPuzzles, Globals.PuzzlesConsidered, Globals.newboards.Count / DateTime.Now.Subtract(moveTime).TotalSeconds);
                }
                step++; //Fairly important.
                
                moveTime = DateTime.Now;
                
                GenerateNextBoards(); //This step takes the longest.

            }
        donehere:
            //Write out results to the results class
            
            
        switch (type)
        {
            case solveType.Solve:
                Results.resultnum = step;
                break;
            case solveType.Diameter:
                Results.resultnum = step - 1;
                break;
            
            
        }

        if (type == solveType.Diameter)
        {
            foreach (KeyValuePair<byte[], int[]> key in Globals.boards)
            {
                Results.results.Add(Unpack(key.Key)); //Yeah, this line isn't confusing at all.
            }
        }
        
            
        
            if (type == solveType.Diameter)
            {
                //Write out the moves to get to the positions which are hardest to get to.
                Console.WriteLine("Finished diameter search! Total time: {0}", DateTime.Now.Subtract(startTime));

                foreach (KeyValuePair<byte[], int[]> temp in Globals.boards)
                {


                    WriteBoard(temp);

                }
                
            }
            else
            {
                Console.WriteLine("Done! Total time: {0}", DateTime.Now.Subtract(startTime));
                if (type==solveType.AllNonSpecificSolutions)
                {
                    Console.WriteLine("{0} solutions found", NumSolutions);
                    Results.resultnum = step-1;

                }
            }
            //Console.WriteLine("Press enter to exit...");

            //Console.ReadLine();

        }
        static void GenerateNextBoards()
        {
            //Generates all the boards of the next step.
            //6/22/2011: SO MUCH SIMPLER. Now, it's a very simple BFT search which calls upon another function,MakeMoves, to do all the hard work.

            Globals.lastboards = new Dictionary<byte[], int[]>(Globals.boards, new IntArrayComparer()); //This is the same as =, but I wanted to be safe.
            Globals.boards = new Dictionary<byte[], int[]>(Globals.newboards, new IntArrayComparer());
            Globals.newboards = new Dictionary<byte[], int[]>(new IntArrayComparer());

            byte[] packed; //The packed board. Changes very often.
            byte[,] unpacked = new byte[Globals.y, Globals.x];

            byte lastmoved = 0; //The last moved piece.
            //int puznum = 0;
            //int goalnum = 1;
            foreach (KeyValuePair<byte[], int[]> puzzle in Globals.boards)
            {
                lastmoved = 0;
                if (puzzle.Value.Length != 0)
                {
                    lastmoved = (byte)(puzzle.Value.Last() >> 18);
                }
                packed = puzzle.Key;
                unpacked = Unpack(packed);
                for (byte p = 1; p <= Globals.numPieces; p++)
                {
                    if (lastmoved != p)
                    {


                        MakeMoves(unpacked, p, packed[Globals.piecePositions[Globals.isA[p - 1, 0] - 1] + Globals.isA[p - 1, 1] - 1], packed, puzzle.Value);
                    }

                }
            }
        }


        static void AddPuzzle(byte[] packedBoard, int[] movesSoFar, int encodedMove)
        {
            //Consider the following...
            Globals.PuzzlesConsidered++;

            if (!Globals.lastboards.ContainsKey(packedBoard)) //This one line is where it all happens. (NOT, see the 527 other lines)
            {
                if (!Globals.boards.ContainsKey(packedBoard)) //I'm not sure if we need this check. I'll have to find a counterexample or prove it.
                { //Ah, apparently we don't. Interesting...
                    // Excersize for the Reader: Prove that we don't need that line.

                    //Oh, it turns out we do for the move metric, but not for the step metric.
                    if (!Globals.newboards.ContainsKey(packedBoard))
                    {
                        int len = movesSoFar.Length;
                        int[] moves = new int[len + 1]; //This could cause a bit of the problem. I'm not sure.

                        for (int i = 0; i < len; i++)
                        {
                            moves[i] = movesSoFar[i]; //Eesh. Think there's a better way to do this?
                        }
                        moves[len] = encodedMove;
                        Globals.newboards.Add(packedBoard, moves);
                    }
                }
            }

        }
        public static void CopyTo(byte[] from, out byte[] to)
        //Copies an array.
        {
            to = new byte[from.Length];
            for (byte i = 0; i < from.Length; i++)
            {

                to[i] = from[i];

            }
        }
        static byte[,] Unpack(byte[] array)
        {
            //Unpacks an array. Could probably be shorter.
            byte[,] retarray = new byte[Globals.y, Globals.x];
            int upperleft;
            int upperup;
            int pos;
            int piecenum;

            for (Int64 pieceType = 0; pieceType < Globals.piecenums.Length; pieceType++)
            {
                for (Int64 piece = 0; piece < Globals.piecenums[pieceType]; piece++)
                {
                    piecenum = Globals.aIs[pieceType][piece] - 1;
                    pos = array[Globals.piecePositions[pieceType] + piece];
                    upperleft = pos % Globals.x;
                    upperup = pos / Globals.x;
                    for (Int64 cell = 0; cell < Globals.pieces[piecenum].Length; cell = cell + 2)
                    {
                        retarray[Globals.pieces[piecenum][cell] + upperup, Globals.pieces[piecenum][cell + 1] + upperleft] = (byte)(piecenum + 1);
                    }
                }
            }
            return retarray;
        }



        //---------------------------------------------------------
        //- This function is only used once. It compresses any unpacked puzzle.
        //----------------------------------------------------------------------

        static byte[] Pack(byte[,] array)
        {
            HashSet<byte> visitedPieces = new HashSet<byte>();
            visitedPieces.Add(0);
            List<List<byte>> templist = new List<List<byte>>();
            List<byte> retlist = new List<byte>();
            for (byte i = 0; i < Globals.aIs.Length; i++)
            {
                templist.Add(new List<byte>());
            }
            for (byte y = 0; y < Globals.y; y++)
            {
                for (byte x = 0; x < Globals.x; x++)
                {

                    if (!visitedPieces.Contains(array[y, x]))
                    {
                        visitedPieces.Add(array[y, x]);
                        templist[Globals.isA[array[y, x] - 1, 0] - 1].Add((byte)(Globals.x * y + x));
                    }
                }
            }
            for (byte i = 0; i < templist.Count; i++)
            {
                foreach (byte j in templist[i])
                {
                    retlist.Add(j);
                }
            }
            return retlist.ToArray();
        }

        //--------------------------------------------
        //The following is a function I'm working on, which should enable SSBPSolver to use the moves metric.
        //Goal: To combine all the CanMove- functions into a single function, eliminating about 75% of time.
        //Additionally, it should use a smart BFT search to find positions to go to, as well as integrate the internals of the GenerateNextBoards function.
        //Lastly, it should reduce the total lines of this program, as well as convert 8 for loops and 4 function calls into about 4 for loops and 1 function call.
        //--------------------------------------------


        static void MakeMoves(byte[,] board, byte piece, byte pieceLocation, byte[] packedPuzzle, int[] movesSoFar)
        {
            //I need to sleep on this.
            //Outline:
            //Get relative block positions, as well as piece position
            //Run a BFT search on "Can it move.." with options U,D,L,R. As you do that, add each new position using AddPuzzle
            //Use a y,x matrix to store already-there positions, with a list to store new nodes.
            //Like the parser.

            //Relative block positions are Globals.pieces[piece][n]
            //int piecelen = Globals.pieces[piece-1].Length; //Should be twice the number it should be
            HashSet<int> nodes = new HashSet<int>(); //TODO: Plz make HashSet.
            nodes.Add(pieceLocation); //pieceLocation starts at zero. To find it, you just use the nightmare function.
            bool[,] AlreadyVisited = new bool[Globals.y, Globals.x]; //Probably should be made 1D.
            AlreadyVisited[pieceLocation / Globals.x, pieceLocation % Globals.x] = true;
            int originalX = pieceLocation % Globals.x;
            int originalY = pieceLocation / Globals.x;
            int x;
            int y;
            byte position;
            byte[] tpack;
            if (piece == 8)
            {
                piece = 8;
                //Console.WriteLine(piecelen);
            }
            while (nodes.Count != 0)
            {
                position = (byte)nodes.First();
                y = position / Globals.x;
                x = position % Globals.x;
                //Check moves up.
                if (y != 0)
                {
                    if (!AlreadyVisited[y - 1, x])
                    {

                        if (CanMoveUp(board, (byte)(piece - 1), position))
                        {
                            nodes.Add(Globals.x * (y - 1) + x);
                            CopyTo(packedPuzzle, out tpack);
                            tpack[Globals.piecePositions[Globals.isA[piece - 1, 0] - 1] + Globals.isA[piece - 1, 1] - 1] = (byte)(position - Globals.x);
                            AlreadyVisited[y - 1, x] = true;
                            AddPuzzle(tpack, movesSoFar, EncodeMove(piece, (byte)Math.Abs(x - originalX), (byte)Math.Abs(y - 1 - originalY), getdirection(x - originalX, originalY - y + 1)));
                        }
                    }

                }
                //Check moves down
                if (y != Globals.y - 1)
                {
                    if (!AlreadyVisited[y + 1, x])
                    {

                        if (CanMoveDown(board, (byte)(piece - 1), position))
                        {
                            nodes.Add(Globals.x * (y + 1) + x);
                            CopyTo(packedPuzzle, out tpack);
                            tpack[Globals.piecePositions[Globals.isA[piece - 1, 0] - 1] + Globals.isA[piece - 1, 1] - 1] = (byte)(position + Globals.x);
                            AlreadyVisited[y + 1, x] = true;
                            AddPuzzle(tpack, movesSoFar, EncodeMove(piece, (byte)Math.Abs(x - originalX), (byte)Math.Abs(y + 1 - originalY), getdirection(x - originalX, originalY - y - 1)));
                        }
                    }
                }
                //Check moves left
                if (x != 0)
                {
                    if (!AlreadyVisited[y, x - 1])
                    {

                        if (CanMoveLeft(board, (byte)(piece - 1), position))
                        {
                            nodes.Add(Globals.x * (y) + x - 1);
                            CopyTo(packedPuzzle, out tpack);
                            tpack[Globals.piecePositions[Globals.isA[piece - 1, 0] - 1] + Globals.isA[piece - 1, 1] - 1] = (byte)(position - 1);
                            AlreadyVisited[y, x - 1] = true;
                            AddPuzzle(tpack, movesSoFar, EncodeMove(piece, (byte)Math.Abs(x - 1 - originalX), (byte)Math.Abs(y - originalY), getdirection(x - 1 - originalX, originalY - y)));
                        }
                    }
                }
                //Check moves right
                if (x != Globals.x - 1)
                {
                    if (!AlreadyVisited[y, x + 1])
                    {

                        if (CanMoveRight(board, (byte)(piece - 1), position))
                        {
                            nodes.Add(Globals.x * (y) + x + 1);
                            CopyTo(packedPuzzle, out tpack);
                            tpack[Globals.piecePositions[Globals.isA[piece - 1, 0] - 1] + Globals.isA[piece - 1, 1] - 1] = (byte)(position + 1);
                            AlreadyVisited[y, x + 1] = true;
                            AddPuzzle(tpack, movesSoFar, EncodeMove(piece, (byte)Math.Abs(x + 1 - originalX), (byte)Math.Abs(y - originalY), getdirection(x + 1 - originalX, originalY - y)));
                        }
                    }
                }
                //Remove node
                nodes.Remove(position);
                //nodes.RemoveAt(0);
            }


        }
        //The next 4 are self-explanatory
        //UPDATE: I've remade these methods to make them both much faster (factor of 3 on benchmark) and also much more cryptic.
        //The Left and Right functions are about 3 times as slow as the others.
        static Boolean CanMoveUp(byte[,] board, byte piece, byte pieceLocation)
        {
            byte temp;
            if (pieceLocation < Globals.x)
            {
                return false;
            }
            for (int i = 0; i < Globals.pieces[piece].Length; i += 2)
            {
                temp = board[pieceLocation / Globals.x + Globals.pieces[piece][i] - 1, pieceLocation % Globals.x + Globals.pieces[piece][i + 1]];
                if (!((temp == 0) || (temp == piece + 1))) //Yeah sure you can use De'Moivre's Theorem here, but that requires 3 operators.
                {
                    return false;
                }
            }

            return true;
        }
        static Boolean CanMoveLeft(byte[,] board, byte piece, byte pieceLocation)
        {
            byte temp;
            if (pieceLocation % Globals.x == 0) //This part may not be needed.
            {
                return false;
            }
            for (int i = 0; i < Globals.pieces[piece].Length; i += 2)
            {
                if ((Globals.pieces[piece][i + 1] + pieceLocation % Globals.x) == 0) //Believe it or not, but adding in %Globals.x actually speeds it up.
                {
                    return false;
                }
                temp = board[pieceLocation / Globals.x + Globals.pieces[piece][i], pieceLocation % Globals.x + Globals.pieces[piece][i + 1] - 1];
                if (!((temp == 0) || (temp == piece + 1)))
                {
                    return false;
                }

            }

            return true;
        }
        static Boolean CanMoveDown(byte[,] board, byte piece, byte pieceLocation)
        {
            byte temp;
            if (pieceLocation / Globals.x == Globals.y - 1)
            {
                return false;
            }
            for (int i = 0; i < Globals.pieces[piece].Length; i += 2)
            {
                if ((Globals.pieces[piece][i] + pieceLocation / Globals.x) == Globals.y - 1)
                {
                    return false;
                }

                temp = board[pieceLocation / Globals.x + Globals.pieces[piece][i] + 1, pieceLocation % Globals.x + Globals.pieces[piece][i + 1]];
                if (!((temp == 0) || (temp == piece + 1)))
                {
                    return false;
                }

            }

            return true;
        }
        static Boolean CanMoveRight(byte[,] board, byte piece, byte pieceLocation)
        {
            byte temp;
            if (pieceLocation % Globals.x == Globals.x - 1)
            {
                return false;
            }
            for (int i = 0; i < Globals.pieces[piece].Length; i += 2)
            {
                if ((Globals.pieces[piece][i + 1] + pieceLocation % Globals.x) == Globals.x - 1)
                {
                    return false;
                }
                temp = board[pieceLocation / Globals.x + Globals.pieces[piece][i], pieceLocation % Globals.x + Globals.pieces[piece][i + 1] + 1];
                if (!((temp == 0) || (temp == piece + 1)))
                {
                    return false;
                }

            }

            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------------------//
        //END ALGORITHMS SECTION                                                                                                              //
        // (There's more, but it's easier than the code above)                                                                                //
        //------------------------------------------------------------------------------------------------------------------------------------//

        static byte getdirection(int x, int y)
        {
            //Encode an x and y as a direction.
            //If this method is slow, something's wrong.
            //Seriously wrong.
            byte result = 0;
            if (x >= 0)
            {
                result = 2;
            }

            if (y >= 0)
            {
                result += 1;
            }
            return result;

        }
        public static bool ArrayEquals(byte[] x, byte[] y)
        {
            //Does this array equal another array?
            /*if (x.Length != y.Length)
            {
                return false;
            }*/
            for (int i = 0; i < x.Length; ++i)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool ArrayEquals(int[] x, int[] y)
        {
            //Does this array equal another array?
            if (x.Length != y.Length)
            {
                return false;
            }
            for (byte i = 0; i < x.Length; ++i)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static bool ArrayEquals(List<byte> x, List<byte> y) //For some reason this is slow. It takes up 50% of the program time.
        {
            //Does this array equal another array?
            /*if (x.Count != y.Count)
            {
                return false;
            }*/

            for (byte i = 0; i < x.Count; ++i)
            {

                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }
        static void MakePieceArray(byte[,] board, byte[] different)
        {
            //Initialize all the arrays of pieces, duplicates, numbers...
            //This is the function I spent the most time on.
            Boolean foundFirstPiece;
            Globals.pieces = new int[Globals.numPieces][];
            List<int> pieceToAdd;
            //Coordinates for width, height, pos of the pieces
            int upperleft = 0;
            int upperup = 0;
            //Separate piece numbers.
            for (int p = 1; p <= Globals.numPieces; p++)
            {
                pieceToAdd = new List<int>();
                foundFirstPiece = false;
                for (byte y = 0; y < Globals.y; y++)
                {
                    for (byte x = 0; x < Globals.x; x++)
                    {
                        if (board[y, x] == p)
                        {
                            if (!foundFirstPiece)
                            {
                                foundFirstPiece = true;
                                upperleft = x;
                                upperup = y;
                            }
                            pieceToAdd.Add(y - upperup);
                            pieceToAdd.Add(x - upperleft);
                        }
                    }
                }
                Globals.pieces[p - 1] = pieceToAdd.ToArray();
            }

            //Do duplicate checking and cross-referencing
            //NOTE: Since version 2011.10.30, the duplicate checker considers pieces as the same if they both:
            //Have same shape and
            //Are not listed in the different array.
            //Speaking of which, the song "Different" by Pendulum is really quite good.

            //Fill out the aIs array (progessive duplicate-checking)
            List<byte[]> unionPieces = new List<byte[]>(); //To become aIs
            Globals.isA = new byte[Globals.numPieces, 2];
            List<byte> piecenums = new List<byte>();
            for (byte p = 1; p <= Globals.numPieces; p++)
            {
                if (Globals.isA[p - 1, 0] != 0 && !different.Contains(p))
                {
                    //Then p is a duplicate of Globals.isA[p-1,0]
                    Globals.isA[p - 1, 1] = ++piecenums[Globals.isA[p - 1, 0] - 1];
                }
                else
                {
                    //It's a new piece!
                    piecenums.Add(1);
                    Globals.isA[p - 1, 0] = (byte)(piecenums.Count);
                    Globals.isA[p - 1, 1] = 1;
                    if (!different.Contains(p))
                    {
                        //Find all other pieces which are the same as the current piece.
                        for (int p2 = p + 1; p2 <= Globals.numPieces; p2++)
                        {
                            if (ArrayEquals(Globals.pieces[p - 1], Globals.pieces[p2 - 1]))
                            {
                                Globals.isA[p2 - 1, 0] = Globals.isA[p - 1, 0];
                            }
                        }
                    }

                }
            }
            Globals.piecenums = piecenums.ToArray();
            Globals.piecePositions = new byte[piecenums.Count];

            //Get piece positions
            byte sum = 0;
            Globals.piecePositions[0] = 0;
            for (int p = 2; p <= piecenums.Count; p++)
            {
                sum += piecenums[p - 2];
                Globals.piecePositions[p - 1] = sum;
            }
            //Cross-reference to aIs.
            for (int n = 0; n < piecenums.Count; n++)
            {
                unionPieces.Add(new byte[piecenums[n]]);
            }
            for (byte p = 1; p <= Globals.numPieces; p++)
            {
                unionPieces[Globals.isA[p - 1, 0] - 1][Globals.isA[p - 1, 1] - 1] = p;
            }
            Globals.aIs = unionPieces.ToArray();

        }

        static void WriteBoard(KeyValuePair<byte[], int[]> pair)
        {
            byte[,] unpackedBoard = Unpack(pair.Key);
            for (int celly = 0; celly < Globals.y; celly++)
            {
                for (int cellx = 0; cellx < Globals.x; cellx++)
                {
                    Console.Write(unpackedBoard[celly, cellx] + " ");
                }
                Console.WriteLine();
            }

            foreach (int move in pair.Value)
            {
                Console.Write(MoveToString(move) + " ");
            }
            Console.WriteLine();
        }
        static string MoveToString(int move)
        {
            int pn = move >> 18;
            int R = (move - (pn << 18)) >> 10;
            int U = (move - (pn << 18) - (R << 10)) >> 2;
            int dir = (move - (pn << 18) - (R << 10) - (U << 2));
            switch (dir)
            {
                case 0: return pn.ToString() + "L" + R.ToString() + "D" + U.ToString();
                case 1: return pn.ToString() + "L" + R.ToString() + "U" + U.ToString();
                case 2: return pn.ToString() + "R" + R.ToString() + "D" + U.ToString();
                case 3: return pn.ToString() + "R" + R.ToString() + "U" + U.ToString();
            }
            return "?";
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
        static byte EncodeMove(byte piecenum, byte dir)
        {
            return (byte)(((piecenum << 2)) | dir);
        }
        static int EncodeMove(byte piecenum, byte R, byte U, byte direction)
        {
            return (piecenum << 18) | (R << 10) | (U << 2) | direction;

            //HAIKU DOCUMENTATION TIME!

            //Sliding block solver
            //511 size
            //Should be good for you

            //larger than Sunshine
            //my program can handle it
            //watch out for your RAM

            //direction is a value from 0 to 3 stating the signs of R and U.
            //0 means negative, 1 means positive.
            //For example, 10=2 -> R is right, U is up.
            //11->R is right, U is down.
        }
        //Finds the maximum number of an array.
        //Used once.
        static byte Max(byte[,] array)
        {
            byte tmax=0;
            for (byte y = 0; y < Globals.y; y++)
            {
                for (byte x = 0; x < Globals.x; x++)
                {
                    if (array[y, x] > tmax)
                    {
                        tmax = array[y, x];
                    }
                }
            }
            return tmax;
        }

        static bool IsAtGoal(byte[] key, int piece, solveType type,int pos)
        {
            if (type != solveType.AllNonSpecificSolutions)
            {
                return pos==key[Globals.piecePositions[Globals.isA[piece - 1, 0] - 1] + Globals.isA[piece - 1, 1] - 1];
            }
            else
            {
                foreach (byte p2 in Globals.aIs[Globals.isA[piece - 1, 0] - 1])
                {
                    if (pos == key[Globals.piecePositions[Globals.isA[p2 - 1, 0] - 1] + Globals.isA[p2 - 1, 1] - 1])
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
    public class Globals
    {
        public static byte x; //Width
        public static byte y; //Height
        public static byte generation; //Never used.
        public static Int64 PuzzlesConsidered;
        public static Int64 TotalPuzzles;
        public static Dictionary<byte[], int[]> boards; //The last generation
        public static Dictionary<byte[], int[]> lastboards; //The generation 2 before.
        public static Dictionary<byte[], int[]> newboards; //The generation currently being constructed.
        public static byte numPieces; //The number of pieces.
        public static int[][] pieces; //Contains the relative positions (y,x) of each of the pieces.
        public static byte[] piecenums; //Contains the number of pieces of each type.
        public static byte[,] isA; //Given a piece number (first argument), [p,0] gives the piece TYPE, and [p,1] gives the unique piece number. See http://wiki.xkcd.com/irc/Blankfest 
        public static byte[][] aIs; //Given a piece type, and the number of that piece, this gives the original piece number.
        public static byte[] piecePositions;//where the starts of each of the ranges of pieces are.
        public static UInt64 sucess = 0;
        public static UInt64 collision = 0;
        public static IntArrayComparer comparer = new IntArrayComparer();



    }
    public class IntArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            //Tests whether two packed positions are equivalent with respect to permutation.
            //n log n time, where n is the number of elements.
            List<byte> tx = new List<byte>();
            List<byte> ty = new List<byte>();
            int numMax;
            for (byte i = 0; i < Globals.piecePositions.Length; i++)
            {
                if (i == Globals.piecePositions.Length - 1)
                {
                    numMax = x.Length;

                }
                else
                {
                    numMax = Globals.piecePositions[i + 1];
                }
                for (byte j = Globals.piecePositions[i]; j < numMax; j++)
                {
                    tx.Add(x[j]);
                    ty.Add(y[j]);
                }
                tx.Sort();
                ty.Sort();
                if (!Solver.ArrayEquals(tx, ty))
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
            int mret;
            int numMax;
            for (int i = 0; i < Globals.piecePositions.Length; i++)
            {
                mret = 1;
                if (i == Globals.piecePositions.Length - 1)
                {
                    numMax = obj.Length;

                }
                else
                {
                    numMax = Globals.piecePositions[i + 1];
                }
                
                for (int j = Globals.piecePositions[i]; j < numMax; j++)
                {
                    mret *= obj[j] + 1; //That way,a 0 won't kill it.
                }
                ret = (ret * 7907 + mret + 1); //Primes should probably be smaller.
            }
            return ret;
        }
    }
}
