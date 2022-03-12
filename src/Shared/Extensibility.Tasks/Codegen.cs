using System.IO;
using System.Reflection.Emit;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Raid.Toolkit.Extensibility.Tasks
{
    public class Codegen : Task
    {
        [Required]
        public string OutputFile { get; set; }
        public override bool Execute()
        {

            Log.LogMessage($"Writing to {OutputFile}...");
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(new("Test.Assembly"), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder mb = ab.DefineDynamicModule("Test.Assembly");
            Lokad.ILPack.AssemblyGenerator assemblyGenerator = new();
            string dirName = Path.GetDirectoryName(OutputFile);
            if (!string.IsNullOrEmpty(dirName))
                Directory.CreateDirectory(dirName);
            assemblyGenerator.GenerateAssembly(ab, OutputFile);
            return true;
        }
    }
}
