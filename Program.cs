using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        string editionArg = null;
        string serverArg = "kms8.msguides.com"; // default value
        string editionKey = null;
        string slmgrPath = @"C:\Windows\System32\slmgr.vbs"; // full path to slmgr.vbs

        // Parse command line arguments
        foreach (string arg in args)
        {
            if (arg.StartsWith("--edition="))
            {
                editionArg = arg.Substring("--edition=".Length);
            }
            else if (arg.StartsWith("--server="))
            {
                serverArg = arg.Substring("--server=".Length);
            }
        }

        // Set edition key based on edition argument
        switch (editionArg)
        {
            case "pro":
                editionKey = "W269N-WFGWX-YVC9B-4J6C9-T83GX";
                break;
            case "enterprise":
                editionKey = "NPPR9-FWDCX-D2C8J-H872K-2YT43";
                break;
        }

        // Check if edition argument was passed and edition key was set
        if (editionArg == null || editionKey == null)
        {
            Console.WriteLine("Usage: --edition=pro|enterprise [--server=<server_address>]");
            return;
        }

        // Run slmgr command with edition key and server address
        string slmgrArgs = $"/c cscript \"{slmgrPath}\" /ipk {editionKey}";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = slmgrArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Verb = "runas"
            }
        };
        process.Start();

        // Wait for the process to finish and print output to console
        process.WaitForExit();
        string output = process.StandardOutput.ReadToEnd();
        Console.WriteLine(RemoveScriptHostMessage(output));

        // Run /skms command with server address
        slmgrArgs = $"/c cscript \"{slmgrPath}\" /skms {serverArg}";
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = slmgrArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Verb = "runas"
            }
        };
        process.Start();

        // Wait for the process to finish and print output to console
        process.WaitForExit();
        output = process.StandardOutput.ReadToEnd();
        Console.WriteLine(RemoveScriptHostMessage(output));

        // Run /ato command
        slmgrArgs = $"/c cscript \"{slmgrPath}\" /ato";
        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = slmgrArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Verb = "runas"
            }
        };
        process.Start();

        // Wait for the process to finish and print output to console
        process.WaitForExit();
        output = process.StandardOutput.ReadToEnd();
        Console.WriteLine(RemoveScriptHostMessage(output));
    }

    static string RemoveScriptHostMessage(string output)
    {
        Regex regex = new Regex(@"Microsoft \(R\) Windows Script Host Version.*\n", RegexOptions.IgnoreCase);
        output = regex.Replace(output, string.Empty);
        regex = new Regex(@"Copyright \(C\) Microsoft Corporation\. All rights reserved\.");
        output = regex.Replace(output, string.Empty);
        return output;
    }

}