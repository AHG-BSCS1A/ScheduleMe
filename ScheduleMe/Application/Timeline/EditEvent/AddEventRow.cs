﻿using LiteDB;

namespace ScheduleMe.Tab;

public partial class AddEventRow : UserControl
{
    public AddEventRow()
    {
        InitializeComponent();
    }

    internal Event eventInfo;
    public ObjectId Id { get; set; }
    public ushort Index { get; set; }

    public string Title
    {
        get { return titleTBox.Text; }
        set { titleTBox.Text = value; }
    }

    public string Description
    {
        get { return descriptionTBox.Text; }
        set { descriptionTBox.Text = value; }
    }

    public DateTime StartDate
    {
        get { return startDatePicker.Value; }
        set { startDatePicker.Value = value; }
    }

    public DateTime EndDate
    {
        get { return endDatePicker.Value; }
        set { endDatePicker.Value = value; }
    }

    public Color Color
    {
        get { return colorPickerBtn.BackColor; }
        set { colorPickerBtn.BackColor = value; }
    }

    private void colorPickerBtn_Click(object sender, EventArgs e)
    {
        colorDialog.ShowDialog();
        colorPickerBtn.BackColor = colorDialog.Color;
    }

    internal Event GetRowInfo()
    {
        eventInfo.EventTitle = titleTBox.Text;
        eventInfo.EventDescription = descriptionTBox.Text;
        eventInfo.EventStartDate = startDatePicker.Value;
        eventInfo.EventEndDate = endDatePicker.Value;
        eventInfo.EventColor = colorPickerBtn.BackColor;

        return eventInfo;
    }

    internal void SetRowInfo(Event eventInfo)
    {
        this.eventInfo = eventInfo;

        titleTBox.Text = eventInfo.EventTitle;
        descriptionTBox.Text = eventInfo.EventDescription;
        startDatePicker.Value = eventInfo.EventStartDate;
        endDatePicker.Value = eventInfo.EventEndDate;
        colorPickerBtn.BackColor = eventInfo.EventColor;
    }

    private void rowMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        if (e.ClickedItem == deleteOption)
        {
            using (var timelineDB = new LiteDatabase(DBConnection.timelineConnection))
            {
                var timelines = timelineDB.GetCollection<Timeline>("Timeline");
                var timeline = timelines.FindById(Id);
                timeline.Events.RemoveAt(Index);
                timelines.Update(timeline);
            }
            this.Dispose();
        }
    }
}