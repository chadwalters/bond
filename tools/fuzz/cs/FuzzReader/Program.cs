using System;
using System.IO;

using Bond;

using unittest.compat;

using Bond.Protocols;
using Bond.IO.Unsafe;

namespace FuzzReader
{

    class Program
    {
        const int MaxFileSize = 0x02000000;

        static int GetMagic(string filename)
        {
            int dotPos = filename.LastIndexOf('.');

            if (dotPos == filename.Length - 3)
            {
                return (((filename[dotPos + 2] - 'a' + 'A')) << 8) + ((filename[dotPos + 1] - 'a' + 'A'));
            }
            return 0;
        }


        static void Main(string[] args)
        {
            byte[] data = null;
            string errMsg = null;
            string fileName = null;

            if (args.Length != 1)
            {
                errMsg = "Expected exactly one command-line argument";
            }
            else
            {
                fileName = args[0];

                try
                {
                    using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
                    {
                        if (stream.Length <= MaxFileSize)
                        {
                            data = new byte[MaxFileSize];

                            int bytesRead = stream.Read(data, 0, (int)stream.Length);

                            if (bytesRead != stream.Length)
                            {
                                errMsg = "Failed to read all bytes";
                                data = null;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                    data = null;
                }
            }

            if (data != null && fileName != null)
            {
                int magic = GetMagic(fileName);
                var input = new InputBuffer(data);

                try
                {
                    switch (magic)
                    {
                        case (int)ProtocolType.COMPACT_PROTOCOL:
                            {
                                var reader = new CompactBinaryReader<InputBuffer>(input);
                                var obj = Deserialize<Compat>.From(reader);
                                Console.WriteLine("Success!");
                                break;
                            }

                        case (int)ProtocolType.FAST_PROTOCOL:
                            {
                                var reader = new FastBinaryReader<InputBuffer>(input);
                                var obj = Deserialize<Compat>.From(reader);
                                Console.WriteLine("Success!");
                                break;
                            }

                        case (int)ProtocolType.SIMPLE_PROTOCOL:
                            {
                                var reader = new SimpleBinaryReader<InputBuffer>(input);
                                var obj = Deserialize<Compat>.From(reader);
                                Console.WriteLine("Success!");
                                break;
                            }

                        case (int)ProtocolType.SIMPLE_JSON_PROTOCOL:
                            {
                                var theString = System.Text.UTF8Encoding.UTF8.GetString(data);
                                var reader = new SimpleJsonReader(new StringReader(theString));
                                var obj = Deserialize<Compat>.From(reader);
                                Console.WriteLine("Success!");
                                break;
                            }

                        case 0:
                            Console.WriteLine("Could not parse extension properly-- expected exactly 2 characters: " + fileName);
                            break;

                        default:
                            Console.WriteLine("Extension did not match known one: " + fileName);
                            break;
                    }
                }
                catch (System.IO.InvalidDataException ex)
                {
                    Console.WriteLine("Caught InvalidDataException: " + ex.Message);
                }
                catch (System.ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine("Caught ArgumentOutOfRangeException: " + ex.Message);
                }
                catch (System.IO.EndOfStreamException ex)
                {
                    Console.WriteLine("Caught EndOfStreamException: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: " + errMsg);
            }
        }
    }
}
