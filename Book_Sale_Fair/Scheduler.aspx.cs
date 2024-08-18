using System;
using Book_Sale_Fair.Model;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;

namespace Book_Sale_Fair {
    public partial class SchedulerModule : System.Web.UI.Page {
        protected void Page_Init(object sender, EventArgs e) {
            ResourcesListBox.DataSource = ResourceDataSourceHelper.GetItems();
            ResourcesListBox.DataBind();
            if(!IsPostBack)
                ResourcesListBox.SelectAll();

            // DXCOMMENT: Setting ViewType: a compact view (Day) for mobile devices, a large view (WorkWeek) for desktops
            Scheduler.ActiveViewType = RenderUtils.Browser.Platform.IsMobileUI ? SchedulerViewType.Day : SchedulerViewType.WorkWeek;

            if(!IsPostBack) {
                // DXCOMMENT: Scroll to actual time
                var currentTime = new TimeSpan(DateTime.Now.Hour - 1, 0, 0);

                Scheduler.DayView.TopRowTime = currentTime;
                Scheduler.WorkWeekView.TopRowTime = currentTime;
                Scheduler.FullWeekView.TopRowTime = currentTime;
            }

            // DXCOMMENT: Map labels by their ids
            Scheduler.Storage.Appointments.Labels.Clear();
            foreach(SchedulerLabel label in SchedulerLabelsHelper.GetItems())
                Scheduler.Storage.Appointments.Labels.Add(label.Id, label.Name, label.Name, label.Color);
        }

        protected void Scheduler_FilterResource(object sender, PersistentObjectCancelEventArgs e) {
            if(ResourcesListBox.SelectedValues.Count != ResourcesListBox.Items.Count) {
                if(ResourcesListBox.SelectedValues.Count == 0)
                    e.Cancel = true;
                else {
                    var resourceId = (Int64)(e.Object as Resource).Id;
                    if(!ResourcesListBox.SelectedValues.Contains(resourceId))
                        e.Cancel = true;
                }
            }
        }
    }
}