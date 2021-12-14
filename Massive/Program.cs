using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MassiveTexRF4
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string[] files = System.IO.Directory.GetFiles(args[0], "*.texture");

                // Create a file to write to.
                using (StreamWriter sw = File.CreateText("MassiveImporter.bat"))
                {
                    foreach (string file in files)
                    {
                        Console.WriteLine(file);
                        sw.WriteLine("RF4STextureExtractor -i \"" + file + "\"");

                    }
                }

                using (StreamWriter sw = File.CreateText("MassiveExporter.bat"))
                {
                    foreach (string file in files)
                    {

                        sw.WriteLine("RF4STextureExtractor \"" + file + "\"");

                    }
                }
                Console.ReadLine();
            }
            else
            { Console.WriteLine("Drag and Drop the folder with textures.");
              Console.WriteLine("Questions? https://github.com/darkxex/");

                Console.ReadLine();
             
            }
           
        }

    }
}
