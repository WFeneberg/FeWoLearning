package fewolearning.exercises.expert.ex097_retrying_http_client

/*
Exercise 097 - Retrying HTTP client (expert).

Goal:   Wrap a suspending HTTP call with a retry policy that gives up after N tries.
Drills: policy composition, suspend networking facade.
*/
interface HttpClientFacade {
    suspend fun get(url: String): String
}

class RetryingHttpClient(private val delegate: HttpClientFacade, private val maxAttempts: Int) {
    suspend fun get(url: String): String {
        TODO()
    }
}
