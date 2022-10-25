using System.ComponentModel.DataAnnotations;

namespace NotificationServices.Core.DTOs
{
    public class EmailDTO
    {
        [EmailAddress]
        [RegularExpression("^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Invalid email format!")]
        public string ToEmail { get; set; } = String.Empty;
        public string Subject { get; set; } = String.Empty;
        public string Message { get; set; } = String.Empty;
    }
}
