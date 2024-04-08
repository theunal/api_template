using Core.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Core.Helpers
{
    public static class GeneralStaticHelper
    {
        public static IDataProtector DataProtector { get; set; } = null!;
        public static string DataProtectionKey { get; set; } = null!;
        public static byte[] DataProtectionSalt { get; set; } = null!;

        public static string CreateGuid(int quantity = 1) => string.Join("", Enumerable.Range(0, quantity).Select(_ => Guid.NewGuid()));

        public static void LogWrite(string? message, LogType? type = null, Exception? exception = null)
        {
            if (string.IsNullOrEmpty(message)) return;

            if (exception is not null)
                message = $"{message} \nsource: {exception.Source}\nmessage: {exception.Message}\nexception: {exception.InnerException}\nstack trace: {exception.StackTrace}\n\n";

            Console.WriteLine($"\n{(type is not null ? $"[{type}]" : "[INFO]")} - {DateTime.Now.ToString("G")} {message}");
        }
    }
}
