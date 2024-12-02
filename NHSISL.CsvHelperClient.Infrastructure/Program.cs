// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------


// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using NHSISL.CsvHelperClient.Infrastructure.Services;

namespace NHSISL.CsvHelperClient.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();
            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "NHSISL.CsvHelperClient",
                dotNetVersion: "9.0.100");
        }
    }
}