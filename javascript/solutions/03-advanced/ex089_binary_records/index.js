// Reference solution — exercise 089.

const encoder = new TextEncoder();
const decoder = new TextDecoder();

export function encodedLength(record) {
  const nameBytes = encoder.encode(record.name).length;
  const tagBytes = record.tags.reduce(
    (total, tag) => total + 1 + encoder.encode(tag).length,
    0,
  );
  return 2 + 1 + nameBytes + 2 + tagBytes;
}

export function encodeRecord(record) {
  const buffer = new ArrayBuffer(encodedLength(record));
  const view = new DataView(buffer);
  const bytes = new Uint8Array(buffer);
  let offset = 0;

  view.setUint16(offset, record.version, false);
  offset += 2;

  const name = encoder.encode(record.name);
  view.setUint8(offset++, name.length);
  bytes.set(name, offset);
  offset += name.length;

  view.setUint16(offset, record.tags.length, false);
  offset += 2;

  for (const tag of record.tags) {
    const encoded = encoder.encode(tag);
    view.setUint8(offset++, encoded.length);
    bytes.set(encoded, offset);
    offset += encoded.length;
  }
  return buffer;
}

export function decodeRecord(buffer) {
  const view = new DataView(buffer);
  const bytes = new Uint8Array(buffer);
  let offset = 0;

  const readBytes = (length) => {
    // DataView bounds-checks; a raw subarray does not, so check here.
    if (offset + length > bytes.length) {
      throw new RangeError("record is truncated");
    }
    const text = decoder.decode(bytes.subarray(offset, offset + length));
    offset += length;
    return text;
  };

  const version = view.getUint16(offset, false);
  offset += 2;
  const name = readBytes(view.getUint8(offset++));
  const tagCount = view.getUint16(offset, false);
  offset += 2;

  const tags = [];
  for (let i = 0; i < tagCount; i++) tags.push(readBytes(view.getUint8(offset++)));
  return { version, name, tags };
}
