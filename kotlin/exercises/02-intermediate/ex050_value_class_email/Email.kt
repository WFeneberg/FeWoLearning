package fewolearning.exercises.intermediate.ex050_value_class_email

/*
Exercise 050 - Value class email (intermediate).

Goal:   Wrap a validated email string in a zero-overhead value class.
Drills: value classes, validation.
*/
@JvmInline
value class Email(val raw: String) {
    init {
        TODO()
    }
}
