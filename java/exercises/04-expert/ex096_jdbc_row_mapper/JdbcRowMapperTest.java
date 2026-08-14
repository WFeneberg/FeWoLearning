package fewolearning.exercises.expert.ex096_jdbc_row_mapper;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Proxy;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Iterator;
import java.util.List;

import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

class JdbcRowMapperTest {

    @Test
    void mapsEveryRowOfTheResultSetIntoAUser() throws SQLException {
        ResultSet resultSet = fakeResultSet(List.of(
                new Object[] {1, "Alice"},
                new Object[] {2, "Bob"}));

        List<JdbcRowMapper.User> users = JdbcRowMapper.mapUsers(resultSet);

        assertEquals(List.of(
                new JdbcRowMapper.User(1, "Alice"),
                new JdbcRowMapper.User(2, "Bob")), users);
    }

    @Test
    void returnsAnEmptyListWhenTheResultSetHasNoRows() throws SQLException {
        ResultSet resultSet = fakeResultSet(List.of());

        List<JdbcRowMapper.User> users = JdbcRowMapper.mapUsers(resultSet);

        assertEquals(List.of(), users);
    }

    /**
     * Builds a minimal {@link ResultSet} fake via {@link Proxy}: hand-writing a
     * class implementing the real interface directly is impractical (140+
     * abstract methods) without a compiler to verify every signature. Only
     * {@code next()}, {@code getInt(String)} and {@code getString(String)} are
     * implemented; anything else throws {@link UnsupportedOperationException}.
     */
    private static ResultSet fakeResultSet(List<Object[]> rows) {
        Iterator<Object[]> iterator = rows.iterator();
        Object[][] current = new Object[1][];
        InvocationHandler handler = (proxy, method, args) -> {
            switch (method.getName()) {
                case "next":
                    if (!iterator.hasNext()) {
                        return false;
                    }
                    current[0] = iterator.next();
                    return true;
                case "getInt":
                    return (Integer) current[0][0];
                case "getString":
                    return (String) current[0][1];
                default:
                    throw new UnsupportedOperationException(method.getName());
            }
        };
        return (ResultSet) Proxy.newProxyInstance(
                JdbcRowMapperTest.class.getClassLoader(),
                new Class<?>[] {ResultSet.class},
                handler);
    }
}
