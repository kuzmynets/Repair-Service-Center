// Laboratory work 7: client-side part of the attribute [FutureDate].
// jQuery Validation does not know this rule, so we add a method and an adapter.
// The server-side check is in Validation/AppValidationAttributes.cs.
(function ($) {
    if (!$ || !$.validator || !$.validator.unobtrusive) {
        return;
    }

    $.validator.addMethod('futuredate', function (value, element) {
        if (this.optional(element)) {
            return true; // an empty field is checked by [Required]
        }
        // <input type="datetime-local"> gives a value like "2026-10-20T10:00"
        var date = new Date(value);
        return !isNaN(date.getTime()) && date > new Date();
    });

    // data-val-futuredate="message" -> rule "futuredate" with this message
    $.validator.unobtrusive.adapters.addBool('futuredate');
})(window.jQuery);
