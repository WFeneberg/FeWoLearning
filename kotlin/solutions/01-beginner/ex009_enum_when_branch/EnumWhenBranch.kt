package fewolearning.exercises.beginner.ex009_enum_when_branch

/*
Exercise 009 - Enum when branch (reference solution).
*/
enum class TrafficLight { RED, YELLOW, GREEN }

fun actionFor(light: TrafficLight): String = when (light) {
    TrafficLight.RED -> "Stop"
    TrafficLight.YELLOW -> "Caution"
    TrafficLight.GREEN -> "Go"
}
