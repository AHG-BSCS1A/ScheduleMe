﻿namespace ScheduleMe.Tab;

partial class Message
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
        messageLbl = new Label();
        okBtn = new Button();
        btnPanel = new Panel();
        btnPanel.SuspendLayout();
        SuspendLayout();
        // 
        // messageLbl
        // 
        messageLbl.Dock = DockStyle.Top;
        messageLbl.Font = new Font("Dubai", 9.749999F, FontStyle.Bold, GraphicsUnit.Point);
        messageLbl.ForeColor = Color.White;
        messageLbl.Location = new Point(0, 5);
        messageLbl.Name = "messageLbl";
        messageLbl.Size = new Size(145, 22);
        messageLbl.TabIndex = 1;
        messageLbl.Text = "Message";
        messageLbl.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // okBtn
        // 
        okBtn.BackColor = Color.White;
        okBtn.Dock = DockStyle.Fill;
        okBtn.FlatAppearance.BorderSize = 0;
        okBtn.FlatStyle = FlatStyle.Flat;
        okBtn.Font = new Font("Dubai", 9.749999F, FontStyle.Bold, GraphicsUnit.Point);
        okBtn.Location = new Point(40, 0);
        okBtn.Margin = new Padding(0);
        okBtn.Name = "okBtn";
        okBtn.Size = new Size(65, 30);
        okBtn.TabIndex = 0;
        okBtn.TabStop = false;
        okBtn.Text = "Ok";
        okBtn.UseVisualStyleBackColor = false;
        okBtn.Click += okBtn_Click;
        // 
        // btnPanel
        // 
        btnPanel.BackColor = Color.Transparent;
        btnPanel.Controls.Add(okBtn);
        btnPanel.Dock = DockStyle.Bottom;
        btnPanel.Location = new Point(0, 41);
        btnPanel.Name = "btnPanel";
        btnPanel.Padding = new Padding(40, 0, 40, 0);
        btnPanel.Size = new Size(145, 30);
        btnPanel.TabIndex = 0;
        // 
        // Message
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        BackColor = SystemColors.ControlDarkDark;
        ClientSize = new Size(145, 79);
        Controls.Add(btnPanel);
        Controls.Add(messageLbl);
        FormBorderStyle = FormBorderStyle.None;
        Name = "Message";
        Padding = new Padding(0, 5, 0, 8);
        StartPosition = FormStartPosition.CenterParent;
        btnPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Label messageLbl;
    private Button okBtn;
    private Panel btnPanel;
}