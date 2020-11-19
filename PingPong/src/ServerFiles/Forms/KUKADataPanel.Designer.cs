﻿namespace PingPong.Forms {
    partial class KUKADataPanel {
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.positionChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.velocityChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.velC = new System.Windows.Forms.TextBox();
            this.velB = new System.Windows.Forms.TextBox();
            this.velA = new System.Windows.Forms.TextBox();
            this.velZ = new System.Windows.Forms.TextBox();
            this.velY = new System.Windows.Forms.TextBox();
            this.velX = new System.Windows.Forms.TextBox();
            this.velCCheck = new System.Windows.Forms.CheckBox();
            this.velBCheck = new System.Windows.Forms.CheckBox();
            this.velACheck = new System.Windows.Forms.CheckBox();
            this.velZCheck = new System.Windows.Forms.CheckBox();
            this.velYCheck = new System.Windows.Forms.CheckBox();
            this.velXCheck = new System.Windows.Forms.CheckBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.positionChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.velocityChart)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(900, 500);
            this.panel2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.splitContainer1.Location = new System.Drawing.Point(160, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.positionChart);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.velocityChart);
            this.splitContainer1.Panel2MinSize = 200;
            this.splitContainer1.Size = new System.Drawing.Size(740, 500);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 1;
            // 
            // positionChart
            // 
            chartArea3.Name = "ChartArea1";
            this.positionChart.ChartAreas.Add(chartArea3);
            this.positionChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Alignment = System.Drawing.StringAlignment.Center;
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend3.Name = "Legend1";
            this.positionChart.Legends.Add(legend3);
            this.positionChart.Location = new System.Drawing.Point(0, 0);
            this.positionChart.Name = "positionChart";
            this.positionChart.Size = new System.Drawing.Size(740, 250);
            this.positionChart.TabIndex = 0;
            this.positionChart.Text = "chart1";
            // 
            // velocityChart
            // 
            chartArea4.Name = "ChartArea1";
            this.velocityChart.ChartAreas.Add(chartArea4);
            this.velocityChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend4.Alignment = System.Drawing.StringAlignment.Center;
            legend4.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend4.Name = "Legend1";
            this.velocityChart.Legends.Add(legend4);
            this.velocityChart.Location = new System.Drawing.Point(0, 0);
            this.velocityChart.Name = "velocityChart";
            this.velocityChart.Size = new System.Drawing.Size(740, 246);
            this.velocityChart.TabIndex = 0;
            this.velocityChart.Text = "chart2";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(160, 500);
            this.panel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.velC);
            this.groupBox2.Controls.Add(this.velB);
            this.groupBox2.Controls.Add(this.velA);
            this.groupBox2.Controls.Add(this.velZ);
            this.groupBox2.Controls.Add(this.velY);
            this.groupBox2.Controls.Add(this.velX);
            this.groupBox2.Controls.Add(this.velCCheck);
            this.groupBox2.Controls.Add(this.velBCheck);
            this.groupBox2.Controls.Add(this.velACheck);
            this.groupBox2.Controls.Add(this.velZCheck);
            this.groupBox2.Controls.Add(this.velYCheck);
            this.groupBox2.Controls.Add(this.velXCheck);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(148, 180);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Velocity";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 155);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "C";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(7, 129);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "B";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(7, 103);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "A";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(7, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Z";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(7, 51);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Y";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(7, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "X";
            // 
            // velC
            // 
            this.velC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velC.Location = new System.Drawing.Point(26, 152);
            this.velC.Name = "velC";
            this.velC.ReadOnly = true;
            this.velC.Size = new System.Drawing.Size(55, 20);
            this.velC.TabIndex = 11;
            // 
            // velB
            // 
            this.velB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velB.Location = new System.Drawing.Point(26, 126);
            this.velB.Name = "velB";
            this.velB.ReadOnly = true;
            this.velB.Size = new System.Drawing.Size(55, 20);
            this.velB.TabIndex = 10;
            // 
            // velA
            // 
            this.velA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velA.Location = new System.Drawing.Point(26, 100);
            this.velA.Name = "velA";
            this.velA.ReadOnly = true;
            this.velA.Size = new System.Drawing.Size(55, 20);
            this.velA.TabIndex = 9;
            // 
            // velZ
            // 
            this.velZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velZ.Location = new System.Drawing.Point(26, 74);
            this.velZ.Name = "velZ";
            this.velZ.ReadOnly = true;
            this.velZ.Size = new System.Drawing.Size(55, 20);
            this.velZ.TabIndex = 8;
            // 
            // velY
            // 
            this.velY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velY.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velY.Location = new System.Drawing.Point(26, 48);
            this.velY.Name = "velY";
            this.velY.ReadOnly = true;
            this.velY.Size = new System.Drawing.Size(55, 20);
            this.velY.TabIndex = 7;
            // 
            // velX
            // 
            this.velX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velX.Location = new System.Drawing.Point(26, 22);
            this.velX.Name = "velX";
            this.velX.ReadOnly = true;
            this.velX.Size = new System.Drawing.Size(55, 20);
            this.velX.TabIndex = 6;
            // 
            // velCCheck
            // 
            this.velCCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velCCheck.AutoSize = true;
            this.velCCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velCCheck.Location = new System.Drawing.Point(87, 154);
            this.velCCheck.Name = "velCCheck";
            this.velCCheck.Size = new System.Drawing.Size(55, 17);
            this.velCCheck.TabIndex = 5;
            this.velCCheck.Text = "visible";
            this.velCCheck.UseVisualStyleBackColor = true;
            // 
            // velBCheck
            // 
            this.velBCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velBCheck.AutoSize = true;
            this.velBCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velBCheck.Location = new System.Drawing.Point(87, 128);
            this.velBCheck.Name = "velBCheck";
            this.velBCheck.Size = new System.Drawing.Size(55, 17);
            this.velBCheck.TabIndex = 4;
            this.velBCheck.Text = "visible";
            this.velBCheck.UseVisualStyleBackColor = true;
            // 
            // velACheck
            // 
            this.velACheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velACheck.AutoSize = true;
            this.velACheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velACheck.Location = new System.Drawing.Point(87, 102);
            this.velACheck.Name = "velACheck";
            this.velACheck.Size = new System.Drawing.Size(55, 17);
            this.velACheck.TabIndex = 3;
            this.velACheck.Text = "visible";
            this.velACheck.UseVisualStyleBackColor = true;
            // 
            // velZCheck
            // 
            this.velZCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velZCheck.AutoSize = true;
            this.velZCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velZCheck.Location = new System.Drawing.Point(87, 76);
            this.velZCheck.Name = "velZCheck";
            this.velZCheck.Size = new System.Drawing.Size(55, 17);
            this.velZCheck.TabIndex = 2;
            this.velZCheck.Text = "visible";
            this.velZCheck.UseVisualStyleBackColor = true;
            // 
            // velYCheck
            // 
            this.velYCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velYCheck.AutoSize = true;
            this.velYCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velYCheck.Location = new System.Drawing.Point(87, 50);
            this.velYCheck.Name = "velYCheck";
            this.velYCheck.Size = new System.Drawing.Size(55, 17);
            this.velYCheck.TabIndex = 1;
            this.velYCheck.Text = "visible";
            this.velYCheck.UseVisualStyleBackColor = true;
            // 
            // velXCheck
            // 
            this.velXCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.velXCheck.AutoSize = true;
            this.velXCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.velXCheck.Location = new System.Drawing.Point(87, 24);
            this.velXCheck.Name = "velXCheck";
            this.velXCheck.Size = new System.Drawing.Size(55, 17);
            this.velXCheck.TabIndex = 0;
            this.velXCheck.Text = "visible";
            this.velXCheck.UseVisualStyleBackColor = true;
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
            // KUKADataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Name = "KUKADataPanel";
            this.Size = new System.Drawing.Size(900, 500);
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.positionChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.velocityChart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox velC;
        private System.Windows.Forms.TextBox velB;
        private System.Windows.Forms.TextBox velA;
        private System.Windows.Forms.TextBox velZ;
        private System.Windows.Forms.TextBox velY;
        private System.Windows.Forms.TextBox velX;
        private System.Windows.Forms.CheckBox velCCheck;
        private System.Windows.Forms.CheckBox velBCheck;
        private System.Windows.Forms.CheckBox velACheck;
        private System.Windows.Forms.CheckBox velZCheck;
        private System.Windows.Forms.CheckBox velYCheck;
        private System.Windows.Forms.CheckBox velXCheck;
        private System.Windows.Forms.DataVisualization.Charting.Chart positionChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart velocityChart;
    }
}
