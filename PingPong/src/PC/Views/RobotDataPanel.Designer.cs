namespace PingPong.Views {
    partial class RobotDataPanel {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.panel2 = new System.Windows.Forms.Panel();
            this.positionChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.posC = new System.Windows.Forms.TextBox();
            this.posB = new System.Windows.Forms.TextBox();
            this.posA = new System.Windows.Forms.TextBox();
            this.posZ = new System.Windows.Forms.TextBox();
            this.posY = new System.Windows.Forms.TextBox();
            this.posX = new System.Windows.Forms.TextBox();
            this.posCCheck = new System.Windows.Forms.CheckBox();
            this.posBCheck = new System.Windows.Forms.CheckBox();
            this.posACheck = new System.Windows.Forms.CheckBox();
            this.posZCheck = new System.Windows.Forms.CheckBox();
            this.posYCheck = new System.Windows.Forms.CheckBox();
            this.posXCheck = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positionChart)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.positionChart);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(900, 500);
            this.panel2.TabIndex = 1;
            // 
            // positionChart
            // 
            chartArea2.Name = "ChartArea1";
            this.positionChart.ChartAreas.Add(chartArea2);
            this.positionChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend2.Alignment = System.Drawing.StringAlignment.Center;
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.Name = "Legend1";
            this.positionChart.Legends.Add(legend2);
            this.positionChart.Location = new System.Drawing.Point(160, 0);
            this.positionChart.Name = "positionChart";
            this.positionChart.Size = new System.Drawing.Size(740, 500);
            this.positionChart.TabIndex = 0;
            this.positionChart.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(160, 500);
            this.panel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.posC);
            this.groupBox1.Controls.Add(this.posB);
            this.groupBox1.Controls.Add(this.posA);
            this.groupBox1.Controls.Add(this.posZ);
            this.groupBox1.Controls.Add(this.posY);
            this.groupBox1.Controls.Add(this.posX);
            this.groupBox1.Controls.Add(this.posCCheck);
            this.groupBox1.Controls.Add(this.posBCheck);
            this.groupBox1.Controls.Add(this.posACheck);
            this.groupBox1.Controls.Add(this.posZCheck);
            this.groupBox1.Controls.Add(this.posYCheck);
            this.groupBox1.Controls.Add(this.posXCheck);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(148, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 155);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "C";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "B";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "A";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "X";
            // 
            // posC
            // 
            this.posC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posC.Location = new System.Drawing.Point(26, 152);
            this.posC.Name = "posC";
            this.posC.ReadOnly = true;
            this.posC.Size = new System.Drawing.Size(55, 20);
            this.posC.TabIndex = 11;
            // 
            // posB
            // 
            this.posB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posB.Location = new System.Drawing.Point(26, 126);
            this.posB.Name = "posB";
            this.posB.ReadOnly = true;
            this.posB.Size = new System.Drawing.Size(55, 20);
            this.posB.TabIndex = 10;
            // 
            // posA
            // 
            this.posA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posA.Location = new System.Drawing.Point(26, 100);
            this.posA.Name = "posA";
            this.posA.ReadOnly = true;
            this.posA.Size = new System.Drawing.Size(55, 20);
            this.posA.TabIndex = 9;
            // 
            // posZ
            // 
            this.posZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posZ.Location = new System.Drawing.Point(26, 74);
            this.posZ.Name = "posZ";
            this.posZ.ReadOnly = true;
            this.posZ.Size = new System.Drawing.Size(55, 20);
            this.posZ.TabIndex = 8;
            // 
            // posY
            // 
            this.posY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posY.Location = new System.Drawing.Point(26, 48);
            this.posY.Name = "posY";
            this.posY.ReadOnly = true;
            this.posY.Size = new System.Drawing.Size(55, 20);
            this.posY.TabIndex = 7;
            // 
            // posX
            // 
            this.posX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posX.Location = new System.Drawing.Point(26, 22);
            this.posX.Name = "posX";
            this.posX.ReadOnly = true;
            this.posX.Size = new System.Drawing.Size(55, 20);
            this.posX.TabIndex = 6;
            // 
            // posCCheck
            // 
            this.posCCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posCCheck.AutoSize = true;
            this.posCCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posCCheck.Location = new System.Drawing.Point(87, 154);
            this.posCCheck.Name = "posCCheck";
            this.posCCheck.Size = new System.Drawing.Size(55, 17);
            this.posCCheck.TabIndex = 5;
            this.posCCheck.Text = "visible";
            this.posCCheck.UseVisualStyleBackColor = true;
            // 
            // posBCheck
            // 
            this.posBCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posBCheck.AutoSize = true;
            this.posBCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posBCheck.Location = new System.Drawing.Point(87, 128);
            this.posBCheck.Name = "posBCheck";
            this.posBCheck.Size = new System.Drawing.Size(55, 17);
            this.posBCheck.TabIndex = 4;
            this.posBCheck.Text = "visible";
            this.posBCheck.UseVisualStyleBackColor = true;
            // 
            // posACheck
            // 
            this.posACheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posACheck.AutoSize = true;
            this.posACheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posACheck.Location = new System.Drawing.Point(87, 102);
            this.posACheck.Name = "posACheck";
            this.posACheck.Size = new System.Drawing.Size(55, 17);
            this.posACheck.TabIndex = 3;
            this.posACheck.Text = "visible";
            this.posACheck.UseVisualStyleBackColor = true;
            // 
            // posZCheck
            // 
            this.posZCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posZCheck.AutoSize = true;
            this.posZCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posZCheck.Location = new System.Drawing.Point(87, 76);
            this.posZCheck.Name = "posZCheck";
            this.posZCheck.Size = new System.Drawing.Size(55, 17);
            this.posZCheck.TabIndex = 2;
            this.posZCheck.Text = "visible";
            this.posZCheck.UseVisualStyleBackColor = true;
            // 
            // posYCheck
            // 
            this.posYCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posYCheck.AutoSize = true;
            this.posYCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posYCheck.Location = new System.Drawing.Point(87, 50);
            this.posYCheck.Name = "posYCheck";
            this.posYCheck.Size = new System.Drawing.Size(55, 17);
            this.posYCheck.TabIndex = 1;
            this.posYCheck.Text = "visible";
            this.posYCheck.UseVisualStyleBackColor = true;
            // 
            // posXCheck
            // 
            this.posXCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.posXCheck.AutoSize = true;
            this.posXCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posXCheck.Location = new System.Drawing.Point(87, 24);
            this.posXCheck.Name = "posXCheck";
            this.posXCheck.Size = new System.Drawing.Size(55, 17);
            this.posXCheck.TabIndex = 0;
            this.posXCheck.Text = "visible";
            this.posXCheck.UseVisualStyleBackColor = true;
            // 
            // RobotDataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Name = "RobotDataPanel";
            this.Size = new System.Drawing.Size(900, 500);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.positionChart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox posC;
        private System.Windows.Forms.TextBox posB;
        private System.Windows.Forms.TextBox posA;
        private System.Windows.Forms.TextBox posZ;
        private System.Windows.Forms.TextBox posY;
        private System.Windows.Forms.TextBox posX;
        private System.Windows.Forms.CheckBox posCCheck;
        private System.Windows.Forms.CheckBox posBCheck;
        private System.Windows.Forms.CheckBox posACheck;
        private System.Windows.Forms.CheckBox posZCheck;
        private System.Windows.Forms.CheckBox posYCheck;
        private System.Windows.Forms.CheckBox posXCheck;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart positionChart;
    }
}
