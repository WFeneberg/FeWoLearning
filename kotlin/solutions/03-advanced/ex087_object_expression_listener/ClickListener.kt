package fewolearning.exercises.advanced.ex087_object_expression_listener

interface ClickListener {
    fun onClick(x: Int, y: Int)
}

/** Implements ClickListener inline via an anonymous object expression that logs each click. */
fun loggingListener(log: MutableList<String>): ClickListener =
    object : ClickListener {
        override fun onClick(x: Int, y: Int) {
            log.add("clicked at ($x, $y)")
        }
    }
