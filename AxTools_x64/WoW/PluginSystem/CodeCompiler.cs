using AxTools.Helpers;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace AxTools.WoW.PluginSystem
{
    internal class CodeCompiler
    {
        private static readonly Log2 log = new Log2("CodeCompiler");

        // Methods
        public CodeCompiler(string path)
        {
            SourceFilePaths = new List<string>();
            string hash = null;
            if (File.Exists(path))
            {
                FileStructure = FileStructureType.SingleFile;
                using (MD5 md5 = MD5.Create())
                {
                    byte[] fileBytes = File.ReadAllBytes(path);
                    byte[] hashBytes = md5.ComputeHash(fileBytes);
                    hash = BitConverter.ToString(hashBytes).Replace("-", "");
                }
            }
            else if (Directory.Exists(path))
            {
                FileStructure = FileStructureType.Folder;
                hash = Utils.CreateMd5ForFolder(path);
            }
            SourcePath = path;
            Options = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = false, IncludeDebugInformation = false };
            string str = "BW_" + Assembly.GetEntryAssembly().GetName().Version.Revision;
            Options.CompilerOptions = $"/d:BW;{str} /unsafe";
            Options.TempFiles = new TempFileCollection(Path.GetTempPath());
            string assemblyPath = Path.Combine(AppFolders.PluginsBinariesDir, (hash ?? Utils.GetRandomString(16, false)) + ".dll");
            DeleteOldAssembly(assemblyPath);
            Options.OutputAssembly = assemblyPath;
            CompiledToLocation = Options.OutputAssembly;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    AddReference(assembly.Location);
                }
                catch (NotSupportedException)
                {
                }
                catch (Exception ex)
                {
                    log.Error($"Can't add {assembly.Location}: {ex.Message}");
                }
            }
            string ver = $"{Environment.Version.Major}.{Environment.Version.MajorRevision}.{Environment.Version.Build}";
            string dllPath = $"C:\\WINDOWS\\Microsoft.NET\\Framework{(Environment.Is64BitProcess ? "64" : "")}\\v{ver}\\Microsoft.CSharp.dll";
            try
            {
                AddReference(dllPath);
            }
            catch (Exception ex)
            {
                log.Error($"Can't add Microsoft.CSharp ({dllPath}): {ex.Message}");
            }
        }

        public Assembly CompiledAssembly { get; private set; }

        public string CompiledToLocation { get; }

        public FileStructureType FileStructure { get; }

        public CompilerParameters Options { get; }

        public List<string> SourceFilePaths { get; }

        public string SourcePath { get; }

        public void AddReference(string assembly)
        {
            if (!Options.ReferencedAssemblies.Contains(assembly))
            {
                log.Info($"Adding reference to {assembly}");
                Options.ReferencedAssemblies.Add(assembly);
            }
        }

        public CompilerResults Compile()
        {
            AddCS();
            ParseFilesForCompilerOptions();
            if (SourceFilePaths.Count == 0)
            {
                return null;
            }
            
            using (Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider provider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider())
            {
                provider.Supports(GeneratorSupport.Resources);
                CompilerResults results = provider.CompileAssemblyFromFile(Options, SourceFilePaths.ToArray());
                if (!results.Errors.HasErrors)
                {
                    CompiledAssembly = results.CompiledAssembly;
                }
                results.TempFiles.Delete();
                return results;
            }
        }

        private static void DeleteOldAssembly(string filepath)
        {
            if (!Directory.Exists(AppFolders.PluginsBinariesDir))
            {
                Directory.CreateDirectory(AppFolders.PluginsBinariesDir);
            }
            else
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
            }
        }

        private void AddRESX(string string2)
        {
            string path = Path.ChangeExtension(string2, ".resources");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (ResXResourceReader reader = new ResXResourceReader(string2))
            {
                reader.BasePath = SourcePath;
                using (ResourceWriter writer = new ResourceWriter(path))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        string name = entry.Key.ToString();
                        writer.AddResource(name, entry.Value);
                    }
                }
            }
            Options.EmbeddedResources.Add(path);
        }

        private void AddCS()
        {
            if (FileStructure == FileStructureType.Folder)
            {
                string[] strArray = Directory.GetFiles(SourcePath, "*.cs", SearchOption.AllDirectories);

                foreach (string s in strArray)
                {
                    SourceFilePaths.Add(s);
                }

                string[] strArray2 = Directory.GetFiles(SourcePath, "*.resx", SearchOption.AllDirectories);

                foreach (string s in strArray2)
                {
                    AddRESX(s);
                }
            }
            else
            {
                SourceFilePaths.Add(SourcePath);
            }
        }

        private bool Method_2(string string2)
        {
            Class153 class2 = new Class153
            {
                String0 = string2
            };
            return AppDomain.CurrentDomain.GetAssemblies().Any(class2.Method_0);
        }

        public void ParseFilesForCompilerOptions()
        {
            foreach (string filePath in SourceFilePaths)
            {
                foreach (string line in File.ReadAllLines(filePath))
                {
                    string lineTrimmed = line.Trim();
                    if (lineTrimmed.StartsWith("//!CompilerOption:"))
                    {
                        string[] compilerOptionArray = lineTrimmed.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string compilerOptionCommand = compilerOptionArray[1];
                        if (compilerOptionCommand != null)
                        {
                            if (compilerOptionCommand != "AddRef")
                            {
                                if ((compilerOptionCommand == "Optimise") && (compilerOptionArray.Length == 3) && !string.IsNullOrEmpty(compilerOptionArray[2]) && (compilerOptionArray[2] == "On") && !Options.CompilerOptions.Contains("/optimise"))
                                {
                                    Options.IncludeDebugInformation = false;
                                    CompilerParameters options = Options;
                                    options.CompilerOptions = options.CompilerOptions + " /optimise";
                                }
                            }
                            else if ((compilerOptionArray.Length == 3) && !string.IsNullOrEmpty(compilerOptionArray[2]) && compilerOptionArray[2].EndsWith(".dll") && !Method_2(compilerOptionArray[2]))
                            {
                                AddReference(compilerOptionArray[2]);
                            }
                        }
                    }
                }
            }
        }

        // Nested Types

        #region Nested type: Class153

        [CompilerGenerated]
        private sealed class Class153
        {
            // Fields
            public string String0;

            // Methods
            public bool Method_0(Assembly assembly0)
            {
                return assembly0.GetName().Name.Contains(String0.Replace(".dll", string.Empty));
            }
        }

        #endregion Nested type: Class153

        #region Nested type: FileStructureType

        internal enum FileStructureType
        {
            SingleFile,
            Folder
        }

        #endregion Nested type: FileStructureType
    }
}