namespace PingPong.Forms {
    partial class CmdWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdInput = new System.Windows.Forms.TextBox();
            this.cmdHistory = new System.Windows.Forms.RichTextBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gainsboro;
            this.panel2.Controls.Add(this.cmdInput);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 239);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.panel2.Size = new System.Drawing.Size(784, 22);
            this.panel2.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.cmdHistory);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.panel1.Size = new System.Drawing.Size(784, 239);
            this.panel1.TabIndex = 2;
            // 
            // cmdInput
            // 
            this.cmdInput.BackColor = System.Drawing.Color.Gainsboro;
            this.cmdInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmdInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdInput.Location = new System.Drawing.Point(6, 3);
            this.cmdInput.Margin = new System.Windows.Forms.Padding(0);
            this.cmdInput.Name = "cmdInput";
            this.cmdInput.Size = new System.Drawing.Size(775, 16);
            this.cmdInput.TabIndex = 0;
            // 
            // cmdHistory
            // 
            this.cmdHistory.BackColor = System.Drawing.SystemColors.Control;
            this.cmdHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cmdHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdHistory.Location = new System.Drawing.Point(6, 0);
            this.cmdHistory.Name = "cmdHistory";
            this.cmdHistory.ReadOnly = true;
            this.cmdHistory.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.cmdHistory.Size = new System.Drawing.Size(778, 239);
            this.cmdHistory.TabIndex = 0;
            this.cmdHistory.Text = "";
            // 
            // CmdWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 261);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "CmdWindow";
            this.Text = "CmdWindow";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox cmdInput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox cmdHistory;
    }
}