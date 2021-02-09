using System;

namespace AspMongo.Shares
{
	public class MyConsole
	{
		public static void Write(string Message, ConsoleColor consoleColor = ConsoleColor.White)
		{
			System.Console.ForegroundColor = consoleColor;
			System.Console.Write(Message);
		}
		public static void WriteLine(string Message, ConsoleColor consoleColor = ConsoleColor.White)
		{
			System.Console.ForegroundColor = consoleColor;
			System.Console.WriteLine(Message);
		}
		public static void NewLine()
		{
			Console.WriteLine();
		}

		~MyConsole()
		{
			System.Console.ResetColor();
		}
	}
}
