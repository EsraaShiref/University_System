// Client-side validation extras. Loaded after jquery.validate.unobtrusive (see _ValidationScriptsPartial).
(function ($) {
    if (!$ || !$.validator || !$.validator.unobtrusive) return;

    const adapters = $.validator.unobtrusive.adapters;

    // ---- [MaxFileSize] -> data-val-filesize ----
    $.validator.addMethod('filesize', function (value, element, max) {
        if (!element.files || element.files.length === 0) return true;
        return element.files[0].size <= max;
    });
    adapters.add('filesize', ['max'], function (options) {
        options.rules['filesize'] = parseInt(options.params.max, 10);
        options.messages['filesize'] = options.message;
    });

    // ---- [AllowedExtensions] -> data-val-fileext ----
    $.validator.addMethod('fileext', function (value, element, allowed) {
        if (!element.files || element.files.length === 0) return true;
        const name = element.files[0].name.toLowerCase();
        return allowed.some(function (ext) { return name.endsWith(ext); });
    });
    adapters.add('fileext', ['allowed'], function (options) {
        options.rules['fileext'] = options.params.allowed.split(',');
        options.messages['fileext'] = options.message;
    });

    // ---- [LessOrEqualToProperty] -> data-val-lteprop ----
    $.validator.addMethod('lteprop', function (value, element, params) {
        if (value === '') return true;
        const other = $(params.form).find('[name="' + params.other + '"]').val();
        if (other === undefined || other === '') return true;
        const a = parseFloat(value), b = parseFloat(other);
        if (isNaN(a) || isNaN(b)) return true;
        return a <= b;
    });
    adapters.add('lteprop', ['other'], function (options) {
        options.rules['lteprop'] = { other: options.params.other, form: options.form };
        options.messages['lteprop'] = options.message;

        // Re-check this field when the field it depends on changes
        $(options.form).on('input change', '[name="' + options.params.other + '"]', function () {
            if ($(options.element).val() !== '') $(options.element).valid();
        });
    });
})(window.jQuery);

document.addEventListener('DOMContentLoaded', function () {
    const $ = window.jQuery;

    // Red asterisk on every label whose input is required
    document.querySelectorAll('[data-val-required]').forEach(function (input) {
        if (!input.id) return;
        const label = document.querySelector('label[for="' + input.id + '"]');
        if (label && !label.querySelector('.required-mark')) {
            label.insertAdjacentHTML('beforeend', ' <span class="required-mark text-danger" aria-hidden="true">*</span>');
        }
    });

    // data-revalidate="Field1,Field2": when this input changes, re-run validation on those fields
    // (e.g. changing the department re-checks "course name is unique in this department")
    if ($) {
        document.querySelectorAll('[data-revalidate]').forEach(function (source) {
            const targets = source.getAttribute('data-revalidate').split(',');
            $(source).on('change', function () {
                targets.forEach(function (name) {
                    const target = $(source.form).find('[name="' + name.trim() + '"]');
                    if (target.length && target.val() !== '') target.valid();
                });
            });
        });

        // Validate a picked file immediately instead of waiting for the form submit
        $(document).on('change', 'input[type="file"][data-val="true"]', function () { $(this).valid(); });

        // Jump to the first invalid field when the form is submitted with errors
        $(document).on('invalid-form.validate', 'form', function (event, validator) {
            const first = validator.errorList.length ? validator.errorList[0].element : null;
            if (first) first.focus();
        });
    }
});
