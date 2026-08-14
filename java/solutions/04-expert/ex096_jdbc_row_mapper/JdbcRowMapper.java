package fewolearning.exercises.expert.ex096_jdbc_row_mapper;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

/*
Exercise 096 - JDBC row mapper (reference solution).
*/
public final class JdbcRowMapper {
    private JdbcRowMapper() {
    }

    public record User(int id, String name) {
    }

    public static List<User> mapUsers(ResultSet resultSet) throws SQLException {
        List<User> users = new ArrayList<>();
        while (resultSet.next()) {
            users.add(new User(resultSet.getInt("id"), resultSet.getString("name")));
        }
        return users;
    }
}
