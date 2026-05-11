using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace KresApp.Application.DTOs;

// ── Ön Başvuru (anonim — basit bilgiler) ─────────────────────────────────────
public class CreateEnrollmentRequestDto
{
    [JsonPropertyName("ParentName")] [JsonProperty("ParentName")] public string ParentName { get; set; } = default!;
    [JsonPropertyName("ParentEmail")] [JsonProperty("ParentEmail")] public string ParentEmail { get; set; } = default!;
    [JsonPropertyName("ParentPhone")] [JsonProperty("ParentPhone")] public string ParentPhone { get; set; } = default!;
    [JsonPropertyName("ParentTcKimlikNo")] [JsonProperty("ParentTcKimlikNo")] public string? ParentTcKimlikNo { get; set; }
    [JsonPropertyName("ParentPassword")] [JsonProperty("ParentPassword")] public string ParentPassword { get; set; } = default!;
    [JsonPropertyName("ParentJob")] [JsonProperty("ParentJob")] public string? ParentJob { get; set; }
    [JsonPropertyName("ParentWorkAddress")] [JsonProperty("ParentWorkAddress")] public string? ParentWorkAddress { get; set; }
    [JsonPropertyName("ParentHomeAddress")] [JsonProperty("ParentHomeAddress")] public string? ParentHomeAddress { get; set; }

    [JsonPropertyName("FatherName")] [JsonProperty("FatherName")] public string? FatherName { get; set; }
    [JsonPropertyName("FatherPhone")] [JsonProperty("FatherPhone")] public string? FatherPhone { get; set; }
    [JsonPropertyName("FatherTcKimlikNo")] [JsonProperty("FatherTcKimlikNo")] public string? FatherTcKimlikNo { get; set; }
    [JsonPropertyName("FatherJob")] [JsonProperty("FatherJob")] public string? FatherJob { get; set; }
    [JsonPropertyName("FatherWorkAddress")] [JsonProperty("FatherWorkAddress")] public string? FatherWorkAddress { get; set; }

    [JsonPropertyName("ChildFullName")] [JsonProperty("ChildFullName")] public string ChildFullName { get; set; } = default!;
    [JsonPropertyName("ChildTcKimlikNo")] [JsonProperty("ChildTcKimlikNo")] public string? ChildTcKimlikNo { get; set; }
    [JsonPropertyName("ChildBirthDate")] [JsonProperty("ChildBirthDate")] public DateOnly? ChildBirthDate { get; set; }
    [JsonPropertyName("ChildGender")] [JsonProperty("ChildGender")] public string? ChildGender { get; set; }
    [JsonPropertyName("ChildBloodType")] [JsonProperty("ChildBloodType")] public string? ChildBloodType { get; set; }
    [JsonPropertyName("ChildAllergies")] [JsonProperty("ChildAllergies")] public string? ChildAllergies { get; set; }
    [JsonPropertyName("Notes")] [JsonProperty("Notes")] public string? Notes { get; set; }

    // Yeni Alanlar
    [JsonPropertyName("ParentSicilNo")] [JsonProperty("ParentSicilNo")] public string? ParentSicilNo { get; set; }
    [JsonPropertyName("ParentUnit")] [JsonProperty("ParentUnit")] public string? ParentUnit { get; set; }
    [JsonPropertyName("ParentTitle")] [JsonProperty("ParentTitle")] public string? ParentTitle { get; set; }
    [JsonPropertyName("ParentServiceYears")] [JsonProperty("ParentServiceYears")] public int? ParentServiceYears { get; set; }
    [JsonPropertyName("SpouseIsAlive")] [JsonProperty("SpouseIsAlive")] public bool? SpouseIsAlive { get; set; }
    [JsonPropertyName("SpouseIsWorking")] [JsonProperty("SpouseIsWorking")] public bool? SpouseIsWorking { get; set; }
    [JsonPropertyName("SpouseWorkplace")] [JsonProperty("SpouseWorkplace")] public string? SpouseWorkplace { get; set; }
    [JsonPropertyName("SpouseWorkplaceHasDaycare")] [JsonProperty("SpouseWorkplaceHasDaycare")] public bool? SpouseWorkplaceHasDaycare { get; set; }
}

// ── Admin listesi için özet ──────────────────────────────────────────────────
public record EnrollmentRequestDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("parentName")] string ParentName,
    [property: JsonPropertyName("parentEmail")] string ParentEmail,
    [property: JsonPropertyName("parentPhone")] string ParentPhone,
    [property: JsonPropertyName("parentTcKimlikNo")] string? ParentTcKimlikNo,
    [property: JsonPropertyName("childFullName")] string ChildFullName,
    [property: JsonPropertyName("childTcKimlikNo")] string? ChildTcKimlikNo,
    [property: JsonPropertyName("childBirthDate")] DateOnly? ChildBirthDate,
    [property: JsonPropertyName("childGender")] string? ChildGender,
    [property: JsonPropertyName("childBloodType")] string? ChildBloodType,
    [property: JsonPropertyName("childAllergies")] string? ChildAllergies,
    [property: JsonPropertyName("score")] int? Score,
    [property: JsonPropertyName("scoringNotes")] string? ScoringNotes,
    [property: JsonPropertyName("notes")] string? Notes,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("adminNote")] string? AdminNote,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("reviewedAt")] DateTime? ReviewedAt,

    // Yeni Alanlar
    [property: JsonPropertyName("parentSicilNo")] string? ParentSicilNo,
    [property: JsonPropertyName("parentUnit")] string? ParentUnit,
    [property: JsonPropertyName("parentTitle")] string? ParentTitle,
    [property: JsonPropertyName("parentServiceYears")] int? ParentServiceYears,
    [property: JsonPropertyName("spouseIsAlive")] bool? SpouseIsAlive,
    [property: JsonPropertyName("spouseIsWorking")] bool? SpouseIsWorking,
    [property: JsonPropertyName("spouseWorkplace")] string? SpouseWorkplace,
    [property: JsonPropertyName("spouseWorkplaceHasDaycare")] bool? SpouseWorkplaceHasDaycare
);

// ── Admin puanlama ───────────────────────────────────────────────────────────
public record ScoreEnrollmentDto(int Score, string? Notes);

// ── Admin onay (sınıf seçimi + not) ─────────────────────────────────────────
public record ApproveEnrollmentDto(Guid ClassId, string? AdminNote);

// ── Admin red ────────────────────────────────────────────────────────────────
public record RejectEnrollmentDto(string? AdminNote);
