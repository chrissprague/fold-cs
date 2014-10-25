
// imports
using System;

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

        static void Main(string[] args)
        {
            /* TODO: Implement these flags
             *  -b      measure width in bytes instead of columns/characters
             *  -s      keep words together in breaks if it reach/exceed width limit
             */

            uint width = DEFAULT_WIDTH;
            bool measureInBytes = false;
            bool retainWordStructure = false;

            // read command line arguments
            bool nextArgumentSpecifiesWidth = false;
            bool readingFiles = false;
            foreach ( string argument in args )
            {
                // User is specifying a flag argument to the program.
                if ( ( argument[0] == '-' ) && ( argument.Length >= 2 ) )
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
            }

            Console.WriteLine("The width is: "+width);

            // pause program for debugging.
            string read = Console.ReadLine();
        }
    }
}
