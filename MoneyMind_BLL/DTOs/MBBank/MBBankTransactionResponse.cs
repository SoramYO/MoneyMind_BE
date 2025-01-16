namespace MoneyMind_BLL.DTOs.MBBank
{
    public class MBBankTransactionResponse
    {
        public string RefNo { get; set; }
        public Result Result { get; set; }
        public List<TransactionHistory> TransactionHistoryList { get; set; }
    }

    public class Result
    {
        public bool Ok { get; set; }
        public string Message { get; set; }
        public string ResponseCode { get; set; }
    }

    public class TransactionHistory
    {
        public string PostingDate { get; set; }
        public string TransactionDate { get; set; }
        public string AccountNo { get; set; }
        public string CreditAmount { get; set; }
        public string DebitAmount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string AddDescription { get; set; }
        public string AvailableBalance { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string RefNo { get; set; }
        public string BenAccountName { get; set; }
        public string BankName { get; set; }
        public string BenAccountNo { get; set; }
    }
}