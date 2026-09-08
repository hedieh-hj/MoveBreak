using MoveBreak.Models;

namespace MoveBreak.Services;
public sealed class ExerciseService
{
    private readonly Random _random = new();
    public IReadOnlyList<Exercise> All { get; } = new List<Exercise>
    {
        new() { Id=1, Title="کشش آرام گردن", Category=ExerciseCategory.Neck, DurationSeconds=40, ImagePath="/Assets/Exercises/neck.png", Instructions="صاف بنشینید، گوش را بدون بالا آوردن شانه به سمت شانه ببرید؛ هر سمت ۲۰ ثانیه." },
        new() { Id=2, Title="چرخش شانه‌ها", Category=ExerciseCategory.Shoulder, DurationSeconds=45, ImagePath="/Assets/Exercises/shoulder.png", Instructions="شانه‌ها را ۱۰ بار آرام به عقب و سپس ۱۰ بار به جلو بچرخانید." },
        new() { Id=3, Title="چرخش نشسته کمر", Category=ExerciseCategory.Back, DurationSeconds=40, ImagePath="/Assets/Exercises/back.png", Instructions="صاف بنشینید و تنه را بدون فشار به یک سمت بچرخانید؛ هر سمت ۲۰ ثانیه." },
        new() { Id=4, Title="کشش مچ دست", Category=ExerciseCategory.Wrist, DurationSeconds=40, ImagePath="/Assets/Exercises/wrist.png", Instructions="دست را کشیده نگه دارید و با دست دیگر انگشت‌ها را آرام به عقب بکشید؛ سپس سمت دیگر." },
        new() { Id=5, Title="بالا بردن پاشنه", Category=ExerciseCategory.Leg, DurationSeconds=45, ImagePath="/Assets/Exercises/leg.png", Instructions="کنار میز بایستید، ۱۲ بار روی پنجه بالا بروید و آرام پایین بیایید." },
        new() { Id=6, Title="قانون ۲۰-۲۰-۲۰", Category=ExerciseCategory.Eye, DurationSeconds=20, ImagePath="/Assets/Exercises/eye.png", Instructions="۲۰ ثانیه به جسمی در فاصله دست‌کم ۶ متر نگاه کنید و چند بار پلک بزنید." }
    };
    public Exercise Next(bool eyeRule, int elapsedMinutes)
    {
        if (eyeRule && elapsedMinutes >= 20 && _random.NextDouble() < .35) return All.Single(x => x.Category == ExerciseCategory.Eye);
        return All[_random.Next(All.Count)];
    }
}
