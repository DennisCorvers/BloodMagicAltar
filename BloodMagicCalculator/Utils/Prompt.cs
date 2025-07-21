namespace BloodMagicCalculator.Utils
{
    internal static class Prompt
    {
        public static TEnum PromptEnum<TEnum>(string prompt) where TEnum : Enum
        {
            while (true)
            {
                WriteColour(prompt, ConsoleColor.Green);
                foreach (var val in Enum.GetValues(typeof(TEnum)))
                {
                    Console.WriteLine($"[{(int)val}] {val}");
                }

                Console.Write("Enter number: ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int num) &&
                    Enum.IsDefined(typeof(TEnum), num))
                {
                    return (TEnum)(object)num;
                }

                Console.WriteLine("Invalid input. Please enter a valid number.\n");
            }
        }

        public static int PromptInt(string prompt, int min, int max)
        {
            while (true)
            {
                WriteColour(prompt + " ", ConsoleColor.Green);
                var input = Console.ReadLine();

                if (int.TryParse(input, out int value) &&
                    value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.\n");
            }
        }

        public static int PromptInt(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                WriteColour(prompt + " ", ConsoleColor.Green);
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input) && allowEmpty)
                {
                    return 0;
                }

                if (int.TryParse(input, out int value) && value >= 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid input. Please enter a non-negative number.\n");
            }
        }

        public static void WriteColour(string text, ConsoleColor colour)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
