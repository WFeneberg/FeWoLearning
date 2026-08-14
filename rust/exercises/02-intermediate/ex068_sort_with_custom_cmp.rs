//! Exercise 068 — Multi-key sorting with `sort_by` (intermediate).
//! Goal:   order a list by department (ascending), then salary
//!         (descending) within a department, then name as a final
//!         tiebreaker.
//! Drills: `Ordering`, `sort_by`, chaining comparisons with `then_with`.

#[derive(Debug, Clone, PartialEq)]
pub struct Employee {
    pub department: String,
    pub name: String,
    pub salary: u32,
}

pub fn sort_employees(employees: &mut Vec<Employee>) {
    todo!("sort_employees over {} employees", employees.len())
}

#[cfg(test)]
mod tests {
    use super::*;

    fn emp(department: &str, name: &str, salary: u32) -> Employee {
        Employee { department: department.to_string(), name: name.to_string(), salary }
    }

    #[test]
    fn sorts_by_department_first() {
        let mut employees = vec![emp("eng", "Bo", 100), emp("art", "Al", 90)];
        sort_employees(&mut employees);
        assert_eq!(employees[0].department, "art");
        assert_eq!(employees[1].department, "eng");
    }

    #[test]
    fn breaks_ties_by_descending_salary_within_a_department() {
        let mut employees = vec![
            emp("eng", "Low", 50),
            emp("eng", "High", 150),
            emp("eng", "Mid", 100),
        ];
        sort_employees(&mut employees);
        let names: Vec<&str> = employees.iter().map(|e| e.name.as_str()).collect();
        assert_eq!(names, vec!["High", "Mid", "Low"]);
    }

    #[test]
    fn breaks_remaining_ties_by_name() {
        let mut employees = vec![
            emp("eng", "Zoe", 100),
            emp("eng", "Amy", 100),
        ];
        sort_employees(&mut employees);
        let names: Vec<&str> = employees.iter().map(|e| e.name.as_str()).collect();
        assert_eq!(names, vec!["Amy", "Zoe"]);
    }

    #[test]
    fn sorting_an_empty_list_is_a_no_op() {
        let mut employees: Vec<Employee> = vec![];
        sort_employees(&mut employees);
        assert!(employees.is_empty());
    }
}
