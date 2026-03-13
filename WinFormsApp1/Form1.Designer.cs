namespace WinFormsApp1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            groupBox1 = new GroupBox();
            button4 = new Button();
            textBox2 = new TextBox();
            label6 = new Label();
            button3 = new Button();
            button2 = new Button();
            label5 = new Label();
            textBox1 = new TextBox();
            dateTimePicker2 = new DateTimePicker();
            label4 = new Label();
            label3 = new Label();
            dateTimePicker1 = new DateTimePicker();
            comboBox2 = new ComboBox();
            label2 = new Label();
            label1 = new Label();
            comboBox1 = new ComboBox();
            button1 = new Button();
            label7 = new Label();
            dataGridView1 = new DataGridView();
            dataGridView2 = new DataGridView();
            label8 = new Label();
            checkBox1 = new CheckBox();
            groupBox2 = new GroupBox();
            label14 = new Label();
            textBox8 = new TextBox();
            textBox7 = new TextBox();
            textBox6 = new TextBox();
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            label13 = new Label();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            button5 = new Button();
            label9 = new Label();
            textBox3 = new TextBox();
            checkBox2 = new CheckBox();
            groupBox3 = new GroupBox();
            label15 = new Label();
            comboBox3 = new ComboBox();
            label16 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            button6 = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(dateTimePicker2);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(comboBox2);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(button1);
            groupBox1.Location = new Point(12, 386);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(372, 254);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "单据查询";
            // 
            // button4
            // 
            button4.Location = new Point(244, 215);
            button4.Name = "button4";
            button4.Size = new Size(121, 29);
            button4.TabIndex = 15;
            button4.Text = "流水号设置";
            button4.UseVisualStyleBackColor = true;
            button4.Visible = false;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(68, 218);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(168, 23);
            textBox2.TabIndex = 14;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(7, 221);
            label6.Name = "label6";
            label6.Size = new Size(56, 17);
            label6.TabIndex = 13;
            label6.Text = "流水号：";
            label6.Click += label6_Click;
            // 
            // button3
            // 
            button3.Location = new Point(245, 174);
            button3.Name = "button3";
            button3.Size = new Size(121, 29);
            button3.TabIndex = 12;
            button3.Text = "查询";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(245, 97);
            button2.Name = "button2";
            button2.Size = new Size(121, 29);
            button2.TabIndex = 11;
            button2.Text = "模板编辑";
            button2.UseVisualStyleBackColor = true;
            button2.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 180);
            label5.Name = "label5";
            label5.Size = new Size(44, 17);
            label5.TabIndex = 10;
            label5.Text = "单号：";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(68, 177);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(168, 23);
            textBox1.TabIndex = 9;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(244, 138);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(121, 23);
            dateTimePicker2.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(210, 143);
            label4.Name = "label4";
            label4.Size = new Size(13, 17);
            label4.TabIndex = 7;
            label4.Text = "-";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 144);
            label3.Name = "label3";
            label3.Size = new Size(44, 17);
            label3.TabIndex = 6;
            label3.Text = "日期：";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(68, 138);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(121, 23);
            dateTimePicker1.TabIndex = 5;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(68, 100);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(168, 25);
            comboBox2.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 103);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 3;
            label2.Text = "模板：";
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 63);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 2;
            label1.Text = "打印机：";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(68, 60);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(298, 25);
            comboBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(252, 22);
            button1.Name = "button1";
            button1.Size = new Size(114, 29);
            button1.TabIndex = 0;
            button1.Text = "条码规则";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 9);
            label7.Name = "label7";
            label7.Size = new Size(56, 17);
            label7.TabIndex = 1;
            label7.Text = "单据列表";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 29);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(372, 351);
            dataGridView1.TabIndex = 2;
            // 
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(390, 29);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(667, 351);
            dataGridView2.TabIndex = 3;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(390, 9);
            label8.Name = "label8";
            label8.Size = new Size(56, 17);
            label8.TabIndex = 4;
            label8.Text = "单据明细";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(452, 8);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(51, 21);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "全部";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(textBox8);
            groupBox2.Controls.Add(textBox7);
            groupBox2.Controls.Add(textBox6);
            groupBox2.Controls.Add(textBox5);
            groupBox2.Controls.Add(textBox4);
            groupBox2.Controls.Add(label13);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label10);
            groupBox2.Location = new Point(390, 460);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(667, 129);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "资料明细";
            groupBox2.Visible = false;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(453, 67);
            label14.Name = "label14";
            label14.Size = new Size(68, 17);
            label14.TabIndex = 9;
            label14.Text = "包装规格：";
            // 
            // textBox8
            // 
            textBox8.Location = new Point(521, 64);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(140, 23);
            textBox8.TabIndex = 8;
            // 
            // textBox7
            // 
            textBox7.Location = new Point(283, 64);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(158, 23);
            textBox7.TabIndex = 7;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(62, 64);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(158, 23);
            textBox6.TabIndex = 6;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(283, 25);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(158, 23);
            textBox5.TabIndex = 5;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(62, 25);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(158, 23);
            textBox4.TabIndex = 4;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(24, 67);
            label13.Name = "label13";
            label13.Size = new Size(44, 17);
            label13.TabIndex = 3;
            label13.Text = "数量：";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(246, 67);
            label12.Name = "label12";
            label12.Size = new Size(44, 17);
            label12.TabIndex = 2;
            label12.Text = "批号：";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(246, 28);
            label11.Name = "label11";
            label11.Size = new Size(44, 17);
            label11.TabIndex = 1;
            label11.Text = "货位：";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(24, 28);
            label10.Name = "label10";
            label10.Size = new Size(44, 17);
            label10.TabIndex = 0;
            label10.Text = "仓库：";
            // 
            // button5
            // 
            button5.Location = new Point(642, 601);
            button5.Name = "button5";
            button5.Size = new Size(169, 38);
            button5.TabIndex = 7;
            button5.Text = "打印";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(390, 592);
            label9.Name = "label9";
            label9.Size = new Size(68, 17);
            label9.TabIndex = 8;
            label9.Text = "打印数量：";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(464, 589);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(100, 23);
            textBox3.TabIndex = 9;
            textBox3.Text = "1";
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(390, 618);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(75, 21);
            checkBox2.TabIndex = 10;
            checkBox2.Text = "打印尾码";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(button6);
            groupBox3.Controls.Add(label15);
            groupBox3.Controls.Add(comboBox3);
            groupBox3.Location = new Point(390, 386);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(667, 68);
            groupBox3.TabIndex = 11;
            groupBox3.TabStop = false;
            groupBox3.Text = "单据";
            groupBox3.Enter += groupBox3_Enter;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(12, 29);
            label15.Name = "label15";
            label15.Size = new Size(68, 17);
            label15.TabIndex = 2;
            label15.Text = "单据选择：";
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(86, 26);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(174, 25);
            comboBox3.TabIndex = 1;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(911, 8);
            label16.Name = "label16";
            label16.Size = new Size(50, 17);
            label16.TabIndex = 12;
            label16.Text = "label16";
            // 
            // button6
            // 
            button6.Location = new Point(453, 22);
            button6.Name = "button6";
            button6.Size = new Size(97, 31);
            button6.TabIndex = 3;
            button6.Text = "导入Excel";
            button6.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1069, 652);
            Controls.Add(label16);
            Controls.Add(groupBox3);
            Controls.Add(checkBox2);
            Controls.Add(textBox3);
            Controls.Add(label9);
            Controls.Add(button5);
            Controls.Add(groupBox2);
            Controls.Add(checkBox1);
            Controls.Add(label8);
            Controls.Add(dataGridView2);
            Controls.Add(dataGridView1);
            Controls.Add(label7);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "标签打印管理系统";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private Button button1;
        private Label label2;
        private Label label1;
        private ComboBox comboBox1;
        private Label label3;
        private DateTimePicker dateTimePicker1;
        private ComboBox comboBox2;
        private Label label6;
        private Button button3;
        private Button button2;
        private Label label5;
        private TextBox textBox1;
        private DateTimePicker dateTimePicker2;
        private Label label4;
        private Button button4;
        private TextBox textBox2;
        private Label label7;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Label label8;
        private CheckBox checkBox1;
        private GroupBox groupBox2;
        private Button button5;
        private Label label9;
        private TextBox textBox3;
        private CheckBox checkBox2;
        private TextBox textBox7;
        private TextBox textBox6;
        private TextBox textBox5;
        private TextBox textBox4;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label14;
        private TextBox textBox8;
        private GroupBox groupBox3;
        private ComboBox comboBox3;
        private Label label15;
        private Label label16;
        private System.Windows.Forms.Timer timer1;
        private Button button6;
    }
}
