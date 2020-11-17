namespace PingPong.Forms
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.incXBtn = new System.Windows.Forms.Button();
            this.decXBtn = new System.Windows.Forms.Button();
            this.incYBtn = new System.Windows.Forms.Button();
            this.decYBtn = new System.Windows.Forms.Button();
            this.incZBtn = new System.Windows.Forms.Button();
            this.decZBtn = new System.Windows.Forms.Button();
            this.incABtn = new System.Windows.Forms.Button();
            this.decABtn = new System.Windows.Forms.Button();
            this.incBBtn = new System.Windows.Forms.Button();
            this.decBBtn = new System.Windows.Forms.Button();
            this.incCBtn = new System.Windows.Forms.Button();
            this.decCBtn = new System.Windows.Forms.Button();
            this.posXText = new System.Windows.Forms.TextBox();
            this.posYText = new System.Windows.Forms.TextBox();
            this.posZText = new System.Windows.Forms.TextBox();
            this.posAText = new System.Windows.Forms.TextBox();
            this.posBText = new System.Windows.Forms.TextBox();
            this.posCText = new System.Windows.Forms.TextBox();
            this.X = new System.Windows.Forms.Label();
            this.Y = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.calibrationBtn = new System.Windows.Forms.Button();
            this.realTimeChart = new PingPong.Forms.ThreadSafeChart();
            this.SuspendLayout();
            // 
            // incXBtn
            // 
            this.incXBtn.Location = new System.Drawing.Point(13, 12);
            this.incXBtn.Name = "incXBtn";
            this.incXBtn.Size = new System.Drawing.Size(45, 23);
            this.incXBtn.TabIndex = 0;
            this.incXBtn.Text = "X+";
            this.incXBtn.UseVisualStyleBackColor = true;
            // 
            // decXBtn
            // 
            this.decXBtn.Location = new System.Drawing.Point(64, 12);
            this.decXBtn.Name = "decXBtn";
            this.decXBtn.Size = new System.Drawing.Size(45, 23);
            this.decXBtn.TabIndex = 1;
            this.decXBtn.Text = "X-";
            this.decXBtn.UseVisualStyleBackColor = true;
            // 
            // incYBtn
            // 
            this.incYBtn.Location = new System.Drawing.Point(13, 41);
            this.incYBtn.Name = "incYBtn";
            this.incYBtn.Size = new System.Drawing.Size(45, 23);
            this.incYBtn.TabIndex = 2;
            this.incYBtn.Text = "Y+";
            this.incYBtn.UseVisualStyleBackColor = true;
            // 
            // decYBtn
            // 
            this.decYBtn.Location = new System.Drawing.Point(64, 41);
            this.decYBtn.Name = "decYBtn";
            this.decYBtn.Size = new System.Drawing.Size(45, 23);
            this.decYBtn.TabIndex = 3;
            this.decYBtn.Text = "Y-";
            this.decYBtn.UseVisualStyleBackColor = true;
            // 
            // incZBtn
            // 
            this.incZBtn.Location = new System.Drawing.Point(13, 70);
            this.incZBtn.Name = "incZBtn";
            this.incZBtn.Size = new System.Drawing.Size(45, 23);
            this.incZBtn.TabIndex = 4;
            this.incZBtn.Text = "Z+";
            this.incZBtn.UseVisualStyleBackColor = true;
            // 
            // decZBtn
            // 
            this.decZBtn.Location = new System.Drawing.Point(64, 70);
            this.decZBtn.Name = "decZBtn";
            this.decZBtn.Size = new System.Drawing.Size(45, 23);
            this.decZBtn.TabIndex = 5;
            this.decZBtn.Text = "Z-";
            this.decZBtn.UseVisualStyleBackColor = true;
            // 
            // incABtn
            // 
            this.incABtn.Location = new System.Drawing.Point(13, 99);
            this.incABtn.Name = "incABtn";
            this.incABtn.Size = new System.Drawing.Size(45, 23);
            this.incABtn.TabIndex = 6;
            this.incABtn.Text = "A+";
            this.incABtn.UseVisualStyleBackColor = true;
            // 
            // decABtn
            // 
            this.decABtn.Location = new System.Drawing.Point(64, 99);
            this.decABtn.Name = "decABtn";
            this.decABtn.Size = new System.Drawing.Size(45, 23);
            this.decABtn.TabIndex = 7;
            this.decABtn.Text = "A-";
            this.decABtn.UseVisualStyleBackColor = true;
            // 
            // incBBtn
            // 
            this.incBBtn.Location = new System.Drawing.Point(13, 128);
            this.incBBtn.Name = "incBBtn";
            this.incBBtn.Size = new System.Drawing.Size(45, 23);
            this.incBBtn.TabIndex = 8;
            this.incBBtn.Text = "B+";
            this.incBBtn.UseVisualStyleBackColor = true;
            // 
            // decBBtn
            // 
            this.decBBtn.Location = new System.Drawing.Point(64, 128);
            this.decBBtn.Name = "decBBtn";
            this.decBBtn.Size = new System.Drawing.Size(45, 23);
            this.decBBtn.TabIndex = 9;
            this.decBBtn.Text = "B-";
            this.decBBtn.UseVisualStyleBackColor = true;
            // 
            // incCBtn
            // 
            this.incCBtn.Location = new System.Drawing.Point(64, 157);
            this.incCBtn.Name = "incCBtn";
            this.incCBtn.Size = new System.Drawing.Size(45, 23);
            this.incCBtn.TabIndex = 10;
            this.incCBtn.Text = "C+";
            this.incCBtn.UseVisualStyleBackColor = true;
            // 
            // decCBtn
            // 
            this.decCBtn.Location = new System.Drawing.Point(13, 157);
            this.decCBtn.Name = "decCBtn";
            this.decCBtn.Size = new System.Drawing.Size(45, 23);
            this.decCBtn.TabIndex = 11;
            this.decCBtn.Text = "C-";
            this.decCBtn.UseVisualStyleBackColor = true;
            // 
            // posXText
            // 
            this.posXText.Location = new System.Drawing.Point(32, 186);
            this.posXText.Name = "posXText";
            this.posXText.ReadOnly = true;
            this.posXText.Size = new System.Drawing.Size(77, 20);
            this.posXText.TabIndex = 14;
            // 
            // posYText
            // 
            this.posYText.Location = new System.Drawing.Point(32, 212);
            this.posYText.Name = "posYText";
            this.posYText.ReadOnly = true;
            this.posYText.Size = new System.Drawing.Size(77, 20);
            this.posYText.TabIndex = 15;
            // 
            // posZText
            // 
            this.posZText.Location = new System.Drawing.Point(32, 238);
            this.posZText.Name = "posZText";
            this.posZText.ReadOnly = true;
            this.posZText.Size = new System.Drawing.Size(77, 20);
            this.posZText.TabIndex = 16;
            // 
            // posAText
            // 
            this.posAText.Location = new System.Drawing.Point(32, 264);
            this.posAText.Name = "posAText";
            this.posAText.ReadOnly = true;
            this.posAText.Size = new System.Drawing.Size(77, 20);
            this.posAText.TabIndex = 17;
            // 
            // posBText
            // 
            this.posBText.Location = new System.Drawing.Point(32, 290);
            this.posBText.Name = "posBText";
            this.posBText.ReadOnly = true;
            this.posBText.Size = new System.Drawing.Size(77, 20);
            this.posBText.TabIndex = 18;
            // 
            // posCText
            // 
            this.posCText.Location = new System.Drawing.Point(32, 316);
            this.posCText.Name = "posCText";
            this.posCText.ReadOnly = true;
            this.posCText.Size = new System.Drawing.Size(77, 20);
            this.posCText.TabIndex = 19;
            // 
            // X
            // 
            this.X.AutoSize = true;
            this.X.Location = new System.Drawing.Point(12, 189);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(14, 13);
            this.X.TabIndex = 20;
            this.X.Text = "X";
            // 
            // Y
            // 
            this.Y.AutoSize = true;
            this.Y.Location = new System.Drawing.Point(12, 215);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(14, 13);
            this.Y.TabIndex = 21;
            this.Y.Text = "Y";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Z";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 267);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "A";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 293);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "B";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 319);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "C";
            // 
            // calibrationBtn
            // 
            this.calibrationBtn.Location = new System.Drawing.Point(13, 342);
            this.calibrationBtn.Name = "calibrationBtn";
            this.calibrationBtn.Size = new System.Drawing.Size(96, 23);
            this.calibrationBtn.TabIndex = 26;
            this.calibrationBtn.Text = "Calibration tool";
            this.calibrationBtn.UseVisualStyleBackColor = true;
            // 
            // realTimeChart
            // 
            this.realTimeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.realTimeChart.BackColor = System.Drawing.SystemColors.Control;
            this.realTimeChart.Enabled = false;
            this.realTimeChart.Location = new System.Drawing.Point(115, 12);
            this.realTimeChart.MaxSamples = 7000;
            this.realTimeChart.Name = "realTimeChart";
            this.realTimeChart.Size = new System.Drawing.Size(957, 354);
            this.realTimeChart.TabIndex = 12;
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1084, 376);
            this.Controls.Add(this.calibrationBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.X);
            this.Controls.Add(this.posCText);
            this.Controls.Add(this.posBText);
            this.Controls.Add(this.posAText);
            this.Controls.Add(this.posZText);
            this.Controls.Add(this.posYText);
            this.Controls.Add(this.posXText);
            this.Controls.Add(this.realTimeChart);
            this.Controls.Add(this.decCBtn);
            this.Controls.Add(this.incCBtn);
            this.Controls.Add(this.decBBtn);
            this.Controls.Add(this.incBBtn);
            this.Controls.Add(this.decABtn);
            this.Controls.Add(this.incABtn);
            this.Controls.Add(this.decZBtn);
            this.Controls.Add(this.incZBtn);
            this.Controls.Add(this.decYBtn);
            this.Controls.Add(this.incYBtn);
            this.Controls.Add(this.decXBtn);
            this.Controls.Add(this.incXBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.RightToLeftLayout = true;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ping Pong";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button incXBtn;
        private System.Windows.Forms.Button decXBtn;
        private System.Windows.Forms.Button incYBtn;
        private System.Windows.Forms.Button decYBtn;
        private System.Windows.Forms.Button incZBtn;
        private System.Windows.Forms.Button decZBtn;
        private System.Windows.Forms.Button incABtn;
        private System.Windows.Forms.Button decABtn;
        private System.Windows.Forms.Button incBBtn;
        private System.Windows.Forms.Button decBBtn;
        private System.Windows.Forms.Button incCBtn;
        private System.Windows.Forms.Button decCBtn;
        private ThreadSafeChart realTimeChart;
        private System.Windows.Forms.TextBox posXText;
        private System.Windows.Forms.TextBox posYText;
        private System.Windows.Forms.TextBox posZText;
        private System.Windows.Forms.TextBox posAText;
        private System.Windows.Forms.TextBox posBText;
        private System.Windows.Forms.TextBox posCText;
        private System.Windows.Forms.Label X;
        private System.Windows.Forms.Label Y;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button calibrationBtn;
    }
}