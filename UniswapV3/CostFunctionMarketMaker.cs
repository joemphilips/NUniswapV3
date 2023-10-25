
namespace UniswapV3;
using System.Numerics;
using System;

/*
interface ICostFunctionMarketMaker<T>
where T : INumberBase<T>, IComparisonOperators<T,T,bool>
{
    T[] TotalSecurities();

    T CostFunction();

    T MinimalPurchaseAmount();
    
    T PriceForPurchase(T[] purchaseVector);
    T PriceForShowing(uint securityIndex);

    public T[] Odds()
    {
        var total = TotalSecurities();
        var sum = total.Aggregate(T.Zero, (current, t) => current + t);
        return total.Select(x => x / sum).ToArray();
    }

    public void Purchase(T[] purchaseVector)
    {
        var allZero = true;
        foreach (var p in purchaseVector)
        {
            if (allZero)
            {
                allZero = T.Abs(p) < MinimalPurchaseAmount();
            }

            if (T.IsInfinity(p))
            {
                throw new ArgumentException("purchaseVector has Inf");
            }

            if (T.IsNaN(p))
            {
                throw new ArgumentException("purchaseVector has NaN");
            }
            if (!T.IsCanonical(p))
            {
                throw new ArgumentException("Non canonical purchase");
            }
        }
        if (allZero)
            throw new ArgumentException("purchaseVector must have at least one non-zero purchase");

        var totalSecurities = TotalSecurities();
        if (totalSecurities.Length != purchaseVector.Length)
        {
            throw new ArgumentException("invalidLength purchaseVector");
        }

        foreach (var (s, p) in totalSecurities.Zip(purchaseVector))
        {
            s += p;
        }
    }
}

public record LogarithmicScoringMarketMaker<T> : ICostFunctionMarketMaker<T>
where T: INumber<T>
{
    private readonly T[] Securities;
    public T MinimalPurchaseAmount() => T.CreateTruncating(0.000001);
    public LogarithmicScoringMarketMaker(int numSecurities)
    {
        Securities = new T[numSecurities];
    }

    public T[] TotalSecurities() => Securities;

    public T CostFunction()
    {
        throw new NotImplementedException();
    }

    private T CostFunctionMd(T[] totalSecurity, T b)
    {
        return b * totalSecurity.Select(q => System.Math.E ^ (q / b))
    ;

    public T PriceForPurchase(T[] purchaseVector)
    {
        var totalSecurityAfter = new T[TotalSecurities().Length];
        
        return CostFunction() - 
    }

    public T PriceForShowing(uint securityIndex)
    {
        throw new NotImplementedException();
    }
}
*/