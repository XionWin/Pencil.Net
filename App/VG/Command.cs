using OpenTK.Mathematics;

namespace App.VG;

public enum CommandType
{
	MoveTo = 0,
	LineTo = 1,
	BezierTo = 2,
	Close = 3,
	Winding = 4,
}


public class Command
{
    public CommandType CommandType { get; init; }
    public CommandPoint[] Points { get; init; }

	public Command(CommandType commandType, IEnumerable<float> values)
	{
		this.CommandType = commandType;
		this.Points = values.ToArray().ToCommandPoints();
	}

	public Command(CommandType commandType, params float[] values)
	{
		this.CommandType = commandType;
		this.Points = values.ToCommandPoints();
	}
}

public static class CommandExtension
{
	public static CommandPoint[] ToCommandPoints(this float[] values)
	{
		if(values.Length % 2 != 0)
			throw new Exception("Point value pattern error");
		var result = new CommandPoint[values.Length / 2];
		unsafe
		{
			fixed(float* ptr = values)
			{
				CommandPoint* pointPtr = (CommandPoint*)ptr;
				for (int i = 0; i < result.Length; i++)
				{
					result[i] = *pointPtr;
					pointPtr++;
				}
			}
		}
		return result;
	}

    public static Command Transfrom(this Command command, Matrix2D transform)
    {
		for (int i = 0; i < command.Points.Length; i++)
		{
			var point = command.Points[i];
			var x = point.X;
			var y = point.Y;
			point.X = x * transform.M11 + y * transform.M12 + transform.M13;
			point.Y = x * transform.M21 + y * transform.M22 + transform.M23;
		}
        return command;
    }
}