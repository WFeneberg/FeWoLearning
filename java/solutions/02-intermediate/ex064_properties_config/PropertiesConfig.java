package fewolearning.exercises.intermediate.ex064_properties_config;

import java.io.IOException;
import java.io.Reader;
import java.util.Properties;

/*
Exercise 064 - Properties config (reference solution).
*/
public final class PropertiesConfig {
    private PropertiesConfig() {
    }

    public static Properties load(Reader source) throws IOException {
        Properties properties = new Properties();
        properties.load(source);
        return properties;
    }

    public static String getOrDefault(Properties properties, String key, String defaultValue) {
        return properties.getProperty(key, defaultValue);
    }
}
