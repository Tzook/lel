mergeInto(LibraryManager.library, {
    SetCookie: function(cookie) {
        document.cookie = cookie;
    },
    GetCookie: function() {
        var returnStr = document.cookie;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    GetHostName: function() {
        var returnStr = location.hostname;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
});