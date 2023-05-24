﻿using LiteDB;

namespace ScheduleMe.Tab;

public partial class TimelineMain : Form
{
    public ObjectId currentID { get; set; }
    private byte columnSize = 42;
    private short currentDateTimePosition = 0;
    private int currentRow = 0;

    public TimelineMain()
    {
        InitializeComponent();
    }

    private void Timeline_Load(object sender, EventArgs e)
    {
        using (var timelineDB = new LiteDatabase(DBConnection.timelineConnection))
        {
            var timelines = timelineDB.GetCollection<Timeline>("Timeline");
            var timelineTabs = timelines.FindAll();
            if (timelineTabs.Any() == true)
            {
                Timeline firstToLoad = timelineTabs.First();
                currentID = firstToLoad.Id;

                // Load all the Timeline Tabs
                foreach (var tab in timelineTabs)
                {
                    addNewTab(tab.TimelineName, tab.Id);
                }

                // Load the first Timeline.Event List only
                if (firstToLoad.Events.Any() == true)
                {
                    // Need to improve the sorting or the overlapping method. Too difficult
                    firstToLoad.Events.Sort((e1, e2) => e1.EventEndDate.CompareTo(e2.EventStartDate));
                    PopulateEvents(firstToLoad.Events, firstToLoad.TimelineStartDate, firstToLoad.Id);
                }
                else
                    panelTimelineContainer.Height = 130;
                PopulateDates(firstToLoad.TimelineStartDate, firstToLoad.TimelineEndDate);
            }
        }
    }

    public void PopulateDates(DateTime startDate, DateTime endDate)
    {
        Label firstMonth = new Label();
        firstMonth.Text = startDate.ToString("MMMM yyyy");
        firstMonth.Font = new Font("Dubai", 10, FontStyle.Bold);
        firstMonth.Location = new Point(0, 0);
        firstMonth.AutoSize = true;
        panelTimelineContainer.Controls.Add(firstMonth);

        for (DateTime currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
        {
            // This can produce overlapping month if the first day is 1 but can't be notice
            if (currentDate.Day == 1)
            {
                Label nextMonths = new Label();
                nextMonths.Text = currentDate.ToString("MMMM yyyy");
                nextMonths.Font = new Font("Dubai", 10, FontStyle.Bold);
                nextMonths.Location = new Point(columnSize * (currentDate - startDate).Days, 0);
                nextMonths.AutoSize = true;
                panelTimelineContainer.Controls.Add(nextMonths);
            }

            DatesLabel dayDates = new DatesLabel();
            dayDates.Day = currentDate.ToString("ddd");
            dayDates.Date = currentDate.Day.ToString();
            dayDates.Location = new Point(columnSize * (currentDate - startDate).Days, firstMonth.Height - 5);
            panelTimelineContainer.Controls.Add(dayDates);

            Panel line = new Panel();
            line.BackColor = Color.Black;
            line.Width = 1;
            line.Height = panelTimelineContainer.Height - 58;
            line.Location = new Point(dayDates.Left + dayDates.Width / 2, dayDates.Height);
            panelTimelineContainer.Controls.Add(line);

            // I was planning to move this outside later
            if (currentDate.DayOfYear == DateTime.Now.DayOfYear && currentDate.Year == DateTime.Now.Year)
            {
                Panel timeIndicatorLine = new Panel();
                timeIndicatorLine.BackColor = Color.FromArgb(15, 76, 129);
                timeIndicatorLine.Width = 3;
                timeIndicatorLine.Height = panelTimelineContainer.Height - 35;
                timeIndicatorLine.Location = new Point((dayDates.Left + dayDates.Width / 2)
                    + ((int)((float)columnSize * (float)(DateTime.Now.Hour / 24.0))),
                    dayDates.Height - 22);
                panelTimelineContainer.Controls.Add(timeIndicatorLine);
                timeIndicatorLine.BringToFront();

                Label timeIndicatorText = new Label();
                timeIndicatorText.BackColor = Color.FromArgb(15, 76, 129);
                timeIndicatorText.ForeColor = Color.White;
                timeIndicatorText.Text = DateTime.Now.ToString("hh:mm tt");
                timeIndicatorText.Font = new Font("Dubai", 8, FontStyle.Bold);
                timeIndicatorText.Location = new Point(timeIndicatorLine.Left, timeIndicatorLine.Top - 14);
                timeIndicatorText.AutoSize = true;
                panelTimelineContainer.Controls.Add(timeIndicatorText);
                timeIndicatorText.BringToFront();

                line.Width = 2;
                currentDateTimePosition = (short)timeIndicatorLine.Left;
            }
        }
    }

    internal void PopulateEvents(List<Event> events, DateTime startDate, ObjectId id)
    {
        int lowestBottom = 0;

        for (ushort i = 0; i < events.Count; i++)
        {
            int eventDuration = (int)(events[i].EventEndDate - events[i].EventStartDate).TotalDays;
            int eventsXAxis = panelTimelineContainer.HorizontalScroll.Value
                        + (events[i].EventStartDate - startDate).Days
                        * columnSize;

            EventButton newEvent = new EventButton();
            newEvent.SetEventProperty(
                events[i].EventTitle,
                events[i].EventDescription,
                events[i].EventStartDate,
                events[i].EventEndDate,
                events[i].EventColor);
            newEvent.Id = id;
            newEvent.Index = i;
            newEvent.Width = eventDuration * columnSize + 1;
            newEvent.Location = new Point(eventsXAxis + 17, 70);

            panelTimelineContainer.Controls.Add(newEvent);
            StackEvents(newEvent, ref lowestBottom);
        }
        panelTimelineContainer.Height = lowestBottom + 30;
    }

    private void StackEvents(EventButton newEvent, ref int lowestBottom)
    {
        int currentRow = 70;

        foreach (EventButton previousEvent in panelTimelineContainer.Controls.OfType<EventButton>())
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
                    currentRow += 40;
                }
            }
        }
        newEvent.Top = currentRow;
        lowestBottom = Math.Max(newEvent.Bottom, lowestBottom);
    }

    public void addNewTab(string timelineName, ObjectId Id)
    {
        TimelineTab newTimelineTab = new TimelineTab();
        newTimelineTab.TabName = timelineName;
        newTimelineTab.Id = Id;
        newTimelineTab.timelineInstance = this;
        newTimelineTab.Dock = DockStyle.Left;
        panelTimelineContainer.Controls.Clear();
        panelTimelineTab.Controls.Add(newTimelineTab);
        newTimelineTab.BringToFront();

        // Highlight the current tab
        if (currentID == Id)
        {
            newTimelineTab.timelineTabBtn.BackColor = Color.White;
            newTimelineTab.timelineTabBtn.ForeColor = Color.Black;
        }
    }

    private void currentDate_Click(object sender, EventArgs e)
    {
        Screen screen = Screen.PrimaryScreen;
        short screenWidth = (short)screen.Bounds.Width;
        panelTimelineContainer.AutoScrollPosition = new Point(currentDateTimePosition - (screenWidth / 2));
    }

    private void additionalInfo_Click(object sender, EventArgs e)
    {
        // Optional feature for now
    }

    private void timelineAddTab_Click(object sender, EventArgs e)
    {
        AddTimeline addTimelineTab = new AddTimeline();
        addTimelineTab.ShowDialog();

        if (addTimelineTab.Id != null)
        {
            // Remove the highlight of active Tab
            foreach (TimelineTab tab in panelTimelineTab.Controls.OfType<TimelineTab>())
            {
                if (currentID == tab.Id)
                {
                    tab.timelineTabBtn.BackColor = Color.FromArgb(15, 76, 129);
                    tab.timelineTabBtn.ForeColor = Color.White;
                    break;
                }
            }
            // Load the new added TimelineTab
            using (var timelineDB = new LiteDatabase(DBConnection.timelineConnection))
            {
                var timelines = timelineDB.GetCollection<Timeline>("Timeline");
                var newtTab = new Timeline();
                newtTab = timelines.FindById(addTimelineTab.Id);
                currentID = newtTab.Id;

                addNewTab(newtTab.TimelineName, newtTab.Id);
                panelTimelineContainer.Controls.Clear();
                panelTimelineContainer.Height = 130;
                PopulateDates(newtTab.TimelineStartDate, newtTab.TimelineEndDate);
            }
        }
    }

    private void timelineMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
        if (e.ClickedItem == editOption)
        {
            EditEvent editEvent = new EditEvent();
            editEvent.ShowDialog();

            panelTimelineTab.Controls.Clear();
            panelTimelineContainer.Controls.Clear();

            using (var timelineDB = new LiteDatabase(DBConnection.timelineConnection))
            {
                var timelines = timelineDB.GetCollection<Timeline>("Timeline");
                var timeline = timelines.FindAll();
                if (timeline.Any() == true)
                {
                    var lastTab = timelines.FindById(currentID);

                    // Load all the Timeline Tabs
                    foreach (var tab in timeline)
                    {
                        addNewTab(tab.TimelineName, tab.Id);
                    }

                    if (lastTab != null) // last tab still exist
                    {
                        if (lastTab.Events != null)
                        {
                            // Need to improve the sorting or the overlapping method. Too difficult
                            lastTab.Events.Sort((e1, e2) => e1.EventEndDate.CompareTo(e2.EventStartDate));
                            PopulateEvents(lastTab.Events, lastTab.TimelineStartDate, lastTab.Id);
                        }
                        else
                            panelTimelineContainer.Height = 130;

                        PopulateDates(lastTab.TimelineStartDate, lastTab.TimelineEndDate);
                    }
                    else // last tab doesn't exist
                    {
                        Timeline firstTab = timelines.FindAll().First();
                        currentID = firstTab.Id;

                        if (firstTab != null) // Load the first Timeline.Event List only
                        {
                            if (firstTab.Events != null)
                            {
                                // Need to improve the sorting or the overlapping method. Too difficult
                                firstTab.Events.Sort((e1, e2) => e1.EventEndDate.CompareTo(e2.EventStartDate));
                                PopulateEvents(firstTab.Events, firstTab.TimelineStartDate, firstTab.Id);
                            }
                            else
                                panelTimelineContainer.Height = 130;
                            PopulateDates(firstTab.TimelineStartDate, firstTab.TimelineEndDate);
                        }
                    }
                }
            }
        }
    }
}
