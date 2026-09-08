# MoveBreak

یک دستیار آفلاین و فارسی برای یادآوری استراحت و تحرک کوتاه در ویندوز.

## فناوری و معماری

- .NET 8 / WPF / MVVM با CommunityToolkit.Mvvm
- SQLite و Entity Framework Core (ایجاد خودکار دیتابیس در `%LocalAppData%\MoveBreak`)
- Dependency Injection، System Tray، اعلان ویندوز و تشخیص Idle/Lock/Fullscreen
- جداسازی لایه‌ها: `Models`، `Data`، `Services`، `ViewModels` و `Views`

## اجرا

```powershell
dotnet restore
dotnet run
```

نیازمند .NET 8 SDK روی Windows 10 1809 یا جدیدتر است.

## جریان برنامه

تایمر فقط هنگام فعالیت کاربر جلو می‌رود. در حالت Lock یا Idle متوقف می‌شود. هنگام رسیدن زمان استراحت، اگر برنامه‌ای تمام‌صفحه باشد نمایش تا خروج از آن به تعویق می‌افتد؛ سپس اعلان و کارت تمرین نشان داده می‌شود. نتیجه انتخاب کاربر در SQLite ثبت شده و آمار روز و هفته به‌روزرسانی می‌شود.

## نقشه راه بعد از MVP

- اعلان Toast تعاملی با دکمه‌های native
- نمودار پیشرفته و مجموع نشست‌ها میان چند اجرای برنامه
- تشخیص دقیق جلسه آنلاین و Presentation Mode از Windows APIs
- پوسته تاریک کامل، محلی‌سازی و تست‌های خودکار
