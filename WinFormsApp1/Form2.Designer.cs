namespace WinFormsApp1
{
    partial class Form2
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
            dgvRules = new DataGridView();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvRules).BeginInit();
            SuspendLayout();
            // 
            // dgvRules
            // 
            dgvRules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRules.Location = new Point(12, 12);
            dgvRules.Name = "dgvRules";
            dgvRules.Size = new Size(209, 373);
            dgvRules.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(35, 404);
            button1.Name = "button1";
            button1.Size = new Size(153, 34);
            button1.TabIndex = 1;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(233, 450);
            Controls.Add(button1);
            Controls.Add(dgvRules);
            Name = "Form2";
            Text = "Form2";
            Load += Form2_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRules).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvRules;
        private Button button1;
    }
}