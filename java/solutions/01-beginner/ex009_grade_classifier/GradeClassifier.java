package fewolearning.exercises.beginner.ex009_grade_classifier;

/*
Exercise 009 - Grade classifier (reference solution).
*/
public final class GradeClassifier {
    private GradeClassifier() {
    }

    public static String classify(int score) {
        if (score < 0 || score > 100) {
            throw new IllegalArgumentException("score must be between 0 and 100: " + score);
        }
        if (score >= 90) {
            return "A";
        } else if (score >= 80) {
            return "B";
        } else if (score >= 70) {
            return "C";
        } else if (score >= 60) {
            return "D";
        } else {
            return "F";
        }
    }

    public static boolean isPassing(int score) {
        return score >= 60;
    }
}
