namespace Laqeeta.Web.Models;

// These classes mirror the JSON that Laqeeta.API sends and receives.

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class CreateLostReportRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public DateTime LostAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SecretQuestion { get; set; }
    public string? SecretAnswer { get; set; }
}

public class ReportCreatedResponse
{
    public int ReportId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ReportLookupRequest
{
    public int ReportId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ReportStatusResponse
{
    public int ReportId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PossibleMatchResponse
{
    public int ItemId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public DateTime FoundAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
}

public class CreateClaimRequest
{
    public int ReportId { get; set; }
    public int ItemId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ClaimStatusRequest
{
    public int ClaimId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}

public class ClaimResponse
{
    public int ClaimId { get; set; }
    public int ReportId { get; set; }
    public int ItemId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? HandedOverAt { get; set; }
}

public class CreateFoundItemRequest
{
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public DateTime FoundAt { get; set; }
    public string Description { get; set; } = string.Empty;
    public int FoundAtStationId { get; set; }
}

public class FoundItemCreatedResponse
{
    public int ItemId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public DateTime FoundAt { get; set; }
}

public class SecretQuestionResponse
{
    public int ClaimId { get; set; }
    public string SecretQuestion { get; set; } = string.Empty;
}

public class VerifyClaimRequest
{
    public bool IdVerified { get; set; }
    public string? SecretAnswer { get; set; }
}

public class StationReportResponse
{
    public int StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public int FoundItemsCount { get; set; }
    public int ReturnedItemsCount { get; set; }
    public decimal ReturnRate { get; set; }
}
