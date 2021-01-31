using System;
using System.IO;
// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree
namespace finder
{
    public class RecursiveFileSearch
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        static int foundCount = 0;
        const int checkCount = 1000;
        static int Main(string[] args)
        {
            // DirectoryInfo root = new DirectoryInfo(@"/cloud9/backup/var/tmp");
            // string pattern = "*.*";

            if (args.Length < 1)
            {
                System.Console.WriteLine("Please enter a path and filespec.");
                return 1;
            }
            DirectoryInfo root = new DirectoryInfo(args[0]);
            string pattern = args[1];


            Console.WriteLine($"Searching {root} for {pattern})");

            // This is the equivalent of: var files = System.IO.Directory.GetFiles(args[0], patt, System.IO.SearchOption.AllDirectories);
            // AllDirectories doesn't give us a chance to interrupt, and we can get a loop on links.
            FilesThenDirs(root, pattern);
            Console.WriteLine($"File count: {foundCount}");
            // Emit UnauthorizedAccessException entries.
            Console.WriteLine($"Files with restricted access: {log.Count}");
            // foreach (string s in log)
            // { Console.WriteLine(s); }

            return 0;
        }

        static void FilesThenDirs(System.IO.DirectoryInfo root, string pattern)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            try // All files.
            {
                files = root.GetFiles(pattern);
            }

            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            } // Log permission exceptions.  

            catch (System.IO.DirectoryNotFoundException e) // Not sure when this will happen.
            {
                Console.WriteLine(e.Message);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                Console.WriteLine(root.FullName);
                foreach (System.IO.FileInfo fi in files)
                {
                    Console.WriteLine($"{fi.Name}");
                    foundCount += 1;
                    if ((foundCount % checkCount) == 0 )
                    {
                        Console.WriteLine($"Found count is {foundCount}; press a key to continue.");
                        Console.Read();
                     }
                }

                // All subdirectories.
                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs) // Recurse.
                { FilesThenDirs(dirInfo, pattern); }
            }
        }
    }
}