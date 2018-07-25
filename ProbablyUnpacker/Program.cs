using AxTools.Helpers.MemoryManagement;
using PeNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProbablyUnpacker
{
    class Program
    {

        static PeNet.Structures.IMAGE_SECTION_HEADER GetSectionHeader(PeFile file, string name)
        {
            foreach (var section in file.ImageSectionHeaders)
            {
                string section_name = Encoding.UTF8.GetString(section.Name).TrimEnd('\0'); ;
                if (section_name == name)
                {
                    return section;
                }
            }

            throw new Exception("section not found");
        }

        static void Main(string[] args)
        {
            int processID = GetProcessID();
            if (processID != 0)
            {
                MemoryManager memoryManager = new MemoryManager(Process.GetProcessById(processID));
                IntPtr startAddress = memoryManager.ImageBase + 0x1000;
                Console.WriteLine($"Enter size of .text segment (module memory size is 0x{memoryManager.Process.MainModule.ModuleMemorySize.ToString("X")})");
                if (int.TryParse(Console.ReadLine(), out int sizeOfTextSegment))
                {
                    byte[] buffer = memoryManager.ReadBytes(startAddress, sizeOfTextSegment);
                    string pathToExecutable = GetPathToOriginalExecutable();
                    PeFile peFile = new PeFile(pathToExecutable);
                    PeNet.Structures.IMAGE_SECTION_HEADER text_section = GetSectionHeader(peFile, ".text");
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(new System.IO.MemoryStream(buffer));
                    System.IO.BinaryWriter writer = new System.IO.BinaryWriter(new System.IO.MemoryStream(buffer));
                    System.IO.StreamWriter log = new System.IO.StreamWriter("log.txt");
                    foreach (var block in peFile.ImageRelocationDirectory)
                    {
                        foreach (var offset in block.TypeOffsets)
                        {
                            UInt64 rva = offset.Offset + block.VirtualAddress;
                            log.WriteLine("relocation : 0x" + rva.ToString("X4"));
                            log.Flush();
                            if (rva >= text_section.VirtualAddress && rva < text_section.VirtualAddress + text_section.VirtualSize)
                            {
                                switch (offset.Type)
                                {
                                    case 3:
                                        int file_ofs = (int)(rva - text_section.VirtualAddress);
                                        reader.BaseStream.Seek(file_ofs, System.IO.SeekOrigin.Begin);
                                        UInt64 oldOffset = reader.ReadUInt64();
                                        writer.Seek(file_ofs, System.IO.SeekOrigin.Begin);
                                        writer.Write((oldOffset - (ulong)memoryManager.ImageBase + peFile.ImageNtHeaders.OptionalHeader.ImageBase));
                                        break;
                                    case 0:
                                        break;
                                    default:
                                        //throw new Exception("wrong relocation offset type");
                                        break;
                                }
                            }
                        }
                    }
                    int text_size = Math.Min(buffer.Length, (int)text_section.SizeOfRawData);
                    System.IO.FileStream wow_file = System.IO.File.OpenRead(pathToExecutable);
                    byte[] wow_data = new byte[wow_file.Length];
                    wow_file.Read(wow_data, 0, (int)wow_file.Length);
                    writer = new System.IO.BinaryWriter(new System.IO.MemoryStream(wow_data));
                    writer.Seek((int)text_section.PointerToRawData, System.IO.SeekOrigin.Begin);
                    writer.Write(buffer);

                    System.IO.File.Open($"{pathToExecutable.TrimEnd('e').TrimEnd('x').Trim('e')}.dumped.exe", System.IO.FileMode.CreateNew).Write(wow_data, 0, wow_data.Length);
                }
            }
        }

        private static int GetProcessID()
        {
            Process defaultProcess = Process.GetProcessesByName("Wow").FirstOrDefault();
            if (defaultProcess != default(Process))
            {
                int processID = defaultProcess.Id;
                Console.WriteLine($"Enter ID of process to dump: ({defaultProcess.Id})");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) || int.TryParse(input, out processID))
                {
                    return processID;
                }
            }
            else
            {
                Console.WriteLine("Enter ID of process to dump: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int processID))
                {
                    return processID;
                }
            }
            return 0;
        }

        private static string GetPathToOriginalExecutable()
        {
            Console.WriteLine("Enter path to original executable: (G:\\games\\World of Warcraft\\Wow.exe)");
            string input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return "G:\\games\\World of Warcraft\\Wow.exe";
            }
            else
            {
                return input;
            }
        }


    }
}
