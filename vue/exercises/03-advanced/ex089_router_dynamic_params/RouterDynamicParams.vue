<!--
  Exercise 089 — RouterDynamicParams component (advanced).
  Goal:   a minimal stand-in for Vue Router's dynamic segments (`/users/:id`)
          and its `useRoute()` composable — matching a concrete path against a
          route pattern, extracting the `:id` param, and reading it from a
          nested `UserProfile` view via `useRoute()` (provide/inject), exactly
          like real Vue Router wires `useRoute()` through the app.
  Drills: dynamic route matching (`:param` segments), provide/inject to expose
          the current route to descendant components, `useRoute()`-style
          composables, reactive re-resolution on navigation.
-->
<script setup lang="ts">
import {
  computed,
  h,
  inject,
  provide,
  ref,
  type Component,
  type InjectionKey,
  type Ref,
} from "vue";

export interface RouteParams {
  [key: string]: string;
}

export interface RouteLocation {
  path: string;
  params: RouteParams;
}

interface RouteRecord {
  path: string;
  component: Component;
}

const ROUTE_KEY: InjectionKey<Ref<RouteLocation>> = Symbol("route");

// TODO: implement useRoute() — inject the current route provided below and
// throw if it is used outside of this router's component tree.
export function useRoute(): Ref<RouteLocation> {
  throw new Error("TODO: implement useRoute");
}

// Matches a concrete path (e.g. "/users/42") against a route pattern that may
// contain dynamic segments (e.g. "/users/:id"). Returns the extracted params
// on a match, or null when the pattern and path don't correspond.
//
// TODO: implement matchRoute — split both `pattern` and `path` into segments,
// require the same segment count, and for each pattern segment either bind it
// as a param (segment starts with ":") or require an exact literal match.
function matchRoute(_pattern: string, _path: string): RouteParams | null {
  throw new Error("TODO: implement matchRoute");
}

const UserProfile: Component = {
  name: "UserProfile",
  setup() {
    const route = useRoute();
    return () => h("p", { class: "user-profile" }, `User #${route.value.params.id}`);
  },
};

const NotFound: Component = {
  name: "NotFound",
  setup() {
    return () => h("p", { class: "not-found" }, "Not found");
  },
};

const routes: RouteRecord[] = [{ path: "/users/:id", component: UserProfile }];

const currentPath = ref("/");

const matched = computed((): { component: Component; params: RouteParams } => {
  for (const record of routes) {
    const params = matchRoute(record.path, currentPath.value);
    if (params) return { component: record.component, params };
  }
  return { component: NotFound, params: {} };
});

const route = computed<RouteLocation>(() => ({
  path: currentPath.value,
  params: matched.value.params,
}));

provide(ROUTE_KEY, route as unknown as Ref<RouteLocation>);

const matchedComponent = computed((): Component => matched.value.component);

function navigate(path: string): void {
  currentPath.value = path;
}

defineExpose({ route, navigate });
</script>

<template>
  <component :is="matchedComponent" />
</template>
