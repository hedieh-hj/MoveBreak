namespace MoveBreak.Models;

public sealed record LanguageOption(string Code, string DisplayName);

public sealed record ExerciseDisplayItem(
    int Id,
    string Title,
    string Instructions,
    ExerciseCategory Category,
    int DurationSeconds,
    string ImagePath);
