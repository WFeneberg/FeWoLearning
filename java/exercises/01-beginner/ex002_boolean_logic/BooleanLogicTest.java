package fewolearning.exercises.beginner.ex002_boolean_logic;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertTrue;

class BooleanLogicTest {

    @Test
    void adminCanAccessUnlessBanned() {
        assertTrue(BooleanLogic.canAccess(true, false, false));
        assertFalse(BooleanLogic.canAccess(true, false, true));
    }

    @Test
    void ticketHolderCanAccessUnlessBanned() {
        assertTrue(BooleanLogic.canAccess(false, true, false));
        assertFalse(BooleanLogic.canAccess(false, true, true));
    }

    @Test
    void neitherAdminNorTicketHolderCannotAccess() {
        assertFalse(BooleanLogic.canAccess(false, false, false));
    }

    @Test
    void bannedNeverAccessesEvenAsAdminWithATicket() {
        assertFalse(BooleanLogic.canAccess(true, true, true));
    }

    @Test
    void isInRangeAcceptsInclusiveBounds() {
        assertTrue(BooleanLogic.isInRange(5, 1, 10));
        assertTrue(BooleanLogic.isInRange(1, 1, 10));
        assertTrue(BooleanLogic.isInRange(10, 1, 10));
    }

    @Test
    void isInRangeRejectsValuesOutsideBounds() {
        assertFalse(BooleanLogic.isInRange(0, 1, 10));
        assertFalse(BooleanLogic.isInRange(11, 1, 10));
    }
}
