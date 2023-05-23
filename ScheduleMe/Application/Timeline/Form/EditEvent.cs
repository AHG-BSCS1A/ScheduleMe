﻿using LiteDB;

namespace ScheduleMe.Tab;

public partial class EditEvent : Form
{
    private string timelineConnection = @"C:\Users\Jhondale\Downloads\Timelines.db";
    public ObjectId currentID;

    public EditEvent()
    {
        InitializeComponent();
    }

    private void EditEvent_Load(object sender, EventArgs e)
    {
        using (var timelineDB = new LiteDatabase(timelineConnection))
        {
            var timelines = timelineDB.GetCollection<TimelineTab>("Timeline");
            var timelineTabs = timelines.FindAll();
            if (timelineTabs.Count() != 0)
            {
                // Load the first TimelineTab.Event List only
                TimelineTab firstToLoad = timelineTabs.First();

                if (firstToLoad.Events != null)
                {
                    foreach (Event firstEvents in firstToLoad.Events)
                    {
                        AddEventRow newRow = new AddEventRow();
                        newRow.SetRowInfo(firstEvents);
                        newRow.Dock = DockStyle.Top;
                        eventInfoPanel.Controls.Add(newRow);
                    }
                    currentID = firstToLoad.Id;
                }

                // Load all the Timeline Tabs
                foreach (var tab in timelineTabs)
                {
                    addNewTab(tab.TimelineName, tab.Id);
                }
            }
        }
    }

    private void addNewTab(string timelineName, ObjectId Id)
    {
        EditEventTabBase newTimelineTab = new EditEventTabBase();
        newTimelineTab.tabName = timelineName;
        newTimelineTab.Id = Id;
        newTimelineTab.editEventInstance = this;
        newTimelineTab.Location = new Point(timelineAddTab.Left, timelineAddTab.Top);
        timelineTabPanel.Controls.Add(newTimelineTab);
        newTimelineTab.BringToFront();

        // Adjust the add button next to new tab
        timelineAddTab.Location = new Point(newTimelineTab.Right, newTimelineTab.Top);
        newTimelineTab.Dock = DockStyle.Left;
        currentID = newTimelineTab.Id;
    }

    private void timelineAddTab_Click(object sender, EventArgs e)
    {
        AddTimelineTab addTimelineTab = new AddTimelineTab();
        addTimelineTab.ShowDialog();

        // Load new added timeline
        using (var timelineDB = new LiteDatabase(timelineConnection))
        {
            var timelines = timelineDB.GetCollection<TimelineTab>("Timeline");
            TimelineTab newtTab = new TimelineTab();
            newtTab = timelines.FindById(addTimelineTab.Id);

            // Add new tab and clear since there is no events yet as expected
            addNewTab(newtTab.TimelineName, newtTab.Id);
            eventInfoPanel.Controls.Clear();
        }
    }

    private void addRowBtn_Click(object sender, EventArgs e)
    {
        AddEventRow newRow = new AddEventRow();
        newRow.Dock = DockStyle.Top; // Need some fix to docked at the bottom instead
        eventInfoPanel.Controls.Add(newRow);
    }

    private void saveBtn_Click(object sender, EventArgs e)
    {
        if (currentID != null)
        {
            using (var timelineDB = new LiteDatabase(timelineConnection))
            {
                var timelines = timelineDB.GetCollection<TimelineTab>("Timeline");
                TimelineTab timeline = timelines.FindById(currentID);
                
                // This will clear the current Events in class and replace with new list of Events
                // Kind of ineficient but I don't know how to fix this right now
                timeline.Events.Clear();
                foreach (AddEventRow newEvent in eventInfoPanel.Controls)
                {
                    timeline.Events.Add(newEvent.GetRowInfo());
                }
                timelines.Update(timeline);
            }
        }
        
    }
}