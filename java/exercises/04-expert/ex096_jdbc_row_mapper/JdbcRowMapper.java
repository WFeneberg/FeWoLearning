package fewolearning.exercises.expert.ex096_jdbc_row_mapper;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

/*
Exercise 096 - JDBC row mapper (expert).

Goal:   Map each row of a ResultSet into a typed record using a mapper function.
Drills: JDBC basics, mapping rows to objects.
*/
public final class JdbcRowMapper {
    private JdbcRowMapper() {
    }

    public record User(int id, String name) {
    }

    public static List<User> mapUsers(ResultSet resultSet) throws SQLException {
        throw new UnsupportedOperationException("TODO");
    }
}
