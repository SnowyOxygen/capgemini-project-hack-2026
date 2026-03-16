using System;

class Program
{
    static void Main()
    {
        string password = "capadmin";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        Console.WriteLine($"Hashed password: {hashedPassword}");
    }
}
