
// imports
using System;

// package
namespace fold_cs
{
    class fold
    {

        public const uint DEFAULT_WIDTH = 80;

        static void Main(string[] args)
        {
            /* TODO: Implement these flags
             *  -b      measure width in bytes instead of columns/characters
             *  -s      keep words together in breaks if it reach/exceed width limit
             */

            uint width = DEFAULT_WIDTH;

            // read command line arguments
            bool nextArgumentSpecifiesWidth = false;
            foreach ( string argument in args )
            {
                if ( nextArgumentSpecifiesWidth )
                {
                    try
                    {
                        // TODO: catch uint overflow ( >~ 4x10E9 )
                        width = uint.Parse(argument);
                        Console.WriteLine(width);
                    } catch (FormatException ex )
                    {
                        Console.Error.WriteLine("ERROR: Invalid width value.");
                        Console.Error.WriteLine(ex.StackTrace);
                    }
                }
                // specifying width - check next argument
                if ( argument.Equals("-w") )
                {
                    nextArgumentSpecifiesWidth = true;
                }
                // Console.WriteLine(argument);
            }

            // pause program for debugging.
            string read = Console.ReadLine();
            Console.WriteLine(read);
        }
    }
}
