package fewolearning.exercises.intermediate.ex064_properties_config;

import java.io.StringReader;
import java.util.Properties;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class PropertiesConfigTest {

    @Test
    void loadReadsKeyValuePairsFromTheSource() throws Exception {
        StringReader source = new StringReader("host=localhost\nport=8080\n");

        Properties properties = PropertiesConfig.load(source);

        assertEquals("localhost", properties.getProperty("host"));
        assertEquals("8080", properties.getProperty("port"));
    }

    @Test
    void getOrDefaultReturnsTheStoredValueWhenPresent() {
        Properties properties = new Properties();
        properties.setProperty("timeout", "30");

        assertEquals("30", PropertiesConfig.getOrDefault(properties, "timeout", "10"));
    }

    @Test
    void getOrDefaultReturnsTheFallbackWhenMissing() {
        Properties properties = new Properties();

        assertEquals("10", PropertiesConfig.getOrDefault(properties, "timeout", "10"));
    }
}
