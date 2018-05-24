namespace CommitTemplateInferrer.Classes
{
    using System.Windows.Forms;
    using GitUI;
    using GitUI.CommandsDialogs;
    using GitUI.SpellChecker;

    public class FormCommitWrapper : Wrapper<FormCommit>
    {
        public FormCommitWrapper(GitUICommands gitUiCommands, string scriptFilePath) : base(new FormCommit(gitUiCommands))
        {
            var commitMessageHelper = new CommitMessageHelper(gitUiCommands.Module, scriptFilePath);

            MessageField = GetField<EditNetSpell>("Message");
            CommitButton = GetField<Button>("Commit");
            FlowCommitPanel = GetField<FlowLayoutPanel>("flowCommitButtons");

            commitMessageHelper.UpdateMessage();
            if (MessageField.Text == commitMessageHelper.PreviousMessage)
            {
                MessageField.Text = commitMessageHelper.Message;
            }

            MessageField.KeyUp += (sender, args) =>
            {
                if (args.Control && args.KeyCode == Keys.Enter)
                {
                    commitMessageHelper.UpdateMessage();
                    MessageField.Text = commitMessageHelper.Message;
                    args.Handled = true;
                }
            };

            CommitButton.Click += (sender, args) =>
            {
                commitMessageHelper.UpdateMessage();
                MessageField.Text = commitMessageHelper.Message;
            };

            gitUiCommands.PostCheckoutBranch += (sender, args) =>
            {
                commitMessageHelper.UpdateMessage();
                MessageField.Text = commitMessageHelper.Message;
            };

            var resetButton = new Button
            {
                Text = @"Reset message",
                AutoSize = true
            };

            resetButton.Click += (sender, args) =>
            {
                commitMessageHelper.UpdateMessage();
                MessageField.Text = commitMessageHelper.Message;
            };

            FlowCommitPanel.Controls.Add(resetButton);
        }

        private EditNetSpell MessageField { get; }
        private Button CommitButton { get; }
        private FlowLayoutPanel FlowCommitPanel { get; }

        public DialogResult ShowDialog()
        {
            return Object.ShowDialog();
        }
    }
}