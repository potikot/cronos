namespace Cronos.Infrastructure.Dashboard;

public interface IDashboardOptions
{
    bool PrintOnStart { get; }
    List<string> DashboardCommands { get; }
}

public class DashboardOptions : IDashboardOptions
{
    public bool PrintOnStart { get; set; }
    public List<string> DashboardCommands { get; set; }
}