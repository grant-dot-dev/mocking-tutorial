namespace Service;

public class NotificationService : INotificationService
{
    public void NotifyUserTaskCompleted(int id, int userId)
    {
        // Would call a Push Notification service notifying user of completed task
    }
}
