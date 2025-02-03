using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{
    public class TransactionTag
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Properties related 
        /// </summary>
        public Guid TagId { get; set; }
        public Guid TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }    
        public virtual Tag Tag { get; set; }

    }
}
