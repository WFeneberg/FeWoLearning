package fewolearning.exercises.advanced.ex084_service_loader_plugin;

/*
Fixture for exercise 084 - a concrete Greeter registered via
META-INF/services so ServiceLoader can discover it. Not part of the
exercise itself (greetAll() is).
*/
public final class EnglishGreeter implements ServiceLoaderPlugin.Greeter {

    @Override
    public String greet() {
        return "Hello!";
    }
}
