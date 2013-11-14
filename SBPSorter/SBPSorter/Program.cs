using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SBPSorter
{
    public class Globals
    {
        public static int sizeX = 4;
        public static int sizeY = 5;
        public static int xy = 20;
        public static PuzComparer puzComparer = new PuzComparer();
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SBPSORTER- a program to divide large .sbp files into smaller batches.");
            Console.WriteLine("Width, please:");
            int.TryParse(Console.ReadLine(),out Globals.sizeX);
            if (Globals.sizeX == 0)
                Console.WriteLine("That simply didn't make sense.\n That was a fatal error, my friend");

            Console.WriteLine("Additionally, I need to know the height of the board:");
            int.TryParse(Console.ReadLine(), out Globals.sizeY);
            if (Globals.sizeY == 0)
                Console.WriteLine("That simply didn't make sense.\n That was a fatal error, my friend");

            Console.WriteLine("One last thing: What's the .sbp file you're inputting?");
            string fileplace=Console.ReadLine();

            Globals.xy = Globals.sizeX * Globals.sizeY;

            //We're going to sort the SBPs by numbers of n-ominoes.

            BinaryReader fileReader=new BinaryReader(File.OpenRead(fileplace));
            Dictionary<byte[], BinaryWriter> tws = new Dictionary<byte[], BinaryWriter>(Globals.puzComparer);

            int numpuz = 65536;
            int CHUNK_SIZE = numpuz * Globals.xy;
            int chunksize;
            byte[] chunk=new byte[CHUNK_SIZE];
            byte[] tempboard;

            if (Directory.Exists("output"))
            {
                foreach (string f in Directory.GetFiles("output"))
                {
                    File.Delete(f);
                }
                Directory.Delete("output");
            }
            Directory.CreateDirectory("output");
            int total = 0;

            do
            {
                chunksize = fileReader.Read(chunk, 0, CHUNK_SIZE);
                if (chunksize !=0)
                {
                    for (int i = 0; i < chunksize/Globals.xy; i++)
                    {
                        tempboard = new byte[Globals.xy];
                        for (int j = 0; j < Globals.xy; j++)
                        {
                            tempboard[j] = chunk[i * Globals.xy + j];
                        }
                        byte[] bucket = Categorize(tempboard);
                        if(!tws.ContainsKey(bucket))tws.Add(bucket,new BinaryWriter(File.OpenWrite("output\\"+GetName(bucket))));
                        tws[bucket].Write(tempboard);
                    }
                }
                total += numpuz;
                Console.WriteLine("Processed {0} positions",total);

            } while (chunksize == CHUNK_SIZE);

            foreach(KeyValuePair<byte[],BinaryWriter> kvp in tws){
                kvp.Value.Close();
            }
            fileReader.Close();
        }

        static byte[] Categorize(byte[] board)
        {
            byte[] buckets = new byte[board.Length];
            int num;
            int p=0;
            do
            {
                p++;
                num = 0;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == p) num++;
                }
                if(num!=0)
                    buckets[num-1]++;
            } while (num != 0);
            return buckets;
        }

        static string GetName(byte[] lis)
        {
            string name = "";
            for (int i = 0; i < lis.Length; i++)
            {
                name += lis[i];
                if (i != lis.Length - 1) name += '-';
            }
            return name;
        }
    }

    public class PuzComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            for (int i = 0; i < Globals.xy; i++)
            {
                if (x[i] != y[i]) return false;
            }

            return true;


        }
        public int GetHashCode(byte[] obj)
        {
            //Gets the hash code, irrespective of permutation.
            int ret = 0;
            //int mret;
            //int numMax;
            for (int i = 0; i < Globals.xy; i++)
            {
                ret = (9973 * ret + obj[i]); //Primes should probably be smaller.
            }
            return ret;
        }
    }
}
