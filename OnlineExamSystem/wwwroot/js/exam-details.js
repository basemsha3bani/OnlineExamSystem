(function ($) {
    "use strict";
    $(function () {
        var button = $("#btnAddSection");
        var error = $("#section-error");
        var container = $("#section-form-container");
        button.on("click", function () {
            error.text("");
            button.prop("disabled", true);
            $.get(button.data("url"))
                .done(function (html) { container.html(html); })
                .fail(function () { error.text("Unable to load the section form. Please try again."); })
                .always(function () { button.prop("disabled", false); });
        });
        container.on("submit", "#createSectionForm", function (event) {
            event.preventDefault();
            var form = $(this);
            var submit = form.find(":submit");
            error.text("");
            submit.prop("disabled", true);
            $.ajax({ url: form.attr("action"), method: "POST", data: form.serialize() })
                .done(function (html) {
                    $("#sections-container").html(html);
                    container.empty();
                })
                .fail(function (xhr) {
                    if (xhr.status === 422) container.html(xhr.responseText);
                    else error.text("Unable to save the section. Please try again.");
                })
                .always(function () { submit.prop("disabled", false); });
        });
    });
})(jQuery);
