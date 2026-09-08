using MoveBreak.Models;

namespace MoveBreak.Services;

public sealed class ExerciseService
{
    private readonly Random _random = new();

    public IReadOnlyList<Exercise> All { get; } = new List<Exercise>
    {
        Exercise(1, ExerciseCategory.Neck, 40, "neck.png",
            "Gentle neck side stretch", "کشش جانبی گردن",
            "Sit tall. Lower one ear toward the same-side shoulder without lifting the shoulder. Hold each side for 20 seconds.",
            "صاف بنشینید. بدون بالا آوردن شانه، گوش را به سمت همان شانه ببرید؛ هر سمت را ۲۰ ثانیه نگه دارید."),
        Exercise(2, ExerciseCategory.Shoulder, 45, "shoulder.png",
            "Shoulder rolls", "چرخش شانه‌ها",
            "Roll both shoulders slowly backward 10 times, then forward 10 times.",
            "شانه‌ها را ۱۰ بار آرام به عقب و سپس ۱۰ بار به جلو بچرخانید."),
        Exercise(3, ExerciseCategory.Back, 40, "back.png",
            "Seated spinal twist", "چرخش نشسته کمر",
            "Sit tall and rotate your torso gently to one side without forcing it. Hold each side for 20 seconds.",
            "صاف بنشینید و تنه را بدون فشار به یک سمت بچرخانید؛ هر سمت را ۲۰ ثانیه نگه دارید."),
        Exercise(4, ExerciseCategory.Wrist, 40, "wrist.png",
            "Wrist flexor stretch", "کشش مچ دست",
            "Extend one arm and use the other hand to draw the fingers gently back. Repeat on the other side.",
            "یک دست را بکشید و با دست دیگر انگشت‌ها را آرام به عقب ببرید؛ سپس سمت دیگر را انجام دهید."),
        Exercise(5, ExerciseCategory.Leg, 45, "leg.png",
            "Calf raises", "بالا بردن پاشنه",
            "Stand beside your desk. Rise onto your toes 12 times and lower slowly, using the desk only for balance.",
            "کنار میز بایستید، ۱۲ بار روی پنجه بالا بروید و آرام پایین بیایید؛ از میز فقط برای حفظ تعادل کمک بگیرید."),
        Exercise(6, ExerciseCategory.Eye, 20, "eye.png",
            "20-20-20 eye break", "قانون ۲۰-۲۰-۲۰",
            "Look at something at least 6 metres (20 feet) away for 20 seconds and blink naturally.",
            "۲۰ ثانیه به جسمی در فاصله دست‌کم ۶ متر نگاه کنید و چند بار به‌آرامی پلک بزنید."),

        Exercise(7, ExerciseCategory.Neck, 35, "chin-tuck.png",
            "Chin tuck", "جمع‌کردن چانه",
            "Keep your eyes level and glide your head straight back, making a gentle double chin. Hold 5 seconds and repeat 6 times.",
            "نگاه را افقی نگه دارید و سر را مستقیم به عقب ببرید تا چانه کمی جمع شود؛ ۵ ثانیه نگه دارید و ۶ بار تکرار کنید."),
        Exercise(8, ExerciseCategory.Neck, 40, "neck-rotation.png",
            "Gentle neck rotation", "چرخش آرام گردن",
            "Turn your head slowly to look over one shoulder. Keep the chin level and hold each side for 15–20 seconds.",
            "سر را آرام بچرخانید و از روی یک شانه نگاه کنید؛ چانه را هم‌سطح نگه دارید و هر سمت را ۱۵ تا ۲۰ ثانیه حفظ کنید."),
        Exercise(9, ExerciseCategory.Shoulder, 35, "chest-opener.png",
            "Standing chest opener", "بازکردن قفسه سینه",
            "Stand tall, clasp your hands behind you and gently draw the shoulders back. Keep the ribs relaxed and hold for 20 seconds.",
            "صاف بایستید، دست‌ها را پشت بدن در هم قفل کنید و شانه‌ها را آرام عقب ببرید؛ ۲۰ ثانیه نگه دارید."),
        Exercise(10, ExerciseCategory.Shoulder, 45, "wall-angel.png",
            "Wall angels", "حرکت فرشته روی دیوار",
            "Rest your back against a wall and slide bent arms slowly up and down. Stay comfortable and repeat 8 times.",
            "پشت خود را به دیوار تکیه دهید و دست‌های خم‌شده را آرام بالا و پایین ببرید؛ در دامنه راحت ۸ بار تکرار کنید."),
        Exercise(11, ExerciseCategory.Back, 35, "seated-forward-fold.png",
            "Seated forward fold", "خم‌شدن رو به جلو روی صندلی",
            "With feet planted, fold forward from the hips and let your arms and head relax. Breathe slowly for 20–30 seconds.",
            "کف پاها را روی زمین بگذارید، از لگن به جلو خم شوید و دست‌ها و سر را رها کنید؛ ۲۰ تا ۳۰ ثانیه آرام نفس بکشید."),
        Exercise(12, ExerciseCategory.Back, 30, "back-extension.png",
            "Standing back extension", "بازکردن کمر در حالت ایستاده",
            "Place your hands on your hips and lean back only slightly while keeping your balance. Return slowly and repeat 5 times.",
            "دست‌ها را روی لگن بگذارید و با حفظ تعادل کمی به عقب متمایل شوید؛ آرام برگردید و ۵ بار تکرار کنید."),
        Exercise(13, ExerciseCategory.Wrist, 30, "wrist-circles.png",
            "Wrist circles", "چرخش مچ‌ها",
            "Relax your hands and draw 8 slow circles with both wrists in each direction.",
            "دست‌ها را رها کنید و با هر دو مچ، در هر جهت ۸ دایره آرام رسم کنید."),
        Exercise(14, ExerciseCategory.Wrist, 30, "prayer-stretch.png",
            "Prayer wrist stretch", "کشش دعایی مچ",
            "Press your palms together at chest height and lower the hands slightly until you feel a gentle wrist stretch. Hold 20 seconds.",
            "کف دست‌ها را جلوی سینه به هم بچسبانید و دست‌ها را کمی پایین ببرید تا کشش ملایمی حس شود؛ ۲۰ ثانیه نگه دارید."),
        Exercise(15, ExerciseCategory.Leg, 40, "knee-extension.png",
            "Seated knee extension", "بازکردن زانو روی صندلی",
            "Sit tall and straighten one knee until the leg is comfortable. Hold 3 seconds, lower, and do 8 repetitions per side.",
            "صاف بنشینید و یک زانو را تا محدوده راحت باز کنید؛ ۳ ثانیه نگه دارید و برای هر پا ۸ بار تکرار کنید."),
        Exercise(16, ExerciseCategory.Leg, 40, "quad-stretch.png",
            "Standing quadriceps stretch", "کشش جلوی ران",
            "Hold a stable desk for balance, bring one heel gently toward your seat and hold the ankle. Hold each side for 20 seconds.",
            "برای تعادل میز ثابت را بگیرید، یک پاشنه را آرام به سمت باسن ببرید و مچ پا را نگه دارید؛ هر سمت ۲۰ ثانیه."),
        Exercise(17, ExerciseCategory.Eye, 30, "eye-palming.png",
            "Eye palming", "استراحت چشم با کف دست",
            "Warm your palms, close your eyes and cup your hands over them without pressure. Breathe slowly for 30 seconds.",
            "کف دست‌ها را گرم کنید، چشم‌ها را ببندید و بدون فشار دست‌ها را روی آن‌ها گود کنید؛ ۳۰ ثانیه آرام نفس بکشید."),
        Exercise(18, ExerciseCategory.Eye, 40, "focus-shift.png",
            "Near–far focus shift", "تغییر تمرکز نزدیک و دور",
            "Focus on a finger at arm’s length for 5 seconds, then a distant object for 5 seconds. Repeat 4 times without straining.",
            "۵ ثانیه به انگشتی در فاصله یک دست و سپس ۵ ثانیه به جسمی دور نگاه کنید؛ بدون فشار ۴ بار تکرار کنید.")
    };

    public Exercise Next(bool eyeRule, int elapsedMinutes)
    {
        var eyeExercises = All.Where(x => x.Category == ExerciseCategory.Eye).ToArray();
        if (eyeRule && elapsedMinutes >= 20 && _random.NextDouble() < .35)
            return eyeExercises[_random.Next(eyeExercises.Length)];
        return All[_random.Next(All.Count)];
    }

    private static Exercise Exercise(int id, ExerciseCategory category, int duration, string image,
        string titleEn, string titleFa, string instructionsEn, string instructionsFa) => new()
    {
        Id = id,
        Category = category,
        DurationSeconds = duration,
        ImagePath = $"/Assets/Exercises/{image}",
        TitleEn = titleEn,
        TitleFa = titleFa,
        InstructionsEn = instructionsEn,
        InstructionsFa = instructionsFa
    };
}
