﻿namespace ScheduleMe.Tab;

partial class Calendar
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
        label1 = new Label();
        monthCalendar1 = new MonthCalendar();
        button1 = new Button();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Location = new Point(0, 0);
        label1.Name = "label1";
        label1.Size = new Size(100, 23);
        label1.TabIndex = 0;
        // 
        // monthCalendar1
        // 
        monthCalendar1.Location = new Point(340, 95);
        monthCalendar1.Name = "monthCalendar1";
        monthCalendar1.TabIndex = 1;
        // 
        // button1
        // 
        button1.Location = new Point(611, 109);
        button1.Name = "button1";
        button1.Size = new Size(62, 47);
        button1.TabIndex = 2;
        button1.Text = "button1";
        button1.UseVisualStyleBackColor = true;
        // 
        // Calendar
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.White;
        ClientSize = new Size(850, 550);
        Controls.Add(button1);
        Controls.Add(monthCalendar1);
        Controls.Add(label1);
        Font = new Font("Dubai", 9.749999F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.None;
        Margin = new Padding(4);
        Name = "Calendar";
        Text = "Calendar";
        WindowState = FormWindowState.Maximized;
        ResumeLayout(false);
    }

    #endregion

    private Label label1;
    private MonthCalendar monthCalendar1;
    private Button button1;
}