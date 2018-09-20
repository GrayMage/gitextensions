using System.IO;
using GitCommands;

namespace CommitTemplateInferrer.Classes
{
    public class CommitMessageHelper
    {
        private readonly GitModule _gitModule;
        private readonly string _scriptExecutorPath;
        private readonly string _scriptFilePath;

        public CommitMessageHelper(GitModule gitModule, string scriptExecutorPath, string scriptFilePath)
        {
            _gitModule = gitModule;
            _scriptFilePath = scriptFilePath;
            _scriptExecutorPath = scriptExecutorPath;
        }

        public string PreviousMessage { get; private set; }
        public string Message { get; private set; }

        public void UpdateMessage(bool force = false)
        {
            string EnsurePathQuotation(string path)
            {
                var trimmed = path.Trim('"');
                return trimmed.Contains(" ") ? $@"""{trimmed}""" : trimmed;
            }

            var gitWorkingDir = _gitModule.WorkingDirGitDir.TrimEnd('/', '\\');
            var commitMsgPath = EnsurePathQuotation($"{gitWorkingDir}\\COMMITMESSAGE");

            if (force)
                File.Delete(commitMsgPath);

            new ShellScript(_scriptExecutorPath,
                    string.IsNullOrWhiteSpace(_scriptFilePath)
                        ? EnsurePathQuotation($"{gitWorkingDir}\\hooks\\prepare-commit-msg")
                        : EnsurePathQuotation(_scriptFilePath), gitWorkingDir)
                .Execute(commitMsgPath);

            var newMessage = CommitHelper.GetCommitMessage(_gitModule);
            if (PreviousMessage != newMessage) PreviousMessage = Message;

            Message = newMessage;
        }
    }
}