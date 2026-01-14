namespace INSS.Platform.AlphaDemo.Web.Models;

public class TaskListViewModel
{
    public bool AboutYouCompleted { get; set; }

    public bool BankDetailsCompleted { get; set; }

    public bool AllTasksCompleted => AboutYouCompleted && BankDetailsCompleted;

    public bool SubmissionError { get; set; }
}
