using GitCommands;

namespace CommitTemplateInferrer.Classes
{
    public class CommitMessageHelper
    {
        private readonly GitModule _gitModule;
        private readonly string _scriptFilePath;

        public CommitMessageHelper(GitModule gitModule, string scriptFilePath)
        {
            _gitModule = gitModule;
            _scriptFilePath = scriptFilePath;
        }

        public string PreviousMessage { get; private set; }
        public string Message { get; private set; }

        public void UpdateMessage()
        {
            string EnsurePathQuotation(string path)
            {
                var trimmed = path.Trim('"');
                return trimmed.Contains(" ") ? $@"""{trimmed}""" : trimmed;
            }

            var gitWorkingDir = _gitModule.WorkingDirGitDir.TrimEnd('/', '\\');

            new ShellScript(string.IsNullOrWhiteSpace(_scriptFilePath)
                ? EnsurePathQuotation($"{gitWorkingDir}\\hooks\\prepare-commit-msg")
                : EnsurePathQuotation(_scriptFilePath), gitWorkingDir).Execute(EnsurePathQuotation($"{gitWorkingDir}\\COMMITMESSAGE"));

            var newMessage = CommitHelper.GetCommitMessage(_gitModule);
            if (PreviousMessage != newMessage)
            {
                PreviousMessage = Message;
            }

            Message = newMessage;
        }
    }
}