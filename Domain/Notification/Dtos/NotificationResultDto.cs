namespace DotNetService.Domain.Notification.Dtos
{
    public class NotificationResultDto : Models.Notification
    {
        public NotificationResultDto(Models.Notification notification)
        {
            Id = notification.Id;
            Title = notification.Title;
            Content = notification.Content;
            Href = notification.Href;
            IsRead = notification.IsRead;
            CreatedAt = notification.CreatedAt;
            UpdatedAt = notification.UpdatedAt;
        }

        public static List<NotificationResultDto> MapRepo(List<Models.Notification> data)
        {
            return data?.Select(data => new NotificationResultDto(data)).ToList();
        }
    }
}
