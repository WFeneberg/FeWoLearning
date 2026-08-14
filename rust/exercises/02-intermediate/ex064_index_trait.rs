//! Exercise 064 — `Index`/`IndexMut` for a custom collection (intermediate).
//! Goal:   let a 2-D grid be indexed with `grid[(x, y)]` syntax, both for
//!         reads and writes.
//! Drills: `impl std::ops::Index`, `impl std::ops::IndexMut`, mapping a
//!         2-D coordinate onto a flat backing `Vec`.

pub struct Grid {
    width: usize,
    cells: Vec<i32>,
}

impl Grid {
    pub fn new(width: usize, height: usize) -> Self {
        Grid { width, cells: vec![0; width * height] }
    }
}

impl std::ops::Index<(usize, usize)> for Grid {
    type Output = i32;

    fn index(&self, (x, y): (usize, usize)) -> &i32 {
        todo!("Grid::index(({x}, {y}))")
    }
}

impl std::ops::IndexMut<(usize, usize)> for Grid {
    fn index_mut(&mut self, (x, y): (usize, usize)) -> &mut i32 {
        todo!("Grid::index_mut(({x}, {y}))")
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn new_grid_is_all_zeros() {
        let grid = Grid::new(3, 2);
        assert_eq!(grid[(0, 0)], 0);
        assert_eq!(grid[(2, 1)], 0);
    }

    #[test]
    fn writes_through_index_mut_are_visible_through_index() {
        let mut grid = Grid::new(3, 3);
        grid[(1, 2)] = 42;
        assert_eq!(grid[(1, 2)], 42);
    }

    #[test]
    fn rows_and_columns_stay_independent() {
        let mut grid = Grid::new(2, 2);
        grid[(0, 0)] = 1;
        grid[(1, 0)] = 2;
        grid[(0, 1)] = 3;
        grid[(1, 1)] = 4;
        assert_eq!(grid[(0, 0)], 1);
        assert_eq!(grid[(1, 0)], 2);
        assert_eq!(grid[(0, 1)], 3);
        assert_eq!(grid[(1, 1)], 4);
    }
}
