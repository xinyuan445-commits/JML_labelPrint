namespace Launcher;

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
        this.label1 = new System.Windows.Forms.Label();
        this.progressBar1 = new System.Windows.Forms.ProgressBar();
        this.labelPercent = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(30, 30);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(120, 17);
        this.label1.TabIndex = 0;
        this.label1.Text = "正在检查更新...";
        // 
        // progressBar1
        // 
        this.progressBar1.Location = new System.Drawing.Point(30, 60);
        this.progressBar1.Name = "progressBar1";
        this.progressBar1.Size = new System.Drawing.Size(300, 23);
        this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Blocks;
        this.progressBar1.TabIndex = 1;
        // 
        // labelPercent
        // 
        this.labelPercent.AutoSize = true;
        this.labelPercent.Location = new System.Drawing.Point(340, 63);
        this.labelPercent.Name = "labelPercent";
        this.labelPercent.Size = new System.Drawing.Size(26, 17);
        this.labelPercent.TabIndex = 2;
        this.labelPercent.Text = "0%";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(400, 120);
        this.ControlBox = false;
        this.Controls.Add(this.labelPercent);
        this.Controls.Add(this.progressBar1);
        this.Controls.Add(this.label1);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "启动器";
        this.Load += new System.EventHandler(this.Form1_Load);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Label labelPercent;
}
