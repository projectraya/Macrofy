using Macrofy.Models;

namespace Macrofy.Services;

public class MacroResult
{
    public int Calories { get; set; }
    public int Protein { get; set; }
    public int Carbs { get; set; }
    public int Fat { get; set; }
}

public class MacroCalculatorService
{
    public MacroResult Calculate(
        int age, decimal weightKg, decimal heightCm,
        Gender gender, ActivityLevel activity, FitnessGoal goal)
    {
        // Mifflin-St Jeor BMR
        double bmr = gender == Gender.Male
            ? 10 * (double)weightKg + 6.25 * (double)heightCm - 5 * age + 5
            : 10 * (double)weightKg + 6.25 * (double)heightCm - 5 * age - 161;

        double activityMultiplier = activity switch
        {
            ActivityLevel.Sedentary => 1.2,
            ActivityLevel.Light => 1.375,
            ActivityLevel.Moderate => 1.55,
            ActivityLevel.Active => 1.725,
            ActivityLevel.VeryActive => 1.9,
            _ => 1.55
        };

        double tdee = bmr * activityMultiplier;

        // Adjust for goal
        tdee += goal switch
        {
            FitnessGoal.LoseWeight => -400,
            FitnessGoal.GainMuscle => +300,
            _ => 0
        };

        int calories = (int)Math.Round(tdee);
        int protein = (int)Math.Round((double)weightKg * 2.0);   // 2g per kg
        int fat = (int)Math.Round(calories * 0.25 / 9);           // 25% of calories
        int carbs = (int)Math.Round((calories - protein * 4 - fat * 9) / 4.0);

        return new MacroResult
        {
            Calories = calories,
            Protein = protein,
            Carbs = Math.Max(0, carbs),
            Fat = fat
        };
    }
}
