package fewolearning.exercises.beginner.ex015_map_count_words

/*
Exercise 015 - Map count words (reference solution).
*/
fun countWords(words: List<String>): Map<String, Int> {
    val counts = mutableMapOf<String, Int>()
    for (word in words) {
        counts[word] = counts.getOrDefault(word, 0) + 1
    }
    return counts
}
