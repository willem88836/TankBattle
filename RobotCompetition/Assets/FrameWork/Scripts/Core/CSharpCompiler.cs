using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Framework.Core
{
	public class CSharpCompiler
	{
		private CSharpCodeProvider provider;
		private CompilerParameters parameters;

		private Assembly assembly;


		public CSharpCompiler(string code, string fileName)
		{
			provider = new CSharpCodeProvider();
			parameters = new CompilerParameters()
			{
				GenerateInMemory = true,
				GenerateExecutable = false,
				TreatWarningsAsErrors = false,
				//CompilerOptions = "/target:library",
				//OutputAssembly = "Temp/" + Path.GetFileNameWithoutExtension(fileName) + ".dll"
			};
			parameters.ReferencedAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.Location).ToArray());

			// TODO: Fix this using: http://www.arcturuscollective.com/archives/22
			// https://github.com/aeroson/mcs-ICodeCompiler
			//https://answers.unity.com/questions/364580/scripting-works-in-editor-try-it-but-not-build.html

			CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

			if (results.Errors.HasErrors)
			{
				StringBuilder sb = new StringBuilder();

				foreach (CompilerError error in results.Errors)
				{
					sb.AppendLine(string.Format(
						"{0} ({1}) at Ln{2}: {3}", 
						(error.IsWarning ? "Warning" : "Error"), 
						error.ErrorNumber, 
						error.Line, 
						error.ErrorText));
				}
				throw new InvalidOperationException(sb.ToString());
			}

			assembly = results.CompiledAssembly;
		}


		public Type GetCompiledType(string typeName)
		{
			return assembly.GetType(typeName);
		}
	}
}
