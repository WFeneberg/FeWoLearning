namespace FeWoLearning.Exercises.Beginner;

// Exercise 009 — Grade Classifier (reference solution).
public static class GradeClassifier
{
    public static string Classify(int score) => score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F",
    };
}
