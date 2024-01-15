// See https://aka.ms/new-console-template for more information
using CommandLine;
using Raid.Toolkit.Build.Cli;

return Parser.Default.ParseArguments<CodegenTaskArgs, PackageTaskArgs>(args)
	.MapResult(
		(CodegenTaskArgs opts) => CodegenTask.Execute(opts),
		(PackageTaskArgs opts) => PackageTask.Execute(opts),
		HandleParseError
	);

static int HandleParseError(IEnumerable<Error> errs)
{
	var result = -2;
	Console.WriteLine("errors {0}", errs.Count());
	if (errs.Any(x => x is HelpRequestedError || x is VersionRequestedError))
		result = -1;
	Console.WriteLine("Exit code {0}", result);
	return result;
}
