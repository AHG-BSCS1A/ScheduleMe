﻿using LiteDB;

namespace ScheduleMe.Tab;

public partial class TimelinePanel : Form
{
    public ObjectId CurrentID { get; set; }
    public ObjectId PreviousID { get; set; }
    public List<ObjectId> EventIds { get; set; } = new List<ObjectId>();

    private readonly byte columnSize = (byte)(42 * MainDesigner.BaseWidthFactor());
    public int CurrentDateTimePosition { get; set; }

    public TimelinePanel()
    {
        InitializeComponent();
    }

    private void Timeline_Load(object sender, EventArgs e)
    {
        if (CurrentID == null)
            LoadFirstTimeline();

        else if (CurrentID != null)
            LoadTimelineById(CurrentID);
    }

    private void LoadFirstTimeline()
    {
        using var timelineConnection = new LiteDatabase(DBConnection.databaseConnection_timeline);
        var timelineDB = timelineConnection.GetCollection<Timeline>("Timeline");
        var timelines = timelineDB.FindAll();

        if (timelines.Any())
        {
            Timeline firstToLoad = timelines.First();
            CurrentID = firstToLoad.Id;

            foreach (var tab in timelines) // Load all the Timeline Tabs
            {
                EventIds.Add(tab.Id);
                AddNewTab(tab.TimelineName, tab.Id);
            }
            PopulateTimeline(firstToLoad);
        }
    }

    private void LoadTimelineById(ObjectId id)
    {
        using var timelineConnection = new LiteDatabase(DBConnection.databaseConnection_timeline);
        var timelineDB = timelineConnection.GetCollection<Timeline>("Timeline");
        var timeline = timelineDB.FindById(id);
        EventIds.Add(id);
        AddNewTab(timeline.TimelineName, timeline.Id);
        PopulateTimeline(timeline);
    }

    internal void PopulateTimeline(Timeline timeline)
    {
        if (timeline.Events.Any())
        {
            // Need to improve the sorting or the overlapping method. Too difficult
            timeline.Events.Sort((e1, e2) => e1.EventEndDate.CompareTo(e2.EventStartDate));
            PopulateEvents(timeline.Events, timeline.TimelineStartDate, timeline.Id);
        }
        else
        {
            pnlEvents.Height = (int)(130 * MainDesigner.BaseHeightFactor());
            Height = pnlEvents.Height + 35;
        }
        PopulateDates(timeline.TimelineStartDate, timeline.TimelineEndDate);
    }

    private void PopulateEvents(List<Event> events, DateTime startDate, ObjectId id)
    {
        int lowestBottom = 0;

        for (ushort i = 0; i < events.Count; i++)
        {
            int eventDuration = (int)(events[i].EventEndDate - events[i].EventStartDate).TotalDays;
            int eventsXAxis = (events[i].EventStartDate - startDate).Days * columnSize;
            int eventsYAxis = (new TimelineDays().Bottom * 2);

            TimelineEvent newEvent = new TimelineEvent();
            newEvent.SetEventProperty(
                events[i].EventTitle,
                events[i].EventDescription,
                events[i].EventStartDate,
                events[i].EventEndDate,
                events[i].EventColor);

            newEvent.Id = id;
            newEvent.Index = i;
            newEvent.Width = (eventDuration * columnSize) + 1;
            newEvent.Location = new Point(eventsXAxis + (new TimelineDays().Width / 2), (int)(70 * MainDesigner.BaseHeightFactor()));

            pnlEvents.Controls.Add(newEvent);
            StackEvents(newEvent, ref lowestBottom);
        }
        pnlEvents.Height = lowestBottom + 30;
        Height = pnlEvents.Height + 35;
    }

    private void StackEvents(TimelineEvent newEvent, ref int lowestBottom)
    {
        int currentRow = (int)(70 * MainDesigner.BaseHeightFactor());

        foreach (TimelineEvent previousEvent in pnlEvents.Controls.OfType<TimelineEvent>())
        {
            // Not the same object and newEvent is above previousEvent
            if (previousEvent != newEvent && newEvent.Top <= previousEvent.Top)
            {
                // Possible overflows
                if ((newEvent.StartDate > previousEvent.StartDate && newEvent.StartDate < previousEvent.EndDate)
                    || (newEvent.StartDate >= previousEvent.StartDate && newEvent.EndDate <= previousEvent.StartDate)
                    || (newEvent.StartDate <= previousEvent.StartDate && newEvent.EndDate > previousEvent.StartDate)
                    || (newEvent.StartDate <= previousEvent.StartDate && newEvent.EndDate >= previousEvent.EndDate))
                {
                    currentRow += (int)(40 * MainDesigner.BaseHeightFactor());
                }
            }
        }
        newEvent.Top = currentRow;
        lowestBottom = Math.Max(newEvent.Bottom, lowestBottom);
    }

    private void PopulateDates(DateTime startDate, DateTime endDate)
    {
        int firstMonthRight = 0;
        int firstMonthBottom = 0;
        for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
        {
            if (currentDate.Day == 1 || currentDate.DayOfYear == startDate.DayOfYear)
                GenerateMonthLabel(currentDate, startDate, ref firstMonthRight, ref firstMonthBottom);

            TimelineDays timelineDays = new TimelineDays();
            timelineDays.Day = currentDate.ToString("ddd");
            timelineDays.Date = currentDate.Day.ToString();
            timelineDays.Location = new Point(columnSize * (currentDate - startDate).Days, firstMonthBottom - 5);
            pnlEvents.Controls.Add(timelineDays);

            Panel line = new Panel();
            line.BackColor = Color.Black;
            line.Width = 1;
            line.Height = pnlEvents.Height - 58;
            line.Location = new Point(timelineDays.Left + (timelineDays.Width / 2), timelineDays.Height);
            pnlEvents.Controls.Add(line);

            if (currentDate.DayOfYear == DateTime.Now.DayOfYear && currentDate.Year == DateTime.Now.Year)
            {
                Point point = new Point(timelineDays.Left + (timelineDays.Width / 2)
                    + ((int)(columnSize * (float)(DateTime.Now.Hour / 24.0))),
                    timelineDays.Height - 22);
                CurrentDateTimePosition = GenerateTimeIndicator(point);
                line.Width = 2;
            }
        }
    }

    private void GenerateMonthLabel(DateTime currentDate, DateTime startDate, ref int firstMonthRight, ref int firstMonthBottom)
    {
        Label nextMonths = new Label();
        nextMonths.Text = currentDate.ToString("MMMM yyyy");
        nextMonths.Font = new Font("Dubai", 10, FontStyle.Bold);
        nextMonths.Location = new Point(columnSize * (currentDate - startDate).Days, 0);
        nextMonths.AutoSize = true;
        pnlEvents.Controls.Add(nextMonths);

        if (nextMonths.Left < firstMonthRight)
            nextMonths.Left = firstMonthRight;

        else if (currentDate.Day == startDate.Day)
        {
            firstMonthRight = nextMonths.Right;
            firstMonthBottom = nextMonths.Bottom;
        }
    }

    private int GenerateTimeIndicator(Point point)
    {
        Panel timeIndicatorLine = new Panel();
        timeIndicatorLine.BackColor = MainDesigner.ThemeColor;
        timeIndicatorLine.Width = 3;
        timeIndicatorLine.Height = pnlEvents.Height - 35;
        timeIndicatorLine.Location = point;
        pnlEvents.Controls.Add(timeIndicatorLine);
        timeIndicatorLine.BringToFront();

        Label timeIndicatorText = new Label();
        timeIndicatorText.BackColor = MainDesigner.ThemeColor;
        timeIndicatorText.ForeColor = Color.White;
        timeIndicatorText.Text = DateTime.Now.ToString("hh:mm tt");
        timeIndicatorText.Font = new Font("Dubai", 8, FontStyle.Bold);
        timeIndicatorText.Location = new Point(timeIndicatorLine.Left, timeIndicatorLine.Top - 14);
        timeIndicatorText.AutoSize = true;
        pnlEvents.Controls.Add(timeIndicatorText);
        timeIndicatorText.BringToFront();
        return timeIndicatorLine.Left;
    }

    public void AddNewTab(string timelineName, ObjectId Id)
    {
        TimelineTab newTimelineTab = new TimelineTab();
        newTimelineTab.TimelineTabMenu_ItemClicked += mnuTimeline_ItemClicked;
        newTimelineTab.AddOption_ItemClicked += btnAddTab_Click;
        newTimelineTab.OpenAtBottomOption_ItemClicked += mnuTimeline_ItemClicked;
        newTimelineTab.TabName = timelineName;
        newTimelineTab.Id = Id;
        newTimelineTab.TimelinePanel = this;
        newTimelineTab.Dock = DockStyle.Left;
        pnlTab.Controls.Add(newTimelineTab);
        newTimelineTab.BringToFront();

        if (CurrentID == Id) // Highlight the current tab
        {
            newTimelineTab.btnTab.BackColor = MainDesigner.HighlightColor;
        }
    }

    private void btnAddTab_Click(object sender, EventArgs e)
    {
        AddTimeline addTimeline = new AddTimeline();
        addTimeline.ShowDialog();

        if (addTimeline.Id != null)
        {
            foreach (TimelineTab tab in pnlTab.Controls) // Remove the highlight of active Tab
            {
                if (CurrentID == tab.Id)
                {
                    tab.btnTab.BackColor = MainDesigner.ThemeColor;
                    break;
                }
            }
            // Load the new added TimelineTab
            pnlEvents.Controls.Clear();
            pnlEvents.Height = 130;
            Height = pnlEvents.Height + 35;
            CurrentID = addTimeline.Id;
            CurrentDateTimePosition = 0;
            LoadTimelineById(addTimeline.Id);
        }
        addTimeline.Dispose();
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        MainForm mainForm = (MainForm)ParentForm;
        EditTimeline editTimeline = new EditTimeline();
        editTimeline.CurrentID = CurrentID;
        editTimeline.EventIds = EventIds;
        editTimeline.ShowDialog();

        CurrentID = editTimeline.CurrentID;
        EventIds = editTimeline.EventIds;
        CurrentDateTimePosition = 0;
        editTimeline.Dispose();
        pnlTab.Controls.Clear();
        pnlEvents.Controls.Clear();

        if (EventIds.Any())
        {
            using var timelineConnection = new LiteDatabase(DBConnection.databaseConnection_timeline);
            var timelineDB = timelineConnection.GetCollection<Timeline>("Timeline");
            foreach (ObjectId id in EventIds)
            {
                var tab = timelineDB.FindById(id);
                AddNewTab(tab.TimelineName, tab.Id);

                if (id == CurrentID)
                    PopulateTimeline(tab);
            }
        }
        else if (mainForm.tabPanel.Controls.Count > 1)
            Dispose();
    }

    private void btnSeek_Click(object sender, EventArgs e)
    {
        pnlEvents.AutoScrollPosition = new Point(CurrentDateTimePosition - (Screen.PrimaryScreen.Bounds.Width / 2));
    }

    private void mnuTimeline_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        if (e.ClickedItem.Name == mnuEdit.Name)
        {
            btnEdit_Click(sender, e);
        }

        else if (e.ClickedItem.Name == mnuOpenAtBottom.Name)
        {
            if (pnlTab.Controls.Count > 1)
            {
                pnlEvents.Controls.Clear();
                MainForm mainForm = (MainForm)ParentForm;
                TimelinePanel newtimelinePanel = new TimelinePanel();
                newtimelinePanel.CurrentID = CurrentID;
                newtimelinePanel.Show();
                newtimelinePanel.TopLevel = false;
                newtimelinePanel.Dock = DockStyle.Top;

                mainForm.tabPanel.Controls.Add(newtimelinePanel);
                newtimelinePanel.BringToFront();
                mainForm.tabPanel.Focus();
                EventIds.Remove(CurrentID);

                foreach (TimelineTab tab in pnlTab.Controls) // Dispose the moved tab
                {
                    if (CurrentID == tab.Id)
                    {
                        tab.Dispose();
                        break;
                    }
                }

                foreach (TimelineTab tab in pnlTab.Controls) // Prevent the loading of moved tab
                {
                    using var timelineConnection = new LiteDatabase(DBConnection.databaseConnection_timeline);
                    var timelineDB = timelineConnection.GetCollection<Timeline>("Timeline");
                    Timeline timelineTab;
                    if (CurrentID != PreviousID && PreviousID != null)
                    {
                        timelineTab = timelineDB.FindById(PreviousID);
                        CurrentID = PreviousID;
                    }
                    else
                    {
                        timelineTab = timelineDB.FindById(tab.Id);
                        tab.btnTab.BackColor = MainDesigner.HighlightColor;
                        CurrentID = tab.Id;
                    }
                    PopulateTimeline(timelineTab);
                    break;
                }
            }
            else
                new Message("Last remaining tab");
        }

        else if (e.ClickedItem == mnuDeletePanel)
        {
            MainForm mainForm = (MainForm)ParentForm;
            if (mainForm.tabPanel.Controls.Count > 1)
            {
                // Look for the a panel to move the tab
                TimelinePanel lastTimelinePanel = new TimelinePanel();
                foreach (TimelinePanel panel in mainForm.tabPanel.Controls.OfType<TimelinePanel>())
                {
                    if (panel != this)
                    {
                        lastTimelinePanel = panel;
                        break;
                    }
                }

                foreach (TimelineTab tab in pnlTab.Controls) // Add the tab
                {
                    lastTimelinePanel.EventIds.Add(tab.Id);
                    lastTimelinePanel.AddNewTab(tab.TabName, tab.Id);
                }
                Dispose();
            }
            else
                new Message("Last remaining panel");
        }
    }
}
