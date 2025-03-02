using Lottery.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Services
{
    public class LuckyNumberService
    {
        private FileService dbService = new FileService();

        public int GetLuckyNumber()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            LuckyNumber dbLuckyNumber = dbService.GetLuckyNumberByDate(today);

            if (dbLuckyNumber == null)
                return GenerateLuckyNumber(today);
            else
                return dbLuckyNumber.Number;
        }

        public int GenerateLuckyNumber(DateOnly today)
        {
            List<Class> Classes = new List<Class>(dbService.GetAllClasses().OrderBy(c => c.Name).ToList());
            if (Classes.Count <= 0)
                return 0;

            int max = Classes.Max(c => c.Students.Count);
            if (max <= 0) return 0;
            Random random = new Random();
            int luckyNumber = random.Next(1, max);

            dbService.AddLuckyNumber(luckyNumber, today);
            return luckyNumber;
        }
    }
}
