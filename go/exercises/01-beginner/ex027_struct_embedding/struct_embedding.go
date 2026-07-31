// Package structembedding — Exercise 027 (beginner).
// Goal:   Define an Animal struct with a Speak() string method, and a Dog
//         struct that embeds Animal and overrides Speak() while still being
//         able to reach the embedded behavior.
// Drills: struct embedding, promoted methods, method overriding.
package structembedding

// Animal has a Name and a generic Speak method.
type Animal struct {
	Name string
}

// Speak returns a generic animal sound sentence.
func (a Animal) Speak() string {
	panic("TODO: implement Animal.Speak")
}

// Dog embeds Animal and overrides Speak with dog-specific behavior.
type Dog struct {
	Animal
}

// Speak returns a dog-specific bark sentence, overriding the promoted
// Animal.Speak method.
func (d Dog) Speak() string {
	panic("TODO: implement Dog.Speak")
}
