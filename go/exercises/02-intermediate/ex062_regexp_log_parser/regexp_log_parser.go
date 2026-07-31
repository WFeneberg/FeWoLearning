// Package regexploglogparser — Exercise 062 (intermediate).
// Goal:   Parse a log line of the form "2024-01-02T15:04:05Z [LEVEL] message"
//         into a map of its fields using a regexp with named capture groups.
// Drills: regexp compilation, named subexpressions, SubexpNames, error handling.
package regexploglogparser

// ParseLogLine parses a log line and returns a map with keys
// "timestamp", "level", and "message" extracted via named capture groups.
// It returns an error if the line does not match the expected format.
func ParseLogLine(line string) (map[string]string, error) {
	panic("TODO: implement ParseLogLine")
}
