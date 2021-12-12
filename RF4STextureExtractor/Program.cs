using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RF4STextureExtractor
{
    struct HeaderTex
    { public long rawOffset;
        public int Width;
        public int Height;
        public int sizeTexture;
    }
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                String namefile;
                bool import = false;
                if (args[0] == "-i")
                { import = true; }
                HeaderTex TextureDecode;
                if (import == true)
                { namefile = args[1]; }
                else
                { namefile = args[0]; }
                byte[] byteBuffer = File.ReadAllBytes(namefile);
                string byteBufferAsString = System.Text.Encoding.UTF8.GetString(byteBuffer);
                Int32 offset1 = byteBufferAsString.IndexOf("PIXL");
                using (BinaryReader reader = new BinaryReader(File.Open(namefile, FileMode.Open)))
                {
                    reader.BaseStream.Position = offset1 + 0x24;
                    TextureDecode.Width = reader.ReadInt32();
                    TextureDecode.Height = reader.ReadInt32();

                    Console.WriteLine("Width: " + TextureDecode.Width);
                    Console.WriteLine("Heigth: " + TextureDecode.Height);


                    reader.BaseStream.Position = 0x30;
                    int TemporalOffset = reader.ReadInt32();
                    reader.BaseStream.Position = TemporalOffset;
                    long initRaw = reader.ReadInt64();
                    TextureDecode.sizeTexture = reader.ReadInt32();
                    TextureDecode.sizeTexture -= 0x10;
                    int dummy = reader.ReadInt32();
                    TextureDecode.rawOffset = reader.BaseStream.Position;

                    reader.BaseStream.Position = TextureDecode.rawOffset;
                    byte[] ddsHeader = Properties.Resources.bc7;
                    byte[] content = reader.ReadBytes(TextureDecode.sizeTexture);

                    if (import == false)

                    {
                        using (BinaryWriter writer = new BinaryWriter(File.Open(Path.GetFileNameWithoutExtension(namefile) + ".dds", FileMode.Create)))
                        {
                            writer.Write(ddsHeader);
                            writer.Write(content);
                            writer.BaseStream.Position = 0xC;
                            writer.Write(TextureDecode.Height);
                            writer.Write(TextureDecode.Width);

                        }
                    }
                   

                }

                byte[] toWrite; 

                if (import == true)
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(Path.GetFileNameWithoutExtension(namefile) + ".dds", FileMode.Open)))
                    {

                        reader.BaseStream.Position = 0x94;
                        toWrite = reader.ReadBytes(TextureDecode.sizeTexture);


                    }

                    using (BinaryWriter writer = new BinaryWriter(File.Open("Modded_" + Path.GetFileName(namefile), FileMode.Create)))
                    {
                        writer.Write(byteBuffer);

                        writer.BaseStream.Position = TextureDecode.rawOffset;
                        writer.Write(toWrite);

                    }
                }
               

            }
            else
            { Console.WriteLine("Drag and Drop your .texture File in the app.");
                Console.WriteLine("To re-import a texture add '-i' before the .texture File");
                Console.WriteLine("Questions? https://github.com/darkxex/");
                Console.ReadLine();
            }
        }
    }
}
