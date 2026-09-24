// Reference solution — exercise 069.

export function packRecord(id, score) {
  const buffer = new ArrayBuffer(12);
  const view = new DataView(buffer);
  view.setUint32(0, id, false); // false = big-endian, and it is not the default
  view.setFloat64(4, score, false);
  return buffer;
}

export function unpackRecord(buffer) {
  const view = new DataView(buffer);
  return { id: view.getUint32(0, false), score: view.getFloat64(4, false) };
}

export function endiannessBytes() {
  const buffer = new ArrayBuffer(8);
  const view = new DataView(buffer);
  view.setUint32(0, 0x01020304, false);
  view.setUint32(4, 0x01020304, true);
  return [...new Uint8Array(buffer)];
}

export function sharedBuffer() {
  const buffer = new ArrayBuffer(4);
  const bytes = new Uint8Array(buffer);
  const words = new Uint32Array(buffer);
  bytes[0] = 255;
  return { bytes: [...bytes], asUint32: words[0] };
}
