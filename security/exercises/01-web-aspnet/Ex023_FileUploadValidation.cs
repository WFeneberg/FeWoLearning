namespace FeWoLearning.Security.Exercises.WebAspNet;

// Exercise 023 — FileUploadValidation (web-aspnet).
// Goal:   Decide whether an uploaded file may be accepted at all, and if so hand
//         back a storage name safe to write to disk - checked against an
//         extension allowlist, checked again against the file's own magic bytes
//         (the extension is a claim, the content is the evidence), checked
//         against a byte-size ceiling, and never built out of anything the
//         caller supplied, so a malicious client name can never smuggle a path
//         segment into where the file ends up.
// Drills: content sniffing, extension allowlists, size limits, safe storage names.
// Passes: attack facts   - "payload.exe" is rejected outright by extension;
//                          "report.pdf" whose bytes begin with "MZ" is rejected -
//                          the extension lies, the content decides; a file over
//                          maxBytes is rejected; "../../evil.png" never yields a
//                          storageName containing ".." or a directory separator;
//         use facts      - a real PNG named "photo.png" is accepted, its
//                          storageName keeps the ".png" extension while being
//                          unpredictable (never the original name); a real PDF
//                          (bytes beginning "%PDF") named "report.pdf" is
//                          accepted too - so ".pdf" has to be on the allowlist,
//                          and the disguised "report.pdf" above can only be
//                          caught by reading its bytes, never by its extension;
//                          and two uploads of the same name produce different
//                          storageNames.
public static class Ex023_FileUploadValidation
{
    public static bool TryAccept(string clientFileName, byte[] content, long maxBytes, out string storageName, out string? rejection) =>
        throw new NotImplementedException(
            "TODO: Ex023 - accept only an allowlisted, size-bounded file whose content matches its extension, and hand back an unpredictable storage name");
}
