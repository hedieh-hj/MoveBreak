namespace MoveBreak.Models;

public enum ExerciseCategory { Neck, Shoulder, Back, Wrist, Leg, Eye }
public enum BreakResult { Completed, Skipped, Snoozed }

public sealed class Exercise
{
    public int Id { get; set; }
    public string TitleEn { get; set; } = "";
    public string TitleFa { get; set; } = "";
    public string TitleEs { get; set; } = "";
    public string InstructionsEn { get; set; } = "";
    public string InstructionsFa { get; set; } = "";
    public string InstructionsEs { get; set; } = "";
    public ExerciseCategory Category { get; set; }
    public int DurationSeconds { get; set; }
    public string ImagePath { get; set; } = "";
    public string GetTitle(string languageCode) => languageCode switch { "fa" => TitleFa, "es" => TitleEs, _ => TitleEn };
    public string GetInstructions(string languageCode) => languageCode switch { "fa" => InstructionsFa, "es" => InstructionsEs, _ => InstructionsEn };
}
public sealed class BreakRecord
{
    public long Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public BreakResult Result { get; set; }
    public int ExerciseId { get; set; }
    public int SittingSeconds { get; set; }
}
public sealed class DailySession
{
    public long Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int ActiveSeconds { get; set; }
}
public sealed class AppSettings
{
    public int Id { get; set; } = 1;
    public int ReminderMinutes { get; set; } = 45;
    public int IdleThresholdMinutes { get; set; } = 3;
    public bool SoundEnabled { get; set; } = true;
    public bool StartWithWindows { get; set; }
    public bool EyeRuleEnabled { get; set; } = true;
    public bool DarkMode { get; set; }
    public string LanguageCode { get; set; } = "en";
}
