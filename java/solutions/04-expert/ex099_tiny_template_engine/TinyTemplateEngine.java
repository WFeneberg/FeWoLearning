package fewolearning.exercises.expert.ex099_tiny_template_engine;

import java.util.Map;

/*
Exercise 099 - Tiny template engine (reference solution).

Missing keys are left unresolved: the literal "{{key}}" placeholder text is
copied through untouched rather than throwing or substituting an empty
string, so a partially-populated context still renders safely. An
unterminated "{{" with no matching "}}" is likewise copied through as-is.
*/
public final class TinyTemplateEngine {
    private static final String OPEN = "{{";
    private static final String CLOSE = "}}";

    private TinyTemplateEngine() {
    }

    public static String render(String template, Map<String, String> context) {
        StringBuilder result = new StringBuilder();
        int position = 0;
        while (position < template.length()) {
            int openIndex = template.indexOf(OPEN, position);
            if (openIndex == -1) {
                result.append(template, position, template.length());
                break;
            }
            int closeIndex = template.indexOf(CLOSE, openIndex + OPEN.length());
            if (closeIndex == -1) {
                result.append(template, position, template.length());
                break;
            }
            result.append(template, position, openIndex);
            String key = template.substring(openIndex + OPEN.length(), closeIndex);
            String value = context.get(key);
            if (value != null) {
                result.append(value);
            } else {
                result.append(template, openIndex, closeIndex + CLOSE.length());
            }
            position = closeIndex + CLOSE.length();
        }
        return result.toString();
    }
}
