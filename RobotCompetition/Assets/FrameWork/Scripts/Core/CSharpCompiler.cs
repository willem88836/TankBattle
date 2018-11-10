using System;
using System.CodeDom.Compiler;
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


		public CSharpCompiler(string code)
		{
			provider = new CSharpCodeProvider();
			parameters = new CompilerParameters()
			{
				GenerateInMemory = true,
				GenerateExecutable = true,
				TreatWarningsAsErrors = false,
			};
			parameters.ReferencedAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.Location).ToArray());


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
