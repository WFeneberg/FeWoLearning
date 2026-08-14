package fewolearning.exercises.expert.ex093_schema_form_renderer

/**
 * Label format: a required field renders as "name *", everything else as plain "name".
 * validate() returns the names (in schema order) of required fields that are either
 * absent from [values] or present but blank.
 */
data class FieldSchema(val name: String, val required: Boolean)

fun renderLabels(schema: List<FieldSchema>): List<String> =
    schema.map { field -> if (field.required) "${field.name} *" else field.name }

fun validate(schema: List<FieldSchema>, values: Map<String, String>): List<String> =
    schema.filter { it.required && values[it.name].isNullOrBlank() }.map { it.name }
