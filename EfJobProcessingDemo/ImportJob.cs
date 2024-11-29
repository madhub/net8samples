namespace EfJobProcessingDemo;

public class ImportJob
{
    public int Id { get; set; }
    public Guid ?DomainId { get; set; }
    public required String JobParams { get; set; }
    public DateTime CreatedAt { get; set; }
    public JobStatus Status { get; set; }
}

public enum JobStatus
{
    Undefined = 0,
    Enqueued = 1,
    Running = 2,
    Completed= 3,
    Failed= 4

}

