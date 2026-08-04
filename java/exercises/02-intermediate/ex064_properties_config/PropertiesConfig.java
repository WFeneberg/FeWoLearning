package fewolearning.exercises.intermediate.ex064_properties_config;

import java.io.IOException;
import java.io.Reader;
import java.util.Properties;

/*
Exercise 064 - Properties config (intermediate).

Goal:   Load key/value configuration from a Properties source with a default fallback.
Drills: Properties, config loading.
*/
public final class PropertiesConfig {
    private PropertiesConfig() {
    }

    public static Properties load(Reader source) throws IOException {
        throw new UnsupportedOperationException("TODO");
    }

    public static String getOrDefault(Properties properties, String key, String defaultValue) {
        throw new UnsupportedOperationException("TODO");
    }
}
