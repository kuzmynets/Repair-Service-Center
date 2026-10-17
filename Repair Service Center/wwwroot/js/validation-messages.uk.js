// Laboratory work 8: Ukrainian texts of the standard rules of jQuery Validation.
// MVC rules (data-val-required, data-val-regex, ...) already come translated from the server,
// this file is needed only for the rules that jQuery Validation adds itself (type="number", min, max).
// It is included in _ValidationScriptsPartial.cshtml only when the language of the site is Ukrainian.
(function ($) {
    if (!$ || !$.validator) {
        return;
    }
    $.extend($.validator.messages, {
        required: "Це поле є обов’язковим.",
        remote: "Виправте це поле.",
        email: "Введіть правильну адресу e-mail.",
        url: "Введіть правильну адресу URL.",
        date: "Введіть правильну дату.",
        dateISO: "Введіть правильну дату (ISO).",
        number: "Введіть число.",
        digits: "Введіть лише цифри.",
        equalTo: "Введіть те саме значення ще раз.",
        maxlength: $.validator.format("Введіть не більше {0} символів."),
        minlength: $.validator.format("Введіть щонайменше {0} символів."),
        rangelength: $.validator.format("Введіть від {0} до {1} символів."),
        range: $.validator.format("Введіть значення від {0} до {1}."),
        max: $.validator.format("Введіть значення, менше або рівне {0}."),
        min: $.validator.format("Введіть значення, більше або рівне {0}."),
        step: $.validator.format("Введіть значення, кратне {0}.")
    });
})(window.jQuery);
