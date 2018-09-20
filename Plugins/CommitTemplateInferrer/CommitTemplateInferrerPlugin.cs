using System.Collections.Generic;
using System.Windows.Forms;
using CommitTemplateInferrer.Classes;
using GitUI;
using GitUIPluginInterfaces;
using ResourceManager;

namespace CommitTemplateInferrer
{
    public class CommitTemplateInferrerPlugin : GitPluginBase, IGitPluginForRepository
    {
        private readonly StringSetting _scriptExecutorPathSetting =
            new StringSetting("Script executor", "Script executor", string.Empty);
        private readonly StringSetting _scriptFilePathSetting =
            new StringSetting("Script to execute", "Script to execute", string.Empty);

        public CommitTemplateInferrerPlugin()
        {
            SetNameAndDescription("Commit template inferrer");
        }

        public override IEnumerable<ISetting> GetSettings()
        {
            yield return _scriptExecutorPathSetting;
            yield return _scriptFilePathSetting;
        }

        public override void Register(IGitUICommands gitUiCommands)
        {
            gitUiCommands.PreCommit += (sender, args) =>
            {
                args.Cancel = true;
                using (var formCommit = new FormCommitWrapper(
                    new GitUICommands(args.GitUICommands.GitModule.WorkingDir),
                    _scriptExecutorPathSetting.ValueOrDefault(Settings),
                    _scriptFilePathSetting.ValueOrDefault(Settings)))
                {
                    formCommit.ShowDialog();
                }

                new Wrapper<IGitUICommands>(gitUiCommands).InvokeEvent(
                    "PostCommit",
                    sender,
                    new GitUIPostActionEventArgs(args.OwnerForm, gitUiCommands, true));
                gitUiCommands.RepoChangedNotifier.Notify();
            };
        }

        public override bool Execute(GitUIBaseEventArgs gitUiCommands)
        {
            MessageBox.Show(@"This plugin is like a passive ability - no need to execute");
            return true;
        }
    }
}