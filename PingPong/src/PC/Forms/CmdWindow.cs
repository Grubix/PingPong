using PingPong.Commands;
using PingPong.KUKA;
using PingPong.OptiTrack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PingPong.Forms {
    public partial class CmdWindow : Form {

        private readonly Dictionary<string, ICommand> registeredCommands = new Dictionary<string, ICommand>();

        private readonly List<string> commandsHistory = new List<string>();

        private int historyIndex = -1;

        private readonly KUKARobot robot1 = null;

        private readonly KUKARobot robot2 = null;

        private readonly OptiTrackSystem optiTrack = null;

        public CmdWindow() {
            InitializeComponent();

            RegisterCommand(new HelpCommand());
            RegisterCommand(new ExitCommand());
            RegisterCommand(new ClearCommand());
            RegisterCommand(new MoveToCommand());

            cmdInput.KeyDown += (s, e) => {
                if (e.KeyData == Keys.Enter) {
                    e.SuppressKeyPress = true;

                    if (commandsHistory.Count == 0 || cmdInput.Text != commandsHistory[0]) {
                        commandsHistory.Insert(0, cmdInput.Text);
                    }

                    string input = cmdInput.Text.ToLower().Trim();

                    LogCommand(input);
                    ExecuteCommand(input);
                    cmdInput.Text = "";
                    historyIndex = -1;
                } else if (e.KeyData == Keys.Up) {
                    e.Handled = true;

                    if (historyIndex < commandsHistory.Count - 1) {
                        cmdInput.Text = commandsHistory[++historyIndex];
                        cmdInput.SelectionStart = cmdInput.Text.Length;
                        cmdInput.SelectionLength = 0;
                    }
                } else if (e.KeyData == Keys.Down) {
                    e.Handled = true;

                    if (historyIndex > 0) {
                        cmdInput.Text = commandsHistory[--historyIndex];
                        cmdInput.SelectionStart = cmdInput.Text.Length;
                        cmdInput.SelectionLength = 0;
                    }
                }
            };
        }

        private void LogCommand(string command) {
            Font boldFont = new Font(cmdHistory.Font, FontStyle.Bold);

            cmdHistory.SelectionStart = cmdHistory.TextLength;
            cmdHistory.SelectionLength = 0;
            cmdHistory.SelectionFont = boldFont;
            cmdHistory.AppendText(">> " + command + Environment.NewLine);
            cmdHistory.SelectionFont = cmdHistory.Font;
            cmdHistory.ScrollToCaret();
        }

        public void RegisterCommand(ICommand command) {
            registeredCommands[command.Name] = command;
        }

        public void ExecuteCommand(string input) {
            if (string.IsNullOrWhiteSpace(input)) {
                cmdInput.Text = "";
                return;
            }

            Regex commandRegex = new Regex(@"(?<=\()[^()]*(?=\))|[\w.]+");
            MatchCollection matches = commandRegex.Matches(input);

            if (matches.Count == 0) {
                Error($"Unknown command '{input}'");
                return;
            }

            string commandName = matches[0].Value;

            if (!registeredCommands.ContainsKey(commandName)) {
                Error($"Unknown command '{commandName}'");
                return;
            }

            ICommand command = registeredCommands[commandName];
            string[] userArgs = new string[matches.Count - 1];

            for (int i = 0; i < matches.Count - 1; i++) {
                userArgs[i] = matches[i + 1].Value;
            }

            CommandArgs commandArgs = new CommandArgs(registeredCommands, this, robot1, robot2, optiTrack, userArgs);
            command.Execute(commandArgs);
        }

        public void Log(string message) {
            cmdHistory.AppendText(">> " + message + Environment.NewLine);
            cmdHistory.ScrollToCaret();
        }

        public void Error(string errorMessage) {
            cmdHistory.SelectionStart = cmdHistory.TextLength;
            cmdHistory.SelectionLength = 0;
            cmdHistory.SelectionColor = Color.Red;
            cmdHistory.AppendText(">> Error: " + errorMessage + Environment.NewLine);
            cmdHistory.SelectionColor = cmdHistory.ForeColor;
            cmdHistory.ScrollToCaret();
        }

        public void Clear() {
            cmdHistory.Text = "";
        }

    }
}