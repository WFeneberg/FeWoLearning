package fewolearning.exercises.advanced.ex080_delegate_validation

import kotlin.reflect.KProperty

/** Property delegate that rejects negative values at the point they are assigned. */
class NonNegative(initialValue: Int) {
    private var value = initialValue

    operator fun getValue(thisRef: Any?, property: KProperty<*>): Int = value

    operator fun setValue(thisRef: Any?, property: KProperty<*>, newValue: Int) {
        require(newValue >= 0) { "${property.name} must not be negative, was $newValue" }
        value = newValue
    }
}
