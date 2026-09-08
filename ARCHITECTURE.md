# معماری MoveBreak

## اجزا

| لایه | مسئولیت | فایل‌های اصلی |
|---|---|---|
| UI | صفحات خانه، تمرین‌ها، تنظیمات و کارت وقفه | `Views/MainWindow.*` |
| Presentation | state، command و اتصال UI به سرویس‌ها | `ViewModels/MainViewModel.cs` |
| Domain | تمرین، تنظیمات، نشست و نتیجه وقفه | `Models/DomainModels.cs` |
| Application services | تایمر، activity ویندوز، اعلان، تنظیمات و انتخاب تمرین | `Services/*` |
| Persistence | مدل EF Core و SQLite محلی | `Data/AppDbContext.cs` |

## جریان اجرا

1. `App` وابستگی‌ها را ثبت و دیتابیس را ایجاد می‌کند.
2. `WorkTimerService` هر ثانیه وضعیت Idle/Lock را بررسی می‌کند و فقط زمان فعال را می‌شمارد.
3. در موعد مقرر، تمام‌صفحه بودن پنجره foreground بررسی می‌شود؛ اعلان تا زمان مناسب عقب می‌افتد.
4. تمرین آفلاین انتخاب و در اعلان و پنجره نمایش داده می‌شود.
5. انتخاب انجام/تعویق/رد در SQLite ثبت و آمار روزانه و هفتگی تازه می‌شود.

## مراحل توسعه

1. **MVP فعلی:** تایمر، Tray، اعلان، SQLite، تمرین‌های تصویری، تنظیمات، آمار و Windows activity detection.
2. **پایداری:** unit test برای timer policy، migrationهای نسخه‌بندی‌شده و logging.
3. **Windows integration:** Toast تعاملی با AppUserModelID و تشخیص Focus Assist/Presentation/meeting.
4. **انتشار:** MSIX، signing، CI در GitHub Actions و auto-update.
