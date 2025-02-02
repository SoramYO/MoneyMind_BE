using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.GoogleSheet
{
public class BankSheetRow
{
    public string Id { get; set; }
    public string Description { get; set; }
    public string Amount { get; set; }
    public string TransactionDate { get; set; }
    public string AccountNumber { get; set; }
    public string ReferenceNumber { get; set; }
    public string Balance { get; set; }
    public string VirtualAccountNumber { get; set; }
    public string VirtualAccountName { get; set; }
    public string CounterAccountNumber { get; set; }
    public string CounterAccountName { get; set; }
    public string BankBinCode { get; set; }
    public string PaymentChannel { get; set; }
} 
}