window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text).then()
            .catch(function (error) {
                alert(error);
            });
    }
};