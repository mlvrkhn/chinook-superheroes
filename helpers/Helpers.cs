namespace ChinookApp.Helpers
{
    public static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Displays help information about available commands.
        /// </summary>
        public static void ShowHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("  init - Initialize the database");
            Console.WriteLine("  add - Add a new customer");
            Console.WriteLine("  update - Update an existing customer");
            Console.WriteLine("  delete - Delete a customer");
            Console.WriteLine("  list - List all customers");
        }

    }
}
