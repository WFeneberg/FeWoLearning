// Package regexploglogparser — Exercise 062 (reference solution).
package regexploglogparser

import (
	"fmt"
	"regexp"
)

var logLineRe = regexp.MustCompile(
	`^(?P<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}Z)\s+\[(?P<level>[A-Z]+)\]\s+(?P<message>.+)$`,
)

// ParseLogLine parses a log line and returns a map with keys
// "timestamp", "level", and "message" extracted via named capture groups.
// It returns an error if the line does not match the expected format.
func ParseLogLine(line string) (map[string]string, error) {
	match := logLineRe.FindStringSubmatch(line)
	if match == nil {
		return nil, fmt.Errorf("regexploglogparser: line does not match expected format: %q", line)
	}

	result := make(map[string]string, len(match))
	for i, name := range logLineRe.SubexpNames() {
		if i == 0 || name == "" {
			continue
		}
		result[name] = match[i]
	}
	return result, nil
}
