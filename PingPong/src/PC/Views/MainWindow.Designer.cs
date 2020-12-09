namespace PingPong.Views {
    partial class MainWindow {
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            this.calibrationBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.robot1Panel = new PingPong.Views.RobotDataPanel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // incXBtn
            // 
            this.incXBtn.Location = new System.Drawing.Point(6, 6);
            this.incXBtn.Name = "incXBtn";
            this.incXBtn.Size = new System.Drawing.Size(61, 23);
            this.incXBtn.TabIndex = 0;
            this.incXBtn.Text = "X+";
            this.incXBtn.UseVisualStyleBackColor = true;
            // 
            // decXBtn
            // 
            this.decXBtn.Location = new System.Drawing.Point(73, 6);
            this.decXBtn.Name = "decXBtn";
            this.decXBtn.Size = new System.Drawing.Size(61, 23);
            this.decXBtn.TabIndex = 1;
            this.decXBtn.Text = "X-";
            this.decXBtn.UseVisualStyleBackColor = true;
            // 
            // incYBtn
            // 
            this.incYBtn.Location = new System.Drawing.Point(6, 35);
            this.incYBtn.Name = "incYBtn";
            this.incYBtn.Size = new System.Drawing.Size(61, 23);
            this.incYBtn.TabIndex = 2;
            this.incYBtn.Text = "Y+";
            this.incYBtn.UseVisualStyleBackColor = true;
            // 
            // decYBtn
            // 
            this.decYBtn.Location = new System.Drawing.Point(73, 35);
            this.decYBtn.Name = "decYBtn";
            this.decYBtn.Size = new System.Drawing.Size(61, 23);
            this.decYBtn.TabIndex = 3;
            this.decYBtn.Text = "Y-";
            this.decYBtn.UseVisualStyleBackColor = true;
            // 
            // incZBtn
            // 
            this.incZBtn.Location = new System.Drawing.Point(6, 64);
            this.incZBtn.Name = "incZBtn";
            this.incZBtn.Size = new System.Drawing.Size(61, 23);
            this.incZBtn.TabIndex = 4;
            this.incZBtn.Text = "Z+";
            this.incZBtn.UseVisualStyleBackColor = true;
            // 
            // decZBtn
            // 
            this.decZBtn.Location = new System.Drawing.Point(73, 64);
            this.decZBtn.Name = "decZBtn";
            this.decZBtn.Size = new System.Drawing.Size(61, 23);
            this.decZBtn.TabIndex = 5;
            this.decZBtn.Text = "Z-";
            this.decZBtn.UseVisualStyleBackColor = true;
            // 
            // incABtn
            // 
            this.incABtn.Location = new System.Drawing.Point(6, 93);
            this.incABtn.Name = "incABtn";
            this.incABtn.Size = new System.Drawing.Size(61, 23);
            this.incABtn.TabIndex = 6;
            this.incABtn.Text = "A+";
            this.incABtn.UseVisualStyleBackColor = true;
            // 
            // decABtn
            // 
            this.decABtn.Location = new System.Drawing.Point(73, 93);
            this.decABtn.Name = "decABtn";
            this.decABtn.Size = new System.Drawing.Size(61, 23);
            this.decABtn.TabIndex = 7;
            this.decABtn.Text = "A-";
            this.decABtn.UseVisualStyleBackColor = true;
            // 
            // incBBtn
            // 
            this.incBBtn.Location = new System.Drawing.Point(6, 122);
            this.incBBtn.Name = "incBBtn";
            this.incBBtn.Size = new System.Drawing.Size(61, 23);
            this.incBBtn.TabIndex = 8;
            this.incBBtn.Text = "B+";
            this.incBBtn.UseVisualStyleBackColor = true;
            // 
            // decBBtn
            // 
            this.decBBtn.Location = new System.Drawing.Point(73, 122);
            this.decBBtn.Name = "decBBtn";
            this.decBBtn.Size = new System.Drawing.Size(61, 23);
            this.decBBtn.TabIndex = 9;
            this.decBBtn.Text = "B-";
            this.decBBtn.UseVisualStyleBackColor = true;
            // 
            // incCBtn
            // 
            this.incCBtn.Location = new System.Drawing.Point(73, 151);
            this.incCBtn.Name = "incCBtn";
            this.incCBtn.Size = new System.Drawing.Size(61, 23);
            this.incCBtn.TabIndex = 10;
            this.incCBtn.Text = "C+";
            this.incCBtn.UseVisualStyleBackColor = true;
            // 
            // decCBtn
            // 
            this.decCBtn.Location = new System.Drawing.Point(6, 151);
            this.decCBtn.Name = "decCBtn";
            this.decCBtn.Size = new System.Drawing.Size(61, 23);
            this.decCBtn.TabIndex = 11;
            this.decCBtn.Text = "C-";
            this.decCBtn.UseVisualStyleBackColor = true;
            // 
            // calibrationBtn
            // 
            this.calibrationBtn.Location = new System.Drawing.Point(6, 180);
            this.calibrationBtn.Name = "calibrationBtn";
            this.calibrationBtn.Size = new System.Drawing.Size(128, 23);
            this.calibrationBtn.TabIndex = 26;
            this.calibrationBtn.Text = "Calibration tool";
            this.calibrationBtn.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chart1);
            this.panel1.Controls.Add(this.incYBtn);
            this.panel1.Controls.Add(this.incXBtn);
            this.panel1.Controls.Add(this.calibrationBtn);
            this.panel1.Controls.Add(this.decXBtn);
            this.panel1.Controls.Add(this.decCBtn);
            this.panel1.Controls.Add(this.decYBtn);
            this.panel1.Controls.Add(this.incCBtn);
            this.panel1.Controls.Add(this.incZBtn);
            this.panel1.Controls.Add(this.decBBtn);
            this.panel1.Controls.Add(this.decZBtn);
            this.panel1.Controls.Add(this.incBBtn);
            this.panel1.Controls.Add(this.incABtn);
            this.panel1.Controls.Add(this.decABtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(760, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 511);
            this.panel1.TabIndex = 28;
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 319);
            this.chart1.Name = "chart1";
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "X";
            series2.BorderWidth = 3;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "V";
            series3.BorderWidth = 3;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "A";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(374, 192);
            this.chart1.TabIndex = 30;
            this.chart1.Text = "chart1";
            // 
            // robot1Panel
            // 
            this.robot1Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robot1Panel.Location = new System.Drawing.Point(0, 0);
            this.robot1Panel.Name = "robot1Panel";
            this.robot1Panel.Size = new System.Drawing.Size(760, 511);
            this.robot1Panel.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1134, 511);
            this.Controls.Add(this.robot1Panel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.RightToLeftLayout = true;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ping Pong";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Button calibrationBtn;
        private System.Windows.Forms.Panel panel1;
        private RobotDataPanel robot1Panel;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}