package fewolearning.exercises.intermediate.ex053_comparator_then_by

data class Employee(val department: String, val name: String, val salary: Double)

/** Sorts employees by department, then by salary descending within each department. */
fun sortEmployees(employees: List<Employee>): List<Employee> =
    employees.sortedWith(compareBy<Employee> { it.department }.thenByDescending { it.salary })
