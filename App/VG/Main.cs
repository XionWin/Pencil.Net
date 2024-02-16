namespace App.VG;

public class Main
{
    static Context context = new Context();
    public static void Test()
    {
        context.GetState().StrokeWidth = 1;
        context.BeginPath();
        context.AddCommand(new Command(CommandType.MoveTo, 100, 100));
        context.AddCommand(new Command(CommandType.LineTo, 120, 100));
        context.AddCommand(new Command(CommandType.LineTo, 120, 120));
        context.Stroke();


    }
}

