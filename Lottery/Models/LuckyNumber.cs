namespace Lottery.Models
{
    public class LuckyNumber
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateOnly Date { get; set; }


        public LuckyNumber() { }

        public LuckyNumber(int num, DateOnly date)
        {
            this.Number = num;
            this.Date = date;
        }
    }
}
