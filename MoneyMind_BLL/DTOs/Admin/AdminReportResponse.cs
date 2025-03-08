namespace MoneyMind_BLL.DTOs.Admin
{
    public class AdminReportResponse
    {
        public int TotalUsers { get; set; }
        public int TotalTransactions { get; set; }
        public Dictionary<string, double> ExpensesByCategory { get; set; }

        public AdminReportResponse()
        {
            ExpensesByCategory = new Dictionary<string, double>();
        }
    }
} 