package fewolearning.exercises.advanced.ex081_record_builder;

/*
Exercise 081 - Record builder (advanced).

Goal:   Provide a fluent builder that produces an immutable Invoice record.
Drills: records, builder ergonomics.
*/
public final class InvoiceBuilder {
    private String customerName;
    private double amount;

    public InvoiceBuilder customerName(String customerName) {
        throw new UnsupportedOperationException("TODO");
    }

    public InvoiceBuilder amount(double amount) {
        throw new UnsupportedOperationException("TODO");
    }

    public Invoice build() {
        throw new UnsupportedOperationException("TODO");
    }

    public record Invoice(String customerName, double amount) {
    }
}
