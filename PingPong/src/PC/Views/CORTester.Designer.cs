namespace PingPong.Views {
    partial class CORTester {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.toSampleInput = new System.Windows.Forms.NumericUpDown();
            this.averageCORText = new System.Windows.Forms.TextBox();
            this.calculateBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.fromSampleInput = new System.Windows.Forms.NumericUpDown();
            this.startBtn = new System.Windows.Forms.Button();
            this.clearBtn = new System.Windows.Forms.Button();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toSampleInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromSampleInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.toSampleInput);
            this.panel1.Controls.Add(this.averageCORText);
            this.panel1.Controls.Add(this.calculateBtn);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.fromSampleInput);
            this.panel1.Controls.Add(this.startBtn);
            this.panel1.Controls.Add(this.clearBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 561);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "to samp.";
            // 
            // toSampleInput
            // 
            this.toSampleInput.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.toSampleInput.Location = new System.Drawing.Point(79, 54);
            this.toSampleInput.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.toSampleInput.Name = "toSampleInput";
            this.toSampleInput.Size = new System.Drawing.Size(58, 20);
            this.toSampleInput.TabIndex = 11;
            // 
            // averageCORText
            // 
            this.averageCORText.Location = new System.Drawing.Point(13, 109);
            this.averageCORText.Name = "averageCORText";
            this.averageCORText.ReadOnly = true;
            this.averageCORText.Size = new System.Drawing.Size(124, 20);
            this.averageCORText.TabIndex = 10;
            this.averageCORText.Text = "0";
            // 
            // calculateBtn
            // 
            this.calculateBtn.Location = new System.Drawing.Point(12, 80);
            this.calculateBtn.Name = "calculateBtn";
            this.calculateBtn.Size = new System.Drawing.Size(126, 23);
            this.calculateBtn.TabIndex = 8;
            this.calculateBtn.Text = "calulate COR";
            this.calculateBtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "from samp.";
            // 
            // fromSampleInput
            // 
            this.fromSampleInput.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.fromSampleInput.Location = new System.Drawing.Point(13, 54);
            this.fromSampleInput.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.fromSampleInput.Name = "fromSampleInput";
            this.fromSampleInput.Size = new System.Drawing.Size(58, 20);
            this.fromSampleInput.TabIndex = 2;
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(12, 12);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(60, 23);
            this.startBtn.TabIndex = 1;
            this.startBtn.Text = "start";
            this.startBtn.UseVisualStyleBackColor = true;
            // 
            // clearBtn
            // 
            this.clearBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.clearBtn.Location = new System.Drawing.Point(78, 12);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(60, 23);
            this.clearBtn.TabIndex = 0;
            this.clearBtn.Text = "clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            // 
            // chart
            // 
            chartArea1.AxisX.Interval = 5D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.Title = "sample";
            chartArea1.AxisY.Title = "[mm]";
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend1.Name = "Legend1";
            this.chart.Legends.Add(legend1);
            this.chart.Location = new System.Drawing.Point(150, 0);
            this.chart.Name = "chart";
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "ball height [mm]";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(1034, 561);
            this.chart.TabIndex = 1;
            this.chart.Text = "chart";
            // 
            // CORTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "CORTester";
            this.Text = "CORTester";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toSampleInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fromSampleInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown fromSampleInput;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Button calculateBtn;
        private System.Windows.Forms.TextBox averageCORText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown toSampleInput;
    }
}