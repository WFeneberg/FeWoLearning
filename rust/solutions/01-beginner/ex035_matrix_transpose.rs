//! Exercise 035 — Matrix transpose (reference solution).

pub fn transpose(matrix: &[Vec<i32>]) -> Vec<Vec<i32>> {
    if matrix.is_empty() {
        return Vec::new();
    }
    let rows = matrix.len();
    let cols = matrix[0].len();
    let mut result = vec![vec![0; rows]; cols];
    for (r, row) in matrix.iter().enumerate() {
        for (c, &value) in row.iter().enumerate() {
            result[c][r] = value;
        }
    }
    result
}

// Kept identical to the stub's test module: overlaying this file onto the stub
// must not remove the tests, otherwise the solution can never be verified.
#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn transposes_a_rectangular_matrix() {
        let matrix = vec![vec![1, 2, 3], vec![4, 5, 6]];
        assert_eq!(
            transpose(&matrix),
            vec![vec![1, 4], vec![2, 5], vec![3, 6]]
        );
    }

    #[test]
    fn transposes_a_square_matrix() {
        let matrix = vec![vec![1, 2], vec![3, 4]];
        assert_eq!(transpose(&matrix), vec![vec![1, 3], vec![2, 4]]);
    }

    #[test]
    fn transposes_an_empty_matrix() {
        let matrix: Vec<Vec<i32>> = vec![];
        assert_eq!(transpose(&matrix), Vec::<Vec<i32>>::new());
    }
}
