package fewolearning.exercises.intermediate.ex070_json_manual_parser

/** Parses a flat, string-valued JSON object into a map, without a JSON library. */
fun parseFlatJsonObject(json: String): Map<String, String> {
    val text = json.trim()
    require(text.startsWith("{") && text.endsWith("}")) { "Expected a JSON object: $json" }

    val result = mutableMapOf<String, String>()
    var index = 1
    val end = text.length - 1

    fun skipWhitespace() {
        while (index < end && text[index].isWhitespace()) index++
    }

    fun parseString(): String {
        require(index < end && text[index] == '"') { "Expected '\"' at position $index in: $json" }
        index++
        val sb = StringBuilder()
        while (index < end && text[index] != '"') {
            sb.append(text[index])
            index++
        }
        require(index < end && text[index] == '"') { "Unterminated string in: $json" }
        index++
        return sb.toString()
    }

    skipWhitespace()
    if (index >= end) return emptyMap()

    while (true) {
        skipWhitespace()
        val key = parseString()
        skipWhitespace()
        require(index < end && text[index] == ':') { "Expected ':' after key \"$key\" in: $json" }
        index++
        skipWhitespace()
        val value = parseString()
        result[key] = value
        skipWhitespace()
        if (index < end && text[index] == ',') {
            index++
            continue
        }
        break
    }
    skipWhitespace()
    require(index == end) { "Unexpected trailing content in: $json" }

    return result
}
