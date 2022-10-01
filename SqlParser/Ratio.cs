namespace Irvin.SqlParser;

public struct Ratio
{
    private double Value { get; set; }

    public static Ratio Zero => new Ratio() { Value = 0 };

    public static Ratio? Of(double numerator, double denominator, bool ignoreDivideByZero = true)
    {
        if (ignoreDivideByZero && denominator == 0)
        {
            return null;
        }
        
        return new Ratio { Value = numerator / denominator };
    }
}
