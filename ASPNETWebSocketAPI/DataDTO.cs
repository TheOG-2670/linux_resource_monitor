namespace LinuxResourceMonitorApi
{
    public class DataDTO
    {
        public DataDTO(string id, string? message = "")
        {
            UserId = id;
            Message = message;
        }
        public string? UserId { get; set; }
        public string? Message { get; set; }
    }
}
