package fewolearning.exercises.expert.ex099_tiny_template_engine;

import java.util.Map;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class TinyTemplateEngineTest {

    @Test
    void substitutesEveryPlaceholderPresentInTheContext() {
        String template = "Hello {{name}}, you have {{count}} messages.";
        Map<String, String> context = Map.of("name", "Alice", "count", "3");

        assertEquals("Hello Alice, you have 3 messages.", TinyTemplateEngine.render(template, context));
    }

    @Test
    void leavesAPlaceholderForAMissingKeyUnresolvedInsteadOfThrowing() {
        String template = "Hello {{name}}, your balance is {{balance}}.";
        Map<String, String> context = Map.of("name", "Bob");

        assertEquals("Hello Bob, your balance is {{balance}}.", TinyTemplateEngine.render(template, context));
    }

    @Test
    void aTemplateWithNoPlaceholdersIsReturnedUnchanged() {
        String template = "No placeholders here.";

        assertEquals("No placeholders here.", TinyTemplateEngine.render(template, Map.of()));
    }

    @Test
    void anUnterminatedPlaceholderIsCopiedThroughLiterally() {
        String template = "Broken {{name here";
        Map<String, String> context = Map.of("name", "Alice");

        assertEquals("Broken {{name here", TinyTemplateEngine.render(template, context));
    }
}
