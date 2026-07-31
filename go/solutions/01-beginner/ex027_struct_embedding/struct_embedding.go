// Package structembedding — Exercise 027 (reference solution).
package structembedding

import "fmt"

// Animal has a Name and a generic Speak method.
type Animal struct {
	Name string
}

// Speak returns a generic animal sound sentence.
func (a Animal) Speak() string {
	return fmt.Sprintf("%s makes a sound.", a.Name)
}

// Dog embeds Animal and overrides Speak with dog-specific behavior.
type Dog struct {
	Animal
}

// Speak returns a dog-specific bark sentence, overriding the promoted
// Animal.Speak method.
func (d Dog) Speak() string {
	return fmt.Sprintf("%s says Woof!", d.Name)
}
