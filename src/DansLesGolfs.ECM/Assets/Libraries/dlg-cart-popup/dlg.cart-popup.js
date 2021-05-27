/**
* plugin name: jQuery DLG Cart Popup
* description: Using for create cart popup.
* author name: Weerayut Teja
* author email: kenessar@gmail.com
*/
if (!window.cartLocale) {
    window.cartLocale = {
        photo: "Photo",
        description: "Description",
        qty: "Qty",
        price: "Price",
        cartEmpty: "Cart Empty",
        checkout: "Checkout",
        continueToShopping: "Continue to shopping"
    };
}
(function ($) {
    CartPopup = function (options) {

        if (typeof (options) == "String") {
            eval(options + "()");
            return;
        }
        /**
        * Default options.
        */
        var defaultOptions = {
            cartItemsUrl: '',
            speed: 'normal',
            align: 'center',
            marginTop: 0,
            marginLeft: 0,
            marginRight: 0,
            marginBottom: 0,
            checkOutUrl: 'javascript:void()',
            shoppingUrl: 'javascript:void()',
            customX: false,
            customY: false
        };
        // extends options from default options.
        options = $.extend({}, defaultOptions, options);

        // Add setting to global variables.
        window.cartItemsUrl = options.cartItemsUrl;
        window.updateItemUrl = options.updateItemUrl;

        var initCartPopup = function () {

            $('.dlg-cart-popup, .dlg-cart-popup-overlay, .dlg-cart-placeholder').remove();

            var $popup = $('<div class="dlg-cart-popup"><div class="dlg-cart-popup-wrapper"><table class="cart-items-table"><thead><tr><th class="column-photo">' + window.cartLocale.photo + '</th><th class="column-description">' + window.cartLocale.description + '</th><th class="column-quantity">Qty</th><th class="column-price">' + window.cartLocale.price + '</th></tr></thead><tbody></tbody></table></div><div class="action-buttons"></div></div>').appendTo('body');
            $('<a href="' + options.shoppingUrl + '" class="btn btn-default pull-left">&gt; ' + window.cartLocale.continueToShopping + '</a><a href="' + options.checkOutUrl + '" class="btn btn-primary pull-right">&gt; ' + window.cartLocale.checkout + '</a>').appendTo('.dlg-cart-popup .action-buttons');
            $('<div class="dlg-cart-popup-overlay"></div>').appendTo('body').click(function (e) {
                $('.dlg-cart-popup, .dlg-cart-popup-overlay').fadeOut(options.speed);
                $('.dlg-cart-placeholder').hide();
            });

            //$('<div class="dlg-cart-placeholder"></div>').appendTo('body').hide();

            $('.dlg-cart-popup-overlay').mouseover(function (e) {
                //$('.dlg-cart-popup, .dlg-cart-popup-overlay').fadeOut(options.speed);
            });
        };

        /**
        * Load cart items by url (JSON Data type)
        * @param url    The url that return data in JSON format.
        */
        loadCartItems = function (url, element) {
            var cartUrl = '';
            if (url)
                cartUrl = url
            else
                cartUrl = window.cartItemsUrl;

            if (cartUrl) {
                $('.dlg-cart-popup .cart-items-table tbody').html('');
                $('.dlg-cart-popup .cart-items-table tfoot').remove();
                $.ajax({
                    url: cartUrl,
                    type: 'post',
                    dataType: 'json',
                    success: function (result) {
                        $('.dlg-cart-popup .cart-items-table tbody').html('');
                        $('.dlg-cart-popup .cart-items-table tfoot').remove();
                        if (result.isSuccess && result.cartItems.length > 0) {
                            var ci = null;
                            if (result.cartItems.length > 0) {
                                for (var i in result.cartItems) {
                                    ci = result.cartItems[i];
                                    $('<tr data-id="' + ci.itemId + '" data-unitPrice="' + ci.unitPrice + '"><td class="column-photo"><a href="' + getUrl('Item/' + ci.itemSlug) + '"><img src="' + ci.itemImage + '" /></a></td><td class="column-name"><a href="' + getUrl('Item/' + ci.itemSlug) + '">' + ci.itemName + '</a></td><td class="column-quantity">' + ci.quantity + '</td><td class="column-price"><span class="price-number">' + ci.totalPrice + '</span> <span class="currency-sign">' + result.currency + '</span></td></tr>').appendTo('.dlg-cart-popup .cart-items-table tbody');
                                    $('.dlg-cart-popup .cart-items-table tbody tr:last-child .column-quantity input').change(function () {
                                        var $this = $(this);
                                        var $row = $this.parents('tr');
                                        var itemId = $row.data('id');
                                        var unitPrice = $row.data('unitprice');
                                        var qty = eval($this.val());
                                        var grossPrice = Math.round(unitPrice * qty);
                                        $row.find('.column-price .price-number').text(grossPrice);

                                        jQuery.ajax({
                                            url: window.updateItemUrl,
                                            data: { itemId: itemId, quantity: qty },
                                            dataType: 'json',
                                            type: 'post',
                                            success: function (result) {
                                                if (result.isSuccess) {
                                                    calculateCartPrice();
                                                }
                                            }
                                        });

                                    });
                                }

                                $('.dlg-cart-notify').text(result.cartItems.length);
                                if (result.cartItems.length > 0) {
                                    $('.dlg-cart-notify').removeClass('hide');
                                } else {
                                    $('.dlg-cart-notify').addClass('hide');
                                }

                                // Add Table Footer.
                                $('.dlg-cart-popup .cart-items-table tfoot').remove();

                                $('.dlg-cart-popup .action-buttons, dlg-cart-popup .cart-items-table tfoot').show();
                            } else {
                                $('<tr><td class="column-empty" colspan="4">Cart empty.</td></tr>').appendTo('.dlg-cart-popup .cart-items-table tbody');
                                $('.dlg-cart-popup .action-buttons, dlg-cart-popup .cart-items-table tfoot').hide();
                            }
                            $('<tfoot><tr><td colspan="4" class="column-total-price"><span class="pull-left">Total TTC</span><span class="cart-total-price pull-right"><span class="price-number">' + result.cartTotalPrice + '</span> <span class="currency-sign">' + result.currency + '</span></span></td></tr></tfoot>').appendTo('.dlg-cart-popup .cart-items-table');
                        }
                    }
                });
            }
        };

        var addCartItemNoty = function (element) {
            $('.dlg-cart-notify').remove();
            $('<span class="dlg-cart-notify hide">0</div>').appendTo(element);
        };

        /**
        * Close cart popup.
        */
        this.close = function () {
            $('.dlg-cart-popup, .dlg-cart-popup-overlay').fadeOut(options.speed);
        };

        /**
        * Initialize Cart Popup.
        */
        var init = function (element) {
            var $this = $(element);
            $this.addClass('dlgCartPopup');
            addCartItemNoty(element);
            initCartPopup();
            loadCartItems(options.cartItemsUrl, element);

            $this.unbind("loadCartItems").bind("loadCartItems", onLoadCartItems);

            // Init CSS.
            $this.css({
                position: 'relative'
            });
        };

        var onLoadCartItems = function () {
            loadCartItems(window.cartItemsUrl, this);
        }

        var calculateCartPrice = function () {
            var qty = 0;
            var unitPrice = 0;
            var totalPrice = 0;
            var sumQty = 0;
            $('.dlg-cart-popup .cart-items-table tbody tr').each(function () {
                qty = eval($(this).find('.column-quantity input').val());
                unitPrice = eval($(this).data('unitprice'));
                totalPrice += qty * unitPrice;
                sumQty += qty;
            });
            // Add Notification.
            $('.dlg-cart-noti-number').remove();
            $(this).find('.dlg-cart-noti-number').text(sumQty);

            // Display total price.
            $('.dlg-cart-popup .cart-items-table tfoot tr').data('totalprice', totalPrice)
            .find('.cart-total-price .price-number').text(Math.round(totalPrice));
        };

        return this.each(function (elementIndex) {

            init(this);

            $(this).unbind('click')
            .bind('click', function (e) {
                // Load data
                init(this);

                var $this = $(this);
                var eleWidth = $this.width();
                var eleHeight = $this.height();
                var eleOffset = $this.offset();

                var $popup = $('.dlg-cart-popup');
                var popupWidth = $popup.outerWidth();
                var popupHeight = $popup.outerHeight();
                var windowWidth = $(window).width();
                var placeholderOrigin = options.align;

                if (options.align == 'left') {
                    $popup.css('left', eleOffset.left);
                    $popup.addClass('popup-align-left').removeClass('popup-align-right').removeClass('popup-align-center');
                } else if (options.align == 'right') {
                    $popup.css('right', windowWidth - (eleOffset.left + eleWidth) + options.marginRight);
                    $popup.addClass('popup-align-right').removeClass('popup-align-left').removeClass('popup-align-center');
                } else {
                    $popup.css('left', (eleOffset.left + (eleWidth / 2)) - (popupWidth / 2)); // Popup will be appeared on the center of element.
                    $popup.addClass('popup-align-center').removeClass('popup-align-left').removeClass('popup-align-right');
                }

                // Assign top position.
                if (!options.customY) {
                    $popup.css('top', eleOffset.top + eleHeight + options.marginTop);
                }

                $('.dlg-cart-placeholder').css({
                    position: 'absolute',
                    top: eleOffset.top,
                    width: popupWidth,
                    height: parseInt($popup.css('top')) + popupHeight,
                    'z-index': 998
                });
                $('.dlg-cart-placeholder').css(placeholderOrigin, parseInt($popup.css(placeholderOrigin))).show();

                $('.dlg-cart-popup-overlay, .dlg-cart-popup').fadeIn(options.speed);
            })
        });
    };

    $.fn.dlgCartPopup = CartPopup;
})(jQuery);