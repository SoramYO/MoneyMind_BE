namespace MoneyMind_BLL.DTOs.MBBank
{
    public class MBBankTransactionRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountNumber { get; set; }
        public int Days { get; set; }
    }
}