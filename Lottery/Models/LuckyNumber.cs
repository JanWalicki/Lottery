using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Models
{
    public class LuckyNumber
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateOnly Date { get; set; }
    }
}
