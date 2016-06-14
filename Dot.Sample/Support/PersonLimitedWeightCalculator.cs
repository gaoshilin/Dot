using Dot.LoadBalance.Weight;

namespace Dot.Sample.Support
{
    public class PersonLimitedWeightCalculator : LimitedWeightCalculator<Person>
    {
        public PersonLimitedWeightCalculator(int limit)
            : base(limit)
        {
        }

        protected override int DoCalculate(Person item)
        {
            return item.Weight;
        }
    }
}