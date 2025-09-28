using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Tasks")]
public class TaskEntity
{
	[Key]
	public int Id { get; set; }

	[Required]
	public string TaskName { get; set; } = string.Empty;

	[Required]
	public DateTime StartDate { get; set; }

	[Required]
	public DateTime EndDate { get; set; }

	public string? Description { get; set; }

	[Required]
	public string? Status { get; set; }

	[Required]
	public double? PlannedHours { get; set; }

	[Required]
	public double? ActualHours { get; set; }

	public string? Remarks { get; set; }
}