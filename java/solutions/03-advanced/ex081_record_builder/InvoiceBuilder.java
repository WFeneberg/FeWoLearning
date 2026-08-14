package fewolearning.exercises.advanced.ex081_record_builder;

/*
Exercise 081 - Record builder (reference solution).
*/
public final class InvoiceBuilder {
    private String customerName;
    private double amount;

    public InvoiceBuilder customerName(String customerName) {
        this.customerName = customerName;
        return this;
    }

    public InvoiceBuilder amount(double amount) {
        this.amount = amount;
        return this;
    }

    public Invoice build() {
        return new Invoice(customerName, amount);
    }

    public record Invoice(String customerName, double amount) {
    }
}
