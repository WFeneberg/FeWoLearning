package fewolearning.exercises.beginner.ex017_lambda_capture

/*
Exercise 017 - Lambda capture (reference solution).
*/
fun makeCounter(): () -> Int {
    var count = 0
    return {
        count += 1
        count
    }
}
