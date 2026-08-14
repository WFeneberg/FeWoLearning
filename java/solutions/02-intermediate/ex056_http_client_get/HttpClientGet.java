package fewolearning.exercises.intermediate.ex056_http_client_get;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;

/*
Exercise 056 - HttpClient GET (reference solution).
*/
public final class HttpClientGet {
    private HttpClientGet() {
    }

    public static String get(HttpClient client, URI uri) throws Exception {
        HttpRequest request = HttpRequest.newBuilder(uri).GET().build();
        HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
        return response.body();
    }
}
