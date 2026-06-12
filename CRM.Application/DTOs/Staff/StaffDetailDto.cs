namespace CRM.Application.DTOs.Staff;

public class StaffDetailDto : StaffSummaryDto
{
    public List<OnboardingItemDto> OnboardingItems { get; set; } = new();
}

public class OnboardingItemDto
{
    public Guid Id { get; set; }
    public string Section { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? CompletedDate { get; set; }
    public string? ExpiryDate { get; set; }
}
