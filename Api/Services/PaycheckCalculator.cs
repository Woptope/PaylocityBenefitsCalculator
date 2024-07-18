using Api.Models;

namespace Api.Services
{
    public class PaycheckCalculator
    {
        /// <summary>
        /// Calculates the per-paycheck benefit cost for an employee based on the given requirements.
        /// </summary>
        /// <param name="employee">The employee for whom the paycheck is being calculated.</param>
        /// <returns>The calculated benefit cost per paycheck.</returns>
        public decimal CalculatePaycheck(Employee employee)
        {
            // Constants defined as per the given requirements
            const int paychecksPerYear = 26; // Number of paychecks per year
            const decimal baseCostPerMonth = 1000; // Base cost per month for employee benefits
            const decimal dependentCostPerMonth = 600; // Cost per month for each dependent
            const decimal additionalCostForHighSalary = 0.02m; // Additional cost for high salary
            const decimal additionalCostForElderDependent = 200; // Additional cost for dependents over 50 years old

            // Calculate the total annual benefit cost for the employee
            decimal totalBenefitCostPerYear = baseCostPerMonth * 12; // Initial base cost for the employee

            // Loop through each dependent to add the appropriate costs
            foreach (var dependent in employee.Dependents)
            {
                // Add the cost for each dependent
                totalBenefitCostPerYear += dependentCostPerMonth * 12;

                // If the dependent is over 50 years old, add the additional cost
                if (DateTime.Now.Year - dependent.DateOfBirth.Year > 50)
                {
                    totalBenefitCostPerYear += additionalCostForElderDependent * 12;
                }
            }

            // If the employee's salary is greater than $80,000, add the additional 2% cost of their yearly salary
            if (employee.Salary > 80000)
            {
                totalBenefitCostPerYear += employee.Salary * additionalCostForHighSalary;
            }

            // Calculate the per-paycheck cost by dividing the total annual benefit cost by the number of paychecks per year
            decimal paycheck = totalBenefitCostPerYear / paychecksPerYear;

            // Round the paycheck value to two decimal places for consistency
            return Math.Round(paycheck, 2);
        }
    }
}
