//! Exercise 035 — Matrix transpose (beginner).
//! Goal:   transpose a rectangular matrix represented as a `Vec` of row
//!         `Vec`s, swapping rows and columns.
//! Drills: nested `Vec`, indexing, pre-allocating with `vec![...; n]`.

pub fn transpose(matrix: &[Vec<i32>]) -> Vec<Vec<i32>> {
    todo!("transpose({matrix:?})")
}

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
