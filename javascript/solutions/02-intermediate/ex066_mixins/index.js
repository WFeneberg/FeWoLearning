// Reference solution — exercise 066.

export function withSerializable(Base) {
  return class Serializable extends Base {
    toJSON() {
      return { ...this };
    }

    get serialized() {
      return JSON.stringify(this);
    }
  };
}

export function withStamp(Base) {
  return class Stamped extends Base {
    stamp(now) {
      this.updatedAt = now;
      return this;
    }
  };
}

export function mix(Base, ...mixins) {
  return mixins.reduce((current, mixin) => mixin(current), Base);
}

export function copyMembers(target, source) {
  // Object.assign READS each property, so a getter would be flattened to
  // whatever it returned at copy time. Descriptors keep the accessor.
  return Object.defineProperties(target, Object.getOwnPropertyDescriptors(source));
}
