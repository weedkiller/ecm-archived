/**
* plugin name: jQuery Number Editor
* description: Make simple input element as Number Editor
* author name: Weerayut Teja
* author email: kenessar@gmail.com
*/
(function ($) {
    NumberEditor = function (options) {
        /**
        * Default options.
        */
        var defaultOptions = {
            min: null,
            max: null,
            step: 1
        };
        // extends options from default options.
        options = $.extend({}, defaultOptions, options);

        return $(this).each(function () {
            var $this = $(this);
            $this.after('<div class="number-editor">');
            var $container = $this.next();
            $this.prependTo($container);
            var $stepUp = $('<span class="step-up">&#43;</span>').appendTo($container)
                            .click(function () {
                                var val = parseInt($this.val()) + options.step;
                                if (((options.max != null && options.max != undefined) && options.max < val) || ($this.attr('max') != null && $this.attr('max') != undefined && $this.attr('max') < val))
                                    val -= options.step;

                                $this.val(val).trigger('change');
                            });
            var $stepDown = $('<span class="step-down">&#45;</span>').appendTo($container)
                            .click(function () {
                                var val = parseInt($this.val()) - options.step;
                                if (((options.min != null && options.min != undefined) && options.min > val) || ($this.attr('min') != null && $this.attr('min') != undefined && $this.attr('min') > val))
                                    val += options.step;

                                $this.val(val).trigger('change');
                            });
            if (!$this.val() || $this.val().trim() == '') {
                $this.val(0);
            }

            // Input number only
            $this.keypress(function (e) {
                if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                    return false;
                }
            });
        });
    };

    $.fn.numberEditor = NumberEditor;
})(jQuery);