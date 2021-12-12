using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RF4STextureExtractor
{
    struct HeaderTex
    {
        public string namedds;
        public int Width;
        public int Height;
        public int sizeTexture;
    }
    class Program
    {
        static void Main(string[] args)
        {
            //recuerda ponerlo mayor
            if (args.Length > 0)
            {
                String namefile;
                bool import = false;
               if (args[0] == "-i")
                { import = true; }
                HeaderTex[] TextureDecode;
                if (import == true)
                { namefile = args[1];
                  //namefile = "propeller_ship_02.texture";
                }
                else
                { namefile = args[0];
                  //namefile = "propeller_ship_02.texture";
                }
                byte[] byteBuffer = File.ReadAllBytes(namefile);
                
                using (BinaryReader reader = new BinaryReader(File.Open(namefile, FileMode.Open)))
                {
                    reader.BaseStream.Position = 0x38;
                    int temp = reader.ReadInt32();
                    reader.BaseStream.Position = temp;
                    temp = reader.ReadInt32();
                    int texCounter = reader.ReadInt32();
                    TextureDecode = new HeaderTex[texCounter];
                    long Dummy2 = reader.ReadInt64();
                    Console.WriteLine("Textures: " + texCounter);
                    int posTemp = (texCounter * 16) + 8;
                    byte[] Dummy = reader.ReadBytes(posTemp);
                    long savePos = reader.BaseStream.Position;
                    char[] PIXL = reader.ReadChars(4);
                    
                    int SizePIXL = reader.ReadInt32();
                    
                    
                    for (int x = 0; x < texCounter; x++)
                    {
                        int result = (SizePIXL * ( x));
                        long newDirection = savePos + result;

                        reader.BaseStream.Position = newDirection;

                        Console.WriteLine("PIXL: " + reader.BaseStream.Position.ToString("X"));
                        PIXL = reader.ReadChars(4);
                        
                        SizePIXL = reader.ReadInt32();
                        reader.ReadBytes(0x1C);
                        TextureDecode[x].Width = reader.ReadInt32();
                        TextureDecode[x].Height = reader.ReadInt32();
                        reader.ReadBytes(0x24);
                        TextureDecode[x].sizeTexture = reader.ReadInt32();
                        TextureDecode[x].namedds = Path.GetFileNameWithoutExtension(namefile)+"_"+x;
                        Console.WriteLine("Name: " + TextureDecode[x].namedds);
                        Console.WriteLine("Size: " + TextureDecode[x].sizeTexture + " bytes " + TextureDecode[x].sizeTexture.ToString("X"));
                        Console.WriteLine("Width: " + TextureDecode[x].Width + " Width: " + TextureDecode[x].Width.ToString("X"));
                        Console.WriteLine("Heigth: " + TextureDecode[x].Height + " Height: " + TextureDecode[x].Height.ToString("X"));
                        Console.WriteLine("------------------------------------------------------------");

                    }

                    
                    
                    reader.BaseStream.Position = 0x30;
                    int TemporalOffset = reader.ReadInt32();
                    reader.BaseStream.Position = TemporalOffset + 0x10;
                    long OriginalOffset = reader.BaseStream.Position;




                    byte[] ddsHeader = Properties.Resources.bc7;
                   
                    
                  if (import == false)

                  {
                        for (int x = 0; x < texCounter; x++)
                        { byte[] content = reader.ReadBytes(TextureDecode[x].sizeTexture);
                            using (BinaryWriter writer = new BinaryWriter(File.Open(TextureDecode[x].namedds + ".dds", FileMode.Create)))
                            {
                                writer.Write(ddsHeader);
                                writer.Write(content);
                                writer.BaseStream.Position = 0xC;
                                writer.Write(TextureDecode[x].Height);
                                writer.Write(TextureDecode[x].Width);

                            }
                            Console.WriteLine(TextureDecode[x].namedds + ".dds");
                            Console.WriteLine("Extraction completed.");
                        }
                          
                        
                  }



                    
                  byte[] toWrite; 

                  if (import == true)
                  {
                    using (BinaryWriter writer = new BinaryWriter(File.Open("new_" + Path.GetFileName(namefile), FileMode.Create)))
                        {
                            writer.Write(byteBuffer);
                            writer.BaseStream.Position = OriginalOffset;
                            for (int x = 0; x < texCounter; x++)
                            { using (BinaryReader reader2 = new BinaryReader(File.Open(TextureDecode[x].namedds + ".dds", FileMode.Open)))
                                {
                                    reader2.BaseStream.Position = 0x94;
                                    toWrite = reader2.ReadBytes(TextureDecode[x].sizeTexture);
                                }

                                writer.Write(toWrite);
                                Console.WriteLine(TextureDecode[x].namedds + ".dds imported...");
                            }

                      
                         

                          
                          
                    }

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
