package fewolearning.exercises.intermediate.ex053_comparator_then_by

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class EmployeeTest {

    @Test
    fun sortsByDepartmentThenBySalaryDescending() {
        val employees = listOf(
            Employee("eng", "Ada", 90_000.0),
            Employee("eng", "Bo", 120_000.0),
            Employee("sales", "Cy", 80_000.0)
        )

        val sorted = sortEmployees(employees)

        assertEquals(listOf("Bo", "Ada", "Cy"), sorted.map { it.name })
    }
}
