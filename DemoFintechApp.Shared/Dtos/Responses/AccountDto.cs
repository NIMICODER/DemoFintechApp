using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Shared.Dtos.Responses
{
	public class AccountDto
    {
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }

}
