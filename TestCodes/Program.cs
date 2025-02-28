using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TestCommission
{
    public static void Main()
    {
        PremiumCalculateService service = new PremiumCalculateService();
        Console.WriteLine("Select a calculation function to test: ");
        Console.WriteLine("1. Agent Commission Calculation");
        Console.WriteLine("2. Claim Amount Calculation");
        Console.WriteLine("3. Surrender Claim");
        Console.WriteLine("4. Loan Claim");
        Console.WriteLine("5. PaidUp Claim");
        Console.WriteLine("6. Outstanding Payments Calculation");
        Console.WriteLine("7. Endorsement Calculation");
        Console.WriteLine("8. Pro-rata Rate Calculation");
        string option = Console.ReadLine();

        switch (option)
        {
            case "1":

                Console.WriteLine("Enter premium amount");
                decimal premiumAmount = Decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter policy start date: ");
                DateTime policyStartDate = DateTime.Parse(Console.ReadLine());
                bool isTerm = false;
                Console.WriteLine("Is the policy a one-year policy? (Yes/No)");
                var ans = Console.ReadLine();
                if (ans.ToLower() == "yes"){
                    isTerm = true;
                }
                else{
                    isTerm = false;
                }
                // Agent Commission Calculation
                TblPayment mockPayment = new TblPayment
                {
                    PremiumAmount = premiumAmount,
                    Proposal = new TblProposal
                    {
                        Product = new TblProduct
                        {
                            Commission = "{\"everyYear\":\"0.15\"}"
                        }
                    },
                    Policy = new TblPolicy
                    {
                        StartAt = policyStartDate,
                        Product = new TblProduct
                        {
                            Commission = isTerm ? "{\"everyYear\":\"0.15\"}" : "{\"firstYear\":\"0.15\", \"remainingYear\":\"0.03\"}"
                        }
                    }
                };
                decimal commissionAmount = service.ComputeAgentComission(mockPayment);
                Console.WriteLine($"Calculated Commission Amount: {commissionAmount}");
                break;

            case "2":
                // Claim Amount Calculation
                Console.WriteLine("Enter claim type (e.g., Death, Maturity, Injury, etc.):");
                string claimType = Console.ReadLine();

                Console.WriteLine("Enter Sum Insured Amount or Units Purchased:");
                decimal siAmount = decimal.Parse(Console.ReadLine());

                bool isDecreasing = false;
                bool isProsper = false;
                double rate = 0;
                int daysOut = 0;

                // Collect additional inputs based on claim type
                switch (claimType)
                {
                    case "Death":
                        Console.WriteLine("Is it a decreasing policy? (yes/no):");
                        isDecreasing = Console.ReadLine().Trim().ToLower() == "yes";
                        if (isDecreasing)
                        {
                            Console.WriteLine("Enter rate (percentage):");
                            rate = double.Parse(Console.ReadLine());
                        }
                        break;

                    case "Maturity":
                        Console.WriteLine("Enter rate (e.g., 0.05 for 5%):");
                        rate = double.Parse(Console.ReadLine());
                        break;

                    case "Injury":
                        Console.WriteLine("Is it a Prosper policy? (yes/no):");
                        isProsper = Console.ReadLine().Trim().ToLower() == "yes";
                        if (!isProsper)
                        {
                            Console.WriteLine("Enter rate (e.g., 0.05 for 5%):");
                            rate = double.Parse(Console.ReadLine());
                        }
                        break;

                    case "Hospitalization":
                    case "UnableToWork":
                        Console.WriteLine("Enter the number of days out:");
                        daysOut = int.Parse(Console.ReadLine());
                        break;

                    case "Surgery":
                        // No additional input required for Surgery
                        break;

                    case "Miscarriage":
                        // No additional input required for Miscarriage
                        break;

                    case "OPD":
                        // No additional input required for OPD
                        break;

                    case "CriticalIllness":
                        // No additional input required for Critical Illness
                        break;

                    default:
                        Console.WriteLine($"Invalid claim type: {claimType}");
                        return;
                }

                // Compute claim amount
                decimal claimAmount = service.ComputeClaimAmount(claimType, siAmount, isDecreasing, isProsper, rate, daysOut);
                Console.WriteLine($"Calculated Claim Amount: {claimAmount}");
                break;


            case "3":
                // Surrender Claim
                Console.WriteLine("Enter Surrender Rate:");
                double surrenderRate = double.Parse(Console.ReadLine());
                Console.WriteLine("Enter Sum Insured Amount:");
                decimal surrenderSiAmount = decimal.Parse(Console.ReadLine());
                decimal surrenderAmount = service.ComputeSurrenderAmount(surrenderRate, surrenderSiAmount);
                Console.WriteLine($"Calculated Surrender Amount: {surrenderAmount}");
                break;

            case "4":
                // Loan Claim
                Console.WriteLine("Enter Surrender Amount:");
                decimal surrenderValue = decimal.Parse(Console.ReadLine());
                decimal eligibleLoanAmount = service.ComputeEligibleLoanAmount(surrenderValue);
                Console.WriteLine($"Eligible Loan Amount: {eligibleLoanAmount}");
                break;

            case "5":
                // Paid-Up Amount Calculation
                Console.WriteLine("Enter Sum Insured Amount:");
                decimal si = decimal.Parse(Console.ReadLine());

                Console.WriteLine("Enter Premium Paid:");
                decimal premiumPaid = decimal.Parse(Console.ReadLine());

                Console.WriteLine("Enter Paid Months:");
                int paidMonths = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter Policy Term (in days):");
                int policyTerm = int.Parse(Console.ReadLine());

                Console.WriteLine("Enter Maturity Amount (optional, press Enter for default):");
                string maturityInput = Console.ReadLine();
                decimal? maturity = string.IsNullOrEmpty(maturityInput) ? (decimal?)null : decimal.Parse(maturityInput);

                ProductEnum.PaymentMethod? paymentMethod = null;

                // Collect additional inputs based on payment method
                if (maturity is not null)
                {
                    Console.WriteLine("Enter Payment Method (biYearly or monthly):");
                    string paymentMethodInput = Console.ReadLine().Trim();
                    paymentMethod = paymentMethodInput switch
                    {
                        "biYearly" => ProductEnum.PaymentMethod.biYearly,
                        "monthly" => ProductEnum.PaymentMethod.monthly,
                        _ => throw new InvalidOperationException("Invalid payment method")
                    };
                }

                // Compute paid-up amount
                decimal paidUpAmount = service.ComputePaidUpAmount(si, premiumPaid, paidMonths, policyTerm, maturity, paymentMethod);
                Console.WriteLine($"Calculated Paid-Up Amount: {paidUpAmount}");
                break;

            case "6":
                // Outstanding Payments Calculation
                Console.WriteLine("Enter Premium Amount:");
                decimal premiumAmount1 = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter Interest Rate (as a decimal, e.g., 0.05 for 5%):");
                decimal interestRate = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter Outstanding Months:");
                int outstandingMonths = int.Parse(Console.ReadLine());
                decimal outstandingPayments = service.ComputeOutstandingPremiumRepayments(premiumAmount1, interestRate, outstandingMonths);
                Console.WriteLine($"Outstanding Payments with Interest: {outstandingPayments}");
                break;

            case "7":
                // Endorsement Calculation
                Console.WriteLine("Enter Old Premium:");
                decimal oldPremium = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter New Premium:");
                decimal newPremium = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Enter Premium Term:");
                int premiumTerm = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Premium Paid:");
                int premiumPaidMonths = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter Interest Rate (as a decimal):");
                decimal endorsementInterestRate = decimal.Parse(Console.ReadLine());
                EndorsementModel endorsement = PremiumCalculateService.ComputeEndorsementAmount(oldPremium, newPremium, premiumTerm, premiumPaidMonths, endorsementInterestRate, 1);
                Console.WriteLine($"Endorsement Fee: {endorsement.EndorsementFee}");
                Console.WriteLine($"Index already Paid Term: {endorsement.IndexPaid}");
                Console.WriteLine($"Index to Pay Endorsement Fee: {endorsement.IndexToPayEndorsementFee}");
                Console.WriteLine($"Extra Premium to Pay (if any): {endorsement.DVInterest}");
                break;

            case "8":
                // Pro-rata Rate Calculation
                Console.WriteLine("Enter Actual Amount:");
                decimal actualAmount = decimal.Parse(Console.ReadLine());
                var (newAmount, adjustmentAmount) = PremiumCalculateService.ComputeAdjustmentAmount(actualAmount);
                Console.WriteLine($"New Amount: {newAmount}");
                Console.WriteLine($"Adjustment Amount: {adjustmentAmount}");
                break;

            default:
                Console.WriteLine("Invalid option selected.");
                break;
        }
    }
}


// Mock classes to mimic the database structure
public class TblPayment
{
    public decimal PremiumAmount { get; set; }
    public TblProposal? Proposal { get; set; }
    public TblPolicy? Policy { get; set; }
}

public class TblProposal
{
    public TblProduct? Product { get; set; }
}

public class TblPolicy
{
    public DateTime StartAt { get; set; }
    public TblProduct? Product { get; set; }
}

public class TblProduct
{
    public string? Commission { get; set; }
}

public class EndorsementModel
{
    public required long PolicyId { get; set; }
    public required long IndexPaid { get; set; }
    public required decimal EndorsementFee { get; set; }
    public required int IndexToPayEndorsementFee { get; set; }
    public decimal? DVInterest { get; set; }
}

public static class DurationUtils
{
    public static DateTime GetEndDate(int interval, DateTime startDate = default)
    {
        if (startDate == default)
        {
            startDate = DateTime.UtcNow;
        }
        DateTime endDate = startDate;

        // Convert the interval to months by dividing by 30
        double standardIntervalInMonths = interval / 30.0;

        // Handle intervals less than a month (i.e., in days)
        if (standardIntervalInMonths < 1)
        {
            endDate = startDate.AddDays(interval);
        }
        else
        {
            // Calculate months and years based on standard interval
            int months = (int)standardIntervalInMonths;
            int years = months / 12;
            int remainingMonths = months % 12;
            int extraDays = interval % 30; // Remaining days after dividing by 30

            if (years > 0)
            {
                endDate = startDate.AddYears(years);
            }

            if (remainingMonths > 0)
            {
                endDate = endDate.AddMonths(remainingMonths);
            }

            if (extraDays > 0)
            {
                endDate = endDate.AddDays(extraDays);
            }
        }

        return endDate;
    }
}

public class PremiumCalculateService
{
    public decimal ComputeAgentComission(TblPayment payment)
    {
        string commission;
        bool isFirstYear;
        double commissionPercentage;
        decimal commissionAmount;
        if (payment.Policy is null)
        {
            commission = payment.Proposal!.Product!.Commission!;
            isFirstYear = true;
        }
        else
        {
            commission = payment.Policy!.Product!.Commission!;
            DateTime firstYearEnd = DurationUtils.GetEndDate(360, payment.Policy!.StartAt);
            Console.WriteLine($"First year end: {firstYearEnd}");
            isFirstYear = firstYearEnd > DateTime.UtcNow;
        }
        JObject commissionJson = JObject.Parse(commission);
        Console.WriteLine(isFirstYear);
        if (commissionJson.ContainsKey("everyYear"))
        {
            commissionPercentage = commissionJson["everyYear"]!.Value<double>();
            commissionAmount = payment.PremiumAmount * (decimal)commissionPercentage;
            Console.WriteLine(commissionJson);
        }
        else
        {
            if (isFirstYear)
            {
                commissionPercentage = commissionJson["firstYear"]!.Value<double>();
                commissionAmount = payment.PremiumAmount * (decimal)commissionPercentage;
                Console.WriteLine(commissionJson);
            }
            else
            {
                commissionPercentage = commissionJson["remainingYear"]!.Value<double>();
                commissionAmount = payment.PremiumAmount * (decimal)commissionPercentage;
                Console.WriteLine(commissionJson);
            }
        }
        return commissionAmount;
    }

    public decimal ComputeSurrenderAmount(double surrenderRate, decimal siAmount, double? puv = null, double? svf = null, double? mdf = null, decimal? surrenderCharge = null)
    {
        decimal surrenderValue;
        if (puv is null || svf is null || mdf is null)
        {
            surrenderValue = (decimal)surrenderRate * siAmount / 1000;
        }
        else
        {
            surrenderValue = (decimal)((decimal)(puv * svf * mdf) * (1 - surrenderCharge))!;
        }
        return surrenderValue;
    }

    public decimal ComputeEligibleLoanAmount(decimal surrenderAmount)
    {
        return surrenderAmount * (decimal)0.9;
    }

    public decimal ComputePaidUpAmount(decimal siAmount, decimal premiumPaid, int paidMonths, int policyTerm, decimal? maturity = 0, ProductEnum.PaymentMethod? paymentMethod = 0)
    {
        decimal amount;
        var policyTermMonths = policyTerm / 30;
        if (maturity is not null)
        {
            if (paymentMethod == ProductEnum.PaymentMethod.biYearly)
            {
                amount = (decimal)((decimal)0.5 * maturity);
            }
            else if (paymentMethod == ProductEnum.PaymentMethod.monthly)
            {
                amount = (decimal)((paidMonths - 1) / 12 * maturity);
            }
            else
            {
                throw new InvalidOperationException("Payment method not allowed");
            }
        }
        else
        {
            amount = siAmount * paidMonths / policyTermMonths;
        }
        return amount;
    }

    public decimal ComputeActualClaimAmount(decimal claimAmount, decimal loanRepayment = 0, decimal outstandingPremiums = 0)
    {
        decimal actualClaimAmount = claimAmount - (outstandingPremiums + loanRepayment)!;
        return actualClaimAmount;
    }

    public decimal ComputeOutstandingPremiumRepayments(decimal premiumAmount, decimal interestRate, int outstandingMonths)
    {
        decimal interest = 0;

        for (int i = outstandingMonths; i > 0; i--)
        {
            interest += premiumAmount * i * interestRate / 12;
        }

        // Total repayment = principal (premiumAmount * months) + interest
        return (premiumAmount * outstandingMonths) + interest;
    }

    public decimal ComputeOutstandingPremiumRepaymentsWithoutInterest(decimal premiumAmount, int outstandingMonths)
    {
        // Total repayment = principal (premiumAmount * months) + interest
        return premiumAmount * outstandingMonths;
    }
    public decimal ComputeClaimAmount(string claimType, decimal siAmount, bool isDecreasing = false, bool isProsper = false, double rate = 0, int daysOut = 0, int requestedAmount = 0)
    {
        decimal claimAmount = claimType switch
        {
            "Death" => isDecreasing && rate > 0 ? (decimal)rate * siAmount / 1000 : siAmount,
            "Maturity" => siAmount * (decimal)rate,
            "Injury" => isProsper ? (decimal)rate : siAmount * (decimal)rate,
            "Hospitalization" or "UnableToWork" => daysOut * siAmount,
            "Surgery" => requestedAmount > (500000 * siAmount) ? 500000 * siAmount : (requestedAmount < 500000 ? 500000 : requestedAmount),
            "Miscarriage" => 300000,
            "OPD" => 10000 * siAmount,
            "CriticalIllness" => siAmount,
            _ => throw new InvalidOperationException($"Invalid claim type: {claimType}")
        };

        return claimAmount;
    }

    public static EndorsementModel ComputeEndorsementAmount(decimal oldPremium, decimal newPremium, int totalDebitNotes, int debitNotesPaid, decimal interestRate, long policyId)
    {
        {
            decimal premiumDifference = newPremium - oldPremium;
            decimal totalPremiumDifference = Math.Abs(premiumDifference) * debitNotesPaid;
            if (premiumDifference > 0)
            {
                decimal anf = ComputeANFPeriod(debitNotesPaid);
                decimal interest = ComputeDVInterest(premiumDifference, totalDebitNotes - debitNotesPaid, interestRate, anf);
                decimal premiumMonths = totalPremiumDifference / newPremium;
                int remainingMonths = totalDebitNotes - debitNotesPaid;
                if (premiumMonths < remainingMonths)
                {
                    var model = new EndorsementModel
                    {
                        PolicyId = policyId,
                        IndexPaid = debitNotesPaid,
                        EndorsementFee = totalPremiumDifference + interest,
                        IndexToPayEndorsementFee = 0,
                        DVInterest = interest
                    };
                    return model;
                }
                else
                {
                    decimal premiumToPay = newPremium * (int)premiumMonths;
                    decimal remainingPremium = totalPremiumDifference - premiumToPay;
                    var model = new EndorsementModel
                    {
                        PolicyId = policyId,
                        IndexPaid = debitNotesPaid,
                        EndorsementFee = totalPremiumDifference + interest,
                        IndexToPayEndorsementFee = 0,
                    };
                    return model;
                }
            }
            else
            {
                int premiumMonths = (int)(totalPremiumDifference / newPremium);
                decimal premiumToPay = newPremium * premiumMonths;
                decimal remainingPremium = totalPremiumDifference - premiumToPay;
                var model = new EndorsementModel
                {
                    PolicyId = policyId,
                    IndexPaid = debitNotesPaid,
                    EndorsementFee = newPremium - remainingPremium,
                    IndexToPayEndorsementFee = premiumMonths + 1,
                };
                return model;
            }
        }
    }

    public static decimal ComputeANFPeriod(int premiumPaid)
    {
        return premiumPaid * (premiumPaid + 1) / 2;
    }

    public static decimal ComputeDVInterest(decimal premiumDifference, int remainingPremiumTerm, decimal interestRate, decimal anf)
    {
        return premiumDifference * anf * interestRate / 12;
    }

    public static (decimal newAmount, decimal adjustmentAmount) ComputeAdjustmentAmount(decimal actualAmount)
    {
        decimal newAmount = Math.Round(actualAmount / (decimal)100.0) * 100;
        decimal adjustmentAmount = newAmount - actualAmount;
        return (newAmount, adjustmentAmount);
    }

    /// <summary>
    /// Calculate refund rate or credit term expired premium rate based on the given parameters. This function works for both pro-rata refund calculation and pro-rata credit term bad debt calculations.
    /// </summary>
    /// <param name="policyStart">Start date of the policy, retrieved from the policy table</param>
    /// <param name="currentDate">The current date or the data from which to start calculating refund or credit term collection</param>
    /// <param name="policyEndDate">The end date of the policy</param>
    /// <param name="totalPremium">The Gross premium of the policy to be paid or already paid by the customer</param>
    /// <param name="isCreditTermExpired">Flag that separates collection type. Enter true if it is a credit term expired situation, and false if it is a refund calculation.</param>
    /// <returns>Refund or bad debt due amount</returns>
    public static decimal CalculateProRataRate(DateTime policyStart, DateTime currentDate, DateTime policyEndDate, decimal totalPremium, bool isCreditTermExpired)
    {
        (int difference, int totalDays) = CalculateLapsedDate(policyStart, currentDate, policyEndDate, isCreditTermExpired);
        decimal amount = totalPremium * (difference / totalDays);
        return amount;
    }

    private static (int, int) CalculateLapsedDate(DateTime policyStart, DateTime currentDate, DateTime policyEndDate, bool isCreditTermExpired)
    {
        // Ensure that the current date is not earlier than the policy start date
        if (currentDate < policyStart)
        {
            throw new ArgumentException("Current date cannot be earlier than the policy start date.");
        }

        // Calculate the difference in days
        int daysDifference = (currentDate - policyStart).Days;

        int totalDays = (policyEndDate - policyStart).Days;

        //For differentiating between CreditTermExpired payment collection and Policy Cancellation Refund payment
        if (isCreditTermExpired)
        {
            return (daysDifference, totalDays);
        }
        else
        {
            int insuredDays = totalDays - daysDifference;

            return (insuredDays, totalDays);
        }
    }
}

public class ProductEnum
{

    public enum PaymentMethod
    {
        singlePremium = 0, // or lumpsum
        monthly = 1,
        quarterly = 2,
        biYearly = 3,
        yearly = 4
    }

}
