
// imports
using System;
using System.Collections.Generic;
using System.IO;

// package
namespace fold_cs
{
    class fold
    {

        public const uint DEFAULT_WIDTH = 80;

        private static uint getWidthFromInput(string input)
        {
            uint width = 80;
            if ( "".Equals(input) ) // also handles null
            {
                // TODO: Error message
                return 0;
            }

            try
            {
                // TODO: catch uint overflow ( >~ 4x10E9 )
                width = uint.Parse(input);
            }
            catch (FormatException ex)
            {
                Console.Error.WriteLine("ERROR: Invalid width value.");
                Console.Error.WriteLine(ex.StackTrace);
                // TODO: exit with non-zero error code here
            }
            return width;
        }

        private static void doFold(StreamReader sr, uint width,
            bool measureInBytes, bool retainWordStructure)
        {
            if ( sr == null )
            {
                Console.Error.WriteLine("WARNING: received a null StreamReader, skipping...");
                return;
            }

            String line = null;

            // read file line-by-line
            while((line = sr.ReadLine()) != null )
            {
                if ( line.Length < width )
                {
                    // the length of the line is less than the width, we can just echo here.
                    Console.WriteLine(line);
                }
                else
                {
                    // the length of the line exceeded the width, so we need to fold here.
                    int currentIndex = 0;
                    while (currentIndex < line.Length)
                    {
                        for (int i = 0; i < width && currentIndex < line.Length; i++)
                        {
                            Console.Write(line[currentIndex++]);
                        }
                        Console.WriteLine();
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            /* TODO: Implement these flags
             *  -b      measure width in bytes instead of columns/characters
             *  -s      keep words together in breaks if it reach/exceed width limit
             */

            uint width = DEFAULT_WIDTH;
            bool measureInBytes = false;
            bool retainWordStructure = false;
            List<String> fileNames = new List<string>();
            
            // read command line arguments
            bool nextArgumentSpecifiesWidth = false;
            bool readingFiles = false;
            foreach ( string argument in args )
            {
                // User is specifying a flag argument to the program.
                if ( ( argument[0] == '-' ) && ( argument.Length >= 2 ) && ( ! readingFiles ) )
                {
                    for ( byte i = 1 ; i < argument.Length ; ++i )
                    {
                        char flag = argument[i];
                        if ( nextArgumentSpecifiesWidth )
                        {
                            // if width value not separated from the 'w' flag with a space
                            // example: 'fold -w10 test.txt'
                            width = getWidthFromInput(argument.Substring(i));
                            nextArgumentSpecifiesWidth = false;
                            break;
                        }
                        if ( ! ( flag == 'w' ) || ( flag == 's' ) || ( flag == 'b' ) )
                        {
                            Console.Error.WriteLine("ERROR: Invalid flag '{0}'.", flag);
                        } else
                        {
                            if (flag == 'b')
                                measureInBytes = true;
                            if (flag == 's')
                                retainWordStructure = true;
                            if (flag == 'w')
                                nextArgumentSpecifiesWidth = true;
                        }
                    }
                } else if ( nextArgumentSpecifiesWidth )
                {
                    nextArgumentSpecifiesWidth = false;
                    width = getWidthFromInput(argument);
                }
                else // specifying a target filename.
                {
                    readingFiles = true;
                    fileNames.Add(argument);
                }
            }

            int numFiles = fileNames.Count;

            Console.WriteLine("The specified width is: " + width);
            Console.WriteLine("The number of files specified is: " + numFiles);
            Console.WriteLine("The files are:");
            Console.Write("     ");

            for ( int i = 0 ; i < numFiles ; i++ )
            {
                Console.Write(fileNames[i]);
                if (i + 1 != numFiles)
                    Console.Write(", ");
                else
                    Console.WriteLine(".");
            }

            Console.WriteLine(Directory.GetCurrentDirectory());

            List<StreamReader> streamReaders = new List<StreamReader>();

            // open StreamReaders
            foreach ( String fileName in fileNames )
            {
                try
                {
                    streamReaders.Add(new StreamReader(@fileName));
                } catch ( System.IO.FileNotFoundException )
                {
                    Console.Error.WriteLine("ERROR: File not found: \"{0}\".", fileName);
                }
            }

            // do stuff with files here
            foreach (StreamReader fs in streamReaders)
            {
                doFold(fs, width, measureInBytes, retainWordStructure);
            }

            // close StreamReaders so resource leaks do not occur
            foreach (StreamReader fs in streamReaders)
            {
                // TODO close StreamReaders as they are completed, not all at once at the end
                fs.Close();
            }

            // pause program for debugging.
            string read = Console.ReadLine();
        }
    }
}
