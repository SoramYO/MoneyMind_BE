using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Utils
{
    public enum IntentType
    {
        Unknown,                // Khi không xác định được ý định

        // --- Đưa ra lời khuyên / tư vấn ---
        AskSpendingAdvice,      // Xin lời khuyên về chi tiêu (Ví dụ: cắt giảm chi tiêu ở đâu?)
        AskSavingAdvice,        // Xin lời khuyên về tiết kiệm (Ví dụ: dành bao nhiêu % để tiết kiệm?)
        AskBudgetAdvice,        // Xin lời khuyên về lập ngân sách
        AskGoalAdvice,          // Xin lời khuyên về cách đặt mục tiêu chi tiêu
        ComparePeriodMonthlyGoal,// So sánh mục tiêu hàng tháng
        CompareTransaction3DayLatest, // So sánh giao dịch tuần này với tuần trước
    }
}
