package fewolearning.exercises.advanced.ex084_service_loader_plugin;

import java.util.ArrayList;
import java.util.List;
import java.util.ServiceLoader;

/*
Exercise 084 - ServiceLoader plugin (reference solution).
*/
public final class ServiceLoaderPlugin {
    private ServiceLoaderPlugin() {
    }

    public interface Greeter {
        String greet();
    }

    public static List<String> greetAll() {
        List<String> greetings = new ArrayList<>();
        for (Greeter greeter : ServiceLoader.load(Greeter.class)) {
            greetings.add(greeter.greet());
        }
        return greetings;
    }
}
