window.subtleCryptoInterop = { 
    encrypt: async function (key, plaintext) {
        const enc = new TextEncoder();
        const encoded = enc.encode(plaintext);
        const iv = window.crypto.getRandomValues(new Uint8Array(12));
    
        const importedKey = await window.crypto.subtle.importKey("raw", key.buffer, { name: "AES-GCM" }, true, ["encrypt", "decrypt"]);
    
        let result = await window.crypto.subtle.encrypt(
            {
                name: "AES-GCM",
                iv: iv
            },
            importedKey,
            encoded
        );
    
        return {
            ciphertext: arrayBufferToBase64String(result),
            iv: uint8ArrayToBase64String(iv)
        };
    },
    decrypt: async function (key, iv, ciphertext) {
        let importedCiphertext = base64StringToUint8Array(ciphertext);
        let importedIv = base64StringToUint8Array(iv);

        const importedKey = await window.crypto.subtle.importKey("raw", key.buffer, { name: "AES-GCM" }, true, ["encrypt", "decrypt"]);
    
        let result = await window.crypto.subtle.decrypt(
            {
                name: "AES-GCM",
                iv: importedIv
            },
            importedKey,
            importedCiphertext
        );
    
        const resultUint8 = new Uint8Array(result);
        const resultString = Array.from(resultUint8).reduce((data, byte) => data + String.fromCharCode(byte), '');
    
        return resultString;
    }
}

function base64StringToUint8Array(base64String) {
    let binary_string = window.atob(base64String);
    let len = binary_string.length;
    let bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary_string.charCodeAt(i);
    }
    return bytes;
};

function arrayBufferToBase64String(arrayBuffer) {
    let bytes = new Uint8Array(arrayBuffer);
    let binary = '';
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    const resultBase64 = window.btoa(binary);

    return resultBase64;
};

function uint8ArrayToBase64String(bytes) {
    let binary = '';
    let len = bytes.byteLength;
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    const resultBase64 = window.btoa(binary);

    return resultBase64;
};
