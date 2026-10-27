window.AIChatBox = {
    scrollToElement: function (element) {
        if (!element) {
            return;
        }

        element.scrollIntoView({
            behavior: "smooth",
            block: "end"
        });
    },

    autoResize: function (textarea) {
        if (!textarea) {
            return;
        }

        textarea.style.height = "auto";
        textarea.style.height =
            Math.min(textarea.scrollHeight, 100) + "px";
    },

    resetTextarea: function (textarea) {
        if (!textarea) {
            return;
        }

        textarea.style.height = "42px";
    }
};