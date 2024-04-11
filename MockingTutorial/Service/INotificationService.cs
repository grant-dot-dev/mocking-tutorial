namespace Service;

public interface INotificationService
{
    void NotifyUserTaskCompleted(int id, int userId);
}
