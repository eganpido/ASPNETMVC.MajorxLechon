// loader.js
const Loader = {
    show: function () {
        let overlay = document.getElementById("loader-overlay");
        if (overlay) overlay.style.display = "flex";
    },
    hide: function () {
        let overlay = document.getElementById("loader-overlay");
        if (overlay) overlay.style.display = "none";
    }
};
