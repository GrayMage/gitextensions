using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CommitTemplateInferrer.Classes
{
    public class ShellScript
    {
        private readonly string _scriptExecutorPath;
        private readonly string _scriptFilePath;
        private readonly string _workingDir;

        public ShellScript(string scriptExecutorPath, string scriptFilePath, string workingDir)
        {
            _scriptExecutorPath = scriptExecutorPath;
            _scriptFilePath = scriptFilePath;
            _workingDir = workingDir;
        }

        public void Execute(params string[] arguments)
        {
            if (File.Exists(_scriptFilePath.Trim('"')))
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = File.Exists(_scriptExecutorPath) ? _scriptExecutorPath : throw new InvalidOperationException($"'{_scriptExecutorPath}' does not exist"),
                        WorkingDirectory = Directory.Exists(_workingDir) ? _workingDir : Directory.GetCurrentDirectory(),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = string.Join(" ", new[] { _scriptFilePath }.Union(arguments))
                    }
                };

                p.Start();
                p.WaitForExit();
            }
        }
    }
}
