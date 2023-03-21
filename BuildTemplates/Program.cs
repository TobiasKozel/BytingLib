﻿namespace BuildTemplates
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string nameSpace = args.Length == 0 ? "NAMESPACE" : args[0];
            string[] referencedDlls = new string[0];
            bool loadOnStartup = false;
            List<(string, string)> customProcessorToDataType = new();
            List<(string, string)> customDataTypeToNameExtension = new();
            List<(string, string)> customExtensionToDataType = new();

            foreach (var arg in args)
            {
                int colonIndex = arg.IndexOf(':');
                if (colonIndex == -1)
                    continue;
                string command = arg.Remove(colonIndex);
                string value = arg.Substring(colonIndex + 1);
                switch (command)
                {
                    case "dlls":
                        referencedDlls = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "loadOnStartup":
                        loadOnStartup = value == "true" || value == "1";
                        break;
                    case "processorToDataType":
                    case "dateTypeToName":
                    case "extensionToDataType":
                        var dict = command == "dateTypeToName" ? customDataTypeToNameExtension 
                            : command == "processorToDataType" ? customProcessorToDataType
                            : customExtensionToDataType;
                        string[] split = value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var s in split)
                        {
                            string[] s2 = s.Split(new char[] { ',' });
                            if (s2.Length < 2)
                                continue;
                            dict.Add((s2[0], s2[1]));
                        }
                        break;
                }
            }

            var projectPath = Environment.CurrentDirectory;
            string contentPath = Path.Combine(projectPath, "Content");
            (string code, string mgcbOutput, string locaCode, ShaderFile[] shaders) = ContentTemplate.Create(contentPath + "/", nameSpace, referencedDlls, loadOnStartup, customProcessorToDataType, customDataTypeToNameExtension, customExtensionToDataType);

            string mgcbFile = Path.Combine(contentPath, "Content.Generated.mgcb");
            File.WriteAllText(mgcbFile, mgcbOutput);


            code = $@"// THIS IS A GENERATED FILE, DO NOT EDIT!
// generate it by saving the file '../_ContentGenerate.tt'. It should be located next to the *.csproj file.

using BytingLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace {nameSpace}
{{
    {code}
}}
";
            string contentFile = Path.Combine(contentPath, "ContentLoader.Generated.cs");
            File.WriteAllText(contentFile, code);

            string locaCodeFile = Path.Combine(contentPath, "Loca.Generated.cs");
            File.WriteAllText(locaCodeFile, locaCode);


            foreach (var shader in shaders)
            {
                shader.WriteToFile();
            }
        }
    }
}
