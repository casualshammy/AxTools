using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using AxTools.Classes;
using Microsoft.CSharp;

namespace AxTools.WoW.PluginSystem
{
    internal class CodeCompiler
    {
        // Methods
        public CodeCompiler(string path)
        {
            DeleteOldAssemblies();
            CompilerVersion = 4f;
            SourceFilePaths = new List<string>();
            if (File.Exists(path))
            {
                FileStructure = FileStructureType.SingleFile;
            }
            else if (Directory.Exists(path))
            {
                FileStructure = FileStructureType.Folder;
            }
            SourcePath = path;
            Options = new CompilerParameters {GenerateExecutable = false, GenerateInMemory = false, IncludeDebugInformation = true};
            string str = "BW_" + Assembly.GetEntryAssembly().GetName().Version.Revision;
            Options.CompilerOptions = string.Format("/d:BW;{0} /unsafe", str);
            Options.TempFiles = new TempFileCollection(Path.GetTempPath());
            Options.OutputAssembly = Path.Combine(Globals.PluginsAssembliesPath, AssemblyName);
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
            }
        }

        // Properties
        public string AssemblyName
        {
            get
            {
                return string.Format("{0}_{1}.dll",
                    (FileStructure == FileStructureType.SingleFile)
                        ? Path.GetFileNameWithoutExtension(SourcePath)
                        : new DirectoryInfo(SourcePath).Name, Utils.GetRandomString(16));
            }
        }

        public Assembly CompiledAssembly { get; private set; }

        public string CompiledToLocation { get; private set; }

        public float CompilerVersion { get; private set; }

        public FileStructureType FileStructure { get; private set; }

        public CompilerParameters Options { get; private set; }

        public List<string> SourceFilePaths { get; private set; }

        public string SourcePath { get; private set; }

        public void AddReference(string assembly)
        {
            if (!Options.ReferencedAssemblies.Contains(assembly))
            {
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

            Dictionary<string, string> providerOptions = new Dictionary<string, string>
            {
                {
                    "CompilerVersion", string.Format(CultureInfo.InvariantCulture.NumberFormat, "v{0:N1}", new object[] {CompilerVersion})
                }
            };
            using (var provider = new CSharpCodeProvider(providerOptions))
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

        public static void DeleteOldAssemblies()
        {
            if (!Directory.Exists(Globals.PluginsAssembliesPath))
            {
                Directory.CreateDirectory(Globals.PluginsAssembliesPath);
            }
            else
            {
                foreach (string str in Directory.GetFiles(Globals.PluginsAssembliesPath))
                {
                    try
                    {
                        File.Delete(str);
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
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
            using (var reader = new ResXResourceReader(string2))
            {
                reader.BasePath = SourcePath;
                using (var writer = new ResourceWriter(path))
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

        private bool method_2(string string2)
        {
            var class2 = new Class153
            {
                String0 = string2
            };
            return AppDomain.CurrentDomain.GetAssemblies().Any(class2.method_0);
        }

        public void ParseFilesForCompilerOptions()
        {
            foreach (string str in SourceFilePaths)
            {
                foreach (string str2 in File.ReadAllLines(str))
                {
                    string str3 = str2.Trim();
                    if (str3.StartsWith("//!CompilerOption:"))
                    {
                        string[] strArray2 = str3.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string str4 = strArray2[1];
                        if (str4 != null)
                        {
                            if (str4 != "AddRef")
                            {
                                if ((((str4 == "Optimise") && (strArray2.Length == 3)) &&
                                     (!string.IsNullOrEmpty(strArray2[2]) && (strArray2[2] == "On"))) &&
                                    !Options.CompilerOptions.Contains("/optimise"))
                                {
                                    Options.IncludeDebugInformation = false;
                                    CompilerParameters options = Options;
                                    options.CompilerOptions = options.CompilerOptions + " /optimise";
                                }
                            }
                            else if (((strArray2.Length == 3) && !string.IsNullOrEmpty(strArray2[2])) &&
                                     (strArray2[2].EndsWith(".dll") && !method_2(strArray2[2])))
                            {
                                AddReference(strArray2[2]);
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
            public bool method_0(Assembly assembly0)
            {
                return assembly0.GetName().Name.Contains(String0.Replace(".dll", string.Empty));
            }
        }

        #endregion

        #region Nested type: FileStructureType

        internal enum FileStructureType
        {
            SingleFile,
            Folder
        }

        #endregion
    }
}
