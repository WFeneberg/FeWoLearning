package fewolearning.exercises.advanced.ex084_service_loader_plugin;

import java.util.List;

/*
Exercise 084 - ServiceLoader plugin (advanced).

Goal:   Discover all registered Greeter implementations using ServiceLoader.
Drills: ServiceLoader, pluggable implementations.
*/
public final class ServiceLoaderPlugin {
    private ServiceLoaderPlugin() {
    }

    public interface Greeter {
        String greet();
    }

    public static List<String> greetAll() {
        throw new UnsupportedOperationException("TODO");
    }
}
