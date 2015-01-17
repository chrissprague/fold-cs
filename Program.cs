
// imports
using System;
using System.Collections.Generic;
using System.IO;

// package
namespace fold_cs
{
    /**
     *  Implementation class of BSD's fold in C#.
     *  Program is run through the command line with arguments
     *  to specify behavior, designed to emulate, as closely as
     *  possible, that of fold.
     *  
     * Author: Christopher Sprague
     * Date: November 2014
     * See: https://github.com/chrissprague/fold-cs
     */
    class fold
    {
        public const uint DEFAULT_WIDTH = 80;

        /**
         * Attempt to parse the given input string into the width parameter.
         * 
         * Params:
         *  String input    - input string, ready to be parsed to get the width
         *  
         * Returns
         *  uint width      - the width, parsed from the input, if op was successful.
         * 
         */
        private static uint getWidthFromInput(String input)
        {
            uint width = 80;
            if ( "".Equals(input) || input == null )
            {
                Console.Error.WriteLine("ERROR: Missing or bad input for width argument.");
                Environment.Exit(2);
            }

            try
            {
                if ( input.Trim().Length > 7 )
                {
                    Console.Error.WriteLine("ERROR: Specified width value ({0}) too large, " +
                        "reverting to max value 999,999.", input);
                    width = 999999;
                }
                else
                {
                    width = uint.Parse(input);
                }
                
            }
            catch (FormatException ex)
            {
                Console.Error.WriteLine("ERROR: Invalid width value.");
                Console.Error.WriteLine(ex.StackTrace);
                Environment.Exit(1);
            }
            return width;
        }

        /**
         * doFold reads through the given StreamReader to echo the text
         * read from it. If the length of a given line read from the StreamReader
         * is longer than the width (specified either by default or by the program's
         * arguments), doFold will terminate ths current line and write the rest
         * of the line that was read in from the StreamReader on the next line
         * (this process will continue to wrap onto new lines until the line read
         * is exhausted.)
         * 
         * Params:
         *  StreamReader sr             - StreamReader for a file specified in program arguments
         *  uint width                  - length of a line in order to do a fold.
         *  bool measureInBytes         - if true, length is measured in bytes rather than chars
         *  bool retainWordStructure    - if true, words are kept together on new lines
         * 
         * Returns:
         *  None
         * 
         */
        private static void doFold(StreamReader sr, uint width,
            bool measureInBytes = false, bool retainWordStructure = false)
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
                if ( measureInBytes )
                {
                    // mib
                    if (retainWordStructure)
                    {
                        // rws

                        if (line.Length < width)
                        {
                            // the length of the line is less than the width, we can just echo here.
                            Console.WriteLine(line);
                        }
                        else
                        {

                            int currIndex = 0;
                            String buffer = null;
                            for (int i = 0; i < width && currIndex < line.Length; i++)
                            {
                                if (line[currIndex].Equals(' ') || line[currIndex].Equals('-') )
                                {
                                    Console.Write(buffer);
                                }
                                buffer += line[currIndex++];
                            }
                            Console.WriteLine();
                            while (buffer.Length > 0)
                            {
                                Console.Write(buffer[0]);
                                buffer = buffer.Remove(0);
                            }
                            Console.WriteLine();
                        }

                    }
                    else
                    {
                        // standard

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
                } else
                {
                    // standard

                    if (retainWordStructure)
                    {
                        // rws

                        if (line.Length < width)
                        {
                            // the length of the line is less than the width, we can just echo here.
                            Console.WriteLine(line);
                        }
                        else
                        {

                            int currIndex = 0;
                            int currLinePos = 0;
                            String buffer = line;

                            // for every character on this line...
                            while (currIndex < line.Length)
                            {
                                if ( ( buffer.Length > 0 ) && 
                                    ( currLinePos + buffer.Length <= width ) &&
                                    ( line[currIndex].Equals(' ') || line[currIndex].Equals('-') ) )
                                {
                                    Console.Write(buffer);
                                    currLinePos += buffer.Length;
                                    buffer = "";
                                } else if ( buffer.Length > width )
                                {
                                    if ( currLinePos != 0 )
                                    {
                                        Console.WriteLine();

                                    }
                                    else
                                    {
                                        int index = 0;
                                        bool hyphenate = true;
                                        while ( index < buffer.Length )
                                        {
                                            for (int i = 0; i < width - 1; i++)
                                            {
                                                if ( index < buffer.Length )
                                                {
                                                    Console.Write(buffer[index++]);
                                                    currLinePos++;
                                                    currIndex++;
                                                }
                                                else
                                                {
                                                    hyphenate = false;
                                                    break;
                                                }
                                            }
                                            if (hyphenate)
                                            {
                                                Console.WriteLine("-");
                                                currLinePos = 0;
                                            }

                                        }
                                        buffer = "";
                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        // standard

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
        }

        /**
         * Main program function. Reads in command-line arguments in order to
         * determine what will be performed at runtime.
         * 
         * Params:
         *  string[] args   - command-line arguments provided to the program
         *  
         * Returns:
         *  0 for exit success,
         *      otherwise the program will exit prematurely with a non-zero 
         *      exit code to signify failure
         */
        static int Main(string[] args)
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
                        if ( ! ( flag == 'w' ) && ! ( flag == 's' ) && ! ( flag == 'b' ) )
                        {
                            Console.Error.WriteLine("ERROR: Invalid flag '{0}'.", flag);
                        } else
                        {
                            if (flag == 'b')
                            {
                                Console.WriteLine("Measuring in bytes instead of characters.");
                                measureInBytes = true;
                            }
                            if (flag == 's')
                            {
                                Console.WriteLine("Word structure will be retained on lines.");
                                retainWordStructure = true;
                            }
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
            // write another new line so output looks nicer.
            Console.WriteLine();

            // do stuff with files here
            foreach (String fileName in fileNames)
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(fileName);
                    Console.WriteLine("===== " + fileName + " =====");
                    doFold(sr, width, measureInBytes, retainWordStructure);
                    Console.WriteLine();
                }
                catch (System.IO.FileNotFoundException)
                {
                    Console.Error.WriteLine("ERROR: File not found: \"{0}\".", fileName);
                }
            }

            // pause program for debugging.
            string read = Console.ReadLine();

            // exit success
            return 0;
        }
    }
}
