<!--
Exercise 055 — DataTable component (intermediate).
Goal:   render a table of rows/columns while letting the parent customize
        how each cell is displayed via a scoped "cell" slot.
Drills: scoped slots with named slots, v-slot:cell="{ row, column, value }",
        rendering a table via nested v-for while delegating per-cell markup
        to the parent (falling back to the plain value when no slot content
        is supplied for a given cell).
-->
<template>
  <table class="data-table">
    <thead>
      <tr>
        <!-- TODO: render one <th> per column, showing column.label -->
      </tr>
    </thead>
    <tbody>
      <!-- TODO: for each row in `rows`, render a <tr>; for each column in
           `columns`, render a <td> that yields the "cell" scoped slot with
           { row, column, value } as slot props, falling back to the raw
           value when the parent supplies no slot content -->
    </tbody>
  </table>
</template>

<script setup lang="ts">
export interface Column {
  key: string;
  label: string;
}

export interface Row {
  [key: string]: unknown;
}

defineProps<{
  columns: Column[];
  rows: Row[];
}>();

defineSlots<{
  cell(props: { row: Row; column: Column; value: unknown }): unknown;
}>();

throw new Error("TODO: implement DataTable scoped slot rendering");
</script>
