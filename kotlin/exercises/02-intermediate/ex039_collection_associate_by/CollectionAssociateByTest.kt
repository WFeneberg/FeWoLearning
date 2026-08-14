package fewolearning.exercises.intermediate.ex039_collection_associate_by

import org.junit.jupiter.api.Test
import org.junit.jupiter.api.Assertions.assertEquals

class CollectionAssociateByTest {

    @Test
    fun indexesEachPersonByItsId() {
        val people = listOf(Person(1, "Ada"), Person(2, "Bo"))

        val result = indexById(people)

        assertEquals(mapOf(1 to Person(1, "Ada"), 2 to Person(2, "Bo")), result)
    }

    @Test
    fun keepsTheLastPersonWhenIdsCollide() {
        val people = listOf(Person(1, "Ada"), Person(1, "Ada-replacement"))

        val result = indexById(people)

        assertEquals("Ada-replacement", result.getValue(1).name)
        assertEquals(1, result.size)
    }
}
