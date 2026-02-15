/**
 * Laedt eine Datei herunter, indem ein Blob erstellt und ein temporaerer Download-Link erzeugt wird.
 * @param {string} fileName - Der Dateiname fuer den Download.
 * @param {string} content - Der Textinhalt der Datei.
 * @param {string} contentType - Der MIME-Typ (z.B. "application/json").
 */
window.downloadFileFromText = function (fileName, content, contentType) {
    const blob = new Blob([content], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

/**
 * Laedt eine Datei aus einem Byte-Array herunter.
 * @param {string} fileName - Der Dateiname fuer den Download.
 * @param {Uint8Array} byteArray - Die Datei als Byte-Array.
 * @param {string} contentType - Der MIME-Typ.
 */
window.downloadFileFromBytes = function (fileName, byteArray, contentType) {
    const blob = new Blob([byteArray], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};
