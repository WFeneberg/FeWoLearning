<!--
Exercise 055 — DataTable component (reference solution).
Goal:   render a table of rows/columns while letting the parent customize
        how each cell is displayed via a scoped "cell" slot.
-->
<template>
  <table class="data-table">
    <thead>
      <tr>
        <th v-for="column in columns" :key="column.key">{{ column.label }}</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(row, rowIndex) in rows" :key="rowIndex">
        <td v-for="column in columns" :key="column.key">
          <slot name="cell" :row="row" :column="column" :value="row[column.key]">
            {{ row[column.key] }}
          </slot>
        </td>
      </tr>
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
</script>
