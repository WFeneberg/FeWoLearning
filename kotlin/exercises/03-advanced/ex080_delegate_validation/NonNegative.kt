package fewolearning.exercises.advanced.ex080_delegate_validation

import kotlin.reflect.KProperty

/*
Exercise 080 - Custom validating delegate (advanced).

Goal:   Reject negative values for any property backed by this delegate.
Drills: custom delegates, centralized validation.
*/
class NonNegative(initialValue: Int) {
    private var value = initialValue

    operator fun getValue(thisRef: Any?, property: KProperty<*>): Int {
        TODO()
    }

    operator fun setValue(thisRef: Any?, property: KProperty<*>, newValue: Int) {
        TODO()
    }
}
