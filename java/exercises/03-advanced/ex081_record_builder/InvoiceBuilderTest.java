package fewolearning.exercises.advanced.ex081_record_builder;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertSame;

class InvoiceBuilderTest {

    @Test
    void buildsAnInvoiceFromTheConfiguredFields() {
        InvoiceBuilder.Invoice invoice = new InvoiceBuilder()
                .customerName("Ada Lovelace")
                .amount(199.99)
                .build();

        assertEquals("Ada Lovelace", invoice.customerName());
        assertEquals(199.99, invoice.amount(), 1e-9);
    }

    @Test
    void eachFluentSetterReturnsTheSameBuilderInstance() {
        InvoiceBuilder builder = new InvoiceBuilder();

        assertSame(builder, builder.customerName("Grace Hopper"));
        assertSame(builder, builder.amount(50.0));
    }

    @Test
    void buildingWithoutSettingFieldsUsesTheirDefaultValues() {
        InvoiceBuilder.Invoice invoice = new InvoiceBuilder().build();

        assertNull(invoice.customerName());
        assertEquals(0.0, invoice.amount(), 1e-9);
    }
}
