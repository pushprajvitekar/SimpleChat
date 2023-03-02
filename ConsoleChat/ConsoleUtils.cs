using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleChat
{
    internal class ConsoleUtils
    {
        const string _readPrompt = "chat> ";
        public static string ReadFromConsole(string promptMessage = "")
        {
            // Show a prompt, and get input:
            Console.Write(_readPrompt + promptMessage);
            var inp = Console.ReadLine();
            return inp ?? string.Empty;
        }
        public static void WriteToConsole(string message = "")
        {
            if (message.Length > 0)
            {
                Console.WriteLine(message);
            }
        }
        public static void WriteMessage(string message, string messageBy)
        {
            try
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}:[{messageBy}]: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


    }
}
