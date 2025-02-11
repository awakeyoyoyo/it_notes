// public class EmptyClass
// { }

// public class ClassNameExample
// {
//     public static void Main()
//     {
//         EmptyClass sc = new EmptyClass();
//         Console.WriteLine(sc.ToString());
//     }
// }
// The example displays the following output:
//        EmptyClass

// var xs = new int[] { 1, 2, 7, 9 };
// var ys = new int[] { 7, 9, 12 };
// Console.WriteLine($"Find {{{string.Join(", ",xs)}}} .");

// Console.WriteLine($"Find the intersection of the {{{string.Join(", ",xs)}}} and {{{string.Join(", ",ys)}}} sets.");
// // Output:
// // Find the intersection of the {1, 2, 7, 9} and {7, 9, 12} sets.

// var userName = "Jane";
// var stringWithEscapes = $"C:\\Users\\{userName}\\Documents";
// var verbatimInterpolated = @$"C:\Users\{userName}\Documents";
// Console.WriteLine(stringWithEscapes);
// Console.WriteLine(verbatimInterpolated);
// // Output:
// // C:\Users\Jane\Documents
// // C:\Users\Jane\Documents


// var cultures = new System.Globalization.CultureInfo[]
// {
//     System.Globalization.CultureInfo.GetCultureInfo("en-US"),
//     System.Globalization.CultureInfo.GetCultureInfo("en-GB"),
//     System.Globalization.CultureInfo.GetCultureInfo("nl-NL"),
//     System.Globalization.CultureInfo.InvariantCulture
// };
// var date = DateTime.Now;
// var number = 31_415_926.536;
// foreach (var culture in cultures)
// {
//     var cultureSpecificMessage = string.Create(culture, $"{date,23}{number,20:N3}");
//     Console.WriteLine($"{culture.Name,-10}{cultureSpecificMessage}");
// }
// Output is similar to:
// en-US       8/27/2023 12:35:31 PM      31,415,926.536
// en-GB         27/08/2023 12:35:31      31,415,926.536
// nl-NL         27-08-2023 12:35:31      31.415.926,536
//               08/27/2023 12:35:31      31,415,926.536

namespace TeleprompterConsole;

internal class Program
{
    //     static void Main(string[] args)
    //     {
    //         Console.WriteLine("Hello World!");
    //         IEnumerable<string> lines = ReadFrom("sampleQuotes.txt");

    //         foreach (String line in lines)
    //         {
    //             Console.Write(line);
    //             if (!string.IsNullOrWhiteSpace(line))
    //             {
    //                 var pause = Task.Delay(200);
    //                 // Synchronously waiting on a task is an
    //                 // anti-pattern. This will get fixed in later
    //                 // steps.
    //                 pause.Wait();
    //             }
    //         }
    //     }

    static IEnumerable<string> ReadFrom(string file)
    {
        string? line;
        using (var reader = File.OpenText(file))
        {
            while ((line = reader.ReadLine()) != null)
            {
                int lineLength = 0;
                var words = line.Split(' ');
                foreach (var word in words)
                {
                    lineLength += word.Length + 1;
                    if (lineLength > 70)
                    {
                        yield return Environment.NewLine;
                        lineLength = 0;
                    }
                    yield return word + " ";
                }
                yield return Environment.NewLine;

            }
        }
    }

    static async Task Main(string[] args)
    {
        await RunTeleprompter();
    }


    private static async Task RunTeleprompter()
    {
        var config = new TelePrompterConfig();
        var displayTask = ShowTeleprompter(config);

        var speedTask = GetInput(config);
        // await speedTask;
        await Task.WhenAny(displayTask, speedTask);
    }

    private static async Task ShowTeleprompter(TelePrompterConfig config)
    {
        var words = ReadFrom("sampleQuotes.txt");
        foreach (var word in words)
        {
            Console.Write(word);
            if (!string.IsNullOrWhiteSpace(word))
            {
                await Task.Delay(config.DelayInMilliseconds);
            }
        }
        config.SetDone();
    }

    private static async Task GetInput(TelePrompterConfig config)
    {
        Action work = () =>
        {
            do
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == '>')
                    config.UpdateDelay(-10);
                else if (key.KeyChar == '<')
                    config.UpdateDelay(10);
                else if (key.KeyChar == 'X' || key.KeyChar == 'x')  
                    config.SetDone();
            } while (!config.Done);
        };
        await Task.Run(work);
    }

    private static async Task ShowTeleprompter()
    {
        var words = ReadFrom("sampleQuotes.txt");
        foreach (var word in words)
        {
            Console.Write(word);
            if (!string.IsNullOrWhiteSpace(word))
            {
                await Task.Delay(200);
            }
        }
    }



}

