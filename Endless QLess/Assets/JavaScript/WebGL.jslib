let WebGL = {
    GetSeedParam: function (gameObject, callbackMethod) {
        gameObject = UTF8ToString(gameObject);
        callbackMethod = UTF8ToString(callbackMethod);
        let url = new URL(document.URL);
        let result = url.searchParams.get("seed");
        if (result == null) { result = ""; }
        SendMessage(gameObject, callbackMethod, result);
    },

    SetSeedParam: function (seed) {
        seed = UTF8ToString(seed);
        window.history.pushState({}, null, `?seed=${seed}`);
    }
};

mergeInto(LibraryManager.library, WebGL);