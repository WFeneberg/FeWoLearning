package fewolearning.exercises.beginner.ex012_default_named_args

/*
Exercise 012 - Default and named arguments (reference solution).
*/
fun formatGreeting(name: String, title: String = "Ms./Mr.", punctuation: String = "."): String =
    "Hello, $title $name$punctuation"
