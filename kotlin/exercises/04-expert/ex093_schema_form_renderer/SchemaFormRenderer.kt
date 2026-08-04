package fewolearning.exercises.expert.ex093_schema_form_renderer

/*
Exercise 093 - Schema form renderer (expert).

Goal:   Render form fields from a schema and validate submitted values against it.
Drills: schema-driven rendering, validation.
*/
data class FieldSchema(val name: String, val required: Boolean)

fun renderLabels(schema: List<FieldSchema>): List<String> {
    TODO()
}

fun validate(schema: List<FieldSchema>, values: Map<String, String>): List<String> {
    TODO()
}
